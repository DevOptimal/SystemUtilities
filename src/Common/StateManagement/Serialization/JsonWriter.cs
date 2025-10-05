using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.Common.StateManagement.Serialization
{
    /// <summary>
    /// Writes .NET objects as JSON using a lightweight, dependency-free implementation.
    /// Supports writing strings, numbers, booleans, arrays, and objects to a stream.
    /// Reference: https://www.json.org/json-en.html
    /// </summary>
    internal class JsonWriter : StreamWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for a file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        public JsonWriter(FileInfo file)
            : this(file.FullName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for a file path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public JsonWriter(string path)
            : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for a stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public JsonWriter(Stream stream)
            : base(stream) { }

        /// <summary>
        /// Writes a string value as a JSON string, escaping special characters as needed.
        /// </summary>
        /// <param name="value">The string value to write.</param>
        public void WriteString(string value)
        {
            var escapedValue = value
                .Replace("\"", "\\\"")
                .Replace("\\", "\\\\")
                .Replace("/", "\\/")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
            Write($"\"{escapedValue}\"");
        }

        /// <summary>
        /// Writes an integer value as a JSON number.
        /// </summary>
        /// <param name="value">The integer value to write.</param>
        public void WriteNumber(int value)
        {
            Write(value);
        }

        /// <summary>
        /// Writes a long value as a JSON number.
        /// </summary>
        /// <param name="value">The long value to write.</param>
        public void WriteNumber(long value)
        {
            Write(value);
        }

        /// <summary>
        /// Writes a float value as a JSON number.
        /// </summary>
        /// <param name="value">The float value to write.</param>
        public void WriteNumber(float value)
        {
            Write(value);
        }

        /// <summary>
        /// Writes a double value as a JSON number.
        /// </summary>
        /// <param name="value">The double value to write.</param>
        public void WriteNumber(double value)
        {
            Write(value);
        }

        /// <summary>
        /// Writes a decimal value as a JSON number.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public void WriteNumber(decimal value)
        {
            Write(value);
        }

        /// <summary>
        /// Writes an object as a JSON value, dispatching to the appropriate method based on its type.
        /// Supports strings, numbers, booleans, arrays, objects, and null.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="NotSupportedException">Thrown if the object type is not supported.</exception>
        public void WriteValue(object value)
        {
            if (value == null)
            {
                Write("null");
            }
            else
            {
                switch (value)
                {
                    case string s:
                        WriteString(s);
                        break;
                    case int i:
                        WriteNumber(i);
                        break;
                    case long l:
                        WriteNumber(l);
                        break;
                    case float f:
                        WriteNumber(f);
                        break;
                    case double d:
                        WriteNumber(d);
                        break;
                    case decimal e:
                        WriteNumber(e);
                        break;
                    case IDictionary<string, object> o:
                        WriteObject(o);
                        break;
                    case IEnumerable<object> a:
                        WriteArray(a);
                        break;
                    case bool b:
                        Write(b ? "true" : "false");
                        break;
                    default: throw new NotSupportedException($"Unsupported object type: {value.GetType().Name}");
                }
            }
        }

        /// <summary>
        /// Writes an enumerable as a JSON array.
        /// </summary>
        /// <param name="value">The enumerable to write.</param>
        public void WriteArray(IEnumerable<object> value)
        {
            Write("[");

            var firstItem = true;
            foreach (var item in value)
            {
                if (firstItem)
                {
                    firstItem = false;
                }
                else
                {
                    Write(",");
                }

                Write(" ");

                WriteValue(item);
            }

            Write(" ]");
        }

        /// <summary>
        /// Writes a dictionary as a JSON object.
        /// </summary>
        /// <param name="value">The dictionary to write.</param>
        public void WriteObject(IDictionary<string, object> value)
        {
            Write("{");

            var firstItem = true;
            foreach (var item in value)
            {
                if (firstItem)
                {
                    firstItem = false;
                }
                else
                {
                    Write(",");
                }

                Write(" ");
                WriteString(item.Key);
                Write(": ");

                WriteValue(item.Value);
            }

            Write(" }");
        }
    }
}
