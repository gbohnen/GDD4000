using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cat_and_Mouse
{
    public enum ActiveBar
    {
        Enabled,
        Disabled
    }
    class Bar : Sprite
    {
        #region Fields
        protected int minValue;                     // the minimum value of the bar
        protected int maxValue;                     // the maximum value of the bar
        protected Texture2D background;             // the background of the bar
        protected Texture2D foreground;             // the sprite filling the bar
        protected Rectangle backRect;               // the draw rectangle for the background
        protected Rectangle foreRect;               // the draw rectangle for the foreground
        protected Color backColor;                  // color of the background
        protected Color foreColor;                  // color of the foreground
        protected Sprite target;                    // the object to which the bar is attached
        protected ActiveBar active;                 // sets the activity state of the bar
        #endregion

        #region Constructors
        /// <summary>
        /// base constructor for a timer bar
        /// </summary>
        /// <param name="background"> background sprite </param>
        /// <param name="foreground"> foreground sprite </param>
        /// <param name="minValue"> minimum value of the bar </param>
        /// <param name="maxValue"> maximum value of the bar </param>
        /// <param name="target"> the object to which the bar is attached </param>
        /// <param name="game"> the game in which the bar is active </param>
        public Bar(Texture2D background, Texture2D foreground, int minValue, int maxValue, Sprite target, Game game) : base (background, game)
        {
            // set values
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.background = background;
            this.foreground = foreground;
            this.target = target;

            // zero the origin
            origin = new Vector2(0, 0);

            // set up both draw rectangles
            backRect = new Rectangle(0, 0, GameConstants.MAX_BAR_WIDTH, GameConstants.MAX_BAR_HEIGHT);
            foreRect = new Rectangle(0, 0, 0, GameConstants.MAX_BAR_HEIGHT);

            drawRect = backRect;

            // set the bar to active
            active = ActiveBar.Enabled;
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// updates the bar based on the attached objects position
        /// </summary>
        public virtual void Update()
        {
            // set position of bar relative to the target
            backRect.Y = target.DrawRectangle.Y - target.DrawRectangle.Height / 2 - GameConstants.BAR_VERTICAL_OFFSET - GameConstants.MAX_BAR_HEIGHT;
            backRect.X = target.DrawRectangle.X - backRect.Width / 2;

            foreRect = backRect;            
        }

        /// <summary>
        /// draws the background and foreground of the bar
        /// </summary>
        /// <param name="spriteBatch"> spritebatch </param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (active == ActiveBar.Enabled)
            {
                spriteBatch.Draw(background, backRect, sourceRect, backColor, rotation, origin, SpriteEffects.None, 0);
                spriteBatch.Draw(foreground, foreRect, sourceRect, foreColor, rotation, origin, SpriteEffects.None, 1);
            }
        }


        #endregion

        #region Private/Protected Methods

        #endregion

        #region Properties
        /// <summary>
        /// exposes the object to which the bar is attached
        /// </summary>
        public Sprite Target
        {
            get { return target; }
            set { target = value; }
        }
        #endregion
    }
}
