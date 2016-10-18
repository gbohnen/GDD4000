using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Cat_and_Mouse___XNA
{
    class MovingSprite : Sprite
    {
        #region Fields

        public static Random rand = new Random();

        #endregion

        #region Constructors

        /// <summary>
        /// empty constructor. maintains inheritence chain
        /// </summary>
        /// <param name="image"></param>
        /// <param name="game"></param>
        public MovingSprite(Texture2D image, Game game) : base(image, game) { }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Updates the position and of the object based on a position
        /// </summary>
        public virtual void Update(User user, GameTime gameTime) { }

        /// <summary>
        /// Updates the position of the object based on keyboard input
        /// </summary>
        /// <param name="state"> the current state of the keyboard </param>
        public virtual void Update(GameTime gameTime) { }

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

        /// <summary>
        /// sets the position of the object to given xy coords
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void SetPosition(float x, float y)
        {
            drawRect.X = (int)(x - (drawRect.Width / 2));
            drawRect.Y = (int)(y - (drawRect.Height / 2));
        }

        /// <summary>
        /// moves the cat to the corner furthest from its current position
        /// </summary>
        public void JumpToFarCorner()
        {
            Vector2 corner = new Vector2(0, 0);
            float distance = 0;

            // check each corner for longest distance
            // top left
            if (Vector2.DistanceSquared(this.Position, new Vector2(0,0)) > distance)
            {
                corner = new Vector2(0, 0);
                distance = Vector2.DistanceSquared(this.Position, new Vector2(0, 0));
            }
            // top right
            if (Vector2.DistanceSquared(this.position, new Vector2(GameConstants.WINDOW_WIDTH, 0)) > distance)
            {
                corner = new Vector2(GameConstants.WINDOW_WIDTH, 0);
                distance = Vector2.DistanceSquared(this.position, new Vector2(GameConstants.WINDOW_WIDTH, 0));
            }
            // bottom left
            if (Vector2.DistanceSquared(this.position, new Vector2(0, GameConstants.WINDOW_HEIGHT)) > distance)
            {
                corner = new Vector2(0, GameConstants.WINDOW_HEIGHT);
                distance = Vector2.DistanceSquared(this.position, new Vector2(0, GameConstants.WINDOW_HEIGHT));
            }
            // bottom right
            if (Vector2.DistanceSquared(this.position, new Vector2(GameConstants.WINDOW_WIDTH, GameConstants.WINDOW_HEIGHT)) > distance)
            {
                corner = new Vector2(GameConstants.WINDOW_WIDTH, GameConstants.WINDOW_HEIGHT);
                distance = Vector2.DistanceSquared(this.position, new Vector2(GameConstants.WINDOW_WIDTH, GameConstants.WINDOW_HEIGHT));
            }

            // move cat and clamp
            SetPosition(corner.X, corner.Y);
            Clamp();
        }

        #endregion
    }
}
