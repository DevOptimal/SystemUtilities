using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOptimal.SystemUtilities.Common.StateManagement.Serialization
{
    /// <summary>
    /// Goal of this serializer is efficient resource usage by streaming caretakers instead of loading all of them into memory at once.
    /// To accomplish this, we parse and yield return caretakers as we read them from the JSON stream.
    /// </summary>
    internal abstract class CaretakerSerializer
    {
        protected const string resourceTypePropertyName = "$resource-type";

        public IEnumerable<ICaretaker> ReadCaretakers(JsonReader reader, DatabaseConnection connection)
        {
            return reader.EnumerateArray().Cast<IDictionary<string, object>>().Select(d => ConvertDictionaryToCaretaker(d, connection));
        }

        public void WriteCaretakers(JsonWriter writer, IEnumerable<ICaretaker> caretakers)
        {
            writer.WriteArray(caretakers.Select(ConvertCaretakerToDictionary));
        }

        protected abstract ICaretaker ConvertDictionaryToCaretaker(IDictionary<string, object> dictionary, DatabaseConnection connection);

        protected abstract IDictionary<string, object> ConvertCaretakerToDictionary(ICaretaker caretaker);

        protected bool AsBoolean(object o)
        {
            if (o is bool b)
            {
                return b;
            }
            else
            {
                throw new JsonParsingException();
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
                throw new JsonParsingException();
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
                throw new JsonParsingException();
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
                throw new JsonParsingException();
            }
        }

        protected string AsString(object o)
        {
            if (o == null)
            {
                return null;
            }
            else if (o is string s)
            {
                return s;
            }
            else
            {
                throw new JsonParsingException();
            }
        }
    }
}
