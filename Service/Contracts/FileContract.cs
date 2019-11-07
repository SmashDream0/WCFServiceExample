using ServiceHost.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Contracts
{
    public class FileContract : BaseLoggerLogic, IFileContract
    {
        public FileContract()
        {
            _fileRepository = Context.FileRepository;
            _authorizationLogic = Context.AuthorizationLogic;
        }

        private void Initialize()
        {

        }

        private readonly Repository.FileRepository _fileRepository;
        private readonly Logic.AuthorizationLogic _authorizationLogic;

        /// <summary>
        /// Добавить вопрос-ответ
        /// </summary>
        /// <param name="guid">Ключ сессии</param>
        /// <param name="question">Вопрос</param>
        /// <param name="answer">Ответ</param>
        /// <returns>Удалось ли добавить</returns>
        public byte[] GetFile(string guid, string fileName)
        {
            Log("Попытка получения файла " + fileName);

            if (String.IsNullOrEmpty(guid) || !_authorizationLogic.IsAuthrorized(guid))
            { throw new Exception("Операция не допустима"); }

            Log(String.Concat("Чтение файла ", fileName, " с диска "));
            var stream = _fileRepository.GetStream(fileName);

            if (stream == null)
            { throw new Exception("Файл не найден"); }

            Log("Продление действия авторизации " + guid);
            _authorizationLogic.Prolongate(guid);

            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            return bytes;
        }
    }
}
