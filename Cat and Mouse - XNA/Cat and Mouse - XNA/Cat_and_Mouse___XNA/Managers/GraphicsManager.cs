using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Cat_and_Mouse___XNA
{
    public enum SpriteType { BLANK, CAT, MOUSE, CYCLE, TANK }

    class GraphicsManager
    {
        #region Fields

        private static GraphicsManager instance;                      // singleton instance of the manager
        Dictionary<SpriteType, Texture2D> spriteLibrary;        // library that holds values for each version of the sprite enumeration
        SpriteFont defaultFont;                                 // the default UI font

        // winning messages
        string catWin = "The enemies got you! Better luck next time.";          // prefab cat win message
        string mouseWin = "You got away! Nice job!";                            // prefab mouse win message
        string winMessage;                                                      // container for the winning message
        string spaceString = "Press Space to play again.";                      // prefab input prompt
        string baseTimeString = "You lasted for ";                              // base feedback string
        string timeString;

        #endregion


        #region Constructors

        /// <summary>
        /// basic constructor. performs all logic that needs to be done before initialization
        /// </summary>
        private GraphicsManager()
        {
            spriteLibrary = new Dictionary<SpriteType, Texture2D>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// accesses the singleton instance of the manager
        /// </summary>
        public static GraphicsManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new GraphicsManager();

                return instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads all content and initializes most variables
        /// </summary>
        /// <param name="Content"></param>
        public void Initialize(ContentManager Content)
        {
            // load sprites for each version of the sprite enum
            spriteLibrary.Add(SpriteType.BLANK, Content.Load<Texture2D>("Assets/Blank"));
            spriteLibrary.Add(SpriteType.CAT, Content.Load<Texture2D>("Assets/cat_nan"));
            spriteLibrary.Add(SpriteType.MOUSE, Content.Load<Texture2D>("Assets/mouse_nan"));
            spriteLibrary.Add(SpriteType.CYCLE, Content.Load<Texture2D>("Assets/cycle"));
            spriteLibrary.Add(SpriteType.TANK, Content.Load<Texture2D>("Assets/tank"));

            // load the default font
            defaultFont = Content.Load<SpriteFont>("MyFont");
        }

        /// <summary>
        /// gets a sprite based on a given enumerable value
        /// </summary>
        /// <param name="type"> key to sprite </param>
        /// <returns> given value for the key </returns>
        public Texture2D GetSprite(SpriteType type)
        {
            return spriteLibrary[type];
        }

        /// <summary>
        /// draws the timer for the main gameplay loop
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="timer"></param>
        public void DrawGameTimer(SpriteBatch spriteBatch, float timer)
        {
            spriteBatch.DrawString(defaultFont, "Time Remaining: " + (int)(timer / 1000 + 1), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(defaultFont, "Button Config: " + InputManager.Instance.HandednessConfig, new Vector2(10, 40), Color.White);
        }

        public void EndGame(Winner win, float timer)
        {
            switch (win)
            {
                case Winner.Cats:
                    winMessage = catWin;
                    AudioManager.Instance.PlayGameover();
                    break;
                case Winner.Mouse:
                    winMessage = mouseWin;
                    AudioManager.Instance.PlayFanfare();
                    break;
                default:
                    break;
            }

            timeString = baseTimeString + ((GameConstants.GAME_TIMER_START_VALUE - timer) / 1000) + " seconds.";

            // this should really be in the gamemanager, but this was the quickest way to get out of looping some weird code in the cat win state
            Game1.gameState = GameState.GameOver;
            Game1.spacePressed = false;
        }

        /// <summary>
        /// draws the appropriate message for the end game screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawEndMessage(SpriteBatch spriteBatch)
        {
            // draw messages
            spriteBatch.DrawString(defaultFont, winMessage, new Vector2(PositionTextHoriz(winMessage), GameConstants.WINDOW_HEIGHT / 2 - 20), Color.White);
            spriteBatch.DrawString(defaultFont, timeString, new Vector2(PositionTextHoriz(timeString), GameConstants.WINDOW_HEIGHT / 2), Color.White);
            spriteBatch.DrawString(defaultFont, spaceString, new Vector2(PositionTextHoriz(spaceString), GameConstants.WINDOW_HEIGHT / 2 + 20), Color.White);
        }

        #endregion

        #region Private Members

        /// <summary>
        /// centers the text horizontally in the screen
        /// </summary>
        /// <param name="text"> the target string to center </param>
        /// <returns></returns>
        protected int PositionTextHoriz(string text)
        {
            return (GameConstants.WINDOW_WIDTH / 2) - (int)(defaultFont.MeasureString(text).X / 2);
        }

        #endregion
    }
}
