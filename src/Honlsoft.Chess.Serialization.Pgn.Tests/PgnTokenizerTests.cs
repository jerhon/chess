using FluentAssertions;

namespace Honlsoft.Chess.Serialization.Pgn.Tests; 

public class PgnTokenizerTests {


    [Theory]
    [MemberData(nameof(SimpleTokens))]
    public void ReadToken_Success(string tokenString, Token expectedToken) {

        PgnTokenizer tokenizer = new PgnTokenizer(tokenString);

        var actualToken = tokenizer.ReadNextToken();

        actualToken.Should().Be(expectedToken);

        var noMoreTokens = tokenizer.ReadNextToken();
        noMoreTokens.Should().BeNull();
    }

    [Fact]
    public void ReadAllTokens_Success() {
        PgnTokenizer tokenizer = new PgnTokenizer("[tag \"value\"]");
        var tokens = tokenizer.ReadAllTokens();

        tokens.Should().HaveCount(4);
    }
    
    public static TheoryData<string, Token> SimpleTokens {
        get
        {
            TheoryData<string, Token> data = new TheoryData<string, Token>()
            {
                { "[", new Token(TokenType.LeftSquareBracket, "[", 0, 0) },
                { "]", new(TokenType.RightSquareBracket, "]", 0, 0) },
                { "symbol", new(TokenType.Symbol, "symbol", 0, 0) },
                { ".", new(TokenType.Period, ".", 0, 0) },
                { "\"string\"", new Token(TokenType.String, "string", 0, 0) },
                { "$0123", new Token(TokenType.NumericAnnotationGlyph, "0123", 0, 0) },
                {"{comment}", new Token(TokenType.Comment, "comment", 0, 0) },
                {"1/2-1/2", new Token(TokenType.Symbol, "1/2-1/2", 0, 0)}
            };
            return data;
        }
    }
    
    
}