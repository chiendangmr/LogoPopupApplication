namespace VTVPCDLogoPopupApplication.Objects
{
    public class FileList
    {
        public bool Use { get; set; }        
        public string FilePath { get; set; }           
    }
    public class PlayList
    {
        public bool Use { get; set; }
        public string GioPhat { get; set; }
        public string Duration { get; set; }
        public string FileName { get; set; }  
        public int Layer { get; set; }     
    }
    public class AdSequence
    {
        public string StartTime { get; set; }
        public lstAdd lstAddObj
        {
            get; set;
        }
    }
    public class lstAdd
    {
        public AdObject adObj { get; set; }
    }
    public class AdObject
    {
        public string FileName { get; set; }
        public long Duration { get; set; }
        public string State { get; set; }
    }
}
