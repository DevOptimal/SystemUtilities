using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using System;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Environment.StateManagement.Serialization
{
    /// <summary>
    /// Goal of this serializer is efficient resource usage by streaming snapshots instead of loading all of them into memory at once.
    /// To accomplish this, we parse and yield return snapshots as we read them from the JSON stream.
    /// </summary>
    internal class EnvironmentSnapshotSerializer(IEnvironment environment) : SnapshotSerializer
    {
        private const string environmentVariableResourceTypeName = "EnvironmentVariable";

        protected override ISnapshot ConvertDictionaryToSnapshot(IDictionary<string, object> dictionary)
        {
            // Get snapshot fields
            var id = AsString(dictionary[nameof(ISnapshot.ID)]);
            var processId = AsInteger(dictionary[nameof(ISnapshot.ProcessID)]);
            var processStartTime = AsDateTime(dictionary[nameof(ISnapshot.ProcessStartTime)]);

            switch (dictionary[resourceTypePropertyName])
            {
                case environmentVariableResourceTypeName:
                    var environmentVariableName = AsString(dictionary[nameof(EnvironmentVariableOriginator.Name)]);
                    var environmentVariableTarget = AsEnum<EnvironmentVariableTarget>(dictionary[nameof(EnvironmentVariableOriginator.Target)]);
                    var environmentVariableValue = AsString(dictionary[nameof(EnvironmentVariableMemento.Value)]);

                    var environmentVariableOriginator = new EnvironmentVariableOriginator(environmentVariableName, environmentVariableTarget, environment);
                    var environmentVariableMemento = new EnvironmentVariableMemento
                    {
                        Value = environmentVariableValue
                    };

                    return new Caretaker<EnvironmentVariableOriginator, EnvironmentVariableMemento>(id, processId, processStartTime, this, environmentVariableOriginator, environmentVariableMemento);
                default: throw new Exception();
            }
        }

        protected override IDictionary<string, object> ConvertSnapshotToDictionary(ISnapshot snapshot)
        {
            var result = new Dictionary<string, object>
            {
                [nameof(ISnapshot.ID)] = snapshot.ID,
                [nameof(ISnapshot.ProcessID)] = snapshot.ProcessID,
                [nameof(ISnapshot.ProcessStartTime)] = snapshot.ProcessStartTime.Ticks
            };

            switch (snapshot)
            {
                case Caretaker<EnvironmentVariableOriginator, EnvironmentVariableMemento> environmentVariableCaretaker:
                    result[resourceTypePropertyName] = environmentVariableResourceTypeName;
                    result[nameof(EnvironmentVariableOriginator.Name)] = environmentVariableCaretaker.Originator.Name;
                    result[nameof(EnvironmentVariableOriginator.Target)] = environmentVariableCaretaker.Originator.Target.ToString();
                    result[nameof(EnvironmentVariableMemento.Value)] = environmentVariableCaretaker.Memento.Value;
                    break;
                default: throw new Exception();
            }

            return result;
        }
    }
}
