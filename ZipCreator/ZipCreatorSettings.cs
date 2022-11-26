using System;
using System.Collections.Generic;
using System.IO;

namespace ZipC
{
    public class ZipCreatorSettings
    {
        public List<string> Filters { get; set; } = new List<string>() { "**/*.*" };
        public List<Action<FileInfo>> Interceptors { get; set; } = new List<Action<FileInfo>>();
        public FileInfo Output { get; set; } = GetTempFilePathWithExtension(".zip");
        public bool Overwrite { get; set; } = false;

        private static FileInfo GetTempFilePathWithExtension(string extension)
        {
            var tempPath = Path.GetTempPath();
            var fileName = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
            var path = Path.Combine(tempPath, fileName);
            return new FileInfo(path);
        }
    }
}
