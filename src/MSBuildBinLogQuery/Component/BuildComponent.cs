using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Messaging;

namespace Microsoft.Build.Logging.Query.Component
{
    public abstract class BuildComponent
    {
        public IList<Message> Messages { get; }
        public IList<Warning> Warnings { get; }
        public IList<Error> Errors { get; }

        public BuildComponent()
        {
            Messages = new List<Message>();
            Warnings = new List<Warning>();
            Errors = new List<Error>();
        }
    }
}