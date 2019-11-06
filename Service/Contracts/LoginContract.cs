using ServiceHost.DTO;
using ServiceHost.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Contracts
{
    public class LoginContract : BaseLoggerLogic, ILoginContract
    {
        static LoginContract()
        {
            _loginLogic = Context.AuthorizationLogic;
        }

        private static Logic.AuthorizationLogic _loginLogic;

        /// <summary>
        /// Произвести логин
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Ключ сесии</returns>
        public SessionInfo Login(string login, string password)
        {
            SessionInfo sessionInfo;

            var result = _loginLogic.Authorize(login, password, out sessionInfo);

            return sessionInfo;
        }

        /// <summary>
        /// Произвести выход
        /// </summary>
        /// <param name="guid">Ключ сессии</param>
        /// <returns>Удалось ли произвести выход</returns>
        public bool Logout(string guid)
        {
            return _loginLogic.UnAuthorize(guid);
        }

        public bool Prolongate(string guid)
        {
            return _loginLogic.Prolongate(guid);
        }
    }
}