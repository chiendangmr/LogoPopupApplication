using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HDVietNam
{
    [Serializable]
    public class SendFrame
    {
        public byte[][] Image { get; set; }
    }

    public class SendFrameArgs : EventArgs
    {
        public Guid Uid { get; set; }

        public SendFrame Frame { get; set; }
    }

    public class TcpRemoteHostState
    {
        public static byte[] SendHeader = new byte[] { 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x50 };

        TcpServer _Server = null;

        internal Guid Uid { get; private set; }

        TcpClient Client { get; set; }
        internal NetworkStream Stream { get; private set; }
        internal Queue<byte[]> SendQueue { get; private set; }

        internal byte[] ReadBuffer { get; private set; }

        internal event EventHandler<EventArgs> GotDataToSend;

        internal event EventHandler<SendFrameArgs> OnFrame;

        internal IPEndPoint EndPoint { get; private set; }

        volatile bool _closed = false;

        internal TcpRemoteHostState(TcpServer server, TcpClient client)
        {
            this._Server = server;

            SendQueue = new Queue<byte[]>();
            Client = client;
            Stream = client.GetStream();
            ReadBuffer = new byte[1024];
            EndPoint = (IPEndPoint)client.Client.RemoteEndPoint;

            running = true;
            LastTime = DateTime.Now;
            Thread thrRemove = new Thread(RemoveThread);
            thrRemove.IsBackground = true;
            thrRemove.Start();
        }

        byte[] dataBuffer = null;
        int dataBufferOffset = 0;
        object lockKeep = new object();
        DateTime LastTime = DateTime.Now;
        internal int DoReadData(IAsyncResult ar)
        {
            if (!_closed)
            {
                lock (lockKeep)
                {
                    LastTime = DateTime.Now;
                    Monitor.PulseAll(lockKeep);
                }

                int len = Stream.EndRead(ar);

                if (len > 0)
                {
                    int offset = 0;
                    if (len >= SendHeader.Length + 16 + 4) // 16 bytes Guid, 4 byte len int
                    {
                        bool ok = true;
                        for (int i = 0; i < SendHeader.Length; ++i)
                        {
                            if (ReadBuffer[i] != SendHeader[i])
                            {
                                ok = false;
                                break;
                            }
                        }

                        if (ok)
                        {
                            byte[] guidData = new byte[16];
                            Buffer.BlockCopy(ReadBuffer, SendHeader.Length, guidData, 0, guidData.Length);
                            Guid uid = new Guid(guidData);
                            int dataLen = BitConverter.ToInt32(ReadBuffer, SendHeader.Length + 16);
                            dataBuffer = new byte[dataLen];
                            dataBufferOffset = 0;
                            Uid = uid;
                            offset = SendHeader.Length + 16 + 4;
                        }
                    }

                    if (dataBuffer != null)
                    {
                        var readLen = Math.Min(dataBuffer.Length - dataBufferOffset, len - offset);
                        if (readLen > 0)
                        {
                            Buffer.BlockCopy(ReadBuffer, offset, dataBuffer, dataBufferOffset, readLen);
                            dataBufferOffset += readLen;
                            if (dataBufferOffset >= dataBuffer.Length)
                            {
                                try
                                {
                                    var sendFrame = dataBuffer.ByteArrayToObject() as SendFrame;
                                    if (sendFrame != null)
                                    {
                                        if (OnFrame != null)
                                            OnFrame(this, new SendFrameArgs() { Uid = Uid, Frame = sendFrame });
                                    }
                                }
                                catch { }

                                dataBuffer = null;
                                dataBufferOffset = 0;
                            }
                        }
                    }
                }

                return len;
            }

            return 0;
        }

        internal void Close()
        {
            running = false;

            lock(lockKeep)
            {
                Monitor.PulseAll(lockKeep);
            }

            if (!_closed)
            {

                _closed = true;
                try
                {
                    Stream.Close();
                    Client.Close();
                }
                catch { }

                try
                {
                    lock (SendQueue)
                    {
                        SendQueue.Clear();
                    }
                }
                catch { }
            }
        }

        bool running = false;
        void RemoveThread()
        {
            while (running)
            {
                bool remove = false;
                lock (lockKeep)
                {
                    while (running && (DateTime.Now - LastTime).TotalSeconds <= 10)
                        Monitor.Wait(lockKeep, 1000);
                    if (running && (DateTime.Now - LastTime).TotalSeconds > 10)
                        remove = true;
                }

                if(remove)
                {
                    _Server.CloseConnection(this);
                    break;
                }
            }
        }

        internal void Send(byte[] data)
        {
            if(!_closed)
            {
                var sendHeader = new byte[SendHeader.Length + 4];
                Buffer.BlockCopy(SendHeader, 0, sendHeader, 0, SendHeader.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, sendHeader, SendHeader.Length, 4);

                bool doNotify = false;
                lock (SendQueue)
                {
                    SendQueue.Enqueue(sendHeader);
                    if (SendQueue.Count == 1)
                        doNotify = true;
                }

                lock (SendQueue)
                {
                    SendQueue.Enqueue(data);
                    if (SendQueue.Count == 1)
                        doNotify = true;
                }

                if (doNotify)
                    OnGotDataToSend();
            }
        }

        void OnGotDataToSend()
        {
            try
            {
                if (!_closed && GotDataToSend != null)
                    GotDataToSend(this, EventArgs.Empty);
            }
            catch { }
        }

        public bool Connected
        {
            get
            {
                return !_closed && Client.Connected;
            }
        }
    }

    public class TcpServer
    {
        public event EventHandler<SendFrameArgs> OnFrame;

        AsyncCallback readCallback = null;
        AsyncCallback writeCallback = null;
        AsyncCallback acceptCallback = null;

        TcpListener listener = null;
        List<TcpRemoteHostState> clients = null;

        public TcpServer()
        {
            clients = new List<TcpRemoteHostState>();
            acceptCallback = new AsyncCallback(AcceptCallback);
            readCallback = new AsyncCallback(ReadCallback);
            writeCallback = new AsyncCallback(WriteCallback);
        }

        public void Start()
        {
            if (listener != null)
                Stop();

            listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            DoBeginAccept();
        }

        public void Stop()
        {
            if(listener != null)
            {
                try
                {
                    listener.Stop();
                }
                catch { }

                listener = null;
                CloseAll();
            }
        }

        void CloseAll()
        {
            lock(clients)
            {
                foreach(var client in clients)
                {
                    client.Close();
                }

                clients.Clear();
            }
        }

        internal void CloseConnection(TcpRemoteHostState state)
        {
            if(state != null)
            {
                lock(clients)
                {
                    if (clients.Contains(state))
                        clients.Remove(state);
                }

                state.Close();
            }
        }

        public int Port
        {
            get
            {
                return listener != null ? ((IPEndPoint)listener.LocalEndpoint).Port : 0;
            }
        }

        void DoBeginAccept()
        {
            try
            {
                listener.BeginAcceptTcpClient(acceptCallback, null);
            }
            catch
            {
                Stop();
            }
        }

        void AcceptCallback(IAsyncResult ar)
        {
            TcpRemoteHostState state = null;
            bool beginNewAccept = true;

            try
            {
                if (ar.IsCompleted)
                {
                    state = new TcpRemoteHostState(this, listener.EndAcceptTcpClient(ar));
                    state.GotDataToSend += state_GotDataToSend;
                    state.OnFrame += State_OnFrame;
                    state.Stream.BeginRead(state.ReadBuffer, 0, state.ReadBuffer.Length, readCallback, state);
                }
                else
                    beginNewAccept = false;
            }
            catch(SocketException se)
            {
                if (se.SocketErrorCode == SocketError.Interrupted)
                    beginNewAccept = false;
            }
            catch(ObjectDisposedException)
            {
                beginNewAccept = false;
            }
            catch
            {
                if (state != null)
                    CloseConnection(state);
                state = null;
            }

            if(state != null)
            {
                lock (clients)
                    clients.Add(state);
            }

            if (beginNewAccept)
                DoBeginAccept();
        }

        private void State_OnFrame(object sender, SendFrameArgs e)
        {
            if (OnFrame != null)
                OnFrame(sender, e);
        }

        void ReadCallback(IAsyncResult ar)
        {
            TcpRemoteHostState state = (TcpRemoteHostState)ar.AsyncState;

            bool isClose = false;
            try
            {
                if (state.DoReadData(ar) == 0)
                {
                    CloseConnection(state);
                    isClose = true;
                }
            }
            catch (System.IO.IOException ioe)
            {
                CloseConnection(state);
                isClose = true;
            }
            catch { }

            if (!isClose)
                try
                {
                    state.Stream.BeginRead(state.ReadBuffer, 0, state.ReadBuffer.Length, readCallback, state);
                }
                catch { }
        }

        void WriteCallback(IAsyncResult ar)
        {
            TcpRemoteHostState state = (TcpRemoteHostState)ar.AsyncState;

            try
            {
                state.Stream.EndWrite(ar);
            }
            catch (System.IO.IOException ioe)
            {
                CloseConnection(state);
                return;
            }
            catch { }

            bool doSendMore = false;
            lock(state.SendQueue)
            {
                if (state.SendQueue.Count > 0)
                    state.SendQueue.Dequeue();

                if (state.SendQueue.Count > 0)
                    doSendMore = true;
            }

            if (doSendMore)
                DoSend(state);
        }

        void state_GotDataToSend(object sender, EventArgs e)
        {
            TcpRemoteHostState state = (TcpRemoteHostState)sender;
            DoSend(state);
        }

        void DoSend(TcpRemoteHostState state)
        {
            try
            {
                byte[] data = null;
                lock(state.SendQueue)
                {
                    if (state.SendQueue.Count > 0)
                        data = state.SendQueue.Peek();
                }

                if (data != null)
                    state.Stream.BeginWrite(data, 0, data.Length, writeCallback, state);
            }
            catch (System.IO.IOException ioe)
            {
                CloseConnection(state);
            }
            catch { }
        }

        public void Send(Guid uid, byte[] data)
        {
            TcpRemoteHostState state = null;
            lock(clients)
            {
                state = clients.Where(c => c.Uid == uid).FirstOrDefault();
            }

            if (state != null)
                state.Send(data);
        }
    }

    public static class SendFrameServer
    {
        static TcpServer _Server = null;

        public static void Start()
        {
            if (_Server == null)
            {
                _Server = new TcpServer();
                _Server.OnFrame += _Server_OnFrame;
                _Server.Start();
            }
        }

        private static void _Server_OnFrame(object sender, SendFrameArgs e)
        {
            if (OnFrame != null)
                OnFrame(sender, e);
        }

        public static int Port
        {
            get
            {
                if (_Server != null)
                    return _Server.Port;
                return 0;
            }
        }

        public static event EventHandler<SendFrameArgs> OnFrame;

        public static void Send(Guid uid, byte[] data)
        {
            if (_Server != null)
                _Server.Send(uid, data);
        }
    }
    
    public class SendFrameClient
    {
        public event EventHandler OnExit;
        public event EventHandler OnContinue;

        AsyncCallback readCallback = null;
        AsyncCallback writeCallback = null;
        AsyncCallback connectCallback = null;

        TcpClient _Client = null;
        NetworkStream stream = null;
        byte[] ReadBuffer = null;

        Queue<byte[]> SendQueue = null;

        volatile bool _closed = false;

        public bool IsConnected
        {
            get { return !_closed && _Client != null && _Client.Connected; }
        }

        byte[] header = null;
        public SendFrameClient(string host, int port, Guid uid)
        {
            header = new byte[TcpRemoteHostState.SendHeader.Length + 16 + 4];
            Buffer.BlockCopy(TcpRemoteHostState.SendHeader, 0, header, 0, TcpRemoteHostState.SendHeader.Length);
            Buffer.BlockCopy(uid.ToByteArray(), 0, header, TcpRemoteHostState.SendHeader.Length, 16);

            connectCallback = new AsyncCallback(ConnectCallback);
            readCallback = new AsyncCallback(ReadCallback);
            writeCallback = new AsyncCallback(WriteCallback);

            SendQueue = new Queue<byte[]>();

            if (string.IsNullOrWhiteSpace(host))
                host = "localhost";

            _Client = new TcpClient();
            try
            {
                _Client.BeginConnect(host, port, connectCallback, _Client);
            }
            catch(Exception ex)
            {
                Disconnect();

                throw ex;
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            TcpClient client = null;

            try
            {
                client = (TcpClient)ar.AsyncState;
                client.EndConnect(ar);
                client.NoDelay = true;

                stream = client.GetStream();

                bool doNotify = false;
                lock(SendQueue)
                {
                    if (SendQueue.Count > 0)
                        doNotify = true;
                }

                if (doNotify)
                    DoSend();

                ReadBuffer = new byte[1024];
                stream.BeginRead(ReadBuffer, 0, ReadBuffer.Length, readCallback, client);
            }
            catch(Exception ex)
            {
                Disconnect();
            }
        }

        byte[] dataBuffer = null;
        int dataBufferOffset = 0;
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                int len = stream.EndRead(ar);

                if(len == 0)
                {
                    Disconnect();
                    return;
                }
                else
                {
                    int offset = 0;
                    if (len >= TcpRemoteHostState.SendHeader.Length + 4)
                    {
                        bool ok = true;
                        for (int i = 0; i < TcpRemoteHostState.SendHeader.Length; ++i)
                        {
                            if(ReadBuffer[i] != TcpRemoteHostState.SendHeader[i])
                            {
                                ok = false;
                                break;
                            }
                        }

                        if (ok)
                        {
                            int dataLen = BitConverter.ToInt32(ReadBuffer, TcpRemoteHostState.SendHeader.Length);
                            dataBuffer = new byte[dataLen];
                            dataBufferOffset = 0;
                            offset = TcpRemoteHostState.SendHeader.Length + 4;
                        }
                    }

                    if (dataBuffer != null)
                    {
                        len = Math.Min(dataBuffer.Length - dataBufferOffset, len - offset);
                        if (len > 0)
                        {
                            Buffer.BlockCopy(ReadBuffer, offset, dataBuffer, dataBufferOffset, len);
                            dataBufferOffset += len;
                            if (dataBufferOffset >= dataBuffer.Length)
                            {
                                try
                                {
                                    string cmd = Encoding.ASCII.GetString(dataBuffer);
                                    if (cmd == "quit" || cmd == "exit")
                                    {
                                        Disconnect();
                                        return;
                                    }
                                    else if (cmd == "continue")
                                    {
                                        if (OnContinue != null)
                                            OnContinue(this, EventArgs.Empty);
                                    }
                                }
                                catch { }

                                dataBuffer = null;
                                dataBufferOffset = 0;
                            }
                        }
                    }
                }
            }
            catch (System.IO.IOException ioe)
            {
                Disconnect();
                return;
            }
            catch { }

            try
            {
                stream.BeginRead(ReadBuffer, 0, ReadBuffer.Length, readCallback, _Client);
            }
            catch { }
        }

        public void SendFrame(SendFrame frame)
        {
            try
            {
                if (!_closed)
                {
                    var data = frame.ObjectToByteArray();

                    var sendHeader = new byte[header.Length];
                    Buffer.BlockCopy(header, 0, sendHeader, 0, header.Length);
                    Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, sendHeader, TcpRemoteHostState.SendHeader.Length + 16, 4);

                    bool doNotify = false;
                    lock (SendQueue)
                    {
                        SendQueue.Enqueue(sendHeader);
                        if (SendQueue.Count == 1)
                            doNotify = true;
                    }
                    
                    lock (SendQueue)
                    {
                        SendQueue.Enqueue(data);
                        if (SendQueue.Count == 1)
                            doNotify = true;
                    }

                    if (doNotify && stream != null)
                        DoSend();
                }
            }
            catch { }
        }

        void DoSend()
        {
            try
            {
                byte[] data = null;
                lock(SendQueue)
                {
                    if (SendQueue.Count > 0)
                        data = SendQueue.Peek();
                }

                if (data != null)
                    stream.BeginWrite(data, 0, data.Length, writeCallback, _Client);
            }
            catch (System.IO.IOException ioe)
            {
                Disconnect();
            }
            catch { }
        }

        private void WriteCallback(IAsyncResult ar)
        {
            try
            {
                stream.EndWrite(ar);
            }
            catch (System.IO.IOException ioe)
            {
                Disconnect();
                return;
            }
            catch { }

            bool doMoreSend = false;
            if (!_closed)
            {
                lock (SendQueue)
                {
                    if (SendQueue.Count > 0)
                        SendQueue.Dequeue();
                    if (SendQueue.Count > 0)
                        doMoreSend = true;
                }

                if (doMoreSend)
                    DoSend();
            }
        }

        public void Disconnect()
        {
            if (!_closed)
            {
                _closed = true;

                if (stream != null)
                    try
                    {
                        stream.Close();
                    }
                    catch { }

                if (_Client != null)
                    try
                    {
                        _Client.Close();
                        _Client = null;
                    }
                    catch { }

                if (OnExit != null)
                    OnExit(this, EventArgs.Empty);

                lock (SendQueue)
                {
                    SendQueue.Clear();
                }
            }
        }
    }
}
