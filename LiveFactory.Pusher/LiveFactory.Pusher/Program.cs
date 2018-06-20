using System.ServiceProcess;
using System.Windows.Forms;

namespace LiveFactory.Pusher
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new JFJTLiveService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
