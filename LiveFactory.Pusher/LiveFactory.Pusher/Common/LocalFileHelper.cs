using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace LiveFactory.Pusher
{
    /// <summary>
    /// 本地文件读取
    /// </summary>
    public static class LocalFileHelper
    {
        public const string _localSettings = @"LocalData\LocalSettings.json";
        public const string _commandsFile = @"LocalData\Commands.json";
        private static readonly string _currentDir = null;

        static LocalFileHelper()
        {
            _currentDir = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 根据文件路径返回本地json
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public static string GetLocalJson(string fileLocation)
        {
            var jsonPath = Path.Combine(_currentDir, fileLocation);
            var jsonContent = string.Empty;
            if (File.Exists(jsonPath))
            {
                try
                {
                    using (var streamReader = new StreamReader(jsonPath))
                    {
                        jsonContent = streamReader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return jsonContent;
        }
        public static void SerializationCommands(string jsonCommand)
        {
            var jsonPath = Path.Combine(_currentDir, _commandsFile);
            try
            {
                using (var streamWriter = new StreamWriter(jsonPath, false))
                {
                    streamWriter.Write(jsonCommand);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
