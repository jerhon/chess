using Spectre.Console;
using Spectre.Console.Rendering;

namespace Honlsoft.Chess.Console; 

public class ChessBoardView : Renderable {

    private readonly IChessBoard _chessBoard;
    

    public ChessBoardView(IChessBoard chessBoard) {
        _chessBoard = chessBoard;
    }


    private List<Segment> RenderChessBoard() {

        List<Segment> lines = new List<Segment>(); 
        
        Grid grid = new Grid();
        foreach (var rank in Rank.AllRanks.Reverse()) {
            
            lines.Add(new Segment(rank + " "));
            foreach (var file in File.AllFiles) {
                var square = _chessBoard.GetSquare(new SquareName(file, rank));
                var style = MapStyle( square );
                lines.Add(new Segment(GetPieceString(square.Piece), style));
            }
            lines.Add(Segment.LineBreak);
        }
        
        lines.Add(new Segment("  "));
        foreach (var file in File.AllFiles) {
            lines.Add(new Segment(" " + file.ToString() + " "));
        }
        
        lines.Add(Segment.LineBreak);
        return lines;
    }

    private string GetPieceString(Piece? piece) {
        return " " + (piece?.ToString() ?? " ") + " ";
    }

    private Style MapStyle(Square square) {
        return new Style(GetPieceColor(square.Piece), GetSquareColor(square.Name), Decoration.Bold);
    }

    private Color? GetPieceColor(Piece piece) {
        return piece switch {
            { Color: PieceColor.Black } => Color.Black,
            { Color: PieceColor.White } => Color.White,
            _ => null
        };
    }

    private Color GetSquareColor(SquareName squareName) {
        var color = squareName.Color;

        if (color == SquareColor.Dark) {
            return Color.Aqua;
        } else {
            return Color.LightSlateGrey;
        }
    }
    
    
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth) {

        return RenderChessBoard();
    }
    
}