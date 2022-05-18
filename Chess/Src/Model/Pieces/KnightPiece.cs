using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Chess.Model.Pieces {
  public class KnightPiece : DirectMovementPiece {
    protected override string AssetName => "knight";
    public override PieceType Type => PieceType.Knight;

    private static readonly (int X, int Y)[] MovementDirectionsInternal = {
        (1, 2),
        (1, -2),
        (2, 1),
        (2, -1),
        (-1, 2),
        (-1, -2),
        (-2, 1),
        (-2, -1),
    };

    protected override (int X, int Y)[] MovementDirections => MovementDirectionsInternal;

    public KnightPiece(PieceArgs args) : base(args) { }
  }
}