using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Gihan.Compiler.Exceptions
{
    public class LexicalException : Exception
    {
        public LexicalException()
        {
        }

        public LexicalException(string message) : base(message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
        }

        public LexicalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LexicalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
