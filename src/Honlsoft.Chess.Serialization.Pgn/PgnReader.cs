namespace Honlsoft.Chess.Serialization.Pgn; 

public class PgnReader(PgnTokenizer tokenizer) {

    private int index = 0;
    private readonly Token[] _tokens = tokenizer.ReadAllTokens().ToArray();
    

    public PgnTag? ReadTag() {
        // Left
        if (PeekToken() is { Type: TokenType.LeftSquareBracket }) {

            Advance();

            var name = ExpectToken(TokenType.Symbol, "Unable to parse an attribute. ");

            var value = ExpectToken(TokenType.String, "Unable to parse an attribute. ");

            ExpectToken(TokenType.RightSquareBracket, "Unable to parse an attribute. ");

            var comment = PeekToken();
            if (comment is { Type: TokenType.Comment }) {
                Advance(1);
            }
            
            return new PgnTag(name.Value, value.Value, comment?.Value);
        }

        return null;
    }


    public PgnMovePart? ReadMovePart() {
        if (PeekToken() is { Type: TokenType.Integer }) {

            Token number = ReadToken();
            
            int periodCount = 0;
            Token? periodToken = PeekToken();
            while (PeekToken() is { Type: TokenType.Period }) {
                ReadToken();
                periodCount++;
            }

            return new PgnMoveNumber(int.Parse(number.Value), periodCount);
        }
        else if (PeekToken() is { Type: TokenType.Symbol }) {
            
            Token token = ReadToken();

            if (token.Value == "1-0") {
                return new PgnResult(1, 0);
            }
            if (token.Value == "0-1") {
                return new PgnResult(0, 1);
            }
            if (token.Value == "1/2-1/2") {
                return new PgnResult(0.5m, 0.5m);
            }
            
            PgnMoveText moveText = PgnMoveText.Parse(token.Value);
            return moveText;
        }

        return null;
    }


    


    public PgnTag[] ReadTagSection() {
        var tags = new List<PgnTag>();
        var pgnTag = ReadTag();

        while (pgnTag != null) {
            tags.Add(pgnTag);
            pgnTag = ReadTag();
        }

        return tags.ToArray();
    }
    
    public PgnMovePart[] ReadMoveParts() {
        var moveParts = new List<PgnMovePart>();
        var pgnMovePart = ReadMovePart();

        while (pgnMovePart != null) {
            moveParts.Add(pgnMovePart);
            pgnMovePart = ReadMovePart();
        }

        return moveParts.ToArray();
    }

    public PgnChessMatch Read() {

        var tags = ReadTagSection();
        var moves = ReadMoveParts();

        PgnChessMatch chessMatch = new(tags, moves);

        return chessMatch;

    }
    
    private Token? PeekToken(int lookAhead = 0) {
        if (index + lookAhead < _tokens.Length) {
            return _tokens[index];
        }
        return null;
    }

    public Token ReadToken() {
        var token = PeekToken();
        if (token != null) {
            index++;
        }
        return token;
    }

    public Token ExpectToken(TokenType tokenType, string errorMessage) {
        var token = PeekToken();
        if (token == null) {
            throw new PgnSerializationException($"End of file reached when {tokenType} was expected.", int.MaxValue, int.MaxValue);
        }
        if (token?.Type != tokenType) {
            throw new PgnSerializationException($"[{token.Line}:{token.LineOffset}] {errorMessage}Expected {tokenType} but found {token.Type} instead.", token.Line, token.LineOffset);
        }
        index++;
        return token;

    }
    
    public void Advance(int number = 1) {
        index += number;
    }
}