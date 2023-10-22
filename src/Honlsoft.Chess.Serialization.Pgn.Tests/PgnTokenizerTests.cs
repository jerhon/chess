using FluentAssertions;

namespace Honlsoft.Chess.Serialization.Pgn.Tests; 

public class PgnTokenizerTests {


    [Theory]
    [MemberData(nameof(SimpleTokens))]
    public void ReadToken_Success(string tokenString, Token expectedToken) {

        PgnTokenizer tokenizer = new PgnTokenizer(tokenString);

        var actualToken = tokenizer.ReadNextToken();

        actualToken.Should().Be(expectedToken);
    }

    public static TheoryData<string, Token> SimpleTokens {
        get {
            TheoryData<string, Token> data = new TheoryData<string, Token>() {
                {"[", new Token(TokenType.LeftSquareBracket, "[", 0, 0)},
                {"]", new(TokenType.RightSquareBracket, "]", 0, 0)},
                {"symbol", new (TokenType.Symbol,"symbol", 0, 0)},
                {".", new (TokenType.Period, ".", 0, 0)},
                {"\"string\"", new Token(TokenType.String, "string", 0, 0)},
                {"$0123", new Token(TokenType.NumericAnnotationGlyph, "0123", 0, 0)}
            };
            return data;
        }
    }
}