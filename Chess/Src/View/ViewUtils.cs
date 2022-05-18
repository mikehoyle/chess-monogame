using System;
using Chess.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable enable
namespace Chess.View {
  public class ViewUtils {
    // Target Window size  (And literal size for this demo)
    public const int TargetViewportWidth = 1333;
    public const int TargetViewportHeight = 1000;
    // Rendering scale of chess squares, in pixels
    public const float SquareScale = 100f;
    public const float AssetScale = SquareScale / 901f /* width of square asset */;

    // TODO update this so that it places the board better than just top-left
    private static readonly Vector2 BoardOrigin = GetBoardOrigin();

    /// <summary>
    /// Top-left render coords for board square x,y
    /// where x is bottom-up, and y is left-right. 
    /// </summary>
    public static Vector2 GetRenderCoordsForBoardSquare(BoardCoords coords, bool centered = false) {
      var x = BoardOrigin.X + (SquareScale * coords.X);
      var y = BoardOrigin.Y + (SquareScale * (7 - coords.Y));
      if (centered) {
        x += SquareScale / 2;
        y += SquareScale / 2;
      }
      return new Vector2(x, y);
    }

    // Top-left coords of board in order for it to be centered within the viewport
    private static Vector2 GetBoardOrigin() {
      var boardDimension = SquareScale * 8;
      if (TargetViewportWidth < boardDimension || TargetViewportHeight < boardDimension) {
        throw new Exception("Window too small for board");
      }

      return new Vector2(
          MathF.Floor((TargetViewportWidth - boardDimension) / 2),
          MathF.Floor((TargetViewportHeight - boardDimension) / 2));
    }

    public static void RenderScaled(SpriteBatch spriteBatch, Texture2D texture, Vector2 position) {
      RenderScaled(spriteBatch, texture, position, Vector2.Zero);
    }

    public static void RenderScaled(
        SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 origin, float layer = 0f) {
      spriteBatch.Draw(
          texture,
          position,
          null,
          Color.White,
          0f,
          origin,
          AssetScale,
          SpriteEffects.None,
          layer
      );
    }

    public static BoardCoords? GetBoardCoordsFromViewCoords(Point viewCoords) {
      // int cast performs floor
      var x = (int) ((viewCoords.X - BoardOrigin.X) / SquareScale);
      var y = 7 - ((int) ((viewCoords.Y - BoardOrigin.Y) / SquareScale));
      
      // Ensure we're actually inside the board
      if (x < 0 || x > 7 || y < 0 || y > 7) {
        return null;
      }
      return new BoardCoords(x, y);
    }
  }
}