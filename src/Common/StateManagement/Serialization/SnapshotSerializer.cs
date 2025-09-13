using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOptimal.SystemUtilities.Common.StateManagement.Serialization
{
    /// <summary>
    /// Goal of this serializer is efficient resource usage by streaming snapshots instead of loading all of them into memory at once.
    /// To accomplish this, we parse and yield return snapshots as we read them from the JSON stream.
    /// </summary>
    internal abstract class SnapshotSerializer
    {
        protected const string resourceTypePropertyName = "$resource-type";

        public IEnumerable<ISnapshot> ReadSnapshots(JsonReader reader)
        {
            return reader.EnumerateArray().Cast<IDictionary<string, object>>().Select(ConvertDictionaryToSnapshot);
        }

        public void WriteSnapshots(JsonWriter writer, IEnumerable<ISnapshot> snapshots)
        {
            writer.WriteArray(snapshots.Select(ConvertSnapshotToDictionary));
        }

        protected abstract ISnapshot ConvertDictionaryToSnapshot(IDictionary<string, object> dictionary);

        protected abstract IDictionary<string, object> ConvertSnapshotToDictionary(ISnapshot snapshot);

        protected bool AsBoolean(object o)
        {
            if (o is bool b)
            {
                return b;
            }
            else
            {
                throw new Exception();
            }
        }

        protected DateTime AsDateTime(object o)
        {
            if (o is DateTime d)
            {
                return d;
            }
            else if (o is string s)
            {
                return DateTime.Parse(s);
            }
            else if (o is long l)
            {
                return new DateTime(l);
            }
            else
            {
                throw new Exception();
            }
        }

        protected T AsEnum<T>(object o) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"{typeof(T).Name} is not an enumerated type.");
            }

            if (o is T t)
            {
                return t;
            }
            else if (o is int i)
            {
                return (T)(object)i;
            }
            else if (o is string s)
            {
                return (T)Enum.Parse(typeof(T), s);
            }
            else
            {
                throw new Exception();
            }
        }

        protected int AsInteger(object o)
        {
            if (o is int i)
            {
                return i;
            }
            else
            {
                throw new Exception();
            }
        }

        protected string AsString(object o)
        {
            if (o is string s)
            {
                return s;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
