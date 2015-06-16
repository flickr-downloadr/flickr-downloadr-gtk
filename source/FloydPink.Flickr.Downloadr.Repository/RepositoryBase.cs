namespace FloydPink.Flickr.Downloadr.Repository {
    using System;
    using System.IO;
    using Helpers;

    public abstract class RepositoryBase {
        private const string CryptKey = "SomeEncryPtionKey123";

        private readonly string _appDataFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "flickr-downloadr");

        protected abstract string RepoFileName { get; }
        private string AbsoluteFilePath { get { return Path.Combine(this._appDataFolder, RepoFileName); } }

        public void Delete() {
            if (File.Exists(AbsoluteFilePath)) {
                File.Delete(AbsoluteFilePath);
            }
        }

        protected string Read() {
            if (File.Exists(AbsoluteFilePath)) {
                return Crypt.Decrypt(File.ReadAllText(AbsoluteFilePath), CryptKey);
            }
            return string.Empty;
        }

        protected void Write(string fileContent) {
            if (!Directory.Exists(this._appDataFolder)) {
                Directory.CreateDirectory(this._appDataFolder);
            }
            File.WriteAllText(AbsoluteFilePath, Crypt.Encrypt(fileContent, CryptKey));
        }
    }
}
