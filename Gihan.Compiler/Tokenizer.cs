using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gihan.Compiler
{
    public class Tokenizer
    {
        private Buffer Buffer { get; set; }

        public Tokenizer(Buffer buffer)
        {
            Buffer = buffer;
        }

        public bool IsTokeningCompleted
            => Buffer.IsEndOfStream;

        public Token GetNextToken()
        {
            (string Key, Match Match) preToken;
            string head = "";
            do
            {
                head += Buffer.Get();
                if (string.IsNullOrEmpty(head))
                    return null;
                preToken = TokenSet.TokenRegexSet
                    .Select(s => (s.Key, Match: s.Value.Match(head)))
                        .FirstOrDefault(m => m.Match.Success);
                if (preToken.Key is null)
                    throw new Exceptions.LexicalException(head);
                if (preToken.Key == TokenSet.Operator)
                {
                    if (head.StartsWith("\""))
                    {
                        preToken.Match = null;
                        var lines = head.Split('\n').Select(l => l.TrimEnd('\r'));
                        var WrongLines = lines.Where(l => !l.EndsWith("\\"));
                        if (WrongLines.Count() > 1 || (WrongLines.Count() == 1 && lines.Last() != WrongLines.First()))
                            throw new Exceptions.LexicalException(head);
                    }
                    else if (head.StartsWith("/*"))
                    {
                        preToken.Match = null;
                    }
                    else if (head.StartsWith("#"))
                    {
                        preToken.Match = null;
                        var lines = head.Split('\n').Select(l => l.TrimEnd('\r'));
                        var WrongLines = lines.Where(l => !l.EndsWith("\\"));
                        if (WrongLines.Count() > 1 || (WrongLines.Count() == 1 && lines.Last() != WrongLines.First()))
                            throw new Exceptions.LexicalException(head);
                    }
                }
                if (preToken.Key == TokenSet.KeyWord)
                {
                    var idMatch = TokenSet.TokenRegexSet[TokenSet.Id].Match(head);
                    if (idMatch.Value != preToken.Match.Value)
                    {
                        preToken.Key = TokenSet.Id;
                        preToken.Match = idMatch;
                    }
                }
                if (preToken.Match?.Length < head.Length || Buffer.IsEndOfStream)
                    break;
                Buffer.Pop(Buffer.HeadStringSize);
            } while (true);
            var value = preToken.Match.Value;
            Buffer.Pop(value.Length % Buffer.HeadStringSize);
            if (preToken.Key == TokenSet.Id)
            {
                var symbolPair = SymbolTable.Global.Symbols.FirstOrDefault(p => p.Value.Value == value);
                if (symbolPair.Value != null)
                    value = symbolPair.Key.ToString();
                else
                    value = SymbolTable.Global.AddSymbol(value).ToString();
            }
            if (preToken.Key == TokenSet.KeyWord)
            {
                var symbolPair = SymbolTable.Global.Symbols.FirstOrDefault(p => p.Value.Value == value);
                if (symbolPair.Value != null)
                    value = symbolPair.Key.ToString();
                else
                    throw new Exceptions.SyntaxException("syntax is not defined");
            }
            return new Token()
            {
                Key = preToken.Key,
                Value = preToken.Key == TokenSet.WhiteSpace || preToken.Key == TokenSet.Comment ?
                    null : value,
                File = Buffer.FileInfo.FullName,
                Line = Buffer.Line,
                Column = Buffer.Column,
            };
        }
    }
}
