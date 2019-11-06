using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServiceHost.Contracts;

namespace ServiceHost
{
    public class Service : ServiceBase
    {
        public Service()
        {
            var configuration = new Configuration(52530);

            LoginHost = configuration.CreateServiceHost(typeof(LoginContract));
            LoginHost.Open();

            FileHost = configuration.CreateServiceHost(typeof(FileContract));
            FileHost.Open();
        }

        /// <summary>
        /// Хост для контракта логина
        /// </summary>
        public System.ServiceModel.ServiceHost LoginHost { get; private set; }

        /// <summary>
        /// Хост для контракта отправки файлов
        /// </summary>
        public System.ServiceModel.ServiceHost FileHost { get; private set; }

        /// <summary>
        /// Запуск
        /// </summary>
        public void Start()
        { base.OnStart(new string[0]); }

        /// <summary>
        /// Остановка
        /// </summary>
        public new void Stop()
        { base.OnStop(); }
    }
}
