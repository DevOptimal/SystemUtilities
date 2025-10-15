// Implements a caretaker serializer for registry resources, supporting streaming and efficient resource usage.
using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevOptimal.SystemUtilities.Registry.StateManagement.Serialization
{
    /// <summary>
    /// Serializes and deserializes caretakers for registry resources (keys and values).
    /// Designed for efficient resource usage by streaming caretakers from a JSON source.
    /// </summary>
    internal class RegistryCaretakerSerializer : CaretakerSerializer
    {
        // ------------------------------------------------------------------------------------------------------------
        //  Discriminator constants
        // ------------------------------------------------------------------------------------------------------------
        //  We serialize a heterogeneous collection (registry keys + registry values). A stable discriminator value
        //  identifies the concrete resource type so that we can reconstruct the correct originator + memento pair.
        //  These values are persisted verbatim into JSON under the base class property name 'resourceTypePropertyName'.
        /// <summary>
        /// Discriminator value written to <see cref="CaretakerSerializer.resourceTypePropertyName"/> for registry key caretakers.
        /// </summary>
        private const string registryKeyResourceTypeName = "RegistryKey";
        /// <summary>
        /// Discriminator value written to <see cref="CaretakerSerializer.resourceTypePropertyName"/> for registry value caretakers.
        /// </summary>
        private const string registryValueResourceTypeName = "RegistryValue";

        // ------------------------------------------------------------------------------------------------------------
        //  Registry value encoding constants
        // ------------------------------------------------------------------------------------------------------------
        //  Registry values can have multiple primitive shapes (byte[], int, long, string, string[] ... ). To avoid
        //  emitting verbose JSON objects for each value we pack them into a compact single Base64 string with a
        //  leading one-byte type discriminator. The current mapping is:
        //    0x0 => byte[]        : raw bytes follow
        //    0x1 => int           : 4 bytes little-endian
        //    0x2 => long          : 8 bytes little-endian
        //    0x3 => string        : ASCII bytes (NOTE: only ASCII supported; extended chars would be lost)
        //    0x4 => string[]      : sequence of ASCII segments separated by single 0x0 terminator bytes
        //  (A future enhancement could introduce UTF-8 support via a new discriminator value.)
        private const byte byteArrayType = 0x0;
        private const byte intType = 0x1;
        private const byte longType = 0x2;
        private const byte stringType = 0x3;
        private const byte stringArrayType = 0x4;
        private const byte segmentTerminator = 0x0; // Terminator for individual strings in a serialized string[]

        /// <summary>
        /// Registry abstraction used to create originators during deserialization.
        /// </summary>
        private readonly IRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryCaretakerSerializer"/> class.
        /// </summary>
        /// <param name="registry">The registry abstraction used for originator operations.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registry"/> is null.</exception>
        public RegistryCaretakerSerializer(IRegistry registry)
        {
            // (We intentionally do not throw here because the interface usage patterns guarantee a valid instance.)
            this.registry = registry;
        }

        /// <summary>
        /// Converts a dictionary representation of a caretaker to an <see cref="ICaretaker"/> instance.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="connection">The database connection context.</param>
        /// <returns>The deserialized caretaker.</returns>
        /// <exception cref="NotSupportedException">Thrown if the resource type discriminator is unknown.</exception>
        protected override ICaretaker ConvertDictionaryToCaretaker(IDictionary<string, object> dictionary, DatabaseConnection connection)
        {
            // Common caretaker metadata (these come from the base serializer contract).
            // DateTime is stored as ticks for space & precision; conversion occurs in AsDateTime.
            var id = AsString(dictionary[nameof(ICaretaker.ID)]);
            var parentId = AsString(dictionary[nameof(ICaretaker.ParentID)]);
            var processId = AsInteger(dictionary[nameof(ICaretaker.ProcessID)]);
            var processStartTime = AsDateTime(dictionary[nameof(ICaretaker.ProcessStartTime)]);

            // Dispatch based on resource type discriminator.
            switch (dictionary[resourceTypePropertyName])
            {
                case registryKeyResourceTypeName:
                    // Registry Key caretaker reconstruction
                    var registryKeyHive = AsEnum<RegistryHive>(dictionary[nameof(RegistryKeyOriginator.Hive)]);
                    var registryKeyView = AsEnum<RegistryView>(dictionary[nameof(RegistryKeyOriginator.View)]);
                    var registryKeySubKey = AsString(dictionary[nameof(RegistryKeyOriginator.SubKey)]);
                    var registryKeyExists = AsBoolean(dictionary[nameof(RegistryKeyMemento.Exists)]);

                    var registryKeyOriginator = new RegistryKeyOriginator(registryKeyHive, registryKeyView, registryKeySubKey, registry);
                    var registryKeyMemento = new RegistryKeyMemento { Exists = registryKeyExists };

                    return new RegistryKeyCaretaker(id, parentId, processId, processStartTime, connection, registryKeyOriginator, registryKeyMemento);
                case registryValueResourceTypeName:
                    // Registry Value caretaker reconstruction
                    var registryValueHive = AsEnum<RegistryHive>(dictionary[nameof(RegistryValueOriginator.Hive)]);
                    var registryValueView = AsEnum<RegistryView>(dictionary[nameof(RegistryValueOriginator.View)]);
                    var registryValueSubKey = AsString(dictionary[nameof(RegistryValueOriginator.SubKey)]);
                    var registryValueName = AsString(dictionary[nameof(RegistryValueOriginator.Name)]);
                    // Decode the compact Base64 payload back into its original .NET representation.
                    var registryValueValue = ConvertToRegistryValue(dictionary[nameof(RegistryValueMemento.Value)]);
                    var registryValueKind = AsEnum<RegistryValueKind>(dictionary[nameof(RegistryValueMemento.Kind)]);

                    var registryValueOriginator = new RegistryValueOriginator(registryValueHive, registryValueView, registryValueSubKey, registryValueName, registry);
                    var registryValueMemento = new RegistryValueMemento { Value = registryValueValue, Kind = registryValueKind };

                    return new RegistryValueCaretaker(id, parentId, processId, processStartTime, connection, registryValueOriginator, registryValueMemento);
                default:
                    // Defensive: Unknown discriminator means either version skew or data corruption.
                    throw new NotSupportedException($"The resource type '{dictionary[resourceTypePropertyName]}' is not supported.");
            }
        }

        /// <summary>
        /// Converts an <see cref="ICaretaker"/> instance to a dictionary representation.
        /// </summary>
        /// <param name="caretaker">The caretaker to convert.</param>
        /// <returns>The dictionary representation.</returns>
        /// <exception cref="NotSupportedException">Thrown if the caretaker type is unknown.</exception>
        protected override IDictionary<string, object> ConvertCaretakerToDictionary(ICaretaker caretaker)
        {
            // Baseline metadata shared by all caretakers.
            var result = new Dictionary<string, object>
            {
                [nameof(ICaretaker.ID)] = caretaker.ID,
                [nameof(ICaretaker.ParentID)] = caretaker.ParentID,
                [nameof(ICaretaker.ProcessID)] = caretaker.ProcessID,
                // Persist high resolution timestamp as ticks (Int64) for compactness.
                [nameof(ICaretaker.ProcessStartTime)] = caretaker.ProcessStartTime.Ticks
            };

            // Persist discriminator + resource specific data.
            switch (caretaker)
            {
                case RegistryKeyCaretaker registryKeyCaretaker:
                    result[resourceTypePropertyName] = registryKeyResourceTypeName;
                    result[nameof(RegistryKeyOriginator.Hive)] = registryKeyCaretaker.Originator.Hive.ToString();
                    result[nameof(RegistryKeyOriginator.View)] = registryKeyCaretaker.Originator.View.ToString();
                    result[nameof(RegistryKeyOriginator.SubKey)] = registryKeyCaretaker.Originator.SubKey;
                    result[nameof(RegistryKeyMemento.Exists)] = registryKeyCaretaker.Memento.Exists; // Whether the key existed at snapshot time.
                    break;
                case RegistryValueCaretaker registryValueCaretaker:
                    result[resourceTypePropertyName] = registryValueResourceTypeName;
                    result[nameof(RegistryValueOriginator.Hive)] = registryValueCaretaker.Originator.Hive.ToString();
                    result[nameof(RegistryValueOriginator.View)] = registryValueCaretaker.Originator.View.ToString();
                    result[nameof(RegistryValueOriginator.SubKey)] = registryValueCaretaker.Originator.SubKey;
                    result[nameof(RegistryValueOriginator.Name)] = registryValueCaretaker.Originator.Name;
                    // Encode value into compact Base64 + type-discriminator form.
                    result[nameof(RegistryValueMemento.Value)] = ConvertFromRegistryValue(registryValueCaretaker.Memento.Value);
                    result[nameof(RegistryValueMemento.Kind)] = registryValueCaretaker.Memento.Kind.ToString();
                    break;
                default:
                    throw new NotSupportedException($"The caretaker type '{caretaker.GetType().Name}' is not supported.");
            }

            return result;
        }

        /// <summary>
        /// Converts an in-memory registry value to a Base64 string suitable for JSON serialization.
        /// A one-byte discriminator is prefixed to the payload:
        /// 0x0 = byte[], 0x1 = int, 0x2 = long, 0x3 = string (ASCII), 0x4 = string[] (null-terminated ASCII segments).
        /// </summary>
        /// <param name="value">The registry value object.</param>
        /// <returns>Base64 encoded representation or null.</returns>
        /// <exception cref="NotSupportedException">Thrown if the value type is not supported.</exception>
        private static string ConvertFromRegistryValue(object value)
        {
            if (value == null)
            {
                return null; // Preserve null distinct from empty containers.
            }

            var result = new List<byte>();
            switch (value)
            {
                case byte[] byteValue:
                    result.Add(byteArrayType);
                    result.AddRange(byteValue);
                    break;
                case int intValue:
                    result.Add(intType);
                    result.AddRange(BitConverter.GetBytes(intValue));
                    break;
                case long longValue:
                    result.Add(longType);
                    result.AddRange(BitConverter.GetBytes(longValue));
                    break;
                case string stringValue:
                    result.Add(stringType);
                    // NOTE: ASCII encoding is chosen for compactness & determinism; non-ASCII chars would not round-trip.
                    result.AddRange(Encoding.ASCII.GetBytes(stringValue));
                    break;
                case string[] stringArrayValue:
                    result.Add(stringArrayType);
                    foreach (var stringValue in stringArrayValue)
                    {
                        // Each element's ASCII bytes followed by a single 0x0 terminator so we can split during decode.
                        result.AddRange(Encoding.ASCII.GetBytes(stringValue));
                        result.Add(segmentTerminator);
                    }
                    break;
                default:
                    // Defensive: Only explicitly supported registry value shapes are encoded.
                    throw new NotSupportedException($"{value.GetType().Name} is not a supported registry type.");
            }
            return Convert.ToBase64String(result.ToArray());
        }

        /// <summary>
        /// Reconstructs a registry value from its serialized Base64 representation.
        /// </summary>
        /// <param name="o">The serialized object (expected to be a Base64 string or null).</param>
        /// <returns>The reconstructed registry value (byte[], int, long, string, or string[]).</returns>
        /// <exception cref="NotSupportedException">Thrown if the serialized form is invalid or unsupported.</exception>
        private static object ConvertToRegistryValue(object o)
        {
            if (o == null)
            {
                return null;
            }

            if (o is string s)
            {
                var bytes = Convert.FromBase64String(s); // Throws if invalid Base64 -> surfaces data corruption early.

                switch (bytes[0])
                {
                    case byteArrayType:
                        return bytes.Skip(1).ToArray();
                    case intType:
                        return BitConverter.ToInt32(bytes, 1);
                    case longType:
                        return BitConverter.ToInt64(bytes, 1);
                    case stringType:
                        return Encoding.ASCII.GetString(bytes, 1, bytes.Length - 1);
                    case stringArrayType:
                        var result = new List<string>();
                        var currentStartIndex = 1; // Start just after the discriminator byte.
                        for (var i = currentStartIndex; i < bytes.Length; i++)
                        {
                            if (bytes[i] == segmentTerminator)
                            {
                                // Extract substring [currentStartIndex, i) and move past terminator.
                                result.Add(Encoding.ASCII.GetString(bytes, currentStartIndex, i - currentStartIndex));
                                currentStartIndex = i + 1;
                            }
                        }
                        return result.ToArray();
                    default:
                        throw new NotSupportedException("Unknown type byte.");
                }
            }
            else
            {
                throw new NotSupportedException($"Expected a string, but got an object of type: {o.GetType().Name}");
            }
        }
    }
}
