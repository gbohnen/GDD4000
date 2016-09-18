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

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // gamestate
        GameState gameState = GameState.Play;                                   // current state in which the game is running

        // agents/actors
        User mouse;                                                             // mouse object
        Agent cat;                                                              // cat object
        //Tank tank;                                                              // tank object
        //Cycle cycle;                                                            // motorcycle object

        // bars
        TimerBar jumpTimer;                                                     // mouse jump timer

        // collections
        List<Agent> agents;                                                     // collection of agents
        List<Bar> bars;                                                         // list of bars

        // sprites
        Texture2D mouseSprite;                                                  // mouse sprite
        Texture2D catSprite;                                                    // cat sprite
        Texture2D tankSprite;                                                   // tank sprite
        Texture2D cycleSprite;                                                  // motorcycle sprite
        Texture2D barSprite;                                                    // sprite for drawing bars
        SpriteFont font;                                                        // base font for all message

        // timer 
        float timer = GameConstants.GAME_TIMER_START_VALUE;                     // sets the game timer

        // winning messages
        string catWin = "The enemies got you! Better luck next time.";           // prefab cat win message
        string mouseWin = "You got away! Nice job!";                            // prefab mouse win message
        string winMessage;                                                      // container for the winning message
        string spaceString = "Press Space to play again.";                      // prefab input prompt
        string baseTimeString = "You lasted for ";                              // base feedback string
        string timeString;                                                      // feedback string

        // flags
        bool hasStarted = false;                                                // checks if the game has started. used for initializing the level each game
        bool spacePressed = false;                                              // checks if space bar has been pressed. prevents duplicate input in menus

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

            // load sprites and font
            catSprite = Content.Load<Texture2D>("Assets/cat_nan");
            mouseSprite = Content.Load<Texture2D>("Assets/mouse_nan");
            tankSprite = Content.Load<Texture2D>("Assets/tank");
            cycleSprite = Content.Load<Texture2D>("Assets/cycle");
            barSprite = Content.Load<Texture2D>("Assets/Blank");
            font = Content.Load<SpriteFont>("MyFont");

            // initialize list
            agents = new List<Agent>();
            bars = new List<Bar>();

            // create entities
            mouse = new User(mouseSprite, this);
            cat = new Agent(catSprite, this);
            //tank = new Tank(tankSprite, this);
            //cycle = new Cycle(cycleSprite, this);

            jumpTimer = new TimerBar(barSprite, barSprite, 0, GameConstants.MOUSE_JUMP_START_VALUE, mouse, this);

            // add each agent to the collection appropriate collections
            agents.Add(cat);
            //agents.Add(tank);
            //agents.Add(cycle);
            bars.Add(jumpTimer);
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

            // move sprites if they have not yet been positioned
            if (hasStarted == false)
            {
                foreach (Agent agent in agents)
                {
                    agent.Jump();

                    // make sure the cuser does not overlap any agents
                    while (mouse.DrawRectangle.Intersects(agent.DrawRectangle))
                    {
                        mouse.Jump();
                    }
                }

                hasStarted = true;
            }

            // update the keyboard state
            KeyboardState state = Keyboard.GetState();

            // update in gameplay mode
            if (gameState == GameState.Play)
            {
                // update user
                mouse.Update(state, gameTime);

                // update bars
                foreach (Bar bar in bars)
                {
                    bar.Update();
                }

                // update agents
                foreach (Agent agent in agents)
                {
                    agent.Update(mouse, gameTime);

                    // check collisions
                    if (mouse.DrawRectangle.Intersects(agent.DrawRectangle))
                    {
                        EndGame(catWin);
                    }
                }

                // update the timer
                timer -= gameTime.ElapsedGameTime.Milliseconds;

                // check if time has run out
                if (timer <= 0)
                {
                    EndGame(mouseWin);
                }
            }

            // update in game over mode
            else if (gameState == GameState.GameOver)
            {
                if (state.IsKeyDown(Keys.Space) && !spacePressed)
                {
                    // reset the game
                    timer = GameConstants.GAME_TIMER_START_VALUE;
                    cat.Jump();
                    mouse.Jump();
                    gameState = GameState.Play;

                    spacePressed = true;
                    hasStarted = false;
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

            // draw sprites
            foreach (Agent agent in agents)
            {
                agent.Draw(spriteBatch);
            }
            foreach (Bar bar in bars)
            {
                bar.Draw(spriteBatch);
            }
            mouse.Draw(spriteBatch);

            // draw timer
            spriteBatch.DrawString(font, "Time Remaining: " + (int)(timer / 1000 + 1), new Vector2(10, 10), Color.White);

            // draw messages
            if (gameState == GameState.GameOver)
            {
                // draw messages
                spriteBatch.DrawString(font, winMessage, new Vector2(PositionTextHoriz(winMessage), GameConstants.WINDOW_HEIGHT / 2 - 20), Color.White);
                spriteBatch.DrawString(font, timeString, new Vector2(PositionTextHoriz(timeString), GameConstants.WINDOW_HEIGHT / 2), Color.White);
                spriteBatch.DrawString(font, spaceString, new Vector2(PositionTextHoriz(spaceString), GameConstants.WINDOW_HEIGHT / 2 + 20), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Private / Protected Methods

        /// <summary>
        /// centers the text horizontally in the screen
        /// </summary>
        /// <param name="text"> the target string to center </param>
        /// <returns></returns>
        protected int PositionTextHoriz(string text)
        {
            return (GameConstants.WINDOW_WIDTH / 2) - (int)(font.MeasureString(text).X / 2);
        }

        /// <summary>
        /// sets up fields to display the endgame message properly
        /// </summary>
        /// <param name="winString"> the string appropriate to the winner to draw </param>
        protected void EndGame(string winString)
        {
            winMessage = winString;
            timeString = baseTimeString + ((GameConstants.GAME_TIMER_START_VALUE - timer) / 1000) + " seconds.";
            gameState = GameState.GameOver;

            spacePressed = false;
        }

        /// <summary>
        /// places all agents and user in the screen without overlapping
        /// </summary>
        protected void PlaceObjects()
        {
            // place each agent
            foreach (Agent agent in agents)
            {
                agent.Jump();
            }

            //while (mouse.DrawRectangle.Intersects())
            {
                mouse.Jump();
            }
        }
        #endregion
    }
}
