using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Gihan.Compiler.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var compiler = new Compiler(args.Select(a=>(new FileInfo(a), Encoding.ASCII)));
                compiler.Run();
            }
            catch
            {

            }
        }
    }
}
