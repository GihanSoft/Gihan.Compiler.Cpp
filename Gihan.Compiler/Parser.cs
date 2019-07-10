using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gihan.Compiler
{
    public class Parser
    {
        private TokenStream TokenStream { get; set; }
        private Token Head() => TokenStream.GetHead();

        public Parser(TokenStream tokenStream)
        {
            TokenStream = tokenStream;
        }

        public void Match(string value)
        {
            string tokenVal;
            var token = TokenStream.Read();
            if (token.Key != TokenSet.KeyWord && token.Key != TokenSet.Id)
                tokenVal = token.Value;
            else
                tokenVal = SymbolTable.Global.GetSymbol(token).Value;
            if (value != tokenVal)
                throw new Exceptions.SyntaxException(token, "Excepted " + value);
        }


        public void MatchTokenType(string type)
        {
            var token = TokenStream.Read();
            if (token.Key != type)
                throw new Exceptions.SyntaxException(Head(), "Excepted type " + type);
        }

        public bool NameSpaceName()
        {
            MatchTokenType(TokenSet.Id);
            var hasInnerNameSpace = false;
            while (Head().Value == "::")
            {
                Match("::");
                MatchTokenType(TokenSet.Id);
                hasInnerNameSpace = true;
            }
            return hasInnerNameSpace;
        }

        public void Using()
        {
            Match("using");
            if (SymbolTable.Global.GetSymbol(Head()).Value == "namespace")
            {
                Match("namespace");
                NameSpaceName();

                Match(";");
            }
            else
            {
                var hasInnerNameSpace = NameSpaceName();
                if (!hasInnerNameSpace)
                {
                    throw new Exceptions.SyntaxException(Head(), "Excepted NameSpace");
                }

                Match(";");
            }
        }

        private void ClassAndStructInternal()
        {
            var symbol = SymbolTable.Global.GetSymbol(Head());
            switch (symbol.Value)
            {
                case "class":
                    Class();
                    break;
                case "struct":
                    Struct();
                    break;
                case "enum":
                    Enum();
                    break;
                default:
                    Type();
                    var isArr = Name();
                    if (Head().Value == "(")
                    {
                        if (isArr)
                            throw new Exceptions.SyntaxException(Head(), "Excepted Function Name");
                        Match("(");
                        Arguments();
                        Match(")");
                        if (Head().Value == ";")
                        {
                            Match(";");
                        }
                        else
                        {
                            Match("{");
                            StmntList();
                            Match("}");
                        }
                    }
                    else
                    {
                        if (Head().Value == ",")
                        {
                            Match(",");
                            NameList();
                        }
                        Match(";");
                    }
                    break;
            }
            if (Head().Value != "}")
                ClassAndStructInternal();
        }

        public void Class()
        {
            Match("class");
            MatchTokenType(TokenSet.Id);
            if (Head().Value == ";")
            {
                Match(";");
                return;
            }
            if (Head().Value == ":")
            {
                // inherinace ...
            }
            Match("{");
            ClassAndStructInternal();
            Match("}");
            Match(";");
        }

        private void Struct()
        {
            Match("struct");
            if (Head().Value != "{")
            {
                MatchTokenType(TokenSet.Id);
            }
            Match("{");
            ClassAndStructInternal();
            Match("}");
            if (Head().Value != ";")
            {
                NameList();
            }
            Match(";");
        }

        public void Enum()
        {
            Match("enum");

            if (Head().Value != "{")
            {
                MatchTokenType(TokenSet.Id);

            }

            Match("{");
            // enum internal
            Match("}");

            if (Head().Value != ";")
            {
                NameList();
            }
            Match(";");
        }

        public bool Type()
        {
            if (Head().Key == TokenSet.Id)
            {
                NameSpaceName();
                if (Head().Key == TokenSet.Operator)
                {
                    if (Head().Value == "&"
                        || Head().Value == "&&")
                    {
                        Match(Head().Value);
                        return false;
                    }
                }
                return false;
            }
            if (Head().Key != TokenSet.KeyWord)
                throw new Exceptions.SyntaxException(Head(), "Excepted KeyWord");
            var symbol = SymbolTable.Global.GetSymbol(Head());
            var keywordTypes = new[] {"signed", "unsigned", "short", "long",
                                      "char", "int", "float", "double", "bool",
                                      "void" };
            if (!keywordTypes.Contains(symbol.Value))
                throw new Exceptions.SyntaxException(Head(), "Excepted Keyword Type");
            if (symbol.Value == "signed" || symbol.Value == "unsigned")
            {
                Match(symbol.Value);
                symbol = SymbolTable.Global.GetSymbol(Head());
                var signAllowed = new[] { "char", "short", "int", "long" };
                if (!signAllowed.Contains(symbol.Value))
                    throw new Exceptions.SyntaxException(Head(), "Excepted char | short | int | long");
            }
            if (symbol.Value == "short")
            {
                Match("short");
                if (Head().Key == TokenSet.Operator)
                {
                    if (Head().Value == "&"
                        || Head().Value == "&&")
                    {
                        Match(Head().Value);
                        return true;
                    }
                    else if (Head().Value == "*")
                        return true;
                }
                symbol = SymbolTable.Global.GetSymbol(Head());
                if (symbol.Value == "int")
                {
                    Match("int");
                    if (Head().Key == TokenSet.Operator)
                    {
                        if (Head().Value == "&"
                            || Head().Value == "&&")
                        {
                            Match(Head().Value);
                            return true;
                        }
                    }
                }
                return true;
            }
            if (symbol.Value == "long")
            {
                Match("long");
                if (Head().Key == TokenSet.Operator)
                {
                    if (Head().Value == "&"
                        || Head().Value == "&&")
                    {
                        Match(Head().Value);
                        return true;
                    }
                    else if (Head().Value == "*")
                        return true;
                }
                symbol = SymbolTable.Global.GetSymbol(Head());
                if (symbol.Value == "long")
                {
                    Match("long");
                    if (Head().Key == TokenSet.Operator)
                    {
                        if (Head().Value == "&"
                            || Head().Value == "&&")
                        {
                            Match(Head().Value);
                            return true;
                        }
                        else if (Head().Value == "*")
                            return true;
                    }
                }
                symbol = SymbolTable.Global.GetSymbol(Head());
                if (symbol.Value == "int" || symbol.Value == "double")
                {
                    Match(symbol.Value);
                    if (Head().Key == TokenSet.Operator)
                    {
                        if (Head().Value == "&"
                            || Head().Value == "&&")
                        {
                            Match(Head().Value);
                            return true;
                        }
                    }
                    return true;
                }
                return true;
            }
            Match(symbol.Value);
            return true;
        }

        public bool Name()
        {
            var isArr = false;

            while (Head().Key != TokenSet.Id)
            {
                Match("*");
            }
            MatchTokenType(TokenSet.Id);

            while (Head().Value == "[")
            {
                Match("[");
                if (Head().Value != "]")
                    MatchTokenType(TokenSet.Int);
                Match("]");
                isArr = true;
            }
            return isArr;
        }

        public void NameList()
        {
            Name();

            while (Head().Value == ",")
            {
                Match(",");

                Name();

            }
        }

        private void Arguments()
        {
            if (Head().Value == ")")
                return;
            Type();
            Name();
            while (Head().Value == ",")
            {
                Match(",");

                Type();
                var isArr = Name();
                if (isArr)
                    throw new Exceptions.SyntaxException(Head(), "Array Argumant not supported");

            }
        }

        #region Expr
        private void Factor()
        {
            if (Head().Value == "(")
            {
                Match("(");
                Expr();
                Match(")");
            }
            var factorAllowed = new[] { TokenSet.String, TokenSet.Char, TokenSet.Int, TokenSet.Float };
            if (factorAllowed.Contains(Head().Key))
            {
                MatchTokenType(Head().Key);
            }
        }

        private void G0()
        {
            if (Head().Key == TokenSet.Id)
            {
                MatchTokenType(TokenSet.Id);

                if (Head().Value == "++" || Head().Value == "--")
                {
                    Match(Head().Value);
                    return;
                }
                while (Head().Value == "(" || Head().Value == "[")
                {
                    if (Head().Value == "(")
                    {
                        Match("(");
                        if (Head().Value != ")")
                        {
                            Expr();
                        }
                        while (Head().Value != ")")
                        {
                            Match(",");
                            Expr();
                        }
                        Match(")");
                    }
                    if (Head().Value == "[")
                    {
                        Match("[");
                        Expr();
                        Match("]");
                    }
                }
            }
            else
                Factor();
        }

        private void G1()
        {
            var g1op = new[] { "++", "--", "~", "!", "-", "+", "&", "*" };
            if (g1op.Contains(Head().Value))
            {
                Match(Head().Value);
                G1();
            }
            else
                G0();
        }

        private void G2()
        {
            G1();
            G2P();
        }

        private void G2P()
        {
            var g2op = new[] { "<<", ">>" };
            if (g2op.Contains(Head().Value))
            {
                Match(Head().Value);
                G2();
            }
        }

        private void G3()
        {
            G2();
            G3P();
        }

        private void G3P()
        {
            var g3op = new[] { "*", "/", "%" };
            if (g3op.Contains(Head().Value))
            {
                Match(Head().Value);
                G3();
            }
        }

        private void G4()
        {
            G3();
            G4P();
        }

        private void G4P()
        {
            var g4op = new[] { "+", "-" };
            if (g4op.Contains(Head().Value))
            {
                Match(Head().Value);
                G4();
            }
        }

        private void G5()
        {
            G4();
            G5P();
        }

        private void G5P()
        {
            var g5op = new[] { "<<", ">>" };
            if (g5op.Contains(Head().Value))
            {
                Match(Head().Value);
                G5();
            }
        }

        private void G6()
        {
            G5();
            G6P();
        }

        private void G6P()
        {
            var g6op = new[] { ">", "<", ">=", "<=" };
            if (g6op.Contains(Head().Value))
            {
                Match(Head().Value);
                G6();
            }
        }

        private void G7()
        {
            G6();
            G7P();
        }

        private void G7P()
        {
            var g7op = new[] { "==", "!=" };
            if (g7op.Contains(Head().Value))
            {
                Match(Head().Value);
                G7();
            }
        }

        private void G8()
        {
            G7();
            G8P();
        }

        private void G8P()
        {
            if (Head().Value == "&")
            {
                Match("&");
                G8();
            }
        }

        private void G9()
        {
            G8();
            G9P();
        }

        private void G9P()
        {
            if (Head().Value == "^")
            {
                Match("^");
                G9();
            }
        }

        private void G10()
        {
            G9();
            G10P();
        }

        private void G10P()
        {
            if (Head().Value == "|")
            {
                Match("|");
                G10();
            }
        }

        private void G11()
        {
            G10();
            G11P();
        }

        private void G11P()
        {
            if (Head().Value == "&&")
            {
                Match("&&");
                G11();
            }
        }

        private void G12()
        {
            G11();
            G12P();
        }

        private void G12P()
        {
            if (Head().Value == "||")
            {
                Match("||");
                G12();
            }
        }

        private void G13()
        {
            G12();
            G13P();
        }

        private void G13P()
        {
            var g13ops = new[] { "=", "*=", "/=", "%=", "+=", "-=", "<<=", ">>=", "&=", "|=", "^=" };
            if (g13ops.Contains(Head().Value))
            {
                Match(Head().Value);
                G13();
            }
        }

        #endregion

        private void Expr()
        {
            G13();
        }

        private void ElsePart()
        {

            var symbol = SymbolTable.Global.GetSymbol(Head());
            if (symbol?.Value == "else")
            {
                Match("else");
                Stmnt();
            }
        }

        private void IfStmnt()
        {
            Match("if");

            Match("(");
            Expr();

            Match(")");

            Stmnt();
            ElsePart();
        }

        private readonly string[] assignAllowOps = new[] { "=", "*=", "/=", "%=", "+=", "-=",
                            "<<=", ">>=", "&=", "|=", "^=" };

        private void Stmnt()
        {
            if (Head().Value == "{")
            {
                Match("{");
                StmntList();
                Match("}");
                return;
            }
            if (Head().Value == ";")
            {
                Match(";");
                return;
            }
            if (Head().Value == "*")
            {
                Name();
                var assignAllowOps = new[] { "=", "*=", "/=", "%=", "+=", "-=",
                            "<<=", ">>=", "&=", "|=", "^=" };
                if (assignAllowOps.Contains(Head().Value))
                {
                    Match(Head().Value);
                    Expr();
                    Match(";");
                    return;
                }
                else if (Head().Value == "++" || Head().Value == "--")
                {
                    Match(Head().Value);
                    Match(";");
                    return;
                }
                else
                    throw new Exceptions.SyntaxException(Head());
            }
            if (Head().Value == "++" || Head().Value == "--")
            {
                Match(Head().Value);
                Name();
                Match(";");
                return;
            }
            var symbol = SymbolTable.Global.GetSymbol(Head());
            switch (symbol.Value)
            {
                case "if":
                    IfStmnt();
                    break;
                case "while":
                    Match("while");
                    Match("(");
                    Expr();
                    Match(")");
                    Stmnt();
                    break;
                case "do":
                    Match("do");
                    Stmnt();
                    Match("while");
                    Match("(");
                    Expr();
                    Match(")");
                    Match(";");
                    break;
                case "for":
                    Match("for");
                    Match("(");

                    if (Head().Value == "*")
                    {
                        Expr();
                    }
                    else
                    {
                        Type();
                        if (Head().Key == TokenSet.Id)
                        {
                            MatchTokenType(TokenSet.Id);
                        }
                        if (!assignAllowOps.Contains(Head().Value))
                        {
                            throw new Exceptions.SyntaxException(Head(), "Excepted Assign");
                        }
                        Match(Head().Value);
                        Expr();
                    }

                    Match(";");
                    Expr();
                    Match(";");
                    Expr();
                    Match(")");
                    Stmnt();
                    break;
                case "return":
                    Match("return");
                    if (Head().Value == ";")
                    {
                        Match(";");
                        return;
                    }
                    if (Head().Value == ";")
                    {
                        Match(";");
                        return;
                    }
                    Expr();
                    Match(";");
                    break;
                default:
                    if (Head().Value == "*")
                    {
                        Name();
                        if (assignAllowOps.Contains(Head().Value))
                        {
                            Match(Head().Value);
                            Expr();
                            Match(";");
                            return;
                        }
                        else if (Head().Value == "++" || Head().Value == "--")
                        {
                            Match(Head().Value);
                            Match(";");
                            return;
                        }
                    }
                    else if (Head().Value == "--" || Head().Value == "++")
                    {
                        Match(Head().Value);
                        Name();
                        Match(";");
                        return;
                    }
                    else if (Head().Key == TokenSet.Id)
                    {
                        MatchTokenType(TokenSet.Id);
                        if (Head().Value == "(")
                        {
                            Match("(");
                            if (Head().Value != ")")
                            {
                                Expr();
                            }
                            while (Head().Value != ")")
                            {
                                Match(",");
                                Expr();
                            }
                            Match(")");
                            Match(";");
                            return;
                        }
                        else if (assignAllowOps.Contains(Head().Value))
                        {
                            Match(Head().Value);
                            Expr();
                            Match(";");
                            return;
                        }
                        else if (Head().Value == "++" || Head().Value == "--")
                        {
                            Match(Head().Value);
                            Match(";");
                            return;
                        }
                        else
                        {
                            if (Head().Value == "&" || Head().Value == "&&")
                            {
                                Match(Head().Value);
                            }
                            NameList();
                            Match(";");
                            return;
                        }
                    }
                    else if (Head().Key == TokenSet.KeyWord)
                    {
                        Type();
                        NameList();
                        if (Head().Value == "=")
                        {
                            Match("=");
                            Expr();
                        }
                        Match(";");
                        return;
                    }
                    throw new Exceptions.SyntaxException(Head(), "No match for code");
            }
        }

        private void StmntList()
        {
            while (Head().Value != "}")
            {
                Stmnt();
            }
        }

        public void CodeList()
        {
            while (Head().Value != "}")
            {
                Code();
            }
        }

        public void Code()
        {
            var symbol = SymbolTable.Global.GetSymbol(Head());
            switch (symbol.Value)
            {
                case "using":
                    Using();
                    break;
                case "namespace":
                    Match("namespace");
                    MatchTokenType(TokenSet.Id);
                    Match("{");
                    if (Head().Value != "}")
                    {
                        CodeList();
                    }
                    Match("}");
                    break;
                case "class":
                    Class();
                    break;
                case "struct":
                    Struct();
                    break;
                case "enum":
                    Enum();
                    break;
                case "typedef":
                    //Match("typedef");
                    //if (SymbolTable.Global.GetSymbol(Head()).Value == "struct")
                    //    Code();
                    //Type();
                    //MatchTokenType(TokenSet.Id);
                    break;
                default:
                    Type();
                    var isArr = Name();
                    if (Head().Value == "(")
                    {
                        if (isArr)
                            throw new Exceptions.SyntaxException(Head(), "Excepet Function Name");
                        Match("(");
                        Arguments();
                        Match(")");
                        if (Head().Value == ";")
                        {
                            Match(";");
                        }
                        else
                        {
                            Match("{");
                            StmntList();
                            Match("}");
                        }
                    }
                    else
                    {
                        if (Head().Value == ",")
                        {
                            Match(",");
                            NameList();
                        }
                        Match(";");
                    }
                    break;
            }
        }

        public bool IsRunning { get; private set; }

        public void Run()
        {
            IsRunning = true;
            while (!TokenStream.Disposed)
            {
                while ((!TokenStream.IsBufferFull) && (!TokenStream.Ended))
                    Thread.Sleep(100);
                switch (Head()?.Key)
                {
                    case null:
                        break;
                    case TokenSet.KeyWord:
                    case TokenSet.Id:
                        Code();
                        break;
                    case TokenSet.PreProcess:
                    case TokenSet.WhiteSpace:
                    case TokenSet.Comment:
                    default:
                        TokenStream.Read();
                        break;
                }
            }
            IsRunning = false;
        }
    }
}
