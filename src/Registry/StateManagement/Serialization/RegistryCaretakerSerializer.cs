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
    /// Goal of this serializer is efficient resource usage by streaming caretakers instead of loading all of them into memory at once.
    /// To accomplish this, we parse and yield return caretakers as we read them from the JSON stream.
    /// </summary>
    internal class RegistryCaretakerSerializer(IRegistry registry) : CaretakerSerializer
    {
        private const string registryKeyResourceTypeName = "RegistryKey";
        private const string registryValueResourceTypeName = "RegistryValue";

        protected override ICaretaker ConvertDictionaryToCaretaker(IDictionary<string, object> dictionary, Snapshotter snapshotter)
        {
            // Get caretaker fields
            var id = AsString(dictionary[nameof(ICaretaker.ID)]);
            var parentId = AsString(dictionary[nameof(ICaretaker.ParentID)]);
            var processId = AsInteger(dictionary[nameof(ICaretaker.ProcessID)]);
            var processStartTime = AsDateTime(dictionary[nameof(ICaretaker.ProcessStartTime)]);

            switch (dictionary[resourceTypePropertyName])
            {
                case registryKeyResourceTypeName:
                    var registryKeyHive = AsEnum<RegistryHive>(dictionary[nameof(RegistryKeyOriginator.Hive)]);
                    var registryKeyView = AsEnum<RegistryView>(dictionary[nameof(RegistryKeyOriginator.View)]);
                    var registryKeySubKey = AsString(dictionary[nameof(RegistryKeyOriginator.SubKey)]);
                    var registryKeyExists = AsBoolean(dictionary[nameof(RegistryKeyMemento.Exists)]);

                    var registryKeyOriginator = new RegistryKeyOriginator(registryKeyHive, registryKeyView, registryKeySubKey, registry);
                    var registryKeyMemento = new RegistryKeyMemento
                    {
                        Exists = registryKeyExists
                    };

                    return new Caretaker<RegistryKeyOriginator, RegistryKeyMemento>(id, parentId, processId, processStartTime, snapshotter, registryKeyOriginator, registryKeyMemento);
                case registryValueResourceTypeName:
                    var registryValueHive = AsEnum<RegistryHive>(dictionary[nameof(RegistryValueOriginator.Hive)]);
                    var registryValueView = AsEnum<RegistryView>(dictionary[nameof(RegistryValueOriginator.View)]);
                    var registryValueSubKey = AsString(dictionary[nameof(RegistryValueOriginator.SubKey)]);
                    var registryValueName = AsString(dictionary[nameof(RegistryValueOriginator.Name)]);
                    var registryValueValue = ConvertToRegistryValue(dictionary[nameof(RegistryValueMemento.Value)]);
                    var registryValueKind = AsEnum<RegistryValueKind>(dictionary[nameof(RegistryValueMemento.Kind)]);

                    var registryValueOriginator = new RegistryValueOriginator(registryValueHive, registryValueView, registryValueSubKey, registryValueName, registry);
                    var registryValueMemento = new RegistryValueMemento
                    {
                        Value = registryValueValue,
                        Kind = registryValueKind
                    };

                    return new Caretaker<RegistryValueOriginator, RegistryValueMemento>(id, parentId, processId, processStartTime, snapshotter, registryValueOriginator, registryValueMemento);
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
                case Caretaker<RegistryKeyOriginator, RegistryKeyMemento> registryKeyCaretaker:
                    result[resourceTypePropertyName] = registryKeyResourceTypeName;
                    result[nameof(RegistryKeyOriginator.Hive)] = registryKeyCaretaker.Originator.Hive.ToString();
                    result[nameof(RegistryKeyOriginator.View)] = registryKeyCaretaker.Originator.View.ToString();
                    result[nameof(RegistryKeyOriginator.SubKey)] = registryKeyCaretaker.Originator.SubKey;
                    result[nameof(RegistryKeyMemento.Exists)] = registryKeyCaretaker.Memento.Exists;
                    break;
                case Caretaker<RegistryValueOriginator, RegistryValueMemento> registryValueCaretaker:
                    result[resourceTypePropertyName] = registryValueResourceTypeName;
                    result[nameof(RegistryValueOriginator.Hive)] = registryValueCaretaker.Originator.Hive.ToString();
                    result[nameof(RegistryValueOriginator.View)] = registryValueCaretaker.Originator.View.ToString();
                    result[nameof(RegistryValueOriginator.SubKey)] = registryValueCaretaker.Originator.SubKey;
                    result[nameof(RegistryValueOriginator.Name)] = registryValueCaretaker.Originator.Name;
                    result[nameof(RegistryValueMemento.Value)] = ConvertFromRegistryValue(registryValueCaretaker.Memento.Value);
                    result[nameof(RegistryValueMemento.Kind)] = registryValueCaretaker.Memento.Kind.ToString();
                    break;
                default: throw new Exception();
            }

            return result;
        }

        private static string ConvertFromRegistryValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            var result = new List<byte>();
            switch (value)
            {
                case byte[] byteValue:
                    result.Add(0x0);
                    result.AddRange(byteValue);
                    break;
                case int intValue:
                    result.Add(0x1);
                    result.AddRange(BitConverter.GetBytes(intValue));
                    break;
                case long longValue:
                    result.Add(0x2);
                    result.AddRange(BitConverter.GetBytes(longValue));
                    break;
                case string stringValue:
                    result.Add(0x3);
                    result.AddRange(Encoding.ASCII.GetBytes(stringValue));
                    break;
                case string[] stringArrayValue:
                    result.Add(0x4);
                    foreach (var stringValue in stringArrayValue)
                    {
                        result.AddRange(Encoding.ASCII.GetBytes(stringValue));
                        result.Add(0x0);
                    }
                    break;
                default:
                    throw new NotSupportedException($"{value.GetType().Name} is not a supported registry type.");
            }
            return Convert.ToBase64String([.. result]);
        }

        private static object ConvertToRegistryValue(object o)
        {
            if (o == null)
            {
                return null;
            }

            if (o is string s)
            {
                var bytes = Convert.FromBase64String(s);

                switch (bytes[0])
                {
                    case 0x0:
                        return bytes.Skip(1).ToArray();
                    case 0x1:
                        return BitConverter.ToInt32(bytes, 1);
                    case 0x2:
                        return BitConverter.ToInt64(bytes, 1);
                    case 0x3:
                        return Encoding.ASCII.GetString(bytes, 1, bytes.Length - 1);
                    case 0x4:
                        var result = new List<string>();
                        var currentStartIndex = 1;
                        for (var i = currentStartIndex; i < bytes.Length; i++)
                        {
                            if (bytes[i] == 0x0)
                            {
                                result.Add(Encoding.ASCII.GetString(bytes, currentStartIndex, i - currentStartIndex));
                                currentStartIndex = i + 1;
                            }
                        }
                        return result.ToArray();
                    default:
                        throw new NotSupportedException($"Unknown type byte.");
                }
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
