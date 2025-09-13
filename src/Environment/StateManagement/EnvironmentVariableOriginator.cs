﻿using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using System;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    internal class EnvironmentVariableOriginator : IOriginator<EnvironmentVariableMemento>
    {
        public string Name { get; }

        public EnvironmentVariableTarget Target { get; }

        public IEnvironment Environment { get; }

        public EnvironmentVariableOriginator(string name, EnvironmentVariableTarget target, IEnvironment environment)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Target = target;
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public EnvironmentVariableMemento GetState()
        {
            return new EnvironmentVariableMemento
            {
                Value = Environment.GetEnvironmentVariable(Name, Target)
            };
        }

        public void SetState(EnvironmentVariableMemento memento)
        {
            Environment.SetEnvironmentVariable(Name, memento.Value, Target);
        }
    }
}
