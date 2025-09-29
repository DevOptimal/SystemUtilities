using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using System;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Environment.StateManagement.Serialization
{
    /// <summary>
    /// Goal of this serializer is efficient resource usage by streaming caretakers instead of loading all of them into memory at once.
    /// To accomplish this, we parse and yield return caretakers as we read them from the JSON stream.
    /// </summary>
    internal class EnvironmentCaretakerSerializer(IEnvironment environment) : CaretakerSerializer
    {
        private const string environmentVariableResourceTypeName = "EnvironmentVariable";

        protected override ICaretaker ConvertDictionaryToCaretaker(IDictionary<string, object> dictionary, DatabaseConnection connection)
        {
            // Get caretaker fields
            var id = AsString(dictionary[nameof(ICaretaker.ID)]);
            var parentId = AsString(dictionary[nameof(ICaretaker.ParentID)]);
            var processId = AsInteger(dictionary[nameof(ICaretaker.ProcessID)]);
            var processStartTime = AsDateTime(dictionary[nameof(ICaretaker.ProcessStartTime)]);

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

                    return new Caretaker<EnvironmentVariableOriginator, EnvironmentVariableMemento>(id, parentId, processId, processStartTime, connection, environmentVariableOriginator, environmentVariableMemento);
                default: throw new Exception();
            }
        }

        protected override IDictionary<string, object> ConvertCaretakerToDictionary(ICaretaker caretaker)
        {
            var result = new Dictionary<string, object>
            {
                [nameof(ICaretaker.ID)] = caretaker.ID,
                [nameof(ICaretaker.ParentID)] = caretaker.ParentID,
                [nameof(ICaretaker.ProcessID)] = caretaker.ProcessID,
                [nameof(ICaretaker.ProcessStartTime)] = caretaker.ProcessStartTime.Ticks
            };

            switch (caretaker)
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
