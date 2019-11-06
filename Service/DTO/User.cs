using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceHost.DTO
{
    [Serializable]
    public class User
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [XmlAttribute]
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [XmlAttribute]
        public string Password { get; set; }

        /// <summary>
        /// Тип учетной записи
        /// </summary>
        [XmlAttribute]
        public EUserType UserType { get; set; }
    }

    /// <summary>
    /// Виды учетных записей пользователя
    /// Тут можно пофантазировать про то что программа-клиент может опросить WCF-сервис: А какие у меня права?
    /// </summary>
    public enum EUserType
    {
        /// <summary>
        /// Админ
        /// </summary>
        Admin,

        /// <summary>
        /// Простой смертный
        /// </summary>
        Simple
    }
}
