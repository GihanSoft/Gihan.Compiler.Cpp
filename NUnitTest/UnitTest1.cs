using Gihan.Compiler;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestBuffer()
        {


            var code = @"char * a = ""hard \
coded"";
int main(){
    a = ""some other text"";
}
            ";
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(code));
            var buffer = new Buffer(memoryStream, System.Text.Encoding.UTF8);
            var tokenizer = new Tokenizer(buffer);
            var tokens = new List<Token>();
            var table = SymbolTable.Global.Symbols;

            Token token;
            while ((token = tokenizer.GetNextToken()) != null)
            {
                tokens.Add(token);
            }

            var file = File.Open(@"D:\tokenStream.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            var writter = new StreamWriter(file);
            foreach (var _token in tokens)
            {
                writter.WriteLine(_token + (_token.Key == TokenSet.Id ? table[ulong.Parse(_token.Value)].Value : ""));
            }
            writter.Dispose();
        }
    }
}