using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace Cat_and_Mouse___XNA
{
    [Serializable()]
    class BoostTimerBar : TimerBar
    {
        #region Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="background"></param>
        /// <param name="foreground"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="back"></param>
        /// <param name="fore"></param>
        /// <param name="rate"></param>
        /// <param name="target"></param>
        /// <param name="game"></param>
        public BoostTimerBar(Texture2D background, Texture2D foreground, int minValue, int maxValue, Color back, Color fore, Sprite target, Game game)
            : base(background, foreground, minValue, maxValue, back, fore, target, game)
        {

        }

        #endregion

        /// <summary>
        /// updates the length of the bar
        /// </summary>
        /// <param name="i"></param>
        public override void Update(int i)
        {
            base.Update(i);

            // set the width of the bar to draw
            foreRect.Width = (int)(target.BoostTimerValue / GameConstants.MOUSE_BOOST_TIMER * backRect.Width);
        }
    }
}
