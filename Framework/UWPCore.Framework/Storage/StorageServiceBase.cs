﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPCore.Framework.Storage
{
    /// <summary>
    /// The storage service base class for app data storage.
    /// </summary>
    public abstract class StorageServiceBase : IStorageService
    {
        #region Members

        public IStorageFolder RootFolder { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a StorageServiceBase instance.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        public StorageServiceBase(IStorageFolder rootFolder)
        {
            RootFolder = rootFolder;
        }

        #endregion

        #region Public Methods

        public async Task WriteFile(string filePath, string data)
        {
            var folder = await CreateOrGetFolderAsync(filePath);

            if (folder == null)
                return;

            var storageFile = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.OpenIfExists);
            await WriteFile(storageFile, data);
        }

        public async Task WriteFile(IStorageFile file, string data)
        {
            await FileIO.WriteTextAsync(file, data);
        }

        public async Task<bool> WriteFile(string filePath, Stream data)
        {
            var folder = await CreateOrGetFolderAsync(filePath);

            if (folder == null)
                return false;

            var storageFile = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.OpenIfExists);
            return await WriteFile(storageFile, data);
        }

        public async Task<bool> WriteFile(IStorageFile file, Stream data)
        {
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            using (var outputStream = stream.GetOutputStreamAt(0))
            {
                var dataWriter = new DataWriter(outputStream);
                using (var inputStream = data.AsInputStream())
                {
                    var dataReader = new DataReader(inputStream);
                    IBuffer buffer;

                    // store loaded data in isolated storage
                    var dataBuffer = new byte[1024];
                    while ((buffer = dataReader.ReadBuffer(1024)) != null)
                    {
                        dataWriter.WriteBuffer(buffer);

                        if (buffer.Length != 1024) // TODO: FIXME: how to detect the end of the stream?
                            break;
                    }
                }
            }

            return true;
        }

        public async Task<bool> WriteFile(string filePath, WriteableBitmap image)
        {
            var folder = await GetStorageFolder(filePath);

            if (folder == null)
                return false;

            var storageFile = await folder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.ReplaceExisting);
            return await WriteFile(storageFile, image);
        }

        public async Task<bool> WriteFile(IStorageFile file, WriteableBitmap image)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    var dataWriter = new DataWriter(outputStream);
                    dataWriter.WriteBuffer(image.PixelBuffer);
                }
            }

            return true;
        }

        public async Task<string> ReadFile(string filePath)
        {
            var folder = await GetStorageFolder(filePath);

            if (folder == null)
                return null;

            try
            {
                var path = Path.GetFileName(filePath);
                var storageFile = await folder.GetFileAsync(Path.GetFileName(filePath));
                return await ReadFile(storageFile);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public async Task<string> ReadFile(IStorageFile file)
        {
            return await FileIO.ReadTextAsync(file);
        }

        public async Task<bool> ContainsFile(string filePath)
        {
            try
            {
                var folder = await GetStorageFolder(filePath);

                if (folder == null)
                    return false;

                await folder.GetFileAsync(Path.GetFileName(filePath));
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public async Task<bool> ContainsDirectory(string directoryPath)
        {
            var folder = await GetStorageFolder(directoryPath, true);
            return folder != null;
        }

        public async Task<IStorageFile> GetFileAsync(string filePath)
        {
            if (await ContainsFile(filePath))
            {
                return await RootFolder.GetFileAsync(filePath);
            }

            return null;
        }

        public async Task<IReadOnlyList<IStorageFile>> GetFilesAsync(string filePath)
        {
            var folder = await GetFolderAsync(filePath);
            if (folder != null)
            {
                return await folder.GetFilesAsync();
            }

            return null;
        }

        public async Task<IStorageFile> CreateOrGetFileAsync(string filePath)
        {
            // ensure to use backslash paths to support subfolder paths
            filePath = filePath.Replace('/', '\\');

            try
            {
                return await RootFolder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IStorageFile> CreateOrReplaceFileAsync(string filePath)
        {
            // ensure to use backslash paths to support subfolder paths
            filePath = filePath.Replace('/', '\\');

            try
            {
                return await RootFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IStorageFile> CreateTempFileAsync(string extension = ".tmp")
        {
            var fileName = await CreateTempFileNameAsync(
                extension,
                DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.ffffff"));

            var file = await RootFolder.CreateFileAsync(
                fileName,
                CreationCollisionOption.GenerateUniqueName);

            return file;
        }

        public async Task<string> CreateTempFileNameAsync(string extension = ".tmp", string prefix = "", string suffix = "")
        {
            string fileName;

            if (string.IsNullOrEmpty(extension))
            {
                extension = ".tmp";
            }
            else if (extension[0] != '.')
            {
                extension = string.Format(".{0}", extension);
            }

            // Try no random numbers
            if (!string.IsNullOrEmpty(prefix) &&
                !string.IsNullOrEmpty(prefix))
            {
                fileName = string.Format(
                    "{0}{1}{2}",
                    prefix,
                    suffix,
                    extension);
                if (!await ContainsFile(fileName))
                {
                    return fileName;
                }
            }

            do
            {
                fileName = string.Format(
                    "{0}{1}{2}{3}",
                    prefix,
                    Guid.NewGuid(),
                    suffix,
                    extension);
            } while (await ContainsFile(fileName));

            return fileName;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            try
            {
                var folder = await GetStorageFolder(filePath);

                if (folder == null)
                    return;

                var file = await folder.GetFileAsync(Path.GetFileName(filePath));
                await file.DeleteAsync();
            }
            catch (FileNotFoundException)
            {
                // NOP
            }
        }

        public async Task<IStorageFolder> GetFolderAsync(string path)
        {
            if (await ContainsDirectory(path))
            {
                return await RootFolder.GetFolderAsync(path);
            }

            return null;
        }

        public async Task<IReadOnlyList<IStorageFolder>> GetFoldersAsync(string path)
        {
            var folder = await GetFolderAsync(path);
            if (folder != null)
            {
                return await folder.GetFoldersAsync();
            }

            return null;
        }

        public async Task<IStorageFolder> CreateOrGetFolderAsync(string path)
        {
            if (string.IsNullOrEmpty(Path.GetDirectoryName(path)))
                return RootFolder;

            path = Path.GetDirectoryName(path);

            return await RootFolder.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists);
        }

        public async Task<IStorageFolder> CreateOrReplaceFolderAsync(string path)
        {
            if (string.IsNullOrEmpty(Path.GetDirectoryName(path)))
                return RootFolder;

            path = Path.GetDirectoryName(path);

            return await RootFolder.CreateFolderAsync(path, CreationCollisionOption.ReplaceExisting);
        }

        public async Task DeleteFolderAsync(string path)
        {
            try
            {
                var folder = await GetStorageFolder(path, true);

                if (folder == null)
                    return;

                await folder.DeleteAsync();
            }
            catch (FileNotFoundException)
            {
                // NOP
            }
        }

        public async Task<IStorageFile> GetFileFromApplicationAsync(Uri uri)
        {
            try
            {
                return await StorageFile.GetFileFromApplicationUriAsync(uri);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IStorageFile> GetFileFromApplicationAsync(string filePath)
        {
            // ensure the path has an appx URI scheme
            if (!filePath.StartsWith(IOConstants.APPX_SCHEME))
                filePath = IOConstants.APPX_SCHEME + filePath;

            return await GetFileFromApplicationAsync(new Uri(filePath));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the folder of the given file or directory path.
        /// </summary>
        /// <param name="fullPath">The file or directory path to get the folder.</param>
        /// <param name="isDirectoryPath">Indicates whether the given path
        /// is a directory path (false) or a file path (true, default).</param>
        /// <returns>Returns the found folder or NULL in case of an error.</returns>
        private async Task<IStorageFolder> GetStorageFolder(string fullPath, bool isDirectoryPath = false)
        {
            string directoryPath = isDirectoryPath ? fullPath : Path.GetDirectoryName(fullPath);
            string[] directoryNames = directoryPath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            var currentFolder = RootFolder;
            foreach (var dirName in directoryNames)
            {
                try
                {
                    currentFolder = await currentFolder.GetFolderAsync(dirName);
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
            }
            return currentFolder;
        }
        
        #endregion
    }

}
