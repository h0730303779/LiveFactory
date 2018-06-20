namespace LiveFactory.Pusher
{
    public class LocalSettings
    {
        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 工厂代码
        /// </summary>
        public string FactoryCode { get; set; }

        /// <summary>
        /// The IP address of the server
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// The port of the server to Request 
        /// </summary>
        public string RequestPort { get; set; }
        /// <summary>
        /// The port of the server to Receive
        /// </summary>
        public string ReceivePort { get; set; }
        /// <summary>
        /// Read settings from local file.
        /// </summary>
        /// <returns></returns>
        private int _loopCheckInterval = 2;

        public int LoopCheckInterval
        {
            get { return _loopCheckInterval; }
            set { _loopCheckInterval = value; }
        }

        public static LocalSettings GetLocalSettings()
        {
            LocalSettings localSettings = null;
            localSettings = JsonHelper.ParseJson<LocalSettings>(LocalFileHelper.GetLocalJson(LocalFileHelper._localSettings));
            return localSettings;
        }
    }
}
