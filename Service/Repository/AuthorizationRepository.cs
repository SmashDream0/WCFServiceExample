using ServiceHost.Logic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Repository
{
    public class AuthorizationRepository: BaseLoggerLogic
    {
        public AuthorizationRepository()
        {
            _authorizations = new ConcurrentDictionary<String, Authorization>();
            _usedLogins = new ConcurrentDictionary<String, Int32>();
        }

        private readonly IDictionary<String, Authorization> _authorizations;
        private readonly IDictionary<String, Int32> _usedLogins;

        /// <summary>
        /// Получить данные авторизации
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Authorization ContainsGuid(string guid)
        {
            Authorization result;

            var checkResult = _authorizations.TryGetValue(guid, out result);

            Log($"Проверка наличия авторизации {guid}, результат {checkResult}");

            return result;
        }

        /// <summary>
        /// Была ли произведена авторизация логина
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Boolean ContainsLogin(string login)
        {
            var result = _usedLogins.ContainsKey(login);

            Log($"Проверка использованного логина {login}, результат {result}");

            return result;
        }

        /// <summary>
        /// Произвести авторизацию
        /// </summary>
        /// <param name="login"></param>
        /// <param name="guid"></param>
        public Boolean Add(string login, string guid)
        {
            Boolean result = Authorization.Equals(ContainsGuid(guid), default(Authorization));

            if (result)
            {
                var newLogin = new Authorization()
                {
                    Login = login,
                    Guid = guid,
                    Date = DateTime.Now,
                };

                _authorizations.Add(guid, newLogin);

                if (!_usedLogins.ContainsKey(login))
                { _usedLogins.Add(login, 0); }

                _usedLogins[login]++;
            }

            Log($"Добавление авторизации {login} - {guid}, результат {result}");

            return result;
        }

        /// <summary>
        /// Удалить авторизацию
        /// </summary>
        /// <param name="guid"></param>
        public Boolean Remove(string guid)
        {
            bool result = false;

            if (_authorizations.ContainsKey(guid))
            {
                var authorization = _authorizations[guid];

                _authorizations.Remove(guid);

                if (_usedLogins[authorization.Login] == 1)
                { _usedLogins.Remove(authorization.Login); }
                else
                { _usedLogins[authorization.Login]--; }

                result = true;
            }

            Log($"Уничтожение авторизации {guid}, результат {result}");

            return result;
        }

        /// <summary>
        /// Продлить время действия логина
        /// </summary>
        /// <param name="guid"></param>
        public Boolean Update(string guid)
        {
            Boolean result = _authorizations.ContainsKey(guid);

            if (result)
            {
                var authorization = _authorizations[guid];
                authorization.Date = DateTime.Now;

                _authorizations[guid] = authorization;
            }

            Log($"Продление авторизации {guid}, результат {result}");

            return result;
        }

        /// <summary>
        /// Удалить старые авторизации
        /// </summary>
        /// <param name="dt">Дата, от которой считать актуальность</param>
        public void RemoveIfOutdated(DateTime dt)
        {
            var removed = 0;
            var authorizations = _authorizations.ToArray();

            foreach (var authorization in authorizations)
            {
                if (authorization.Value.Date < dt)
                {
                    _authorizations.Remove(authorization.Key);

                    if (_usedLogins[authorization.Value.Login] == 1)
                    { _usedLogins.Remove(authorization.Value.Login); }
                    else
                    { _usedLogins[authorization.Value.Login]--; }

                    removed++;
                }
            }

            Log($"Уничтожение не используемых авторизаций Уничтожено: {removed}");
        }
    }

    public struct Authorization
    {
        public string Login { get; set; }
        public string Guid { get; set; }
        public DateTime Date { get; set; }
    }
}