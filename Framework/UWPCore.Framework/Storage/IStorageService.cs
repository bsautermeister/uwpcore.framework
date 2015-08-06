﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPCore.Framework.Storage
{
    /// <summary>
    /// The data storage service interface.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Gets the root folder.
        /// </summary>
        IStorageFolder RootFolder { get; }

        /// <summary>
        /// Writes data to a given file path.
        /// </summary>
        /// <param name="filePath">The full path to write to.</param>
        /// <param name="data">The data to write.</param>
        /// <returns>The async task to wait for.</returns>
        Task WriteFile(string filePath, string data);

        /// <summary>
        /// Writes data to a given file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="data">The data to write.</param>
        /// <returns>The async task to wait for.</returns>
        Task WriteFile(IStorageFile file, string data);

        /// <summary>
        /// Writes data to a given file path.
        /// </summary>
        /// <param name="filePath">The full path to write to.</param>
        /// <param name="data">The data stream to write.</param>
        /// <returns>An async indicator whether the operation was successful.</returns>
        Task<bool> WriteFile(string filePath, Stream data);

        /// <summary>
        /// Writes data to a given file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="data">The data stream to write.</param>
        /// <returns>An async indicator whether the operation was successful.</returns>
        Task<bool> WriteFile(IStorageFile file, Stream data);

        /// <summary>
        /// Writes image data to a given file path.
        /// </summary>
        /// <param name="filePath">The full path to write to.</param>
        /// <param name="image">The image data.</param>
        /// <returns>An async indicator whether the operation was successful.</returns>
        Task<bool> WriteFile(string filePath, WriteableBitmap image);

        /// <summary>
        /// Writes image data to a given file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="image">The image data.</param>
        /// <returns>An async indicator whether the operation was successful.</returns>
        Task<bool> WriteFile(IStorageFile file, WriteableBitmap image);

        /// <summary>
        /// Reads data from the given file.
        /// </summary>
        /// <param name="filePath">The full path to read from.</param>
        /// <returns>The async data result or NULL in case of an error.</returns>
        Task<string> ReadFile(string filePath);

        /// <summary>
        /// Reads data from the given file.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <returns>The async data result or NULL in case of an error.</returns>
        Task<string> ReadFile(IStorageFile file);

        /// <summary>
        /// Checks whether the file exists.
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        /// <returns>The async result that indicates whether the file was found or not.</returns>
        Task<bool> ContainsFile(string filePath);

        /// <summary>
        /// Checks whether the directory exists.
        /// </summary>
        /// <param name="directoryPath">The full directory path.</param>
        /// <returns>The async result that indicates whether the directory was found or not.</returns>
        Task<bool> ContainsDirectory(string directoryPath);

        /// <summary>
        /// Get a file async.
        /// </summary>
        /// <param name="path">Path of file to get.</param>
        /// <returns>Returns file or null if file does not exist.</returns>
        Task<StorageFile> GetFileAsync(string path);

        /// <summary>
        /// Get a list of files async.
        /// </summary>
        /// <param name="path">Folder path to list of containing files.</param>
        /// <returns>Returns list of storage files.</returns>
        Task<IReadOnlyList<StorageFile>> GetFilesAsync(string path);

        /// <summary>
        /// Create or get a file async.
        /// </summary>
        /// <param name="path">Path of file to create or get.</param>
        /// <returns>The async result that contains the storage file.</returns>
        Task<StorageFile> CreateOrGetFileAsync(string path);

        /// <summary>
        /// Create or replace a file async.
        /// </summary>
        /// <param name="path">Path of file to create or replace.</param>
        /// <returns>The async result that contains the storage file.</returns>
        Task<StorageFile> CreateOrReplaceFileAsync(string path);

        /// <summary>
        /// Delete a file async.
        /// </summary>
        /// <param name="path">Path of file to delete.</param>
        /// <returns>Task to wait for.</returns>
        Task DeleteFileAsync(string path);

        /// <summary>
        /// Get a folder async.
        /// </summary>
        /// <param name="path">Folder path of folder to get.</param>
        /// <returns>Returns folder or null if folder does not exist.</returns>
        Task<StorageFolder> GetFolderAsync(string path);

        /// <summary>
        /// Get a list of folders async.
        /// </summary>
        /// <param name="path">Path to list of containing folder.</param>
        /// <returns>Returns list of storage folders.</returns>
        Task<IReadOnlyList<StorageFolder>> GetFoldersAsync(string path);

        /// <summary>
        /// Create or get a folder async.
        /// </summary>
        /// <param name="path">Path of folder to create or get.</param>
        /// <returns>The async result that contais the storage folder.</returns>
        Task<StorageFolder> CreateOrGetFolderAsync(string path);

        /// <summary>
        /// Create or replace a folder async.
        /// </summary>
        /// <param name="path">Path of folder to create or replace.</param>
        /// <returns>The async result rhat contains the storage folder.</returns>
        Task<StorageFolder> CreateOrReplaceFolderAsync(string path);

        /// <summary>
        /// Delete a folder async.
        /// </summary>
        /// <param name="path">Path to folder to delete.</param>
        /// <returns>Task to wait for.</returns>
        Task DeleteFolderAsync(string path);
    }

}