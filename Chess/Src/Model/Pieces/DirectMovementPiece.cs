using System.Collections.Generic;

namespace Chess.Model.Pieces {
  public abstract class DirectMovementPiece : Piece {
    protected abstract (int X, int Y)[] MovementDirections { get; }
    protected DirectMovementPiece(PieceArgs args) : base(args) { }
    
    public override HashSet<BoardCoords> GetMovementPossibilities() {
      var result = new HashSet<BoardCoords>();
      foreach (var coord in MovementDirections) {
        if (CurrentBoardPlacement.IsValidTranslation(coord.X, coord.Y)) {
          var destination = CurrentBoardPlacement.Translate(coord.X, coord.Y);
          if (!Match.BoardState.ContainsKey(destination)) {
            result.Add(destination);
          }
        }
      }
      return result;
    }

    public override HashSet<BoardCoords> GetCapturePossibilities() {
      var result = new HashSet<BoardCoords>();
      foreach (var coord in MovementDirections) {
        if (CurrentBoardPlacement.IsValidTranslation(coord.X, coord.Y)) {
          var destination = CurrentBoardPlacement.Translate(coord.X, coord.Y);
          if (IsEnemyPiece(destination)) {
            result.Add(destination);
          }
        }
      }
      return result;
    }
  }
}