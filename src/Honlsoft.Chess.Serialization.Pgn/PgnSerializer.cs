using System.IO.Compression;
using System.Text;
using Honlsoft.Chess.Serialization.Pgn;

namespace Honlsoft.Chess.Serialization; 

/// <summary>
/// Serializes and deserializes chess games in PGN format.
/// </summary>
public class PgnSerializer() {



    public PgnChessMatch DeserializePgnChessMatch(string text) {
        PgnTokenizer tokenizer = new PgnTokenizer(text);
        PgnReader reader = new PgnReader(tokenizer);

        return reader.Read();
    }
}