using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ProCode.PlusHosting.IpUpdate.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if !DEBUG
            // This is original release code!
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PlusHostingIpUpdateService()
            };
            ServiceBase.Run(ServicesToRun);
#else
            // This is debug auxiliary code.
            var debugService = new PlusHostingIpUpdateService();
            debugService.OnStartDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);   // To keep service alive.
            //System.Threading.Thread.Sleep(10000);   // To keep service alive.
#endif
        }
    }
}
