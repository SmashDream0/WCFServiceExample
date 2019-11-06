using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Logic
{
    public class ConfigurationLogic
    {
        private static readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        private static T GetValue<T>(string name, T defaultValue = default(T))
        {
            if (!_values.ContainsKey(name))
            {
                var value = DirectGetValue(name);

                if (value == null)
                { value = defaultValue; }
                else
                { value = Convert.ChangeType(value, typeof(T)); }

                _values.Add(name, value);
            }

            return (T)_values[name];
        }

        private static string GetValue(string name, string defaultValue = "")
        {
            var value = GetValue<string>(name, defaultValue);

            return value;
        }

        private static object DirectGetValue(string name)
        { return ConfigurationManager.AppSettings[name]; }

        /// <summary>
        /// Имя сервиса
        /// </summary>
        public String ServiceName => GetValue("serviceName");

        /// <summary>
        /// Время жизни сесси логина в секундах
        /// </summary>
        public Int32 LoginTimeoutSec => GetValue<Int32>("LoginTimeoutSec", 10);

        /// <summary>
        /// Разрешить множественный логин
        /// </summary>
        public Boolean AllowMultyLogin => GetValue<Boolean>("allowMultyLogin");

        /// <summary>
        /// Путь до папки с файлами
        /// </summary>
        public String FilePath => GetValue("filePath", "\\");
    }
}
