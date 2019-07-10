using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gihan.Compiler
{
    public class Compiler
    {
        private (FileInfo file, Encoding encoding)[] Files { get; set; }

        public Compiler(params (FileInfo file, Encoding encoding)[] files)
        {
            Files = files.ToArray();
        }

        public void Run()
        {
            foreach (var file in Files)
            {
                var tokenStream = new TokenStream();
                Task.Run(() =>
                {
                    var buffer = new Buffer(file.file.FullName, file.encoding ?? Encoding.ASCII);
                    var tokenizer = new Tokenizer(buffer);
                    while (!tokenizer.IsTokeningCompleted)
                    {
                        while (tokenStream.IsBufferFull)
                            Thread.Sleep(100);
                        tokenStream.Write(tokenizer.GetNextToken());
                    }
                    tokenStream.Ended = true;
                    while (!tokenStream.IsBufferEmpty)
                        Thread.Sleep(100);
                    tokenStream.Dispose();
                });
                Parser parser = null;
                Task.Run(() =>
                {
                    parser = new Parser(tokenStream);
                    parser.Run();
                });

                while (parser is null || !parser.IsRunning)
                    Thread.Sleep(100);
                while (parser?.IsRunning ?? false)
                    Thread.Sleep(100);
            }
        }
    }
}
