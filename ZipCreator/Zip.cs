using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ZipCreator
{
    public class Zip
    {
        public List<FileInfo> Files
        {
            get
            {
                var matcher = new Matcher();

                foreach (var filter in Settings.Filters)
                {
                    var trimmedLine = filter.Trim();
                    if (trimmedLine.StartsWith("!"))
                    {
                        matcher.AddExclude(filter.Substring(1));
                    }
                    else
                    {
                        matcher.AddInclude(trimmedLine);
                    }
                }

                var wrapper = new DirectoryInfoWrapper(new DirectoryInfo(currentDirectory));
                var result = matcher.Execute(wrapper);
                var files = result.Files.Select(file => new FileInfo(file.Path));

                return files.ToList();
            }
        }

        public ZipSettings Settings { get; set; }

        private readonly string currentDirectory = Directory.GetCurrentDirectory();

        public Zip(ZipSettings settings = null)
        {
            Settings = settings ?? new ZipSettings();
        }

        public static Zip CreateFromFile(FileInfo inputManifestFile)
        {
            if (!inputManifestFile.Exists)
            {
                throw new ArgumentException($"Input file does not exist at path: {inputManifestFile.FullName}");
            }

            var settings = new ZipSettings()
            {
                Filters = File.ReadLines(inputManifestFile.FullName).ToList()
            };
            return new Zip(settings);
        }

        public ZipWriteResult Write()
        {
            var files = Files;

            InvokeInterceptors(files);
            
            if (Settings.Output.Exists && !Settings.Overwrite)
            {
                return ZipWriteResult.OverwriteError;
            }

            if (Settings.Output.Exists && Settings.Overwrite)
            {
                ForceDelete(Settings.Output);
            }

            CreateZip(files);

            return ZipWriteResult.Success;
        }

        private void CreateZip(List<FileInfo> files)
        {
            using (var zipFile = ZipFile.Open(Settings.Output.FullName, ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    var relativePath = GetRelativePath(currentDirectory, file.FullName);
                    var entryName = CreateEntryNameWithAncestorDirectoriesRemoved(relativePath);
                    zipFile.CreateEntryFromFile(file.FullName, entryName);
                }
            }
        }

        private void ForceDelete(FileInfo file)
        {
            File.SetAttributes(file.FullName, file.Attributes & ~FileAttributes.ReadOnly);
            file.Delete();
        }

        private void InvokeInterceptors(List<FileInfo> files)
        {
            foreach (var file in files)
            {
                foreach (var interceptor in Settings.Interceptors)
                {
                    interceptor(file);
                }
            }
        }

        private static string CreateEntryNameWithAncestorDirectoriesRemoved(string relativePath)
        {
            return relativePath.Replace("..\\", "");
        }

        // https://stackoverflow.com/questions/51179331/is-it-possible-to-use-path-getrelativepath-net-core2-in-winforms-proj-targeti
        private static string GetRelativePath(string relativeTo, string path)
        {
            var uri = new Uri(relativeTo);
            var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return rel;
        }
    }
}
