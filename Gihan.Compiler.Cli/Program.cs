using System;
using System.IO;

namespace Gihan.Compiler.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var compiler = new Compiler((new FileInfo(@"D:\WorkSpace\test\test.cpp"), System.Text.Encoding.ASCII));
                compiler.Run();
            }
            catch
            {

            }
        }
    }
}
