#nullable enable
using System;
using System.Collections.Generic;
using Chess.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace Chess.Model.Pieces {
  public enum PieceType {
    Pawn,
    Bishop,
    Knight,
    Rook,
    Queen,
    King,
  }

  public class PieceArgs {
    public Game Game { get; set; }
    public ChessMatch Match { get; set; }
    public bool IsDark { get; set; }
    public BoardCoords Placement { get; set; }
    public Piece.TryPlacePiece PlacePieceCallback { get; set; }
  }
  
  public abstract class Piece : DrawableGameComponent, IDragAndDroppable {
    private Texture2D _texture;
    private readonly TryPlacePiece _placePieceCallback;
    
    public abstract PieceType Type { get; }
    protected abstract string AssetName { get; }
    protected readonly ChessMatch Match;

    private readonly bool _isDark;
    private BoardCoords _currentBoardPlacement;

    public virtual BoardCoords CurrentBoardPlacement { get; set; }

    public DragAndDropState DragAndDropState { get; set; }
    public delegate void TryPlacePiece(Piece piece, BoardCoords position);

    protected Piece(PieceArgs args) : base(args.Game) {
      _isDark = args.IsDark;
      Match = args.Match;
      CurrentBoardPlacement = args.Placement;
      _placePieceCallback = args.PlacePieceCallback;
      DragAndDropState = new DragAndDropState {
          IsSelected = false,
          Position = Point.Zero,
      };
    }

    /// <summary>
    /// Builds specified Piece object
    /// </summary>
    public static Piece BuildPiece(PieceType type, PieceArgs args) {
      switch (type) {
        case PieceType.Pawn:
          return new PawnPiece(args);
        case PieceType.Bishop:
          return new BishopPiece(args);
        case PieceType.Knight:
          return new KnightPiece(args);
        case PieceType.Rook:
          return new RookPiece(args);
        case PieceType.Queen:
          return new QueenPiece(args);
        case PieceType.King:
          return new KingPiece(args);
        default:
          throw new InvalidOperationException();
      }
    }

    // TODO fix so dragging piece renders on top
    public override void Draw(GameTime gameTime) {
      var origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
      var position = DragAndDropState.IsSelected
          ? DragAndDropState.Position.ToVector2()
          : ViewUtils.GetRenderCoordsForBoardSquare(CurrentBoardPlacement, centered: true);
      // TODO using arbitrary magic numbers like this is terrible
      var layer = DragAndDropState.IsSelected ? 0.2f : 0.1f;
      ViewUtils.RenderScaled(
          (SpriteBatch) Game.Services.GetService(typeof(SpriteBatch)),
          _texture,
          position,
          origin,
          layer);
    }
    
    public void LoadContent(ContentManager contentManager) {
      var color = _isDark ? "b_" : "w_";
      _texture = contentManager.Load<Texture2D>($"pieces/{color}{AssetName}_2x");
    }

    public bool IsInteractable() {
      var gameState = Match.GameState;
      return (gameState == GameState.DarkTurn && _isDark) ||
             (gameState == GameState.LightTurn && !_isDark);
    }

    // TODO add all movement controls and interfaces
    public abstract HashSet<BoardCoords> GetMovementPossibilities();
    public abstract HashSet<BoardCoords> GetCapturePossibilities();

    public void OnMouseDrop(Point mouseLoc) {
      var targetLocation = ViewUtils.GetBoardCoordsFromViewCoords(mouseLoc);
      if (targetLocation == null) {
        // Do nothing if dropped outside board
        return;
      }

      _placePieceCallback(this, targetLocation);
    }

    
    public class AttemptMoveResult {
      public bool MoveValid { get; set; }
      public IPieceMoveSideEffect? SideEffect { get; set; }
    }

    public AttemptMoveResult VerifyMove(BoardCoords target) {
      if (!IsInteractable()) {
        return new AttemptMoveResult { MoveValid = false };
      }

      if (GetCapturePossibilities().Contains(target)) {
        var result = new AttemptMoveResult {
            MoveValid = true,
            SideEffect = new CaptureSideEffect { CaptureTarget = target }
        };
        
        // Check for en-passant
        if (Type == PieceType.Pawn) {
          if (!Match.BoardState.ContainsKey(target)) {
            // Must be en-passant if we can validly attack a not-present piece
            var targetYDiff = _isDark ? 1 : -1;
            result.SideEffect = new CaptureSideEffect {
                CaptureTarget = new BoardCoords(target.X, target.Y + targetYDiff),
            };
          }
        }
        
        return result;
      }

      if (GetMovementPossibilities().Contains(target)) {
        var result = new AttemptMoveResult { MoveValid = true };
        if (Type == PieceType.King) {
          // Castle validity checked in GetMovementPossbilities, just detect attempt here
          var movementDiff = CurrentBoardPlacement.X - target.X;
          if (Math.Abs(movementDiff) == 2) {
            var rookOriginX = movementDiff == 2 ? 0 : 7;
            var rookDestinationX = movementDiff == 2 ? 3 : 5;
            result.SideEffect = new CastleSideEffect {
                RookOrigin = new BoardCoords(rookOriginX, target.Y),
                RookDestination = new BoardCoords(rookDestinationX, target.Y),
            };
          }
        }
        return result;
      }

      return new AttemptMoveResult { MoveValid = false };
    }

    protected bool IsEnemyPiece(BoardCoords target) {
      return Match.BoardState.ContainsKey(target) && _isDark != Match.BoardState[target]._isDark;
    }
  }
}