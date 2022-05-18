using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Chess.Model.Pieces {
  public class QueenPiece : LineOfSightMovementPiece {
    protected override string AssetName => "queen";
    public override PieceType Type => PieceType.Queen;
    
    private static readonly (int X, int Y)[] MovementDirectionsInternal = {
        (0, 1),
        (0, -1),
        (-1, 0),
        (1, 0),
        (1, 1),
        (1, -1),
        (-1, 1),
        (-1, 1),
    };

    protected override (int X, int Y)[] MovementDirections => MovementDirectionsInternal;
    
    public QueenPiece(PieceArgs args) : base(args) {}
  }
}