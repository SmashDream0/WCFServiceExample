using ServiceHost.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceHost
{
    public class Configuration
    {
        public Configuration(Int32 port)
        {
            Port = port;
            TimeoutSec = 5;
            IsDebug = false;
        }

        /// <summary>
        /// Порт
        /// </summary>
        public Int32 Port
        { get; private set; }

        /// <summary>
        /// Таймаут в секундах
        /// </summary>
        public Int32 TimeoutSec
        { get; private set; }

        /// <summary>
        /// Режим отладки?
        /// </summary>
        public Boolean IsDebug
        { get; set; }

        private Binding GetBinding()
        {
            var binding = new NetTcpBinding();

            var timeSpan = GetTimeSpan();

            binding.CloseTimeout = timeSpan;
            binding.OpenTimeout = timeSpan;
            binding.ReceiveTimeout = timeSpan;
            binding.SendTimeout = timeSpan;

            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message = new MessageSecurityOverTcp()
            {
                ClientCredentialType = MessageCredentialType.None,
                AlgorithmSuite = SecurityAlgorithmSuite.TripleDesSha256,
            };

            binding.ReliableSession.Enabled = true;
            binding.ReliableSession.InactivityTimeout = timeSpan;

            return binding;
        }

        private TimeSpan GetTimeSpan()
        {
            var seconds = TimeoutSec % 60;
            var totalMinutes = (TimeoutSec - seconds) / 60;
            var minutes = totalMinutes % 60;
            var hours = (totalMinutes - minutes) / 60;

            var result = new TimeSpan(hours, minutes, seconds);

            return result;
        }
        
        public System.ServiceModel.ServiceHost CreateServiceHost(Type typeContract)
        {
            Type contractInterface = typeContract.GetInterfaces().First();
            var contractInterfaceName = contractInterface.FullName;
            var subServiceName = GetSubServiceName(contractInterfaceName);

            var serviceHost = new System.ServiceModel.ServiceHost(typeContract, new Uri($"net.tcp://{Environment.MachineName}:{Port}/{subServiceName}", UriKind.RelativeOrAbsolute));

            EndpointAddress endpointAdress = null;

            var binding = GetBinding();

            var endpoint = new ServiceEndpoint(new ContractDescription(contractInterfaceName), binding, endpointAdress);

            serviceHost.SetEndpointAddress(endpoint, String.Empty);
            
            serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = IsDebug });

            return serviceHost;
        }

        private String GetSubServiceName(String interfaceName)
        {
            var gx = new Regex(".*I(\\w+)Contract");
            var matches = gx.Matches(interfaceName);
            var result = matches[0].Groups[1].Value;

            return result;
        }
    }
}