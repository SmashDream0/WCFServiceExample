using ServiceHost.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Contracts
{
    [ServiceContract]
    public interface ILoginContract
    {
        /// <summary>
        /// Произвести логин
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Ключ сессии</returns>
        [OperationContract]
        SessionInfo Login(string login, string password);

        /// <summary>
        /// Произвести разлогин
        /// </summary>
        /// <param name="guid">Ключ сессии</param>
        /// <returns>Удалось ли произвести выход</returns>
        [OperationContract]
        bool Logout(string guid);

        /// <summary>
        /// Продлить время действия логина
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [OperationContract]
        bool Prolongate(string guid);
    }
}
