using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost
{
    public static class Context
    {
        static Context()
        {
            ConfigurationLogic = new Logic.ConfigurationLogic();
            
            var absolutePath = ConfigurationLogic.FilePath;

            if (absolutePath.StartsWith("\\"))
            {
                absolutePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + absolutePath;
            }

            UserRepository = new Repository.UserRepository();
            AuthorizationRepository = new Repository.AuthorizationRepository();
            FileRepository = new Repository.FileRepository(absolutePath);

            var hashComparer = Logic.HashComparerLogic.Create();
            AuthorizationLogic = new Logic.AuthorizationLogic(
                ConfigurationLogic.AllowMultyLogin,
                ConfigurationLogic.LoginTimeoutSec, 
                UserRepository, 
                AuthorizationRepository, 
                hashComparer);
        }

        /// <summary>
        /// Репозиторий пользователей
        /// </summary>
        public static Repository.UserRepository UserRepository
        { get; private set; }

        /// <summary>
        /// Репозиторий файлов
        /// </summary>
        public static Repository.FileRepository FileRepository
        { get; private set; }

        /// <summary>
        /// Репозиторий авторизаций
        /// </summary>
        public static Repository.AuthorizationRepository AuthorizationRepository
        { get; private set; }

        /// <summary>
        /// Логика авторизаций
        /// </summary>
        public static Logic.AuthorizationLogic AuthorizationLogic
        { get; private set; }

        /// <summary>
        /// Список настроек конфига
        /// </summary>
        public static Logic.ConfigurationLogic ConfigurationLogic
        { get; private set; }
    }
}
