using System;
using Antlr4.Runtime.Misc;

namespace CppLint
{
    public class Listener : CPPLINTBaseListener
    {
        public Listener()
        {
        }

        public override void EnterSelectionstatement([NotNull] CPPLINTParser.SelectionstatementContext context)
        {
        }

        public override void ExitSelectionstatement([NotNull] CPPLINTParser.SelectionstatementContext context)
        {
        }
    }
}
