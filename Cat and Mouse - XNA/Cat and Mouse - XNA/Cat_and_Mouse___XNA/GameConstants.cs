using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cat_and_Mouse___XNA
{
    class GameConstants
    {
        // movement
        public const float CAT_TOP_SPEED = .5f;
        public const float CAT_MAX_ACCEL = .003f;

        public const float MOUSE_TOP_SPEED = .6f;

        public const float TANK_TOP_SPEED = .05f;
        public const float TANK_MAX_ACCEL = .01f;
        public const float TANK_LEAD_FRAMES = 15;

        public const float CYCLE_MAX_ACCEL = .001f;
        public const float CYCLE_TOP_SPEED = .7f;

        // sprite dimensions
        public const int MOUSE_WIDTH = 32;
        public const int MOUSE_HEIGHT = 32;

        public const int CAT_WIDTH = 32;
        public const int CAT_HEIGHT = 32;

        public const int TANK_WIDTH = 64;
        public const int TANK_HEIGHT = 64;

        public const int CYCLE_WIDTH = 48;
        public const int CYCLE_HEIGHT = 24;

        // window dimensions
        public const int WINDOW_WIDTH = 1024;
        public const int WINDOW_HEIGHT = 768;

        // time logic
        public const int GAME_TIMER_START_VALUE = 30000;
        public const int MOUSE_JUMP_START_VALUE = 5000;
        public const int CAT_TIME_TO_TARGET = 600;

        // health / timer bar values
        public const int MAX_BAR_WIDTH = 64;
        public const int MAX_BAR_HEIGHT = 16;
        public const int BAR_VERTICAL_OFFSET = 10;
        public static Color TIMER_BACKGROUND_COLOR = Color.DarkBlue;
        public static Color TIMER_FOREGROUND_COLOR = Color.Green;
    }
}
