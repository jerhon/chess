namespace Honlsoft.Chess.Serialization.Pgn; 

public record Token(TokenType Type, string Value, int Line, int LineOffset);