namespace Chess.Model.Pieces {
  public interface IPieceMoveSideEffect {}
  
  /* When a piece captures another piece */
  public class CaptureSideEffect : IPieceMoveSideEffect {
    public BoardCoords CaptureTarget { get; set; }
  }

  /* Handle the rook move in a castle (king move handled separately) */
  public class CastleSideEffect : IPieceMoveSideEffect {
    public BoardCoords RookOrigin { get; set; }
    public BoardCoords RookDestination { get; set; }
  }
  
  // TODO handle promotion
}