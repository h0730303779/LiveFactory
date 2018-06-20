using LiveFactory.Core.Models.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveFactory.Core
{
    public enum FileType
    {
        /// <summary>
        ///图片
        /// </summary>
        image = 1,
        /// <summary>
        /// 视频
        /// </summary>
        video = 2
    }
    public class FileInfoResult
    {
        public FileInfoResult()
        {

        }
        public FileInfoResult(string meg)
        {
            this.Success = false;
            this.Message = meg;
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = true;
        /// <summary>
        /// 文件类型异常消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 保存的文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 1=image,2=video
        /// </summary>
        public FileType FileType { get; set; }
    }
    public class SaveFileResult
    {
        public string SaveDriveFileName { get; set; }
        public string SaveFileName { get; set; }
        public string SaveDirectory { get; set; }
    }
    public class FilesBaseHelper
    {
        private readonly IOptions<AppSettings> _appSettings;
        public FilesBaseHelper(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
        }
        #region 需要参数
        /// <summary>
        /// 保存的盘符根目录
        /// </summary>
        private string DriveFolder
        {
            get
            {
                return _appSettings.Value.UploadConfig.DriveFolder;
            }
        }
        /// <summary>
        /// 文件根目录名称
        /// </summary>
        private string BasePath
        {
            get
            {
                return _appSettings.Value.UploadConfig.BasePath;
            }
        }
        private string ImgFileLength
        {
            get
            {
                return _appSettings.Value.UploadConfig.ImgFileLength;
            }
        }
        private string ImgExts
        {
            get
            {
                return _appSettings.Value.UploadConfig.ImgExts;
            }
        }
        private string VideoFileLength
        {
            get
            {
                return _appSettings.Value.UploadConfig.VideoFileLength;
            }
        }
        private string VideoExts
        {
            get
            {
                return _appSettings.Value.UploadConfig.VideoExts;
            }
        }
        private string ImageFileName
        {
            get
            {
                return _appSettings.Value.UploadConfig.ImageFileName;
            }
        }
        private string VideoFileName
        {
            get
            {
                return _appSettings.Value.UploadConfig.VideoFileName;
            }
        }
        #endregion
        private string GetFileTypeFolder(FileType filetype, bool IsBase = false)
        {
            string _path = IsBase ? BasePath : DriveFolder;
            switch (filetype)
            {
                case FileType.image:
                    _path += ImageFileName;
                    break;
                case FileType.video:
                    _path += VideoFileName;
                    break;
            }
            return _path;
        }


        public SaveFileResult GetSaveFilePath(FileType fileType, string fileName)
        {
            string _folder = DateTime.Now.ToString("yyyy-MM") + "/";
            string driveFolder = GetFileTypeFolder(fileType) + _folder;
            string baseFolder = GetFileTypeFolder(fileType, true) + _folder;

            string bas =Path.Combine(Directory.GetCurrentDirectory() , "wwwroot");//获取服务器目录
            if (!Directory.Exists(bas + baseFolder))
            {
                Directory.CreateDirectory(bas + baseFolder);
            }

            //Abp.IO.DirectoryHelper.CreateIfNotExists(driveFolder);

            string savefilename = Guid.NewGuid() + GetExtensionName(fileName);
            SaveFileResult result = new SaveFileResult()
            {
                SaveDriveFileName = driveFolder + savefilename,
                SaveFileName = baseFolder + savefilename,
                SaveDirectory= bas + baseFolder + savefilename
            };
            return result;
        }

        public string GetExtensionName(string fileName)
        {
            return Path.GetExtension(fileName);
        }
        public bool Validate(string fileName, long fileSize, FileType filetype, out string Error)
        {
            Error = "";
            string extensValue = "";
            long size = 0;
            switch (filetype)
            {
                case FileType.image:
                    extensValue = ImgExts;
                    size = Convert.ToInt32(ImgFileLength) * 1024;
                    break;
                case FileType.video:
                    extensValue = VideoExts;
                    size = Convert.ToInt32(VideoFileLength) * 1024;
                    break;
            }
            if (!ValidateeExtension(extensValue, fileName))
            {
                Error = "文件类型错误";
                return false;
            }
            if (!ValidateeSize(size, fileSize))
            {
                Error = "文件大小不可超过" + size / 1024 + "KB";
                return false;
            }
            return true;
        }
        bool ValidateeExtension(string ExtensionValue, string fileName)
        {
            string ext = GetExtensionName(fileName).ToLower();
            return ExtensionValue.ToLower().Contains(ext);
        }
        bool ValidateeSize(long Size, long fileSize)
        {
            return fileSize <= Size;
        }
    }
}
