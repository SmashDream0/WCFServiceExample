using ServiceHost.Logic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Repository
{
    public class FileRepository: BaseLoggerLogic
    {
        public FileRepository(string path)
        { _path = path; }

        private readonly String _path;

        /// <summary>
        /// Получить файловый поток
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns></returns>
        public Stream GetStream(string fileName)
        {
            Stream result = null;

            var file = Directory.GetFiles(_path, fileName);

            if (file.Any())
            {
                var fileNameWithPath = file.First();

                result = new FileStream(fileNameWithPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            Log($"Получение потока для файла {fileName}. Результат {result != null}");

            return result;
        }
    }
}
