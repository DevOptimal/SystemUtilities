using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using System;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Environment.StateManagement.Serialization
{
    /// <summary>
    /// Serializes and deserializes environment variable caretakers for efficient resource usage.
    /// Streams caretakers from JSON to avoid loading all into memory at once.
    /// </summary>
    internal class EnvironmentCaretakerSerializer : CaretakerSerializer
    {
        /// <summary>
        /// The resource type name for environment variable caretakers in serialized data.
        /// </summary>
        private const string environmentVariableResourceTypeName = "EnvironmentVariable";

        private readonly IEnvironment environment;

        public EnvironmentCaretakerSerializer(IEnvironment environment)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        /// <summary>
        /// Converts a dictionary representation of a caretaker to an <see cref="ICaretaker"/> instance.
        /// Supports only environment variable caretakers.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="connection">The database connection context.</param>
        /// <returns>The deserialized <see cref="ICaretaker"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown if the resource type is not supported.</exception>
        protected override ICaretaker ConvertDictionaryToCaretaker(IDictionary<string, object> dictionary, DatabaseConnection connection)
        {
            // Extract common caretaker fields
            var id = AsString(dictionary[nameof(ICaretaker.ID)]);
            var parentId = AsString(dictionary[nameof(ICaretaker.ParentID)]);
            var processId = AsInteger(dictionary[nameof(ICaretaker.ProcessID)]);
            var processStartTime = AsDateTime(dictionary[nameof(ICaretaker.ProcessStartTime)]);

            // Determine resource type and deserialize accordingly
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

                    return new EnvironmentVariableCaretaker(id, parentId, processId, processStartTime, connection, environmentVariableOriginator, environmentVariableMemento);
                default: throw new NotSupportedException($"The resource type '{dictionary[resourceTypePropertyName]}' is not supported.");
            }
        }

        /// <summary>
        /// Converts an <see cref="ICaretaker"/> instance to a dictionary representation for serialization.
        /// Supports only environment variable caretakers.
        /// </summary>
        /// <param name="caretaker">The caretaker to convert.</param>
        /// <returns>The dictionary representation.</returns>
        /// <exception cref="NotSupportedException">Thrown if the caretaker type is not supported.</exception>
        protected override IDictionary<string, object> ConvertCaretakerToDictionary(ICaretaker caretaker)
        {
            // Add common caretaker fields
            var result = new Dictionary<string, object>
            {
                [nameof(ICaretaker.ID)] = caretaker.ID,
                [nameof(ICaretaker.ParentID)] = caretaker.ParentID,
                [nameof(ICaretaker.ProcessID)] = caretaker.ProcessID,
                [nameof(ICaretaker.ProcessStartTime)] = caretaker.ProcessStartTime.Ticks
            };

            // Serialize based on caretaker type
            switch (caretaker)
            {
                case EnvironmentVariableCaretaker environmentVariableCaretaker:
                    result[resourceTypePropertyName] = environmentVariableResourceTypeName;
                    result[nameof(EnvironmentVariableOriginator.Name)] = environmentVariableCaretaker.Originator.Name;
                    result[nameof(EnvironmentVariableOriginator.Target)] = environmentVariableCaretaker.Originator.Target.ToString();
                    result[nameof(EnvironmentVariableMemento.Value)] = environmentVariableCaretaker.Memento.Value;
                    break;
                default: throw new NotSupportedException($"The caretaker type '{caretaker.GetType().Name}' is not supported.");
            }

            return result;
        }
    }
}
