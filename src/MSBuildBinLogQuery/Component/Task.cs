using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Component
{
    public class Task : Component, IEquatable<Task>
    {
        public int Id { get; }
        public string Name { get; }
        public string TaskFile { get; }
        public Target ParentTarget { get; }
        public PropertyManager Parameters { get; }

        public Task(int id, string name, string taskFile, Target parentTarget) : base()
        {
            Id = id;
            Name = name;
            TaskFile = taskFile;
            ParentTarget = parentTarget;
            Parameters = new PropertyManager();
        }

        public bool Equals([AllowNull] Task other)
        {
            return other != null &&
                   Id == other.Id &&
                   EqualityComparer<Target>.Default.Equals(ParentTarget, other.ParentTarget);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
