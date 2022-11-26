using System;
using System.Collections.Generic;
using System.IO;

namespace Zc
{
    public class ZipCreatorSettings
    {
        public List<string> Filters { get; set; } = new List<string>() { "**/*.*" };
        public Action<FileInfo> Interceptor { get; set; } = (fileInfo) => { };
        public bool Overwrite { get; set; } = false;
        public FileInfo ZipOutputFile { get; set; } = GetTempFilePathWithExtension(".zip");

        private static FileInfo GetTempFilePathWithExtension(string extension)
        {
            var tempPath = Path.GetTempPath();
            var fileName = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
            var path = Path.Combine(tempPath, fileName);
            return new FileInfo(path);
        }
    }
}
