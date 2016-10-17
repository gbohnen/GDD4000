using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cat_and_Mouse___XNA
{
    class TimerBar : Bar
    {
        #region Fields

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
        public TimerBar(Texture2D background, Texture2D foreground, int minValue, int maxValue, Color back, Color fore, Sprite target, Game game) : base(background, foreground, minValue, maxValue, target, game)
        {
            backColor = back;
            foreColor = fore;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// updates the state of the bar
        /// </summary>
        public override void Update(int i)
        {
            base.Update(i);

            // set the width of the bar to draw
            foreRect.Width = (int)(target.JumpTimerValue / GameConstants.MOUSE_JUMP_START_VALUE * backRect.Width);
        }
        #endregion
    }
}
