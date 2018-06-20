using System;
using System.IO;
using System.Reflection;

namespace LiveFactory.Pusher
{
    /// <summary>
    /// 本地文件读取
    /// </summary>
    public static class LocalFileReader
    {
        public const string _localSettings = @"LocalData\LocalSettings.json";
        private static readonly string _currentDir = null;

        static LocalFileReader()
        {
            _currentDir = new FileInfo(Uri.UnescapeDataString(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath)).DirectoryName;
        }

        /// <summary>
        /// 根据文件路径返回本地json
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public static string GetLocalJson(string fileLocation)
        {
            var jsonPath = Path.Combine(_currentDir, fileLocation);
            StreamReader streamReader = null;
            string jsonContent = null;
            try
            {
                using (streamReader = new StreamReader(jsonPath))
                {
                    jsonContent = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
            }
            return jsonContent;
        }
    }
}
