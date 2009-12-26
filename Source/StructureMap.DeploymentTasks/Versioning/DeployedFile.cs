using System.IO;
using System.Security.Cryptography;

namespace StructureMap.DeploymentTasks.Versioning
{
    public class DeployedFile
    {
        private byte[] _contentsHash;
        private string _fileName;

        public DeployedFile()
        {
        }

        public DeployedFile(string fileName)
        {
            _fileName = fileName.ToUpper();
        }

        public string FileName { get { return _fileName; } set { _fileName = value.ToUpper(); } }

        public byte[] ContentsHash { get { return _contentsHash; } set { _contentsHash = value; } }

        public static DeployedFile CreateFile(FileInfo fileInfo)
        {
            var deployedFile = new DeployedFile(fileInfo.Name);
            var crypto = new SHA1CryptoServiceProvider();

            using (FileStream stream = fileInfo.OpenRead())
            {
                deployedFile.ContentsHash = crypto.ComputeHash(stream);
            }

            return deployedFile;
        }

        public void CheckVersion(DeployedDirectory directory, IVersionReport report)
        {
            DeployedFile deployedFile = directory.FindFile(FileName);
            if (deployedFile == null)
            {
                report.MissingFile(FileName);
            }
            else if (!CheckContents(deployedFile.ContentsHash))
            {
                report.VersionMismatchFile(FileName);
            }
        }

        public bool CheckContents(byte[] targetHash)
        {
            if (ContentsHash.Length != targetHash.Length)
            {
                return false;
            }

            for (int i = 0; i < targetHash.Length; i++)
            {
                byte targetByte = targetHash[i];
                byte internalByte = _contentsHash[i];

                if (targetByte != internalByte)
                {
                    return false;
                }
            }

            return true;
        }
    }
}