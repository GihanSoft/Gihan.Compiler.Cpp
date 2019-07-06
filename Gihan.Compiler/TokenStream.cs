using System;
using System.Collections.Generic;
using System.Text;

namespace Gihan.Compiler
{
    public class TokenStream
    {
        public const int QueueBufferLength = 1024; 

        private List<Token> TokenBuffer { get; set; }

        public bool IsBufferFull
            => TokenBuffer.Capacity == TokenBuffer.Count;

        public TokenStream()
        {
            TokenBuffer = new List<Token>(QueueBufferLength);
        }

        public void Write(Token token)
        {
            TokenBuffer.Add(token);
        }

        public Token Read()
        {
            var token = TokenBuffer[0];
            TokenBuffer.RemoveAt(0);
            return token; 
        }
    }
}
