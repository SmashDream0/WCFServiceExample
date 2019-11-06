using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceHost.Repository
{
    public class UserRepository
    {
        public UserRepository()
        { Initialize(); }

        private void Initialize()
        {
            _users = Logic.SerializerLogic.Deserialize<DTO.User>().ToList();
        }

        private List<DTO.User> _users;

        /// <summary>
        /// Проверить существование логина
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        public bool LoginExist(string login)
        {
            return _users.Exists(x => 
                        String.Equals(x.Login, login, StringComparison.InvariantCultureIgnoreCase));  //пароль чувствителен к регистру
        }

        /// <summary>
        /// Получить пароль
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public string GetPassword(string login)
        { return _users.FirstOrDefault(x => String.Equals(x.Login, login))?.Password; }
    }
}
