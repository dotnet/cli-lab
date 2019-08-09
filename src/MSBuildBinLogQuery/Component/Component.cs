using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Messaging;

namespace Microsoft.Build.Logging.Query.Component
{
    public abstract class Component
    {
        public abstract Component Parent { get; }
        public IReadOnlyList<Message> Messages => _messages;
        public IReadOnlyList<Warning> Warnings => _warnings;
        public IReadOnlyList<Error> Errors => _errors;

        private readonly List<Message> _messages;
        private readonly List<Warning> _warnings;
        private readonly List<Error> _errors;

        public Component()
        {
            _messages = new List<Message>();
            _warnings = new List<Warning>();
            _errors = new List<Error>();
        }

        public void AddMessage(Message message)
        {
            _messages.Add(message);
        }

        public void AddWarning(Warning warning)
        {
            _warnings.Add(warning);
        }

        public void AddError(Error error)
        {
            _errors.Add(error);
        }
    }
}