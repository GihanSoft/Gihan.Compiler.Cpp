using System;
using System.Collections.Generic;
using System.Text;

namespace Gihan.Compiler
{
    public class Token
    {
        public string File { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
#if DEBUG
            if (Key == TokenSet.Id || Key == TokenSet.KeyWord)
                return $"<{Key}, {Value}>  ({SymbolTable.Global.GetSymbol(this).Value})";
#endif
            return Value is null ? $"<{Key}>" : $"<{Key}, {Value}>";
        }
    }
}
