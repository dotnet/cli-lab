using System;
using System.Collections.Generic;

namespace Microsoft.DotNet.Tools.Uninstall.Shared.Configs.Verbosity
{
    internal class VerbosityHandler<TArg>
    {
        private readonly Dictionary<VerbosityLevel, Action<TArg>> _actions;

        public VerbosityHandler()
        {
            _actions = new Dictionary<VerbosityLevel, Action<TArg>>();
            Register(VerbosityLevel.Quiet, bundle => { });
        }

        public void Register(VerbosityLevel level, Action<TArg> action)
        {
            _actions.Add(level, action);
        }

        public void Execute(VerbosityLevel level, TArg argValue)
        {
            for (; level >= 0; level--)
            {
                if (_actions.TryGetValue(level, out var action))
                {
                    action.Invoke(argValue);
                    return;
                }
            }
        }
    }
}
