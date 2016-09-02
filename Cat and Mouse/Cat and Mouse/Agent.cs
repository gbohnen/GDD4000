using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Cat_and_Mouse
{
    class Agent : Sprite
    {
        #region Constructors

        /// <summary>
        /// builds the cat object with a given sprite
        /// </summary>
        /// <param name="image"> the sprite that the object will draw as </param>
        public Agent(Texture2D image, Game game) : base(image, game)
        {
            // create the draw rectangle
            drawRect = new Rectangle(GameConstants.WINDOW_WIDTH / 2, GameConstants.WINDOW_HEIGHT / 2, GameConstants.CAT_WIDTH, GameConstants.CAT_HEIGHT);

            // set physics properties
            maxAccel = GameConstants.CAT_MAX_ACCEL;
            maxVelocity = GameConstants.CAT_TOP_SPEED;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the position and rotation of the cat based on the mouse's position
        /// </summary>
        /// <param name="pos"> position of the mouse </param>
        public override void Update(Sprite user, GameTime gameTime)
        {
            // set the direction based on the mouse position
            direction = user.Position - Position;

            // currently bugged arrive code
            //// set the speed based on proximity to the mouse
            //if (Vector2.Distance(Position, pos) >= GameConstants.CAT_ARRIVE_RANGE)
            //{
            //    topSpeed = GameConstants.CAT_TOP_SPEED;
            //}
            //else
            //{
            //    topSpeed = GameConstants.CAT_TOP_SPEED - Vector2.Distance(pos, origin) / GameConstants.CAT_TIME_TO_TARGET;
            //}

            // move the object
            Move(direction, gameTime);

            // make the cat face the direction the cat is moving
            rotation = (float)Math.Atan2(velocity.Y, velocity.X);
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
                // set acceleration vector
                linearAccel = direction;

                // check if under max accel
                if (linearAccel.Length() > maxAccel)
                {
                    // clamp to max accel
                    linearAccel.Normalize();
                    linearAccel = linearAccel * maxAccel;
                }

                // update the velocity
                velocity += linearAccel * gameTime.ElapsedGameTime.Milliseconds;

                // check if under max velocity
                if (velocity.Length() > maxVelocity)
                {
                    // clamp to max velocity
                    velocity.Normalize();
                    velocity *= maxVelocity;
                }

                // update the position
                position += velocity * gameTime.ElapsedGameTime.Milliseconds + .5f * linearAccel * gameTime.ElapsedGameTime.Milliseconds * gameTime.ElapsedGameTime.Milliseconds;

                // update the position of the drawrectangle based on the calculated physics position
                drawRect.X = (int)(position.X) + drawRect.Width / 2;
                drawRect.Y = (int)(position.Y) + drawRect.Height / 2;

                // keep the sprite in the bounds of the screen
                Clamp();
            }
        }

        #endregion
    }
}
