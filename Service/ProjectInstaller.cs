using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ServiceHost
{
    /// <summary>
    ///     Инсталятор службы
    /// </summary>
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        /// <summary>
        ///     Имя сервиса
        /// </summary>
		private const string _serviceName = "ServiceHost";

        /// <summary>
        /// Имя сервиса, если оно задано в config-файле, то возвращается имя из config-файла, иначе значение по умолчанию: MegatecCalculateTourBalanser
        /// </summary>
        public static string ServiceName
        {
            get
            {
                try
                {
                    var serviceName = ServiceHost.Context.ConfigurationLogic.ServiceName;

                    if (string.IsNullOrEmpty(serviceName))
                    {
                        return _serviceName;
                    }
                    return serviceName;
                }
                catch (System.Configuration.ConfigurationErrorsException)
                {
                    return _serviceName;
                }
            }
        }

        private readonly ServiceProcessInstaller _process;
        private readonly ServiceInstaller _service;

        public ProjectInstaller()
        {
            _process = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };

            _service = new ServiceInstaller { ServiceName = ServiceName, StartType = ServiceStartMode.Automatic };

            Installers.Add(_process);

            Installers.Add(_service);
        }

        /// <summary>
        ///     Проверяет установлена ли служба или нет
        /// </summary>
        /// <returns></returns>
        private static bool IsInstalled()
        {
            using (var controller = new ServiceController(ServiceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        ///     Проверяет запущена ли служба или нет
        /// </summary>
        /// <returns></returns>
        private static bool IsRunning()
        {
            using (var controller = new ServiceController(ServiceName))
            {
                if (!IsInstalled()) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        /// <summary>
        ///     Возвращает инсталятор
        /// </summary>
        /// <returns></returns>
        private static AssemblyInstaller GetInstaller()
        {
            var installer = new AssemblyInstaller(typeof(Service).Assembly, null) { UseNewContext = true };
            return installer;
        }

        /// <summary>
        ///     Устанавливает  службу
        /// </summary>
        internal static void InstallService()
        {
            if (IsInstalled())
            {
                return;
            }

            using (AssemblyInstaller installer = GetInstaller())
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                }
                catch
                {
                    installer.Rollback(state);
                    throw;
                }
            }
        }

        /// <summary>
        ///     Удаляет службу
        /// </summary>
        internal static void UninstallService()
        {
            if (!IsInstalled())
            {
                return;
            }

            using (AssemblyInstaller installer = GetInstaller())
            {
                IDictionary state = new Hashtable();
                installer.Uninstall(state);
            }
        }

        /// <summary>
        ///     Запускает службу
        /// </summary>
        internal static void StartService()
        {
            if (!IsInstalled())
            {
                return;
            }

            using (var controller = new ServiceController(ServiceName))
            {
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running,
                        TimeSpan.FromSeconds(100));
                }
            }
        }

        /// <summary>
        ///     Останавливает службу
        /// </summary>
        internal static void StopService()
        {
            if (!IsInstalled())
            {
                return;
            }

            using (var controller = new ServiceController(ServiceName))
            {
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped,
                        TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}