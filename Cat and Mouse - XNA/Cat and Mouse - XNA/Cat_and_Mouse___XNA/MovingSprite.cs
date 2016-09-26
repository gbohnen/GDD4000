using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace Cat_and_Mouse___XNA
{
    class MovingSprite : Sprite
    {
        public static Random rand = new Random();

        #region Constructors

        public MovingSprite(Texture2D image, Game game) : base(image, game)
        {

        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Updates the position and of the object based on a position
        /// </summary>
        public virtual void Update(Sprite user, GameTime gameTime) { }

        /// <summary>
        /// Updates the position of the object based on keyboard input
        /// </summary>
        /// <param name="state"> the current state of the keyboard </param>
        public virtual void Update(KeyboardState state, GameTime gameTime) { }

        /// <summary>
        /// moves the character to a random position within the game window
        /// </summary>
        public virtual void Jump()
        {
            // set new location
            drawRect.X = rand.Next(0, GameConstants.WINDOW_WIDTH);
            drawRect.Y = rand.Next(0, GameConstants.WINDOW_HEIGHT);

            position = Position;

            // bring the character back within the window bounds
            Clamp();
        }

        public virtual void SetPosition(float x, float y)
        {
            drawRect.X = (int)(x - (drawRect.Width / 2));
            drawRect.Y = (int)(y - (drawRect.Height / 2));
        }

        #endregion
    }
}
