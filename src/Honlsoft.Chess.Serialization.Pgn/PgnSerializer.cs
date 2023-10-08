using System.Text;
using Honlsoft.Chess.Serialization.Pgn;

namespace Honlsoft.Chess.Serialization; 

public class PgnSerializer {

    private char EscapeCharacter = '\\';
    private char StringQuote = '"';
    
    private enum StringState {
        InQuotes,
        EscapeCharacter
    }

    public async Task<PgnChessGame> ReadPgnAsync(TextReader reader) {
        var allText = await reader.ReadToEndAsync();
        int idx = 0;

        List<PgnAttribute> attributes = new List<PgnAttribute>();
        
        var attribute = ParseNextAttribute(allText, ref idx);
        while (attribute != null) {
            attributes.Add(attribute);
            attribute = ParseNextAttribute(allText, ref idx);
        }
        
        return new PgnChessGame(attributes);
    }

    public PgnAttribute? ParseAttribute(string pgnText) {
        int garbage = 0;
        return ParseNextAttribute(pgnText, ref garbage);
    }
    
    
    public PgnAttribute? ParseNextAttribute(string pgnText, ref int index) {

        // Find the next tag, then start parsing mode
        int nextStart = pgnText.IndexOf('[', index);
        if (nextStart >= 0) {
            index++;
            SkipWhitespace(pgnText, ref index);
            var name = ReadAttributeName(pgnText, ref index);
            if (string.IsNullOrWhiteSpace(name)) {
                var (line, offset) = GetLineOffset(pgnText, index);
                throw new PgnSerializationException($"[{line}:{offset}] Unable to read attribute name after [ in PGN file. ", line, offset);
            }
            SkipWhitespace(pgnText, ref index);
            var value = ReadAttributeValue(pgnText, ref index);
            if (value == null) {
                var (line, offset) = GetLineOffset(pgnText, index);
                throw new PgnSerializationException($"[{line}:{offset}] Unable to read attribute value for attribute {name}.", line, offset);
            }

            // TODO: parse a comment if there is one at the end of the line

            var endingIndex = pgnText.IndexOf(']', index);
            if (endingIndex < 0) {
                var (line, offset) = GetLineOffset(pgnText, index);
                throw new PgnSerializationException($"[{line}:{offset}] No ending bracket for PGN attribute {name}.", line, offset);
            }
            
            return new PgnAttribute(name, value, string.Empty);
        }

        return null;
    }

    public string? ReadAttributeName(string pgnText, ref int index) {
        var name = string.Concat(pgnText.Skip(index).TakeWhile(Char.IsLetter));
        index += name.Length;
        return name;
    }

    public string? ReadAttributeValue(string pgnText, ref int index) {
        var startingIndex = pgnText.IndexOf("\"", index, StringComparison.InvariantCulture);
        if (startingIndex < 0) {
            return null;
        }
        
        StringBuilder value = new StringBuilder();
        char currentChar = pgnText[startingIndex++];
        StringState state = StringState.InQuotes;
        while (startingIndex < pgnText.Length) {
            currentChar = pgnText[startingIndex++];
            if (state == StringState.EscapeCharacter) {
                value.Append(currentChar);
                state = StringState.InQuotes;
            }
            else if (currentChar == StringQuote) {
                index = startingIndex;
                return value.ToString();
            }
            else if (currentChar == EscapeCharacter) {
                state = StringState.EscapeCharacter;
            } else {
                value.Append(currentChar);
            }
        }

        // Otherwise, we couldn't read the value and will return null.
        return null;
    }
    
    
    

    public string ReadName(string text, ref int index) {
        var name = string.Concat(text.Skip(index).TakeWhile(Char.IsLetter));
        return name;
    }

    public void SkipWhitespace(string text, ref int index) {
        index += text.Skip(index).TakeWhile(Char.IsWhiteSpace).Count();
    }


    private (int Line, int Offset) GetLineOffset(string pgnText, int index) {
        var line = pgnText.Take(index).Count((c) => c == '\n');
        int lastLineEnding = 0;
        if (index >= 0) {
            var tempLineEnding = pgnText.LastIndexOf('\n', index);
            if (tempLineEnding >= 0) {
                lastLineEnding = tempLineEnding;
            }
        }
        return (line, index - lastLineEnding);
    }
}