using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using System;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    internal class EnvironmentVariableOriginator(string name, EnvironmentVariableTarget target, IEnvironment environment) : IOriginator<EnvironmentVariableMemento>
    {
        public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

        public EnvironmentVariableTarget Target { get; } = target;

        public IEnvironment Environment { get; } = environment ?? throw new ArgumentNullException(nameof(environment));

        public string GetID()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return $@"{Target}\{Name}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $@"{Target}\{Name}".ToLower();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return $@"{Target}\{Name}";
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
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
