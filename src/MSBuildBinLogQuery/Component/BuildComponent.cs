using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Build.Logging.Query.Messaging;

namespace Microsoft.Build.Logging.Query.Component
{
    public abstract class BuildComponent
    {
        public IList<Message> Messages => _messages.ToImmutableList();
        public IList<Warning> Warnings => _warnings.ToImmutableList();
        public IList<Error> Errors => _errors.ToImmutableList();
        public IList<Message> AllMessages => _allMessages.ToImmutableList();
        public IList<Warning> AllWarnings => _allWarnings.ToImmutableList();
        public IList<Error> AllErrors => _allErrors.ToImmutableList();
        public abstract BuildComponent Parent { get; }

        private readonly IList<Message> _messages;
        private readonly IList<Warning> _warnings;
        private readonly IList<Error> _errors;
        private readonly IList<Message> _allMessages;
        private readonly IList<Warning> _allWarnings;
        private readonly IList<Error> _allErrors;

        public BuildComponent()
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

            for (var current = this; current != null; current = current.Parent)
            {
                current._allMessages.Add(message);
            }
        }

        public void AddWarning(Warning warning)
        {
            _warnings.Add(warning);

            for (var current = this; current != null; current = current.Parent)
            {
                current._allWarnings.Add(warning);
            }
        }

        public void AddError(Error error)
        {
            _errors.Add(error);

            for (var current = this; current != null; current = current.Parent)
            {
                current._allErrors.Add(error);
            }
        }
    }
}