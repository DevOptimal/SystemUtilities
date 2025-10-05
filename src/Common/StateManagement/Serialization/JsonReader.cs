﻿using DevOptimal.SystemUtilities.Common.StateManagement.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevOptimal.SystemUtilities.Common.StateManagement.Serialization
{
    /// <summary>
    /// Reads a JSON array as an enumerable. Implementing our own lightweight JSON reader to avoid dependencies.
    /// Reference: https://www.json.org/json-en.html
    /// </summary>
    internal class JsonReader : StreamReader
    {
        public JsonReader(FileInfo file)
            : this(file.FullName) { }

        public JsonReader(string path)
            : base(path) { }

        public JsonReader(Stream stream)
            : base(stream) { }

        public char PeekChar()
        {
            return (char)Peek();
        }

        public char ReadChar()
        {
            return (char)Read();
        }

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

        public void ReadWhiteSpace()
        {
            var whiteSpaceCharacters = new List<char> { ' ', '\n', '\r', '\t' };
            while (whiteSpaceCharacters.Contains(PeekChar()))
            {
                ReadChar();
            }
        }

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

        public object[] ReadArray()
        {
            return [.. EnumerateArray()];
        }

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
