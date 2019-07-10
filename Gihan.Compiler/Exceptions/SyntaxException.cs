using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Gihan.Compiler.Exceptions
{
    public class SyntaxException : Exception
    {
        public SyntaxException(Token token, string msg = null)
        {
            if (token is null)
            {
                Console.WriteLine(msg);
                return;
            }
            var tokenValue = token.Key == TokenSet.KeyWord || token.Key == TokenSet.Id ?
                SymbolTable.Global.GetSymbol(token).Value : token.Value;
            Console.WriteLine($"Error in Line:{token.Line}, Column:{token.Column - tokenValue.Length}");
            if (token.File != null)
            {
                var file = new StreamReader(token.File, Encoding.ASCII);
                for (int i = 0; i < token.Line - 1; i++)
                {
                    file.ReadLine();
                }
                Console.WriteLine(file.ReadLine());
                for (int i = 0; i < token.Column - tokenValue.Length - 1; i++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine("^");
                if (msg != null)
                {
                    Console.WriteLine(msg);
                }
            }
            Console.WriteLine();
        }
    }
}
