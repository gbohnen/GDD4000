using System;
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
        public static string keyCombo;                              // placeholder for the target cheat string   
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
                keyCombo = GameConstants.ATTACK_MODE_ACTIVATE_LEFT;
                comboReference = keyCombo;
                ResetCombo();
            }
            else if (currState.IsKeyDown(Keys.D2))
            {
                handedness = true;
                keyCombo = GameConstants.ATTACK_MODE_ACTIVATE_RIGHT;
                comboReference = keyCombo;
                ResetCombo();
            }

            // check for space-bar. this will fire various events based on the gameplay state
            if (currState.IsKeyDown(Keys.Space))
                OnSpace();

            // check for gamestate
            if (Game1.gameState == GameState.Play)
            {
                // check for combo
                // if any keys were pressed
                if (currState.GetPressedKeys().Length > 0)
                {
                    // add current keys to buffer
                    // this doesn't account for the ordering of keys pressed, or multiple correct keys,
                    // but the user shouldn't be allowed or able to input a combo that quickly
                    foreach(Keys key in currState.GetPressedKeys())
                    { 
                        // only add if it was not pressed last frame
                        if (currState.IsKeyDown(key) && !lastState.IsKeyDown(key))
                            keyCombo += GetChar(key);
                    }

                    // check if buffer is equal to combo
                    if (keyCombo != comboReference.Substring(0, keyCombo.Length))
                    {
                        // no match
                        ResetCombo();
                    }
                    else
                    {
                        comboOn = true;

                        // check for full combo
                        if (keyCombo == comboReference)
                        {
                            OnAttackCombo();
                        }
                    }
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

                // check for boosting
                if (currState.IsKeyDown(Keys.LeftShift) || currState.IsKeyDown(Keys.RightShift))
                    shift = true;
                else
                    shift = false;
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
            keyCombo = "";
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

        /// <summary>
        /// gets the char for the current keypress. only returns alpha chars. non-alpha returns space, which will break a combo
        /// </summary>
        /// <param name="key"> the key pressed </param>
        /// <returns></returns>
        private string GetChar(Keys key)
        {
            switch (key)
            {
                case Keys.A: { return "a"; }
                case Keys.B: { return "b"; }
                case Keys.C: { return "c"; }
                case Keys.D: { return "d"; }
                case Keys.E: { return "e"; }
                case Keys.F: { return "f"; }
                case Keys.G: { return "g"; }
                case Keys.H: { return "h"; }
                case Keys.I: { return "i"; }
                case Keys.J: { return "j"; }
                case Keys.K: { return "k"; }
                case Keys.L: { return "l"; }
                case Keys.M: { return "m"; }
                case Keys.N: { return "n"; }
                case Keys.O: { return "o"; }
                case Keys.P: { return "p"; }
                case Keys.Q: { return "q"; }
                case Keys.R: { return "r"; }
                case Keys.S: { return "s"; }
                case Keys.T: { return "t"; }
                case Keys.U: { return "u"; }
                case Keys.V: { return "v"; }
                case Keys.W: { return "w"; }
                case Keys.X: { return "x"; }
                case Keys.Y: { return "y"; }
                case Keys.Z: { return "z"; }

                default:
                    return " ";
            }
        }

        #endregion
    }
}
