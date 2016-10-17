using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Cat_and_Mouse___XNA
{
    /// <summary>
    /// keeps track of the current state of the game
    /// </summary>
    public enum GameState
    {
        Menu,
        Play,
        GameOver
    }

    public enum Winner { Cats, Mouse }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // gamestate
        public static GameState gameState = GameState.Play;                                   // current state in which the game is running

        // timer 
        public static float timer = GameConstants.GAME_TIMER_START_VALUE;                     // sets the game timer

        // flags
        public static bool spacePressed = false;                                              // checks if space bar has been pressed. prevents duplicate input in menus

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set the screen resolution
            graphics.PreferredBackBufferWidth = GameConstants.WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = GameConstants.WINDOW_HEIGHT;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here        

            // initialize each game manager instance
            InputManager.Instance.Initialize();
            AudioManager.Instance.Initialize(Content);
            GraphicsManager.Instance.Initialize(Content);
            EntityManager.Instance.Initialize(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // add the spritebatch to services, so that it may be called from DrawableGameComponent
            Services.AddService(typeof(SpriteBatch), spriteBatch);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // update the keyboard state
            KeyboardState state = Keyboard.GetState();

            // update the input manager
            InputManager.Instance.Update(state, gameTime);

            // update in gameplay mode
            if (gameState == GameState.Play)
            {
                EntityManager.Instance.Update(state, gameTime);

                // update the timer
                timer -= gameTime.ElapsedGameTime.Milliseconds;

                // check if time has run out
                if (timer <= 0)
                {
                    GraphicsManager.Instance.EndGame(Winner.Mouse, timer);

                    // possibly refactor
                    Game1.gameState = GameState.GameOver;
                    Game1.spacePressed = false;
                }
            }

            // update in game over mode
            else if (gameState == GameState.GameOver)
            {
                if (state.IsKeyDown(Keys.Space) && !spacePressed)
                {
                    // reset the game
                    timer = GameConstants.GAME_TIMER_START_VALUE;
                    gameState = GameState.Play;
                    AudioManager.Instance.PlayBackground();
                    EntityManager.Instance.ResetGame();

                    spacePressed = true;
                }

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // draw timer
            GraphicsManager.Instance.DrawGameTimer(spriteBatch, timer);

            // draw messages
            if (gameState == GameState.GameOver)
            {
                GraphicsManager.Instance.DrawEndMessage(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
