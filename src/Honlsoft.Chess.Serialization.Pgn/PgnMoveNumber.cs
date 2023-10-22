namespace Honlsoft.Chess.Serialization.Pgn; 

public record PgnMoveNumber(int Number, int Periods) : PgnMovePart {

    public bool IsWhiteMove() {
        return Periods == 1;
    }

    public bool IsBlackMove() {
        return Periods == 3;
    }
}