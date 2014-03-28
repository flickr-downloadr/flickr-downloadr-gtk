using System.IO;
using FloydPink.Flickr.Downloadr.Repository.Helpers;

namespace FloydPink.Flickr.Downloadr.Repository
{
    public abstract class RepositoryBase
    {
        protected readonly string CryptKey = "SomeEncryPtionKey123";
        internal abstract string RepoFileName { get; }

        public void Delete()
        {
            if (File.Exists(RepoFileName))
            {
                File.Delete(RepoFileName);
            }
        }

        protected string Read()
        {
            if (File.Exists(RepoFileName))
            {
                return Crypt.Decrypt(File.ReadAllText(RepoFileName), CryptKey);
            }
            return string.Empty;
        }

        protected void Write(string fileContent)
        {
            File.WriteAllText(RepoFileName, Crypt.Encrypt(fileContent, CryptKey));
        }
    }
}