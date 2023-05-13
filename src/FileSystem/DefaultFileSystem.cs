﻿using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem
{
    public class DefaultFileSystem : IFileSystem
    {
        public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            File.Copy(sourcePath, destinationPath, overwrite);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void CreateFile(string path)
        {
            File.Create(path);
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string[] GetDirectories(string path, bool recursive)
        {
            return Directory.GetDirectories(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public string[] GetFiles(string path, bool recursive)
        {
            return Directory.GetFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return File.Open(path, mode, access, share);
        }
    }
}
