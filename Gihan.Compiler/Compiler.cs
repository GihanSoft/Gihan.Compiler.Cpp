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

        public Compiler(IEnumerable<(FileInfo file, Encoding encoding)> files)
        {
            Files = files.ToArray();
        }

        public Task Run()
        {
            return Task.Run(() =>
            {
                foreach (var file in Files)
                {
                    var tokenStream = new TokenStream();
                    Task.Run(() =>
                    {
                        var buffer = new Buffer(file.file.FullName, file.encoding ?? Encoding.ASCII);
                        var tokenizer = new Tokenizer(buffer);
                        while (tokenizer.IsTokeningCompleted)
                        {
                            while (tokenStream.IsBufferFull)
                                Thread.Sleep(1);
                            tokenStream.Write(tokenizer.GetNextToken());
                        }
                    });
                }
            });
        }
    }
}
