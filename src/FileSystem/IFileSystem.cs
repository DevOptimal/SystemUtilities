using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem
{
    public interface IFileSystem
    {
        void CopyFile(string sourcePath, string destinationPath, bool overwrite);

        void CreateDirectory(string path);

        void CreateFile(string path);

        void DeleteDirectory(string path, bool recursive);

        void DeleteFile(string path);

        bool DirectoryExists(string path);

        bool FileExists(string path);

        string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);

        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);

        FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);
    }
}
