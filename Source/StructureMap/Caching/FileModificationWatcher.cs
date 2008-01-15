using System.IO;

namespace StructureMap.Caching
{
    public class FileModificationWatcher : ClearEventDispatcher
    {
        private string _fullPath;
        private string _key;
        private FileSystemWatcher _watcher;

        public FileModificationWatcher(string FilePath)
            : base(SubjectNameFromFilePath(FilePath))
        {
            _fullPath = Path.GetFullPath(FilePath).ToUpper();
            _key = SubjectNameFromFilePath(_fullPath);

            string _directory = Path.GetDirectoryName(_fullPath);
            _watcher = new FileSystemWatcher(_directory);
            _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
            _watcher.EnableRaisingEvents = true;
            _watcher.Created += new FileSystemEventHandler(_watcher_Changed);
            _watcher.Deleted += new FileSystemEventHandler(_watcher_Changed);
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                    | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size |
                                    NotifyFilters.Attributes | NotifyFilters.CreationTime;
            _watcher.Filter = string.Empty;
        }


        public string Key
        {
            get { return _key; }
        }


        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.ToUpper() == _fullPath)
            {
                Dispatch();
            }
        }


        public static string SubjectNameFromFilePath(string FilePath)
        {
            string _fullPath = Path.GetFullPath(FilePath).ToUpper();
            return "MODIFIED::" + _fullPath;
        }
    }
}