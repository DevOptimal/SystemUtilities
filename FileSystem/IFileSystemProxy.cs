using System.IO;

namespace bradselw.System.Resources.FileSystem
{
    public interface IFileSystemProxy
    {
        void CreateDirectory(string path);

        void CreateFile(string path);

        void DeleteDirectory(string path, bool recursive);

        bool DirectoryExists(string path);

        void CopyFile(string sourcePath, string destinationPath, bool overwrite);

        void DeleteFile(string path);

        bool FileExists(string path);

        FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);
    }
}
