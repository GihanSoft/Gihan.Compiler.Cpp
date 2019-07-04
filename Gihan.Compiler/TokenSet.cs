using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gihan.Compiler
{
    public static class TokenSet
    {
        public static readonly string String = "String";
        public static readonly string Char = "Char";

        public static readonly string WhiteSpace = "WhiteSpace";
        public static readonly string Comment = "Comment";
        public static readonly string PreProcess = "PreProcess";

        public static readonly string Int = "Int";
        public static readonly string Float = "Float";


        public static readonly string KeyWord = "KeyWord";
        public static readonly string Id = "Id";

        public static readonly string Operator = "Operator";

        public static Dictionary<string, Regex> TokenRegexSet { get; set; }
            = new Dictionary<string, Regex>()
            {
                { String, new Regex(@"^((u8|L|u|U)?(R""\()(.*?(\\\"")?(\\\r?\n)?)*?(\)"")(s?)|(u8|L|u|U)?("")(.*?(\\\"")?(\\\r?\n)?)*?("")(s?))") },
                { Char, new Regex(@"^((')((\\.?)|([^\n\r'])|(\\[\dxuU].*?))('))") },

                { WhiteSpace, new Regex(@"^\s+") },
                { Comment, new Regex(@"(^(\/\/[\w\W]*?)(\n|$))|(^\/\*[\w\W]*?\*\/)") },
                { PreProcess, new Regex(@"^((#)((.*?)(\\\r?\n)?)*)(\n|$)") },

                { Int, new Regex(@"^([-+])?((0[xX][\da-fA-F]+('[\da-fA-F]+)*)|(0[bB]\d+('\d+)*)|(\d+('\d+)*))([^\d\W]+)?") },
                { Float, new Regex(@"^([-+]?)(((\d+\.\d*)|(\d*\.\d+)))((e[-+]?\d+)?)([^\d\W]*)") },

                { KeyWord, new Regex(@"^(alignas|alignof|and|and_eq|asm|auto|bitand|bitor|bool|break|case|catch|char|char8_t|char16_t|char32_t|class|compl|concept|const|consteval|constexpr|const_cast|continue|co_await|co_return|co_yield|decltype|default|delete|do|double|dynamic_cast|else|enum|explicit|export|extern|false|float|for|friend|goto|if|inline|int|long|mutable|namespace|new|noexcept|not|not_eq|nullptr|operator|or|or_eq|private|protected|public|register|reinterpret_cast|requires|return|short|signed|sizeof|static|static_assert|static_cast|struct|switch|template|this|thread_local|throw|true|try|typedef|typeid|typename|union|unsigned|using|virtual|void|volatile|wchar_t|while|xor|xor_eq)") },
                { Id,  new Regex(@"^[a-zA-Z_]\w*") },

                { Operator, new Regex(@"^(::|->|\+\+|--|<<|>>|<=|>=|==|!=|&&|\|\||\*=|\/=|%=|\+=|-=|<<=|>>=|&=|\|=|\^=|{|}|\[|\]|#|\(|\)|<|>|%|:|;|\.|\?|\*|\+|-|\/|\^|&|\||~|!|=|,|\\|\""|\')") },
            };
    }
}
