using System;
using System.Collections.Generic;
using System.Text;

namespace LiveFactory.Core.Models.Config
{
    public class AppSettings {
        public UploadConfig UploadConfig { get; set; }
        public Qiniu Qiniu { get; set; }
    }
    public class UploadConfig
    {
        public string DriveFolder { get; set; }
        public string BasePath { get; set; }
        public string ImgFileLength { get; set; }
        public string ImgExts { get; set; }
        public string VideoFileLength { get; set; }
        public string VideoExts { get; set; }
        public string ImageFileName { get; set; }
        public string VideoFileName { get; set; }
    }
    public class Qiniu
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string SpaceName { get; set; }
        public string Domain { get; set; }
    }
}
