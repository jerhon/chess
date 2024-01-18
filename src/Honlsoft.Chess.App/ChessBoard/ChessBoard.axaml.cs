using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Honlsoft.Chess.Serialization;
using Panel = Avalonia.Controls.Panel;

namespace Honlsoft.Chess.App;

public partial class ChessBoard : UserControl
{
    
    public static readonly DirectProperty<ChessBoard, string> FenStringProperty =
        AvaloniaProperty.RegisterDirect<ChessBoard, string>(
            nameof(FenString),
            o => o.FenString,
            (o, v) => o.FenString = v);
    
    
    private ChessGame _chessGame = new ChessGameFactory().CreateStandardGame();
    
    Dictionary<SquareName, Panel> _squareMap = new Dictionary<SquareName, Panel>();
    
    public ChessBoard()
    {
        InitializeComponent();
        
        
        foreach (var file in SquareFile.AllFiles) {
            foreach (var rank in SquareRank.AllRanks) {
                var square = new Panel();
                square.SetValue(Grid.ColumnProperty, file.Index);
                square.SetValue(Grid.RowProperty, rank.Index);
                
                var squareName = new SquareName(file, rank);
                square.Tag = squareName;
                square.Classes.Add("square");
                square.Children.Add(new TextBlock() {Text = squareName.ToString(), Foreground = Avalonia.Media.Brushes.Black});
                _squareMap.Add(squareName, square);
                
                square.Background = (file.Index + rank.Index) % 2 == 0 ? Avalonia.Media.Brushes.White : Avalonia.Media.Brushes.LightGreen;
                ChessGrid.Children.Add(square);
            }
        }
        
        PopulateSquaresFromFenString();
        
    }

    public string FenString {
        get {
            FenSerializer fenSerializer = new FenSerializer();
            return fenSerializer.Serialize(_chessGame.CurrentPosition);
        }
        set {
            _chessGame = new ChessGameFactory().CreateGameFromFen(value);
            PopulateSquaresFromFenString();
        }
    }


    private void PopulateSquaresFromFenString() {
        
        foreach (var squareName in SquareName.AllSquares()) {
            var square = _chessGame.CurrentPosition.GetSquare(squareName);
            var squarePanel = _squareMap[squareName];
            squarePanel.Children.Clear();
            if (square.Piece != null) {
                var pieceText = new TextBlock() {Text = square.Piece.ToString(), Foreground = Avalonia.Media.Brushes.Black};
                squarePanel.Children.Add(pieceText);
            }
        }
    }

}