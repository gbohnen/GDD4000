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
        public const float CAT_TOP_SPEED = .4f;
        public const float CAT_MAX_ACCEL = .003f;

        public const float MOUSE_DEFAULT_SPEED = .6f;
        public const float MOUSE_ATTACK_SPEED_MODIFIER = .75f;
        public const float MOUSE_SHIFT_MODIFIER = 1.25f;

        public const float TANK_TOP_SPEED = .05f;
        public const float TANK_MAX_ACCEL = .01f;
        public const float TANK_LEAD_FRAMES = 15;

        public const float CYCLE_MAX_ACCEL = .001f;
        public const float CYCLE_TOP_SPEED = .7f;

        // unit data
        public const int TOTAL_ENEMIES = 12;
        public const float CAT_DETECTION_RANGE = 250;
        public const float CAT_NEIGHBOR_THRESHOLD = 50;
        public const float CAT_PLAYER_THRESHOLD = 100; 

        // sprite dimensions
        public const int MOUSE_WIDTH = 13;
        public const int MOUSE_HEIGHT = 13;
        public const int MOUSE_ATTACK_WIDTH = 50;
        public const int MOUSE_ATTACK_HEIGHT = 50;

        public const int CAT_WIDTH = 25;
        public const int CAT_HEIGHT = 25;

        public const int TANK_WIDTH = 64;
        public const int TANK_HEIGHT = 64;

        public const int CYCLE_WIDTH = 48;
        public const int CYCLE_HEIGHT = 24;

        // sprite colors
        public static Color SPRITE_DEFAULT_COLOR = Color.White;
        public static Color SPRITE_BLINK_COLOR = Color.Yellow;

        // window dimensions
        public const int WINDOW_WIDTH = 1024;
        public const int WINDOW_HEIGHT = 768;

        // time logic
        public const int GAME_TIMER_START_VALUE = 30000;
        public const int MOUSE_JUMP_START_VALUE = 5000;
        public const int MOUSE_ATTACK_TIMER = 10000;
        public const float MOUSE_ATTACK_DEPLETION_RATE = 2;
        public const int MOUSE_BOOST_TIMER = 2000;
        public const float MOUSE_BOOST_DEPLETION_RATE = 2;
        public const int CAT_TIME_TO_TARGET = 600;
        public const int KEY_COMBO_PRESS_THRESHOLD = 3000;
        public const int KEY_COMBO_TOTAL_THRESHOLD = 20000;

        // health / timer bar values
        public const int MAX_BAR_WIDTH = 64;
        public const int MAX_BAR_HEIGHT = 16;
        public const int BAR_VERTICAL_OFFSET = 10;
        public static Color JUMP_TIMER_BACKGROUND_COLOR = Color.DarkBlue;
        public static Color JUMP_TIMER_FOREGROUND_COLOR = Color.LightBlue;
        public static Color ATTACK_TIMER_BACKGROUND_COLOR = Color.DarkBlue;
        public static Color ATTACK_TIMER_FOREGROUND_COLOR = Color.Yellow;
        public static Color BOOST_TIMER_BACKGROUND_COLOR = Color.Red;
        public static Color BOOST_TIMER_FOREGROUND_COLOR = Color.Green;

        // input data
        public const string ATTACK_MODE_ACTIVATE_LEFT = "nmnmjkjkjjlln";
        public const string ATTACK_MODE_ACTIVATE_RIGHT = "vcvcfdfdffssv";
    }
}
