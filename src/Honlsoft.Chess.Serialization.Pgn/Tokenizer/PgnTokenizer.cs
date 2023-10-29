using System.Text;

namespace Honlsoft.Chess.Serialization.Pgn; 

public class PgnTokenizer(string pgnString) {

    private static Dictionary<char, TokenType> _individualSymbols = new() {
        {'[', TokenType.LeftSquareBracket},
        {']', TokenType.RightSquareBracket},
        {'(', TokenType.LeftRoundBracket},
        {')', TokenType.RightRoundBracket},
        {'.', TokenType.Period},
        {'*', TokenType.Astrix},
    };
    
    
    private int _index;
    private int _lineOffset;
    private int _line;

    public int Line => _line;
    public int LineOffset => _lineOffset;
    
    private enum StringState {
        InQuotes,
        EscapeCharacter
    }

    public IEnumerable<Token> ReadAllTokens() {
        var token = ReadNextToken();
        while (token is not null) {
            yield return token;

            token = ReadNextToken();
        }
    }


    public Token? ReadNextToken() {

        SkipWhitespace();

        var c = PeekChar();
        if (c == null) {
            return null;
        }

        // Check for simple match with an individual character as a symbol
        if (_individualSymbols.TryGetValue(c.Value, out var tokenType)) {
            var token = new Token(tokenType, c.Value.ToString(), _line, _lineOffset);
            Advance(1);
            return token;
        }


        int line = _line;
        int lineOffset = _lineOffset;

        // Line ending comment
        if (c == ';') {
            Advance(1);
            var commentString = TakeWhile((c) => c != '\n');
            return new Token(TokenType.Comment, commentString, line, lineOffset);
        }

        // A comment in braces
        if (c == '{') {
            Advance(1);
            var commentString = TakeWhile((c) => c != '}');
            return new Token(TokenType.Comment, commentString, line, lineOffset);
        }

        // A symbol
        if (Char.IsLetter(c.Value) || Char.IsDigit(c.Value)) {

            var symbolString = TakeWhile(IsSymbolChar);
            if (int.TryParse(symbolString, out _)) {
                return new Token(TokenType.Integer, symbolString, line, lineOffset);
            } else {
                return new Token(TokenType.Symbol, symbolString, line, lineOffset);
            }
        }

        // A string
        if (c == '"') {
            Advance(1);
            var value = ReadString();
            return new Token(TokenType.String, value, line, lineOffset);
        }


        if (c == '$') {
            Advance(1);
            var value = TakeWhile(Char.IsDigit);
            return new Token(TokenType.NumericAnnotationGlyph, value, line, lineOffset);
        }
        
        throw new PgnSerializationException($"[{line}:{lineOffset}] unknown character {c}.", line, lineOffset);
    }


    private bool IsSymbolChar(char c) {
        return (c is '_' or '+' or '#' or '=' or ':' or '-') || Char.IsDigit(c) || Char.IsLetter(c);
    }


    private string ReadString() {

        StringBuilder value = new();
        StringState stringState = StringState.InQuotes;

        var c = Read();
        if (c == null) {
            return string.Empty;
        }

        while (c != null) {
            
            if (stringState == StringState.EscapeCharacter) {
                value.Append(c);
                stringState = StringState.InQuotes;
            }
            else if (c == '\\') {
                stringState = StringState.EscapeCharacter;
            } else if (c == '"') {
                return value.ToString();
            } else {
                value.Append(c);
                stringState = StringState.InQuotes;
            }

            c = Read();
        }

        return value.ToString();

    }

    public char? Read() {
        var c = PeekChar();
        Advance(1);
        return c;
    }


    private void Advance(int characters) {
        _index += characters;
        _lineOffset++;
    }
    

    private string TakeWhile(Func<char, bool> predicate) {
        StringBuilder value = new();

        var c = PeekChar();
        while (c != null && predicate(c.Value)) {
            value.Append(c);
            Advance(1);
            c = PeekChar();
        }
        return value.ToString();
    }

    private void SkipWhitespace() {
        var c = PeekChar();
        
        if (c == null) {
            return;
        }
        
        while (c != null && (Char.IsWhiteSpace(c.Value) || c == '\n' || c == '\r')) {

            if (c == '\n') {
                _line++;
            }
            
            Advance(1);
            c = PeekChar();
        }
    }

    private char? PeekChar() {
        if (_index < pgnString.Length) {
            return pgnString[_index];
        }
        return null;
    }

    
    
}