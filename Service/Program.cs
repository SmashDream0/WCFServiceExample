using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceInstance = new Service();

            if (args.Length == 0)
            {
                var servicesToRun = new ServiceBase[] { ServiceInstance };

                ServiceBase.Run(servicesToRun);
            }
            else if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "install":
                        {
                            ProjectInstaller.InstallService();
                            ProjectInstaller.StartService();
                            break;
                        }
                    case "uninstall":
                        {
                            ProjectInstaller.StopService();
                            ProjectInstaller.UninstallService();
                            break;
                        }
                    case "console":
                        {
                            // если запустили с параметром console то запускаем как консольное окно                            
                            {
                                ServiceInstance.Start();

                                Console.WriteLine($"Service \"{Context.ConfigurationLogic.ServiceName}\"");
                                Console.WriteLine($"Login started at adress \"{String.Join(",", ServiceInstance.LoginHost.BaseAddresses.Select(x => x.AbsoluteUri))}\"");
                                Console.WriteLine($"File started at adress \"{String.Join(",", ServiceInstance.FileHost.BaseAddresses.Select(x => x.AbsoluteUri))}\"");
                                Console.WriteLine("Press <Enter> to stop the service.");

                                Console.ReadLine();

                                ServiceInstance.Stop();
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public static Service ServiceInstance { get; private set; }
    }
}
