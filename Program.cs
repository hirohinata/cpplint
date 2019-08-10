using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CppLint
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputStream = new AntlrInputStream(
@"#include <iostream>

int main()
{
    return 0;
}
");
            var lexer = new CPPLINTLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CPPLINTParser(tokenStream);
            var tree = parser.translationunit();
            var walker = new ParseTreeWalker();
            walker.Walk(new Listener(), tree);
        }
    }
}
