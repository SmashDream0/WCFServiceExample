using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Xml.Serialization;

namespace ServiceHost.Logic
{
    public static class SerializerLogic
    {
        /// <summary>
        /// Считать данные из источника
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Deserialize<T>()
        {
            var fileName = GetFileName<T>();
            var formatter = GetSerializer<T>();

            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var result = formatter.Deserialize(fs) as IEnumerable<T>;

                return result;
            }
        }

        /// <summary>
        /// Сохранить данные в источнике
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <returns></returns>
        public static void Serialize<T>(IEnumerable<T> items)
        {
            var fileName = GetFileName<T>();
            var formatter = GetSerializer<T>();

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.Read))
                {
                    ///Тут происходить полная перезапись данных в файле
                    ///Вообще-то это ни разу не оптимально
                    ///Однако для демонстрации этого вполне хватит
                    fs.SetLength(0);

                    formatter.Serialize(fs, items.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static XmlSerializer GetSerializer<T>()
        {
            var formatter = new XmlSerializer(typeof(T[]), new XmlRootAttribute("Items"));

            return formatter;
        }

        private static string GetFileName<T>()
        {
            var fileName = $@"{AppDomain.CurrentDomain.BaseDirectory}DATA\{typeof(T).Name}.xml";

            return fileName;
        }
    }
}
