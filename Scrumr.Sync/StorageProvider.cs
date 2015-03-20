using DropNet;
using DropNet.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Sync
{
    public class StorageProvider
    {
        DropNetClient client;

        public void Connect()
        {
            client = new DropNetClient("e68hlm6xcdpgema", "0iu2v5ccm4qm2a8");
            client.UseSandbox = true;
            client.UserLogin = new DropNet.Models.UserLogin { Token = "jdukos6bnbdo8b9v", Secret = "zg2t3o1qcjjy4ul" };
        }

        public async Task<FileInfo> RetrieveAsync()
        {
            return await Task.Factory.StartNew(() => Retrieve());
        }

        public async Task StoreAsync(FileInfo file)
        {
            await Task.Factory.StartNew(() => Store(file));
        }

        private FileInfo Retrieve()
        {
            var data = client.GetFile("default.scrumr");

            using (var fs = new FileStream(SyncPath.FullName, FileMode.Create))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    fs.WriteByte(data[i]);
                }
            }

            return SyncPath;
        }

        private void Store(FileInfo file)
        {
            client.UploadFile(file.FullName, "default.scrumr", File.ReadAllBytes(file.FullName), overwrite: true);
        }

        private FileInfo SyncPath
        {
            get
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var directory = new DirectoryInfo(Path.Combine(appDataPath, "Jay Wick Labs", "Sync"));

                if (!directory.Exists)
                    directory.Create();

                return new FileInfo(Path.Combine(directory.FullName, "default-synced.scrumr"));
            }
        }
    }
}
