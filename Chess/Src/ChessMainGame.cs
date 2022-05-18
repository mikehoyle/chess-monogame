using Chess.Model;
using Chess.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chess {
  public class ChessMainGame : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    // Model files
    private ChessMatch _chessMatch;

    public ChessMainGame() {
      _graphics = new GraphicsDeviceManager(this);
      _chessMatch = new ChessMatch(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
    }

    protected override void Initialize() {
      _graphics.PreferredBackBufferWidth = ViewUtils.TargetViewportWidth;
      _graphics.PreferredBackBufferHeight = ViewUtils.TargetViewportHeight;
      _graphics.ApplyChanges();
      base.Initialize();
    }

    protected override void LoadContent() {
      _spriteBatch = new SpriteBatch(GraphicsDevice);
      this.Services.AddService(typeof(SpriteBatch), _spriteBatch);
      _chessMatch.LoadContent(Content);
      // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime) {
      if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
        Exit();
      }
      _chessMatch.Update(gameTime);
      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
      GraphicsDevice.Clear(Color.AntiqueWhite);
      _spriteBatch.Begin(SpriteSortMode.FrontToBack);
      _chessMatch.Draw(gameTime);
      _spriteBatch.End();
      base.Draw(gameTime);
    }
  }
}