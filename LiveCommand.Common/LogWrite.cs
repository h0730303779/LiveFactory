using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveCommand.Common
{
    public static class LogWrite
    {
        public static void Error(string meg)
        {
            try
            {
                Error(meg, "");
            }
            catch (Exception ex)
            {

            }
        }
        public static void Error(Exception ex)
        {
            try
            {
                Error(ex.ToString());
            }
            catch (Exception ex2)
            {

            }
        }
        public static void Error(string meg, string code = "")
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "/logs_netmq/";
                string filename = DateTime.Now.ToString("yyyy-MM-dd");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                meg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + meg + "\r\n";
                System.IO.File.AppendAllText(path + "mq" + code + "-" + filename + ".log", meg);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
