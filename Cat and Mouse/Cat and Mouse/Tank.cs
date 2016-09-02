using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Cat_and_Mouse
{
    class Tank : Agent
    {
        #region Constructors

        /// <summary>
        /// builds the tank object with a given sprite
        /// </summary>
        /// <param name="image"> the sprite that the object will draw as </param>
        public Tank(Texture2D image, Game game) : base(image, game)
        {
            // set physics properties
            maxAccel = GameConstants.TANK_MAX_ACCEL;
            maxVelocity = GameConstants.TANK_TOP_SPEED;

            // set draw rectangle
            drawRect = new Rectangle(0, 0, GameConstants.TANK_WIDTH, GameConstants.TANK_HEIGHT);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the position and rotation of the cat based on the mouse's position
        /// </summary>
        /// <param name="pos"> position of the mouse </param>
        public override void Update(Sprite user, GameTime gameTime)
        {
            base.Update(user, gameTime);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
