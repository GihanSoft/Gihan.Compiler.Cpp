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

        public ulong AddSymbol(string symbol)
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

        public static SymbolTable Global { get; } = new SymbolTable("global");
    }
}
