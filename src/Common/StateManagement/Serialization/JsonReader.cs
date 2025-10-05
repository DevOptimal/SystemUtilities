using DevOptimal.SystemUtilities.Common.StateManagement.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevOptimal.SystemUtilities.Common.StateManagement.Serialization
{
    /// <summary>
    /// Reads a JSON array as an enumerable. Implements a lightweight JSON reader to avoid external dependencies.
    /// Supports reading JSON values, arrays, and objects from a stream.
    /// Reference: https://www.json.org/json-en.html
    /// </summary>
    internal class JsonReader : StreamReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class from a <see cref="FileInfo"/>.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        public JsonReader(FileInfo file)
            : this(file.FullName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class from a file path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public JsonReader(string path)
            : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        public JsonReader(Stream stream)
            : base(stream) { }

        /// <summary>
        /// Peeks at the next character in the stream without advancing the reader.
        /// </summary>
        /// <returns>The next character.</returns>
        public char PeekChar()
        {
            return (char)Peek();
        }

        /// <summary>
        /// Reads the next character from the stream and advances the reader.
        /// </summary>
        /// <returns>The next character.</returns>
        public char ReadChar()
        {
            return (char)Read();
        }

        /// <summary>
        /// Asserts that the next characters in the stream match the expected sequence.
        /// Throws <see cref="JsonParserException"/> if the assertion fails.
        /// </summary>
        /// <param name="expectedChars">The expected characters.</param>
        public void AssertChar(params char[] expectedChars)
        {
            foreach (var expectedChar in expectedChars)
            {
                var actualChar = ReadChar();
                if (expectedChar != actualChar)
                {
                    throw new JsonParserException($"Expected char '{expectedChar}', but found '{actualChar}' instead.");
                }
            }
        }

        /// <summary>
        /// Reads and discards whitespace characters from the stream.
        /// </summary>
        public void ReadWhiteSpace()
        {
            var whiteSpaceCharacters = new List<char> { ' ', '\n', '\r', '\t' };
            while (whiteSpaceCharacters.Contains(PeekChar()))
            {
                ReadChar();
            }
        }

        /// <summary>
        /// Reads a JSON number from the stream and returns it as an int, long, float, double, or decimal.
        /// Throws <see cref="JsonParserException"/> if the number cannot be parsed.
        /// </summary>
        /// <returns>The parsed number as an object.</returns>
        public object ReadNumber()
        {
            var digitCharacters = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            var sb = new StringBuilder();

            if (PeekChar() == '-')
            {
                sb.Append(ReadChar());
            }

            if (PeekChar() == '0')
            {
                sb.Append(ReadChar());
            }
            else if (digitCharacters.Contains(PeekChar()))
            {
                do
                {
                    sb.Append(ReadChar());
                } while (digitCharacters.Contains(PeekChar()));
            }
            else
            {
                throw new JsonParserException($"Unexpected character encountered: {PeekChar()}");
            }

            // Handle fractional part
            if (PeekChar() == '.')
            {
                sb.Append(ReadChar());
                if (!digitCharacters.Contains(PeekChar()))
                {
                    throw new JsonParserException($"Unexpected character encountered: {PeekChar()}");
                }

                do
                {
                    sb.Append(ReadChar());
                } while (digitCharacters.Contains(PeekChar()));
            }

            // Handle exponent part
            var exponentCharacters = new List<char> { 'e', 'E' };
            if (exponentCharacters.Contains(PeekChar()))
            {
                sb.Append(ReadChar());

                var signCharacters = new List<char> { '+', '-' };
                if (signCharacters.Contains(PeekChar()))
                {
                    sb.Append(ReadChar());
                }

                if (!digitCharacters.Contains(PeekChar()))
                {
                    throw new JsonParserException($"Unexpected character encountered: {PeekChar()}");
                }

                do
                {
                    sb.Append(ReadChar());
                } while (digitCharacters.Contains(PeekChar()));
            }

            // Try parsing to various numeric types
            if (int.TryParse(sb.ToString(), out var intResult))
            {
                return intResult;
            }
            else if (long.TryParse(sb.ToString(), out var longResult))
            {
                return longResult;
            }
            else if (float.TryParse(sb.ToString(), out var floatResult))
            {
                return floatResult;
            }
            else if (double.TryParse(sb.ToString(), out var doubleResult))
            {
                return doubleResult;
            }
            else if (decimal.TryParse(sb.ToString(), out var decimalResult))
            {
                return decimalResult;
            }
            else
            {
                throw new JsonParserException($"Unable to parse number: {sb}");
            }
        }

        /// <summary>
        /// Reads a JSON string from the stream, handling escape sequences.
        /// Throws <see cref="JsonParserException"/> for invalid escape sequences.
        /// </summary>
        /// <returns>The parsed string.</returns>
        public string ReadString()
        {
            var sb = new StringBuilder();

            AssertChar('"');

            while (true)
            {
                switch (PeekChar())
                {
                    case '"':
                        ReadChar();
                        return sb.ToString();
                    case '\\':
                        ReadChar();
                        var nextChar = ReadChar();
                        switch (nextChar)
                        {
                            case '"':
                                sb.Append('"');
                                break;
                            case '\\':
                                sb.Append('\\');
                                break;
                            case '/':
                                sb.Append('/');
                                break;
                            case 'b':
                                sb.Append('\b');
                                break;
                            case 'f':
                                sb.Append('\f');
                                break;
                            case 'n':
                                sb.Append('\n');
                                break;
                            case 'r':
                                sb.Append('\r');
                                break;
                            case 't':
                                sb.Append('\t');
                                break;
                            case 'u':
                                // Handle Unicode escape sequence
                                var hexDigitCharacters = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F' };
                                var hexStringBuilder = new StringBuilder();
                                for (var i = 0; i < 4; i++)
                                {
                                    if (!hexDigitCharacters.Contains(PeekChar()))
                                    {
                                        throw new JsonParserException($"Unexpected character encountered: {PeekChar()}");
                                    }

                                    hexStringBuilder.Append(ReadChar());
                                }
                                sb.Append((char)int.Parse(hexStringBuilder.ToString(), System.Globalization.NumberStyles.HexNumber));
                                break;
                            default:
                                throw new JsonParserException($"Unexpected character encountered: {nextChar}");
                        }
                        break;
                    default:
                        sb.Append(ReadChar());
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a JSON value (string, number, object, array, true, false, or null) from the stream.
        /// </summary>
        /// <returns>The parsed value as an object.</returns>
        public object ReadValue()
        {
            ReadWhiteSpace();

            object result;
            switch (PeekChar())
            {
                case '"':
                    result = ReadString();
                    break;
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    result = ReadNumber();
                    break;
                case '{':
                    result = ReadObject();
                    break;
                case '[':
                    result = ReadArray();
                    break;
                case 't':
                    AssertChar('t', 'r', 'u', 'e');
                    result = true;
                    break;
                case 'f':
                    AssertChar('f', 'a', 'l', 's', 'e');
                    result = false;
                    break;
                case 'n':
                    AssertChar('n', 'u', 'l', 'l');
                    result = null;
                    break;
                default: throw new JsonParserException($"Unexpected character encountered: {PeekChar()}");
            }

            ReadWhiteSpace();

            return result;
        }

        /// <summary>
        /// Reads a JSON array from the stream and returns it as an object array.
        /// </summary>
        /// <returns>The parsed array.</returns>
        public object[] ReadArray()
        {
            return [.. EnumerateArray()];
        }

        /// <summary>
        /// Enumerates the elements of a JSON array from the stream.
        /// </summary>
        /// <returns>An enumerable of parsed array elements.</returns>
        public IEnumerable<object> EnumerateArray()
        {
            AssertChar('[');

            ReadWhiteSpace();

            while (PeekChar() != ']')
            {
                yield return ReadValue();

                if (PeekChar() == ',')
                {
                    ReadChar();
                }
                else
                {
                    break;
                }
            }

            AssertChar(']');
        }

        /// <summary>
        /// Reads a JSON object from the stream and returns it as a dictionary.
        /// </summary>
        /// <returns>The parsed object as a dictionary.</returns>
        public IDictionary<string, object> ReadObject()
        {
            var result = new Dictionary<string, object>();

            AssertChar('{');

            ReadWhiteSpace();

            while (PeekChar() != '}')
            {
                ReadWhiteSpace();

                var key = ReadString();

                ReadWhiteSpace();

                AssertChar(':');

                var value = ReadValue();

                result.Add(key, value);

                if (PeekChar() == ',')
                {
                    ReadChar();
                }
                else
                {
                    break;
                }
            }

            AssertChar('}');

            return result;
        }
    }
}
