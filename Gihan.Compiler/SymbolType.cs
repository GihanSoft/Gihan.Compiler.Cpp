namespace Gihan.Compiler
{
    public enum SymbolType : ushort
    {
        None = 0,
        KeyWord = 0b_1000_0000_0000_0000,
        Type = 0b_0000_0000_0000_0001,
        TypeModifier = 0b_0000_0000_0000_0010,

        

    }
}