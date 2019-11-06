using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Contracts
{
    [ServiceContract]
    public interface IFileContract
    {
        /// <summary>
        /// Получить файл
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Ответы</returns>
        [OperationContract]
        Byte[] GetFile(string guid, string fileName);
    }
}
