using ServiceHost.Contracts;
using ServiceHost.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sender.MVVM
{
    public class ClientModel
    {
        private Timer _timer;

        /// <summary>
        /// Продлить время действия сессии
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool UpdateSession(string guid)
        {
            var loginClient = GetLoginChannel();

            var result = loginClient.Prolongate(guid);

            return result;
        }

        /// <summary>
        /// Запустить автоматическое обновлении сессии
        /// </summary>
        /// <param name="updateIntervalSec"></param>
        /// <param name="guid"></param>
        public void StartRegularSessionUpdate(int updateIntervalSec, string guid)
        {
            StopRegularSessionUpdate();

            _timer = new Timer(UpdateTimer, guid, 0, updateIntervalSec * 1000);
        }

        /// <summary>
        /// Остановить атвоматическое обновление сессии
        /// </summary>
        public void StopRegularSessionUpdate()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Произвести логин
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="sessionInfo">Ключ сессии</param>
        /// <param name="error">Ошибка</param>
        /// <returns></returns>
        public bool Login(string login, string password, out SessionInfo sessionInfo)
        {
            bool result = false;

            try
            {
                if (String.IsNullOrEmpty(password))
                {
                    sessionInfo = new SessionInfo() { Message = "Строка пароля пустая" };
                }
                else
                {
                    var hash = ServiceHost.Logic.HashComparerLogic.Create().GetHash(password);

                    var loginClient = GetLoginChannel();
                    sessionInfo = loginClient.Login(login, hash);

                    result = !String.IsNullOrEmpty(sessionInfo.Guid);
                }
            }
            catch (Exception ex)
            {
                sessionInfo = new SessionInfo() { Message = ex.Message };
            }

            return result;
        }

        /// <summary>
        /// Произвести разлогин
        /// </summary>
        /// <param name="guid">Ключ сессии</param>
        public bool Unlogin(string guid, out string error)
        {
            bool result = false;

            try
            {
                var loginClient = GetLoginChannel();
                result = loginClient.Logout(guid);
                error = String.Empty;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Продлить действие логина
        /// </summary>
        /// <param name="guid">Ключ сессии</param>
        public bool Prolongate(string guid, out string error)
        {
            bool result = false;

            try
            {
                var loginClient = GetLoginChannel();
                result = loginClient.Logout(guid);
                error = String.Empty;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Получить ответ
        /// </summary>
        /// <param name="guid">Ключ сессии</param>
        /// <param name="request">Запрос</param>
        /// <returns></returns>
        public byte[] GetFile(string guid, string fileName, out string error)
        {
            byte[] result = null;

            try
            {
                var fileClient = GetFileChannel();

                result = fileClient.GetFile(guid, fileName);

                error = String.Empty;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return result;
        }


        private ILoginContract GetLoginChannel()
        {
            ChannelFactory<ILoginContract> httpFactory =
             new ChannelFactory<ILoginContract>(
               new NetTcpBinding() ,
               new EndpointAddress("net.tcp://localhost:52530/Login"));

            ILoginContract tcpProxy =
              httpFactory.CreateChannel();

            return tcpProxy;
        }
        private IFileContract GetFileChannel()
        {
            ChannelFactory<IFileContract> httpFactory =
             new ChannelFactory<IFileContract>(
               new NetTcpBinding(),
               new EndpointAddress("net.tcp://localhost:52530/File"));

            IFileContract tcpProxy =
              httpFactory.CreateChannel();

            return tcpProxy;
        }

        private void UpdateTimer(object state)
        {
            var guid = state as string;

            var result = UpdateSession(guid);

            if (!result)
            { StopRegularSessionUpdate(); }
        }
    }
}