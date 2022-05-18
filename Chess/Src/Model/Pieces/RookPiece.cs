using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Chess.Model.Pieces {
  public class RookPiece : LineOfSightMovementPiece {
    private BoardCoords _currentBoardPlacement;
    public bool IsCastleable { get; private set; }
    
    public override BoardCoords CurrentBoardPlacement {
      get => _currentBoardPlacement;
      set {
        IsCastleable = false;
        _currentBoardPlacement = value;
      }
    }
    protected override string AssetName => "rook";
    public override PieceType Type => PieceType.Rook;
    
    private static readonly (int X, int Y)[] MovementDirectionsInternal = {
        (0, 1),
        (0, -1),
        (-1, 0),
        (1, 0),
    };

    protected override (int X, int Y)[] MovementDirections => MovementDirectionsInternal;

    public RookPiece(PieceArgs args) : base(args) {
      IsCastleable = true;
    }
  }
}