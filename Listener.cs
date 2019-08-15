using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace CppLint
{
    public class Listener : CPPLINTBaseListener
    {
        private enum StateType { NoStatement, FallThrough, Break, Switch };
        private class State
        {
            public StateType Type { get; set; }
            public IToken Token { get; set; }
        }
        private Stack<State> _stateStack = new Stack<State>();

        public Listener()
        {
        }

        public override void EnterSwitchstatement([NotNull] CPPLINTParser.SwitchstatementContext context)
        {
            _stateStack.Push(new State { Type = StateType.Switch, Token = context.Switch().Symbol });
        }

        public override void ExitSwitchstatement([NotNull] CPPLINTParser.SwitchstatementContext context)
        {
            var state = _stateStack.Pop();
            switch (state.Type)
            {
                case StateType.FallThrough:
                case StateType.Switch:
                    FallThroughError(state.Token);
                    break;
                case StateType.NoStatement:
                case StateType.Break:
                    break;
            }
        }

        public override void EnterCasestatement([NotNull] CPPLINTParser.CasestatementContext context)
        {
            var state = _stateStack.Pop();
            switch (state.Type)
            {
                case StateType.FallThrough:
                    FallThroughError(state.Token);
                    break;
                case StateType.NoStatement:
                case StateType.Break:
                case StateType.Switch:
                    break;
            }
            _stateStack.Push(new State { Type = StateType.NoStatement, Token = context.Case().Symbol });
        }

        public override void EnterDefaultstatement([NotNull] CPPLINTParser.DefaultstatementContext context)
        {
            var state = _stateStack.Pop();
            switch (state.Type)
            {
                case StateType.FallThrough:
                    FallThroughError(state.Token);
                    break;
                case StateType.NoStatement:
                case StateType.Break:
                case StateType.Switch:
                    break;
            }
            _stateStack.Push(new State { Type = StateType.NoStatement, Token = context.Default().Symbol });
        }

        public override void EnterStatement([NotNull] CPPLINTParser.StatementContext context)
        {
            if (context.labeledstatement() != null) return;

            if (!_stateStack.TryPeek(out var state)) return;
            switch (state.Type)
            {
                case StateType.NoStatement:
                case StateType.FallThrough:
                case StateType.Break:
                    state.Type = StateType.FallThrough;
                    break;
                case StateType.Switch:
                    break;
            }
        }

        public override void EnterJumpstatement([NotNull] CPPLINTParser.JumpstatementContext context)
        {
            if (!_stateStack.TryPeek(out var state)) return;
            switch (state.Type)
            {
                case StateType.NoStatement:
                case StateType.FallThrough:
                case StateType.Break:
                    state.Type = StateType.Break;
                    break;
                case StateType.Switch:
                    break;
            }
        }

        private void FallThroughError(IToken token)
        {
            Console.WriteLine($"{token.TokenSource.SourceName} ({token.Line},{token.Column}): fall through error");
        }
    }
}
