using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ZipC
{
    public enum ZipCreatorResult
    {
        Success,
        OverwriteError
    }

    internal class ArchiveMember
    {
        public bool IsReadOnly { get; set; }
        public FileInfo File { get; set; }
    }

    public class ZipCreator
    {
        public ZipCreatorSettings Settings { get; set; }

        public ZipCreator(ZipCreatorSettings settings = null)
        {
            Settings = settings ?? new ZipCreatorSettings();
        }

        public static ZipCreator CreateFromFile(FileInfo inputManifestFile)
        {
            if (!inputManifestFile.Exists)
            {
                throw new ArgumentException($"Input file does not exist at path: {inputManifestFile.FullName}");
            }

            var settings = new ZipCreatorSettings()
            {
                Filters = File.ReadLines(inputManifestFile.FullName).ToList()
            };
            return new ZipCreator(settings);
        }

        public ZipCreatorResult MakeZips()
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

            var currentDirectory = Directory.GetCurrentDirectory();
            var wrapper = new DirectoryInfoWrapper(new DirectoryInfo(currentDirectory));
            var result = matcher.Execute(wrapper);
            var archiveMembers = result.Files.Select(match => {
                var file = new FileInfo(match.Path);
                var isReadOnly = file.IsReadOnly;
                return new ArchiveMember()
                {
                    File = file,
                    IsReadOnly = isReadOnly
                };
            });

            foreach (var member in archiveMembers)
            {
                Console.WriteLine($"Adding file {member.File.FullName}");

                // Clear readonly attributes
                if (member.IsReadOnly)
                {
                    File.SetAttributes(member.File.FullName, File.GetAttributes(member.File.FullName) & ~FileAttributes.ReadOnly);
                }

                // Call each of the interceptors
                foreach (var interceptor in Settings.Interceptors)
                {
                    interceptor(member.File);
                }

                //// TODO BG move
                //// Sign unsigned exes and dlls
                //var filePath = member.File.FullName.ToLower();
                //if (filePath.EndsWith(".exe") || filePath.EndsWith(".dll"))
                //{
                //    using (Process proc = new Process())
                //    {
                //        var signToolPath = Path.Combine(makezipsDirectoryPath, "signTool.exe");
                //        var certificatePath = Path.Combine(makezipsDirectoryPath, "BugSplatCodeSigning.pfx");
                //        proc.StartInfo.UseShellExecute = false;
                //        proc.StartInfo.RedirectStandardOutput = true;
                //        proc.StartInfo.FileName = signToolPath;
                //        proc.StartInfo.Arguments = $"sign /q /f {certificatePath} /t http://timestamp.comodoca.com/authenticode {member.File.FullName}";
                //        proc.Start();
                //        var output = proc.StandardOutput.ReadToEnd();
                //        proc.WaitForExit();
                //        if (proc.ExitCode != 0)
                //        {
                //            Console.WriteLine(String.Format("Error process {0} {1} exited with code = {2}", proc.StartInfo.FileName, proc.StartInfo.Arguments, proc.ExitCode));
                //            Environment.Exit(1);
                //        }
                //        Console.WriteLine(output);
                //    }
                //}
            }

            // Delete zip if it exists and overwrite is true
            if (Settings.ZipOutputFile.Exists && !Settings.Overwrite)
            {
                return ZipCreatorResult.OverwriteError;
            }
            if (Settings.ZipOutputFile.Exists && Settings.Overwrite)
            {
                File.SetAttributes(Settings.ZipOutputFile.FullName, Settings.ZipOutputFile.Attributes & ~FileAttributes.ReadOnly);
                Settings.ZipOutputFile.Delete();
            }

            // Create the zip file
            using (var zipFile = ZipFile.Open(Settings.ZipOutputFile.FullName, ZipArchiveMode.Create))
            {
                foreach (var member in archiveMembers)
                {
                    var relativePath = GetRelativePath(currentDirectory, member.File.FullName);
                    var entryName = CreateEntryNameWithAncestorDirectoriesRemoved(relativePath);
                    zipFile.CreateEntryFromFile(member.File.FullName, entryName);
                }
            }

            // Restore readonly attributes
            foreach (var member in archiveMembers)
            {
                if (member.IsReadOnly)
                {
                    File.SetAttributes(member.File.FullName, member.File.Attributes | FileAttributes.ReadOnly);
                }
            }

            return ZipCreatorResult.Success;
        }

        // https://stackoverflow.com/questions/51179331/is-it-possible-to-use-path-getrelativepath-net-core2-in-winforms-proj-targeti
        private static string GetRelativePath(string relativeTo, string path)
        {
            var uri = new Uri(relativeTo);
            var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return rel;
        }

        private static string CreateEntryNameWithAncestorDirectoriesRemoved(string relativePath)
        {
            return relativePath.Replace("..\\", "");
        }
    }
}
