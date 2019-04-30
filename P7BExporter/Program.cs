using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace P7BExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            int allCert = 0;
            Logger.InitLogger();
            Logger.Log.Info($"Приложение запущено");
            string startDir = Directory.GetCurrentDirectory();
            Logger.Log.Info($"Директория с хранилищами для экспорта {startDir}");
            if (!Directory.Exists(startDir+@"\export\"))
            {
                Directory.CreateDirectory(startDir + @"\export\");
                Logger.Log.Info($"Директория для экспорта не существует. Создаем - {startDir + @"\export\"}.");
            }
            DirectoryInfo directory = new DirectoryInfo(startDir);
            Logger.Log.Info($"Начинаем экспорт сертификатов из {startDir}.");
            foreach (var p7bFile in directory.GetFiles("*.p7b"))
            {
                Logger.Log.Info($"Извлечение из хранилща {p7bFile.Name}.");
                byte[] data;
                using (FileStream file = new FileStream(p7bFile.FullName, FileMode.Open))
                {
                    data = new byte[file.Length + 4096];
                    int len = 0;
                    int readed = 0;
                    do
                    {
                        readed = file.Read(data, len, 4096);
                        len += readed;
                    } while (readed > 0);

                }
                X509Certificate2Collection collection = new X509Certificate2Collection();
                collection.Import(data);
                Logger.Log.Info($"Извлечено. Сертификатов в хранилище - {collection.Count}.");
                allCert += collection.Count;
                foreach (X509Certificate2 cert in collection)
                {
                    string filename = startDir + @"\export\" + cert.Thumbprint + ".cer";
                    using (FileStream file = new FileStream(filename, FileMode.Create))
                    {
                        byte[] bytes = cert.Export(X509ContentType.Cert);
                        file.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            Logger.Log.Info($"Экспорт завершен. Всего извлечено сертификатов - {allCert}.");
        }
    }
}
