using System;
using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Result
{
    public abstract class Component : IQueryResult
    {
        public abstract Component Parent { get; }
        public IReadOnlyList<Message> Messages => _messages;
        public IReadOnlyList<Warning> Warnings => _warnings;
        public IReadOnlyList<Error> Errors => _errors;
        
        /// <summary>
        /// `AllMessages`, `AllWarnings` and `AllErrors` are messages, warnings
        /// and errors that occur under this component, either directly or
        /// indirectly.
        /// </summary>
        public IReadOnlyList<Message> AllMessages => _allMessages;
        public IReadOnlyList<Warning> AllWarnings => _allWarnings;
        public IReadOnlyList<Error> AllErrors => _allErrors;

        private readonly List<Message> _messages;
        private readonly List<Warning> _warnings;
        private readonly List<Error> _errors;
        private readonly List<Message> _allMessages;
        private readonly List<Warning> _allWarnings;
        private readonly List<Error> _allErrors;

        public Component()
        {
            _messages = new List<Message>();
            _warnings = new List<Warning>();
            _errors = new List<Error>();

            _allMessages = new List<Message>();
            _allWarnings = new List<Warning>();
            _allErrors = new List<Error>();
        }

        public void AddMessage(Message message)
        {
            _messages.Add(message);
            GoUpToRoot(component => component._allMessages.Add(message));
        }

        public void AddWarning(Warning warning)
        {
            _warnings.Add(warning);
            GoUpToRoot(component => component._allWarnings.Add(warning));
        }

        public void AddError(Error error)
        {
            _errors.Add(error);
            GoUpToRoot(component => component._allErrors.Add(error));
        }

        private void GoUpToRoot(Action<Component> action)
        {
            for (var current = this; current != null; current = current.Parent)
            {
                action.Invoke(current);
            }
        }
    }
}