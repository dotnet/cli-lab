using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Build.Logging.Query.Token
{
    public class TaskToken : Token, IEquatable<TaskToken>
    {
        public TaskToken() : base()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TaskToken);
        }

        public bool Equals([AllowNull] TaskToken other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return nameof(TaskToken).GetHashCode();
        }
    }
}