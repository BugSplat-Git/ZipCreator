using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipC
{
    public enum LogLevel
    {
        Normal,
        Verbose
    }

    public static class Log
    {
        public static LogLevel Level { get; set; } = LogLevel.Normal;

        public static void Info(string line)
        {
            Console.WriteLine(line);
        }

        public static void Verbose(string line)
        {
            if (Level == LogLevel.Verbose)
            {
                Console.WriteLine(line);
            }
        }
    }
}
