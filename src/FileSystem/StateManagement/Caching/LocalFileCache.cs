﻿using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching
{
    public class LocalFileCache : IFileCache
    {
        public string RootPath { get; }

        public IFileSystem FileSystem { get; }

        public LocalFileCache(DirectoryInfo rootDirectory, IFileSystem fileSystem)
            : this(rootDirectory.FullName, fileSystem)
        { }

        public LocalFileCache(string rootDirectoryPath, IFileSystem fileSystem)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            RootPath = Path.GetFullPath(rootDirectoryPath);
            if (!fileSystem.DirectoryExists(RootPath))
            {
                fileSystem.CreateDirectory(RootPath);
            }
        }

        public void DownloadFile(string id, string destinationPath)
        {
            var blobPath = Path.Combine(RootPath, id);
            if (!FileSystem.FileExists(blobPath))
            {
                throw new FileNotFoundException("The file does not exist in the cache.", blobPath);
            }

            FileSystem.CopyFile(blobPath, destinationPath, overwrite: true);

            FileSystem.DeleteFile(blobPath);
        }

        public string UploadFile(string sourcePath)
        {
            if (!FileSystem.FileExists(sourcePath))
            {
                throw new FileNotFoundException("The source file does not exist.", sourcePath);
            }

            var id = Guid.NewGuid().ToString();

            var blobPath = Path.Combine(RootPath, id);

            FileSystem.CopyFile(sourcePath, blobPath, overwrite: false);

            return id;
        }
    }
}
