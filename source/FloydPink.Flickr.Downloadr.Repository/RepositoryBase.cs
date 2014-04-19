using System;
using System.IO;
using FloydPink.Flickr.Downloadr.Repository.Helpers;

namespace FloydPink.Flickr.Downloadr.Repository {
    public abstract class RepositoryBase {
        protected readonly string CryptKey = "SomeEncryPtionKey123";

        private readonly string _appDataFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "flickr-downloadr");

        internal abstract string RepoFileName { get; }

        private string AbsoluteFilePath { get { return Path.Combine(this._appDataFolder, RepoFileName); } }

        public void Delete() {
            if (File.Exists(AbsoluteFilePath)) {
                File.Delete(AbsoluteFilePath);
            }
        }

        protected string Read() {
            if (File.Exists(AbsoluteFilePath)) {
                return Crypt.Decrypt(File.ReadAllText(AbsoluteFilePath), this.CryptKey);
            }
            return string.Empty;
        }

        protected void Write(string fileContent) {
            if (!Directory.Exists(this._appDataFolder)) {
                Directory.CreateDirectory(this._appDataFolder);
            }
            File.WriteAllText(AbsoluteFilePath, Crypt.Encrypt(fileContent, this.CryptKey));
        }
    }
}
