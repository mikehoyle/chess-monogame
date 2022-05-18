using System.Collections.Generic;
using System.Linq;

namespace Chess.Model.Pieces {
  public class BishopPiece : LineOfSightMovementPiece {
    protected override string AssetName => "bishop";
    public override PieceType Type => PieceType.Bishop;

    private static readonly (int X, int Y)[] MovementDirectionsInternal = {
        (1, 1),
        (1, -1),
        (-1, 1),
        (-1, -1),
    };

    protected override (int X, int Y)[] MovementDirections => MovementDirectionsInternal;

    public BishopPiece(PieceArgs args) : base(args) { }
  }
}