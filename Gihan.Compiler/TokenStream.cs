using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Gihan.Compiler
{
    public class TokenStream : IDisposable
    {
        public const int QueueBufferLength = 1024;

        private List<Token> TokenBuffer { get; set; }

        public bool IsBufferFull
            => TokenBuffer is null ? false : TokenBuffer.Capacity == TokenBuffer.Count;
        public bool IsBufferEmpty => TokenBuffer is null ? true : TokenBuffer.Count == 0;
        public bool Disposed => TokenBuffer is null;

        public bool Ended { get; set; } = false;

        public Token PreviewsToken { get; set; } = null;
        public Token CurrentToken { get; set; } = null;

        public Token GetHead()
        {
            if (Disposed || (IsBufferEmpty && Ended))
                return null;
            
            if (TokenBuffer[0].Key == TokenSet.WhiteSpace ||
                TokenBuffer[0].Key == TokenSet.Comment ||
                TokenBuffer[0].Key == TokenSet.PreProcess)
            {
                TokenBuffer.RemoveAt(0);
                return GetHead();
            }
            return (CurrentToken = TokenBuffer[0]);
        }

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
            var token = GetHead();
            TokenBuffer.RemoveAt(0);
            return (PreviewsToken = token);
        }

        public void Dispose()
        {
            TokenBuffer = null;
            GC.Collect();
        }
    }
}
