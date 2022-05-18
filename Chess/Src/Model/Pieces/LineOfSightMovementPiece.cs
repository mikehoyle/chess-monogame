using System.Collections.Generic;

namespace Chess.Model.Pieces {
  public abstract class LineOfSightMovementPiece : Piece {
    protected abstract (int X, int Y)[] MovementDirections { get; }

    protected LineOfSightMovementPiece(PieceArgs args) : base(args) {}
    
    public override HashSet<BoardCoords> GetMovementPossibilities() {
      var result = new HashSet<BoardCoords>();
      foreach (var diff in MovementDirections) {
        var coords = CurrentBoardPlacement;
        while (coords.IsValidTranslation(diff.X, diff.Y)) {
          coords = coords.Translate(diff.X, diff.Y);
          if (Match.BoardState.ContainsKey(coords)) {
            break;
          }
          result.Add(coords);
        }
      }
      return result;
    }

    public override HashSet<BoardCoords> GetCapturePossibilities() {
      var result = new HashSet<BoardCoords>();
      foreach (var diff in MovementDirections) {
        var coords = CurrentBoardPlacement;
        while (coords.IsValidTranslation(diff.X, diff.Y)) {
          coords = coords.Translate(diff.X, diff.Y);
          if (IsEnemyPiece(coords)) {
            result.Add(coords);
            break;
          }
          
          if (Match.BoardState.ContainsKey(coords)) {
            break;
          }
        }
      }
      return result;
    }
  }
}