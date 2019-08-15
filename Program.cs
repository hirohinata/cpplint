using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CppLint
{
    class Program
    {
        static int Main(string[] args)
        {
            var errorList = new List<string>();
            foreach (var fileName in args)
            {
                try
                {
                    Lint(ref errorList, fileName);
                }
                catch (Exception e)
                {
                    errorList.Add(e.Message);
                }
            }

            foreach (var error in errorList)
            {
                Console.WriteLine(error);
            }
            return errorList.Count == 0 ? 0 : -1;
        }

        private static void Lint(ref List<string> errorList, string fileName)
        {
            var inputStream = new AntlrFileStream(fileName);
            var lexer = new CPPLINTLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CPPLINTParser(tokenStream);
            var tree = parser.translationunit();
            var walker = new ParseTreeWalker();
            walker.Walk(new Listener(ref errorList), tree);
        }
    }
}
