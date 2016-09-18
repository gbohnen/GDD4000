using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Cat_and_Mouse___XNA
{
    class User : Sprite
    {
        #region Fields


        #endregion

        #region Constructors

        /// <summary>
        /// builds the mouse object with a given sprite
        /// </summary>
        /// <param name="image"> the sprite that the object will draw as </param>
        public User(Texture2D image, Game game) : base(image, game)
        {
            // create the draw rectangle
            drawRect = new Rectangle(GameConstants.WINDOW_WIDTH / 2 - GameConstants.MOUSE_WIDTH / 2, GameConstants.WINDOW_HEIGHT / 2 - GameConstants.MOUSE_HEIGHT / 2,
                                GameConstants.MOUSE_WIDTH, GameConstants.MOUSE_HEIGHT);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the position and rotation of the mouse based on user input
        /// </summary>
        /// <param name="state"> the current state of the keyboard </param>
        public override void Update(KeyboardState state, GameTime gameTime)
        {
            // zero the direction vector
            direction = new Vector2(0, 0);

            // change direction based on keypresses
            if (state.IsKeyDown(Keys.W))
            {
                direction.Y -= 1;
            }
            if (state.IsKeyDown(Keys.D))
            {
                direction.X += 1;
            }
            if (state.IsKeyDown(Keys.A))
            {
                direction.X -= 1;
            }
            if (state.IsKeyDown(Keys.S))
            {
                direction.Y += 1;
            }

            Move(direction, gameTime);

            // if space is pressed and the timer is up, jump to a new locaion and reset
            if (state.IsKeyDown(Keys.Space))
                if (jumpTimer >= GameConstants.MOUSE_JUMP_START_VALUE)
                {
                    Jump();
                    jumpTimer = 0;
                }

            // update the jump timer
            if (jumpTimer < GameConstants.MOUSE_JUMP_START_VALUE)
                jumpTimer += gameTime.ElapsedGameTime.Milliseconds;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// move the object based on a direction
        /// </summary>
        /// <param name="direction"> the direction in which we want to move </param>
        protected override void Move(Vector2 direction, GameTime gameTime)
        {
            // if the direction is non-zero
            if (direction != new Vector2(0, 0))
            {
                // normalize the direction vector
                direction.Normalize();

                // move in that direction
                drawRect.X += (int)(direction.X * GameConstants.MOUSE_TOP_SPEED * gameTime.ElapsedGameTime.Milliseconds);
                drawRect.Y += (int)(direction.Y * GameConstants.MOUSE_TOP_SPEED * gameTime.ElapsedGameTime.Milliseconds);

                // orient the sprite
                rotation = (float)Math.Atan2(direction.Y, direction.X);

                // keep the sprite in the bounds of the screen
                Clamp();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// returns the velocity vector of the user
        /// </summary>
        public override Vector2 Velocity
        {
            get
            {
                direction.Normalize();
                return (direction * GameConstants.MOUSE_TOP_SPEED);
            }
        }
        #endregion
    }
}
