using NLog;

namespace LiveFactory.Pusher
{
    public static class LogHelper
    {
        private static Logger _logger;
        static LogHelper()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
        public static void SetLogError(string logInfo)
        {
            _logger.Error(logInfo);
        }
        public static void SetLogInfo(string logInfo)
        {
            _logger.Info(logInfo);
        }
    }
}
