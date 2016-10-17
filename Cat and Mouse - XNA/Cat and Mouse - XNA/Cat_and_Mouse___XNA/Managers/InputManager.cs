using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Cat_and_Mouse___XNA
{
    /// <summary>
    /// determines the behaviors of the movement keys event
    /// </summary>
    public class MovementArgs : EventArgs
    {
        public Vector2 Direction { get; set; }
        public MovementArgs(Vector2 dir)
        {
            Direction = dir;
        }
    }

    class InputManager
    {
        #region Fields

        static InputManager instance;
        KeyboardState lastState;                        // keyboard state from the previous frame
        KeyboardState currState;                        // keyboard state from the current frame
        string comboReference;                          // holds the cheat string for comparison
        string comboValue;                              // placeholder for the target cheat string   
        float intervalTimer;                            // timer that checks between keystrokes 
        float comboTimer;                               // timer that tracks the total time of combo from start to finish 
        bool comboOn;
        bool handedness;                                // does the user prefer a left- or right-handed configuration: left/false and right/true
        bool shift;                                     // is a shift key pressed or not?

        // events
        public event EventHandler<MovementArgs> MoveKeysPressed;
        public event EventHandler SpacePressed;
        public event EventHandler AttackComboExecuted;

        #endregion

        #region Constructors

        /// <summary>
        /// basic constructor, performs all pre-initialization logic
        /// </summary>
        private InputManager()
        {
            handedness = false;
            comboReference = GameConstants.ATTACK_MODE_ACTIVATE_LEFT;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// accesses the singleton instance of the manager
        /// </summary>
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new InputManager();

                return instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// performs first time run actions
        /// </summary>
        public void Initialize()
        {
            ResetCombo();
        }

        /// <summary>
        /// updates the state of the inputmanager
        /// </summary>
        /// <param name="state"> the keyboard state for the current frame </param>
        /// <param name="gameTime"> GameTime object from game1 </param>
        public void Update(KeyboardState state, GameTime gameTime)
        {
            // set states
            lastState = currState;
            currState = state;

            // check for handed-ness
            if (currState.IsKeyDown(Keys.D1))
            {
                handedness = false;
                comboValue = GameConstants.ATTACK_MODE_ACTIVATE_LEFT;
                comboReference = comboValue;
                ResetCombo();
            }
            else if (currState.IsKeyDown(Keys.D2))
            {
                handedness = true;
                comboValue = GameConstants.ATTACK_MODE_ACTIVATE_RIGHT;
                comboReference = comboValue;
                ResetCombo();
            }

            // check for gamestate
            if (Game1.gameState == GameState.Play)
            {
                // check for combo input
                string c = comboValue[0].ToString();                  // grab first character from reference string
                Keys key;                                               // blank key
                if (Enum.TryParse<Keys>(c, out key))                    // check if first character exists as a key
                {
                    if (currState.IsKeyDown(key))                       // if that key was pressed
                    {
                        comboOn = true;                                 // enable the combo

                        if (comboValue.Length > 0)                      // if there are still characters in the string
                            comboValue.Remove(0, 1);                    // remove the first character from the string
                        else                                            // if the string is empty
                            OnAttackCombo();                            // trigger the combo
                    }
                }

                // check for other key presses
                if (currState.GetPressedKeys().Length > 0 && !currState.IsKeyDown(key)) // if a key was pressed and the current combo check is false
                {
                    ResetCombo();
                }

                // update timers
                if (comboOn)
                {
                    intervalTimer += gameTime.ElapsedGameTime.Milliseconds;
                    comboTimer += gameTime.ElapsedGameTime.Milliseconds;
                }

                // time-out combo
                if (comboTimer > GameConstants.KEY_COMBO_TOTAL_THRESHOLD || intervalTimer > GameConstants.KEY_COMBO_PRESS_THRESHOLD)
                    ResetCombo();

                // left-handed
                if (!handedness)
                {
                    // mouse movement code
                    if (Game1.gameState == GameState.Play)
                    {
                        // zero the direction vector
                        Vector2 direction = new Vector2(0, 0);

                        // change direction based on keypresses
                        if (currState.IsKeyDown(Keys.W))
                        {
                            direction.Y -= 1;
                        }
                        if (currState.IsKeyDown(Keys.D))
                        {
                            direction.X += 1;
                        }
                        if (currState.IsKeyDown(Keys.A))
                        {
                            direction.X -= 1;
                        }
                        if (currState.IsKeyDown(Keys.S))
                        {
                            direction.Y += 1;
                        }

                        // trigger event
                        OnMoveKeys(direction);
                    }
                }
                // right-handed
                else
                {
                    // mouse movement code
                    if (Game1.gameState == GameState.Play)
                    {
                        // zero the direction vector
                        Vector2 direction = new Vector2(0, 0);

                        // change direction based on keypresses
                        if (currState.IsKeyDown(Keys.I))
                        {
                            direction.Y -= 1;
                        }
                        if (currState.IsKeyDown(Keys.L))
                        {
                            direction.X += 1;
                        }
                        if (currState.IsKeyDown(Keys.J))
                        {
                            direction.X -= 1;
                        }
                        if (currState.IsKeyDown(Keys.K))
                        {
                            direction.Y += 1;
                        }

                        // trigger event
                        OnMoveKeys(direction);
                    }
                }

                // hand independent actions

                // check for space-bar
                if (currState.IsKeyDown(Keys.Space))
                    OnSpace();

                // check for boosting
                if (currState.IsKeyDown(Keys.LeftShift) || currState.IsKeyDown(Keys.RightShift))
                    shift = true;
                else
                    shift = false;
            }

            // debug - check for attacking
            if (currState.IsKeyDown(Keys.B))
            {
                OnAttackCombo();
            }

            // debug - arrow key movement
            // mouse movement code
            if (Game1.gameState == GameState.Play)
            {
                // zero the direction vector
                Vector2 direction = new Vector2(0, 0);

                // change direction based on keypresses
                if (currState.IsKeyDown(Keys.Up))
                {
                    direction.Y -= 1;
                }
                if (currState.IsKeyDown(Keys.Right))
                {
                    direction.X += 1;
                }
                if (currState.IsKeyDown(Keys.Left))
                {
                    direction.X -= 1;
                }
                if (currState.IsKeyDown(Keys.Down))
                {
                    direction.Y += 1;
                }

                // trigger event
                OnMoveKeys(direction);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// returns the value of the shift keys
        /// </summary>
        public bool IsShiftPressed
        {
            get { return shift; }
        }

        // returns a string based on the handedness configuration
        public string HandednessConfig
        {
            get
            {
                if (handedness)
                    return "Right";
                else
                    return "Left";                        
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// resets all elements involved in the cheat combo
        /// </summary>
        private void ResetCombo()
        {
            comboValue = comboReference;
            intervalTimer = 0;
            comboTimer = 0;
            comboOn = false;
        }

        /// <summary>
        /// fires the movement keys event 
        /// </summary>
        /// <param name="dir"> the direction vector, extracted from the assigned movement keys </param>
        private void OnMoveKeys(Vector2 dir)
        {
            // normalize direction
            if (dir != new Vector2(0, 0))
            {
                dir.Normalize();
            }

            // fire event
            if (MoveKeysPressed != null)
            {
                MoveKeysPressed(this, new MovementArgs(dir));
            }
        }

        /// <summary>
        /// fires an event when the spacebar is pressed
        /// </summary>
        private void OnSpace()
        {
            // fire event
            if (SpacePressed != null)
            {
                SpacePressed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// fires an event when the attack combo is executed
        /// </summary>
        private void OnAttackCombo()
        {
            // fire event
            if (AttackComboExecuted != null)
            {
                AttackComboExecuted(this, EventArgs.Empty);
            }

            ResetCombo();
        }

        #endregion

    }
}
