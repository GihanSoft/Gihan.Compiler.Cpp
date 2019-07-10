using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gihan.Compiler
{
    public class SymbolTable
    {
        public class Symbol
        {
            public string Value { get; set; }
            public SymbolType Type { get; set; }

            public override string ToString()
            {
                return $"\"{Value}\"}} {{{Type}";
            }
        }
        public string Scop { get; }
        public SymbolTable SupperTable { get; }
        public List<SymbolTable> SubTables { get; }

        public Dictionary<ulong, Symbol> Symbols { get; }

        private SymbolTable(string scop, SymbolTable supperTable = null)
        {
            Scop = scop;
            SupperTable = supperTable;
            SubTables = new List<SymbolTable>();
            Symbols = new Dictionary<ulong, Symbol>();
        }

        public ulong AddSymbol(string symbol, SymbolType type = SymbolType.None)
        {
            var index = Symbols.Count > 0 ? Symbols.Keys.Last() + 1 : 0;
            var newSymbol = new Symbol()
            {
                Value = symbol
            };
            Symbols.Add(index, newSymbol);
            return index;
        }

        public void CreatSubTable(string scop)
        {
            var table = new SymbolTable(scop, this);
            SubTables.Add(table);
        }

        public Symbol GetSymbol(Token token)
        {
            var succeed = ulong.TryParse(token.Value, out var key);
            if (!succeed)
                return null;
            var symbol = Symbols.ContainsKey(key) ? Symbols[key] : SupperTable.GetSymbol(token);
            return symbol;
        }

        public static SymbolTable Global { get; }

        static SymbolTable()
        {
            Global = new SymbolTable("global");

            Global.AddSymbol("using", SymbolType.KeyWord);
            Global.AddSymbol("namespace", SymbolType.KeyWord);

            Global.AddSymbol("void", SymbolType.KeyWord | SymbolType.Type);
            Global.AddSymbol("bool", SymbolType.KeyWord | SymbolType.Type);
            Global.AddSymbol("int", SymbolType.KeyWord | SymbolType.Type);
            Global.AddSymbol("char", SymbolType.KeyWord | SymbolType.Type);
            Global.AddSymbol("wchar_t", SymbolType.KeyWord | SymbolType.Type);
            Global.AddSymbol("float", SymbolType.KeyWord | SymbolType.Type);
            Global.AddSymbol("double", SymbolType.KeyWord | SymbolType.Type);

            Global.AddSymbol("short", SymbolType.KeyWord | SymbolType.TypeModifier | SymbolType.Type);
            Global.AddSymbol("long", SymbolType.KeyWord | SymbolType.TypeModifier | SymbolType.Type);

            Global.AddSymbol("signed", SymbolType.KeyWord | SymbolType.TypeModifier);
            Global.AddSymbol("unsigned", SymbolType.KeyWord | SymbolType.TypeModifier);

            Global.AddSymbol("void", SymbolType.KeyWord | SymbolType.Type);

            Global.AddSymbol("if", SymbolType.KeyWord);
            Global.AddSymbol("while", SymbolType.KeyWord);
            Global.AddSymbol("do", SymbolType.KeyWord);
            Global.AddSymbol("for", SymbolType.KeyWord);
            Global.AddSymbol("return", SymbolType.KeyWord);

            Global.AddSymbol("throw", SymbolType.KeyWord);

            Global.AddSymbol("class", SymbolType.KeyWord);
            Global.AddSymbol("struct", SymbolType.KeyWord);
            Global.AddSymbol("enum", SymbolType.KeyWord);

            Global.AddSymbol("typedef", SymbolType.KeyWord);

        }
    }
}
