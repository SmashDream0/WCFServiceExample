using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Logic
{
    public class HashComparerLogic
    {
        /// <summary>
        /// Создать экземпляр
        /// </summary>
        /// <returns></returns>
        public static HashComparerLogic Create()
        { return new HashComparerLogic(SHA512.Create(), Encoding.UTF32); }

        protected HashComparerLogic(HashAlgorithm hasher, Encoding encoding)
        {
            _hasher = hasher;
            _encoding = encoding;
        }

        private readonly HashAlgorithm _hasher;
        private readonly Encoding _encoding;

        /// <summary>
        /// Сравнить строку
        /// </summary>
        /// <param name="incomeHash"></param>
        /// <returns></returns>
        public Boolean Compare(string incomeHash, string password)
        {
            var base64Hash = GetHash(password);

            return String.Equals(base64Hash, incomeHash);
        }

        /// <summary>
        /// Получить хеш
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetHash(string password)
        {
            var bytes = _encoding.GetBytes(password);

            var hash = _hasher.ComputeHash(bytes);

            var base64Hash = Convert.ToBase64String(hash);

            return base64Hash;
        }
    }
}