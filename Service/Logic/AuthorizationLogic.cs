using ServiceHost.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Logic
{
    public class AuthorizationLogic: BaseLoggerLogic
    {
        public AuthorizationLogic(
            Boolean allowMultyLogin,
            Int32 LoginTimeoutSec,
            Repository.UserRepository userRepository, 
            Repository.AuthorizationRepository authorizationRepository, 
            HashComparerLogic hashComparerLogic)
        {
            _allowMultyLogin = allowMultyLogin;
            _loginTimeOut = LoginTimeoutSec;
            _userRepository = userRepository;
            _authorizationRepository = authorizationRepository;
            _hashComparerLogic = hashComparerLogic;
        }

        private readonly Boolean _allowMultyLogin;
        private readonly Int32 _loginTimeOut;
        private readonly Repository.UserRepository _userRepository;
        private readonly Repository.AuthorizationRepository _authorizationRepository;
        private readonly HashComparerLogic _hashComparerLogic;

        /// <summary>
        /// Произвести авторизацию
        /// </summary>
        /// <param name="login"></param>
        /// <param name="incomeHash"></param>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        public Boolean Authorize(string login, string incomeHash, out SessionInfo sessionInfo)
        {
            _authorizationRepository.RemoveIfOutdated(DateTime.Now.AddSeconds(-_loginTimeOut));

            var userPassword = _userRepository.GetPassword(login);

            Boolean result = !String.IsNullOrEmpty(userPassword);

            var guid = String.Empty;
            String message;

            if (result)
            {
                result = _allowMultyLogin || !_authorizationRepository.ContainsLogin(login);

                if (result)
                {
                    result = _hashComparerLogic.Compare(incomeHash, userPassword);

                    if (result)
                    {
                        guid = Guid.NewGuid().ToString();

                        result = _authorizationRepository.Add(login, guid);

                        if (result)
                        { message = "Успешная авторизация"; }
                        else
                        {
                            guid = String.Empty;
                            message = String.Empty;
                        }
                    }
                    else
                    { message = "Логин/пароль не верный"; }
                }
                else
                { message = "Логин запрещен"; }
            }
            else
            { message = "Логин/пароль не верный"; }

            sessionInfo = new SessionInfo()
            {
                Guid = guid,
                TimeoutSec = _loginTimeOut,
                Message = message,
            };

            Log($"Попытка авторизации. Логин {login}, хеш {incomeHash}. Результат {result}");

            return result;
        }

        public Boolean UnAuthorize(String guid)
        {
            _authorizationRepository.RemoveIfOutdated(DateTime.Now.AddSeconds(-_loginTimeOut));

            return _authorizationRepository.Remove(guid);
        }

        /// <summary>
        /// Проверить гуид
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Boolean IsAuthrorized(string guid)
        {
            _authorizationRepository.RemoveIfOutdated(DateTime.Now.AddSeconds(-_loginTimeOut));

            var authorization = _authorizationRepository.ContainsGuid(guid);

            var result = authorization.Date.AddSeconds(_loginTimeOut) < DateTime.Now;

            Log($"Проверка действия авторизации {guid}. Результат {result}");

            return result;
        }

        /// <summary>
        /// Продлить жизнь авторизации
        /// </summary>
        /// <param name="guid"></param>
        public bool Prolongate(string guid)
        {
            _authorizationRepository.RemoveIfOutdated(DateTime.Now.AddSeconds(-_loginTimeOut));

            var result = _authorizationRepository.Update(guid);

            return result;
        }
    }
}