using DevOptimal.SystemUtilities.Common.StateManagement.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOptimal.SystemUtilities.Common.StateManagement.Serialization
{
    /// <summary>
    /// Abstract base class for serializing and deserializing ICaretaker objects.
    /// Designed for efficient resource usage by streaming caretakers from a JSON source,
    /// rather than loading all caretakers into memory at once.
    /// </summary>
    internal abstract class CaretakerSerializer
    {
        /// <summary>
        /// The property name used to indicate the resource type in serialized data.
        /// </summary>
        protected const string resourceTypePropertyName = "$resource-type";

        /// <summary>
        /// Reads caretakers from a JSON stream, yielding each as it is parsed.
        /// </summary>
        /// <param name="reader">The JSON reader to read from.</param>
        /// <param name="connection">The database connection context.</param>
        /// <returns>An enumerable of ICaretaker objects.</returns>
        public IEnumerable<ICaretaker> ReadCaretakers(JsonReader reader, DatabaseConnection connection)
        {
            // Enumerate the JSON array, convert each dictionary to an ICaretaker instance.
            return reader.EnumerateArray().Cast<IDictionary<string, object>>().Select(d => ConvertDictionaryToCaretaker(d, connection));
        }

        /// <summary>
        /// Writes a collection of caretakers to a JSON stream.
        /// </summary>
        /// <param name="writer">The JSON writer to write to.</param>
        /// <param name="caretakers">The caretakers to serialize.</param>
        public void WriteCaretakers(JsonWriter writer, IEnumerable<ICaretaker> caretakers)
        {
            // Convert each ICaretaker to a dictionary and write as a JSON array.
            writer.WriteArray(caretakers.Select(ConvertCaretakerToDictionary));
        }

        /// <summary>
        /// Converts a dictionary representation of a caretaker to an ICaretaker instance.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="connection">The database connection context.</param>
        /// <returns>The deserialized ICaretaker.</returns>
        protected abstract ICaretaker ConvertDictionaryToCaretaker(IDictionary<string, object> dictionary, DatabaseConnection connection);

        /// <summary>
        /// Converts an ICaretaker instance to a dictionary representation.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="caretaker">The caretaker to convert.</param>
        /// <returns>The dictionary representation.</returns>
        protected abstract IDictionary<string, object> ConvertCaretakerToDictionary(ICaretaker caretaker);

        /// <summary>
        /// Converts an object to a boolean, or throws if the conversion is not possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>The boolean value.</returns>
        /// <exception cref="JsonParserException">Thrown if the object is not a boolean.</exception>
        protected bool AsBoolean(object o)
        {
            if (o is bool b)
            {
                return b;
            }
            else
            {
                throw new JsonParserException($"Expected a boolean, but got an object of type: {o.GetType().Name}");
            }
        }

        /// <summary>
        /// Converts an object to a DateTime, supporting DateTime, string, and long types.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>The DateTime value.</returns>
        /// <exception cref="JsonParserException">Thrown if the object cannot be converted to DateTime.</exception>
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
                throw new JsonParserException($"Expected a DateTime, but got an object of type: {o.GetType().Name}");
            }
        }

        /// <summary>
        /// Converts an object to an enum value of type T.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="o">The object to convert.</param>
        /// <returns>The enum value.</returns>
        /// <exception cref="ArgumentException">Thrown if T is not an enum type.</exception>
        /// <exception cref="JsonParserException">Thrown if the object cannot be converted to the enum.</exception>
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
                throw new JsonParserException($"Expected an enumeration, but got an object of type: {o.GetType().Name}");
            }
        }

        /// <summary>
        /// Converts an object to an integer, or throws if the conversion is not possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>The integer value.</returns>
        /// <exception cref="JsonParserException">Thrown if the object is not an integer.</exception>
        protected int AsInteger(object o)
        {
            if (o is int i)
            {
                return i;
            }
            else
            {
                throw new JsonParserException($"Expected an integer, but got an object of type: {o.GetType().Name}");
            }
        }

        /// <summary>
        /// Converts an object to a string, or returns null if the object is null.
        /// Throws if the object is not a string or null.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>The string value or null.</returns>
        /// <exception cref="JsonParserException">Thrown if the object is not a string or null.</exception>
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
                throw new JsonParserException($"Expected a string, but got an object of type: {o.GetType().Name}");
            }
        }
    }
}
