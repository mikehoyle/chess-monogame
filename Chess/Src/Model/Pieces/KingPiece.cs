using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Chess.Model.Pieces {
  public class KingPiece : DirectMovementPiece {
    private BoardCoords _currentBoardPlacement;
    public bool IsCastleable { get; private set; }
    
    public override BoardCoords CurrentBoardPlacement {
      get => _currentBoardPlacement;
      set {
        IsCastleable = false;
        _currentBoardPlacement = value;
      }
    }
    protected override string AssetName => "king";
    public override PieceType Type => PieceType.King;
    
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

    public KingPiece(PieceArgs args) : base(args) {
      IsCastleable = true;
    }

    public override HashSet<BoardCoords> GetMovementPossibilities() {
      var result = base.GetMovementPossibilities();
      result.UnionWith(GetCastleCandidates());
      return result;
    }

    private HashSet<BoardCoords> GetCastleCandidates() {
      var result = new HashSet<BoardCoords>();
      if (!IsCastleable) {
        return result;
      }

      if (!Match.BoardState.ContainsKey(CurrentBoardPlacement.Translate(-1, 0))
          && !Match.BoardState.ContainsKey(CurrentBoardPlacement.Translate(-2, 0))
          && !Match.BoardState.ContainsKey(CurrentBoardPlacement.Translate(-3, 0))
          && Match.BoardState.ContainsKey(CurrentBoardPlacement.Translate(-4, 0))) {
        if (Match.BoardState[CurrentBoardPlacement.Translate(-4, 0)] is
            RookPiece { IsCastleable: true }) {
          result.Add(CurrentBoardPlacement.Translate(-2, 0));
        }
      }
      
      if (!Match.BoardState.ContainsKey(CurrentBoardPlacement.Translate(1, 0))
          && !Match.BoardState.ContainsKey(CurrentBoardPlacement.Translate(2, 0))
          && Match.BoardState.ContainsKey(CurrentBoardPlacement.Translate(3, 0))) {
        if (Match.BoardState[CurrentBoardPlacement.Translate(3, 0)] is
            RookPiece { IsCastleable: true }) {
          result.Add(CurrentBoardPlacement.Translate(2, 0));
        }
      }
      
      return result;
    }
  }
}