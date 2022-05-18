#nullable enable
using System;
using System.Collections.Generic;
using Chess.Model.Pieces;
using Chess.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Chess.Model.Pieces.PieceType;

namespace Chess.Model {
  public enum GameState {
    LightTurn,
    DarkTurn,
  }
  public class ChessMatch : DrawableGameComponent {
    private static readonly PieceType?[,] DefaultBoardState = {
        {Rook, Pawn, null, null, null, null, Pawn, Rook},
        {Knight, Pawn, null, null, null, null, Pawn, Knight},
        {Bishop, Pawn, null, null, null, null, Pawn, Bishop},
        {Queen, Pawn, null, null, null, null, Pawn, Queen},
        {King, Pawn, null, null, null, null, Pawn, King},
        {Bishop, Pawn, null, null, null, null, Pawn, Bishop},
        {Knight, Pawn, null, null, null, null, Pawn, Knight},
        {Rook, Pawn, null, null, null, null, Pawn, Rook},
    };

    // Textures
    private Texture2D _lightSquare;
    private Texture2D _darkSquare;
    
    // Actively-updated current board state.
    public Dictionary<BoardCoords, Piece> BoardState { get; private set; }
    public GameState GameState { get; private set; }
    private DragAndDropTarget _dragAndDropTarget;

    public ChessMatch(Game game): base(game) {
      BoardState = BuildDefaultBoardState();
      GameState = GameState.LightTurn;
      _dragAndDropTarget = new DragAndDropTarget();
    }

    private Dictionary<BoardCoords, Piece> BuildDefaultBoardState() {
      var result = new Dictionary<BoardCoords, Piece>();
      ForEachPosition((pos) => {
        var pieceType = DefaultBoardState[pos.X, pos.Y];
        if (pieceType.HasValue) {
          var isDark = pos.Y > 4;
          result.Add(pos, Piece.BuildPiece(pieceType.Value, new PieceArgs() {
              Game = Game,
              Match = this,
              IsDark = isDark,
              Placement = pos,
              PlacePieceCallback = TryPlacePiece,
          }));
        }
      });

      return result;
    }

    public override void Draw(GameTime gameTime) {
      DrawBoard((SpriteBatch) Game.Services.GetService(typeof(SpriteBatch)));
      DrawPieces(gameTime);
    }

    public void TryPlacePiece(Piece piece, BoardCoords destination) {
      if (!piece.IsInteractable()) {
        return;
      }

      var validateMoveResult = piece.VerifyMove(destination);
      if (!validateMoveResult.MoveValid) {
        return;
      }

      // Process side-effects first
      if (validateMoveResult.SideEffect != null) {
        switch (validateMoveResult.SideEffect) {
          case CastleSideEffect castleEffect: {
            var affectedRook = BoardState[castleEffect.RookOrigin];
            MovePiece(affectedRook, castleEffect.RookDestination);
            break;
          }
          case CaptureSideEffect captureEffect:
            if (!BoardState.Remove(captureEffect.CaptureTarget)) {
              throw new Exception("Attempted to capture non-existent piece!");
            }
            break;
        }
      }
      
      // Move piece
      MovePiece(piece, destination);
      
      // Swap turns
      GameState = GameState == GameState.DarkTurn ? GameState.LightTurn : GameState.DarkTurn;
    }

    // TODO calculating all this every frame for a static window is dumb as hell
    private void DrawBoard(SpriteBatch spriteBatch) {
      ForEachPosition((pos) => {
        var texture = pos.IsDarkSquare() ? _darkSquare : _lightSquare;
        var scale = ViewUtils.SquareScale / texture.Height;
        ViewUtils.RenderScaled(
            spriteBatch, texture, ViewUtils.GetRenderCoordsForBoardSquare(pos));
      });
    }
    
    private void DrawPieces(GameTime gameTime) {
      foreach (var piece in BoardState.Values) {
        piece.Draw(gameTime);
      }
    }

    public void LoadContent(ContentManager contentManager) {
      _lightSquare = contentManager.Load<Texture2D>("squares/light_square_grey");
      _darkSquare = contentManager.Load<Texture2D>("squares/dark_square_grey");
          
      foreach (var piece in BoardState.Values) {
        piece.LoadContent(contentManager);
      }
    }
    
    public override void Update(GameTime gameTime) {
      // Mouse drag-and-drop handling
      var mouseState = Mouse.GetState();
      var coordsForPosition = ViewUtils.GetBoardCoordsFromViewCoords(mouseState.Position);
      Piece? hoveredPiece = null;
      if (coordsForPosition != null) {
        BoardState.TryGetValue(coordsForPosition, out hoveredPiece);
      }
      
      hoveredPiece = hoveredPiece?.IsInteractable() == true ? hoveredPiece : null;
      _dragAndDropTarget.Update(mouseState, hoveredPiece);
    }

    private void ForEachPosition(Action<BoardCoords> action) {
      for (var i = 0; i < (8 * 8); i++) {
        action.Invoke(new BoardCoords(i));
      }
    }

    private void MovePiece(Piece piece, BoardCoords destination) {
      BoardState.Remove(piece.CurrentBoardPlacement);
      piece.CurrentBoardPlacement = destination;
      BoardState.Add(destination, piece);
    }
  }
}