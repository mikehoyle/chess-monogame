using System;
using Microsoft.Xna.Framework;

namespace Chess.Model {
  /// <summary>
  /// Board coordinates where x is bottom-up, and y is left-right. 
  /// </summary>
  public class BoardCoords {
    public BoardCoords(int x, int y) {
      X = x;
      Y = y;
      assertInBounds(X);
      assertInBounds(Y);
    }
    
    /// <summary>
    /// Builds from bottom-top, left-right 0-8*8 index
    /// </summary>
    public BoardCoords(int index) {
      X = index % 8;
      Y = index / 8;
      
      assertInBounds(X);
      assertInBounds(Y);
    }

    // TODO I should've just used Point here
    public int X { get; }
    public int Y { get; }

    public bool IsDarkSquare() {
      return (X % 2) == 0 ^ (Y % 2) == 1;
    }

    public BoardCoords Translate(int xDiff, int yDiff) {
      return new BoardCoords(X + xDiff, Y + yDiff);
    }

    public bool IsValidTranslation(int xDiff, int yDiff) {
      return CoordsInRange(X + xDiff, Y + yDiff);
    }

    public static bool CoordsInRange(int x, int y = 0) {
      return x < 8 && x > -1 && y < 8 && y > -1;
    } 

    public override bool Equals(object obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }
      BoardCoords other = (BoardCoords) obj;
      return X == other.X && Y == other.Y;
    }

    public override int GetHashCode() {
      return HashCode.Combine(X, Y);
    }

    private void assertInBounds(int coord) {
      if (!CoordsInRange(coord)) {
        throw new IndexOutOfRangeException("Attempt to set board coords out of range");
      }
    }
  }
}