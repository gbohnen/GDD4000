using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_and_Mouse___XNA
{
    abstract class Sprite : DrawableGameComponent
    {
        #region Fields

        // fields
        protected Rectangle drawRect;               // draw rectangle
        protected Rectangle sourceRect;             // source rectangle
        protected float rotation;                   // current angle of rotation
        protected Vector2 origin;                   // origin of rotation (vertex)
        protected Texture2D sprite;                 // Mouse sprite
        protected Vector2 direction;                // direction of movement

        protected Vector2 linearAccel;              // direction of primary force
        protected Vector2 velocity;                 // object's current velocity
        protected Vector2 position;                 // object's position
        protected float maxVelocity;                // object's maximum speed
        protected float maxAccel;                   // object's maximum acceleration

        protected float jumpTimer = 0;              // timer that limits the frequency of the mouse jumping
        protected float boostTimer;                 // timer that limits the frequency of mouse boosting
        protected float attackTimer;                // timer that limits the frequency of the mouse attacking
        protected Color color;                      // the color of the sprite

        #endregion

        #region Constructors

        /// <summary>
        /// builds the object with a given sprite
        /// </summary>
        /// <param name="image"> the sprite that the object will draw as </param>
        protected Sprite(Texture2D image, Game game) : base(game)
        {
            // save the sprite 
            sprite = image;

            // create the draw rectangle
            drawRect = new Rectangle(GameConstants.WINDOW_WIDTH / 2 - sprite.Width / 2, GameConstants.WINDOW_HEIGHT / 2 - sprite.Height / 2, sprite.Width / 2, sprite.Height / 2);
            sourceRect = new Rectangle(0, 0, sprite.Width, sprite.Height);

            // set the rotation
            rotation = 0.0f;
            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);

            // zero the direction vector
            direction = new Vector2(0, 0);

            // set physics properties
            linearAccel = new Vector2(0, 0);
            velocity = new Vector2(0, 0);

            // set color
            color = GameConstants.SPRITE_DEFAULT_COLOR;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// draws the entity
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(sprite, drawRect, sourceRect, color, rotation, origin, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        #endregion

        #region Private / Protected Methods

        /// <summary>
        /// move the object based on a direction
        /// </summary>
        /// <param name="direction"> the direction in which we want to move </param>
        protected virtual void Move(Vector2 direction, GameTime gameTime) { }

        /// <summary>
        /// restrains the object within the window bounds
        /// </summary>
        protected virtual void Clamp()
        {
            // check left
            if (drawRect.X <= drawRect.Width / 2)
                drawRect.X = drawRect.Width / 2;

            // check right
            if (drawRect.X >= GameConstants.WINDOW_WIDTH - drawRect.Width / 2)
                drawRect.X = GameConstants.WINDOW_WIDTH - drawRect.Width / 2;

            // check top
            if (drawRect.Y <= drawRect.Width / 2)
                drawRect.Y = drawRect.Width / 2;

            // check bottom
            if (drawRect.Y >= GameConstants.WINDOW_HEIGHT - drawRect.Height / 2)
                drawRect.Y = GameConstants.WINDOW_HEIGHT - drawRect.Height / 2;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// returns the position as a vector2
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(drawRect.Center.X, drawRect.Center.Y); }
        }

        /// <summary>
        /// returns the draw rectangle
        /// </summary>
        public Rectangle DrawRectangle
        {
            get { return drawRect; }
        }

        /// <summary>
        /// exposes the value of the jump timer
        /// </summary>
        public float JumpTimerValue
        {
            get { return jumpTimer; }
            set { jumpTimer = value; }
        }

        /// <summary>
        /// exposes the value of the boost timer
        /// </summary>
        public float BoostTimerValue
        {
            get { return boostTimer; }
            set { boostTimer = value; }
        }

        /// <summary>
        /// exposes the value of the attack timer
        /// </summary>
        public float AttackTimerValue
        {
            get { return attackTimer; }
            set { attackTimer = value; }
        }

        #endregion
    }
}