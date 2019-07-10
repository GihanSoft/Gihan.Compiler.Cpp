using System;
using System.IO;

namespace Gihan.Compiler.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var compiler = new Compiler((new FileInfo(@"D:\WorkSpace\test\main.cpp"), System.Text.Encoding.ASCII));
            compiler.Run();
        }
    }
}
