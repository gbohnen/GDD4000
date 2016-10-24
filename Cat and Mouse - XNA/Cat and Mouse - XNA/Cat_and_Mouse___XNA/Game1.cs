using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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

    /// <summary>
    /// possible winners of the game
    /// </summary>
    public enum Winner { Cats, Mouse }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    [Serializable()]
    public class Game1 : Game, ISerializable
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // gamestate
        public static GameState gameState = GameState.Play;                                   // current state in which the game is running

        // timer 
        public static float timer = GameConstants.GAME_TIMER_START_VALUE;                     // sets the game timer

        public static Game1 instance;

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

            if (instance == null)
                instance = this;

            // initialize each game manager instance
            InputManager.Instance.Initialize();
            AudioManager.Instance.Initialize(Content);
            GraphicsManager.Instance.Initialize(Content);
            EntityManager.Instance.Initialize(this);
            SaveLoadManager.Instance.Initialize(this);

            // register events
            InputManager.Instance.SpacePressed += SpacePressed;
            EntityManager.Instance.EndGame += EndGame;

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
                    EndGame(this, new EndingArgs(Winner.Mouse));
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

        /// <summary>
        /// event delegate for the space bar being pressed
        /// </summary>
        /// <param name="sender"> object from which the vent was called </param>
        /// <param name="e"> event parameters </param>
        private void SpacePressed(object sender, EventArgs e)
        {
            // update in game over mode
            if (gameState == GameState.GameOver)
            {
                    // reset the game
                    timer = GameConstants.GAME_TIMER_START_VALUE;
                    gameState = GameState.Play;
                    AudioManager.Instance.PlayBackground();
                    EntityManager.Instance.ResetGame();
            }
        }

        /// <summary>
        /// event that fires when the game ends
        /// </summary>
        /// <param name="sender"> sending object </param>
        /// <param name="e"> event parameters </param>
        private void EndGame(object sender, EndingArgs e)
        {
            GraphicsManager.Instance.EndGame(e.Winner, timer);
            Game1.gameState = GameState.GameOver;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("TimeRemaining", timer);
            info.AddValue("GameMode", gameState);

            Console.WriteLine("\t TimeRemaining logged");
            Console.WriteLine("\t GameState logged");

            Console.WriteLine("Game1 added...");
        }

        public void ReloadObject(SerializationInfo info, StreamingContext ctxt)
        {
            gameState = (GameState)info.GetValue("GameMode", typeof(int));
            timer = (float)info.GetValue("TimeRemaining", typeof(float));
        }
    }
}
