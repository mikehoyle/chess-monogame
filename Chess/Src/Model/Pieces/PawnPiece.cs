using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Chess.Model.Pieces {
  public class PawnPiece : Piece {
    protected override string AssetName => "pawn";
    public override PieceType Type => PieceType.Pawn;
    public override BoardCoords CurrentBoardPlacement {
      get => _currentBoardPlacement;
      set {
        // after first move, can no longer move two
        if (_canMoveTwo 
            && value.Equals(new BoardCoords(
                _currentBoardPlacement.X, _currentBoardPlacement.Y + (2 * _direction)))) {
          _canMoveTwo = false;
          IsValidEnPassantTarget = true;
        }
        else {
          IsValidEnPassantTarget = false;
        }
        
        _currentBoardPlacement = value;
      }
    }
    public bool IsValidEnPassantTarget { get; private set; }

    private BoardCoords _currentBoardPlacement;
    private bool _canMoveTwo;
    // Directional modifier -1 or 1
    private int _direction;
    
    public PawnPiece(PieceArgs args) : base(args) {
      IsValidEnPassantTarget = false;
      _canMoveTwo = true;
      _direction = args.IsDark ? -1 : 1;
    }
    
    public override HashSet<BoardCoords> GetMovementPossibilities() {
      var result = new HashSet<BoardCoords>();
      BoardCoords oneAhead;
      try {
        oneAhead = CurrentBoardPlacement.Translate(0, _direction);
      } catch (IndexOutOfRangeException e) {
        return result;
      }

      if (!Match.BoardState.ContainsKey(oneAhead)) {
        result.Add(oneAhead);
        if (_canMoveTwo) {
          var twoAhead = CurrentBoardPlacement.Translate(0, 2*_direction);
          if (!Match.BoardState.ContainsKey(twoAhead)) {
            result.Add(twoAhead);
          }
        }
      }
      return result;
    }

    public override HashSet<BoardCoords> GetCapturePossibilities() {
      var result = new HashSet<BoardCoords>();
      if (CurrentBoardPlacement.Y == 0 || CurrentBoardPlacement.Y == 7) {
        return result;
      }
      
      foreach (var capturePoint in new[] {-1, 1}) {
        if (CurrentBoardPlacement.IsValidTranslation(capturePoint, _direction)) {
          var captureCoords = CurrentBoardPlacement.Translate(capturePoint, _direction);
          var potentialEnPassantTarget =
              CurrentBoardPlacement.Translate(capturePoint, 0);
          if (IsEnemyPiece(captureCoords)) {
            result.Add(captureCoords);
          } else if (IsEnemyPiece(potentialEnPassantTarget)) {
            if (Match.BoardState[potentialEnPassantTarget]
                is PawnPiece { IsValidEnPassantTarget: true }) {
              result.Add(captureCoords);
            }
          }
        }
      }
      return result;
    }
  }
}