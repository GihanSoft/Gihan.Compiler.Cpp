using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gihan.Compiler
{
    public class Buffer
    {
        public const int HeadStringSize = 128;
        protected const int BufferStringSize = 1024;

        protected StreamReader CodeStreamReader { get; set; }
        protected string PendingBufferStr { get; set; } // size = BufferStringSize
        protected string BufferStr { get; set; } // size = BufferStringSize
        private string HeadStr { get; set; } // size = BufferStringSize

        public int Line { get; private set; } = 1;
        public int Column { get; private set; } = 1;
        public FileInfo FileInfo { get; set; }

        public bool IsEndOfStream =>
            string.IsNullOrEmpty(PendingBufferStr) &&
            string.IsNullOrEmpty(BufferStr) &&
            string.IsNullOrEmpty(HeadStr);

        public Buffer(Stream codeStream, Encoding encoding)
        {
            CodeStreamReader = new StreamReader(codeStream, encoding, true);
            FillPendingBuffer();
            BufferStr = PendingBufferStr;
            FillPendingBuffer();
            HeadStr = BufferStr.Substring(0, Math.Min(HeadStringSize, BufferStr.Length));
            BufferStr = BufferStr.Substring(HeadStr.Length);
        }

        public Buffer(FileStream codeFileStream, Encoding encoding)
            : this(codeFileStream as Stream, encoding)
        {
            FileInfo = new FileInfo(codeFileStream.Name);
        }

        public Buffer(string codePath, Encoding encoding)
            : this(new FileStream(codePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), encoding) { }

        private void FillPendingBuffer()
        {
            var readLength = (int)Math.Min(BufferStringSize, 
                CodeStreamReader.BaseStream.Length - CodeStreamReader.BaseStream.Position);
            var buffer = new char[readLength];
            CodeStreamReader.Read(buffer, 0, readLength);
            PendingBufferStr = new string(buffer);
        }

        public string Get()
        {
            return string.Copy(HeadStr);
        }

        public int Pop(int count)
        {
#if DEBUG
            if (HeadStr.Length > HeadStringSize)
                throw new Exception("wtf?!");
#endif         

            if (count > HeadStringSize || count < 1)
                throw new ArgumentOutOfRangeException(nameof(count), "count must be between 1 and 100");

            var poped = Math.Min(HeadStr.Length, count);
            string popedValue;
            string[] lines;

            if (poped < count)
            {
                popedValue = HeadStr.Substring(0, poped);

                HeadStr = "";

                lines = popedValue.Split('\n');
                Line += lines.Length - 1;
                Column = lines.Length > 1 ? lines.Last().Length+1 : Column + poped;

                return poped;
            }

            if (BufferStr.Length < poped)
            {
                BufferStr += PendingBufferStr;
                PendingBufferStr = "";
                FillPendingBuffer();
            }
            if (BufferStr.Length < poped)
            {
                HeadStr += BufferStr;
                BufferStr = "";

                popedValue = HeadStr.Substring(0, poped);

                HeadStr = HeadStr.Substring(poped);

                lines = popedValue.Split('\n');
                Line += lines.Length - 1;
                Column = lines.Length > 1 ? lines.Last().Length+1 : Column + poped;

                return poped;
            }

            HeadStr += BufferStr.Substring(0, poped);

            popedValue = HeadStr.Substring(0, poped);

            HeadStr = HeadStr.Substring(poped);
            BufferStr = BufferStr.Substring(poped);

            lines = popedValue.Split('\n');
            Line += lines.Length - 1;
            Column = lines.Length > 1 ? lines.Last().Length+1 : Column + poped;

            return poped;
        }

    }
}
