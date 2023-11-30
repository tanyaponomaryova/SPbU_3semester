using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.ComponentModel;

namespace CheckSum
{
    public static class CheckSumEvaluation
    {
        private static byte[] ComputeFileCheckSum(string path)
        {
            using MD5 md5 = MD5.Create();
            var fileStream = File.OpenRead(path);
            var hash = md5.ComputeHash(fileStream);
            return hash;
        }

        /// <summary>
        /// Computes directory check sum in single thread.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static byte[] ComputeSingleThreaded(string path)
        {
            if (Directory.Exists(path))
            {
                var entries = Directory.EnumerateFileSystemEntries(path).Order();
                var value = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path)!);

                foreach( var entry in entries) 
                {
                    value = value.Concat(ComputeSingleThreaded(entry)).ToArray();
                }

                using MD5 md5 = MD5.Create();
                return md5.ComputeHash(value);
            }
            else if (File.Exists(path))
            {
                 return ComputeFileCheckSum(path);
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        /// <summary>
        /// Computes directory check sum in multiple threads.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static byte[] ComputeMultiThreaded(string path)
        {
            if (Directory.Exists(path))
            {
                var entries = Directory.EnumerateFileSystemEntries(path).Order();
                var dict = new Dictionary<string, byte[]>();
                foreach ( var entry in entries)
                {
                    dict.Add(entry, Array.Empty<byte>());
                }

                var value = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path)!);

                Parallel.ForEach(entries, entry =>
                {
                    dict[entry] = ComputeMultiThreaded(entry);
                });

                foreach (var entry in entries)
                {
                    value = value.Concat(dict[entry]).ToArray();
                }

                using MD5 md5 = MD5.Create();
                return md5.ComputeHash(value);
            }
            else if (File.Exists(path))
            {
                return ComputeFileCheckSum(path);
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }
    }
}