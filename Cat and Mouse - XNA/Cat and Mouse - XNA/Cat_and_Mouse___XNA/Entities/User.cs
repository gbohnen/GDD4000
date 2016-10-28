using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace Cat_and_Mouse___XNA
{
    [Serializable()]
    class User : MovingSprite
    {
        #region Fields

        bool jumpReady = false;                         // is the hyperjump ready?
        bool attacking;                                 // is the mouse attacking

        #endregion

        #region Constructors

        /// <summary>
        /// builds the mouse object with a given sprite
        /// </summary>
        /// <param name="image"> the sprite that the object will draw as </param>
        public User(Texture2D image, Game game) : base(image, game)
        {
            // create the draw rectangle
            drawRect = new Rectangle(GameConstants.WINDOW_WIDTH / 2 - GameConstants.MOUSE_WIDTH / 2, GameConstants.WINDOW_HEIGHT / 2 - GameConstants.MOUSE_HEIGHT / 2,
                                GameConstants.MOUSE_WIDTH, GameConstants.MOUSE_HEIGHT);

            // register events
            InputManager.Instance.SpacePressed += HyperJump;
            InputManager.Instance.AttackComboExecuted += AttackMode;

            // set timers to 0
            boostTimer = 0;
            attackTimer = 0;
            attacking = false;
        }

        public User(SerializationInfo info, StreamingContext ctxt) :  base(GraphicsManager.Instance.GetSprite(SpriteType.MOUSE), Game1.instance)
        {
            // create the draw rectangle
            drawRect = new Rectangle(GameConstants.WINDOW_WIDTH / 2 - GameConstants.MOUSE_WIDTH / 2, GameConstants.WINDOW_HEIGHT / 2 - GameConstants.MOUSE_HEIGHT / 2,
                                GameConstants.MOUSE_WIDTH, GameConstants.MOUSE_HEIGHT);

            // register events
            InputManager.Instance.SpacePressed += HyperJump;
            InputManager.Instance.AttackComboExecuted += AttackMode;

            position = (Vector2)info.GetValue("Position", typeof(Vector2));
            boostTimer = (float)info.GetValue("BoostTimer", typeof(float));
            attackTimer = (float)info.GetValue("AttackTimer", typeof(float));
            jumpTimer = (float)info.GetValue("JumpTimer", typeof(float));
            attacking = (bool)info.GetValue("Attacking", typeof(bool));

            Console.WriteLine("User Loaded...");
            Console.WriteLine("\t Position: " + position.ToString());
            Console.WriteLine("\t BoostTimer: " + boostTimer);
            Console.WriteLine("\t AttackTimer: " + attackTimer);
            Console.WriteLine("\t JumpTimer: " + jumpTimer);
            Console.WriteLine("\t Attacking: " + attacking);

            if (attacking)
            {
                attacking = false;
                ToggleAttackMode();
            }
        }

        #endregion
        
        #region Public Members

        /// <summary>
        /// Updates timers related to the mouse
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (!attacking)
            {
                // update the jump timer
                if (jumpTimer < GameConstants.MOUSE_JUMP_START_VALUE)
                    jumpTimer += gameTime.ElapsedGameTime.Milliseconds;
                else if (!jumpReady)
                {
                    jumpReady = true;
                    AudioManager.Instance.PlaySound(SoundKeys.SadMouse);
                }

                // update boost timer
                if (!InputManager.Instance.IsShiftPressed && boostTimer < GameConstants.MOUSE_BOOST_TIMER)
                {
                    boostTimer += gameTime.ElapsedGameTime.Milliseconds;
                }
                else if (InputManager.Instance.IsShiftPressed && boostTimer > 0)
                {
                    boostTimer -= gameTime.ElapsedGameTime.Milliseconds * GameConstants.MOUSE_BOOST_DEPLETION_RATE;
                }
            }

            // update the attack timer
            if (attacking && attackTimer > 0)
            {
                attackTimer -= gameTime.ElapsedGameTime.Milliseconds * GameConstants.MOUSE_ATTACK_DEPLETION_RATE;

                if ((int)(attackTimer / 1000) % 2 == 0)
                    color = GameConstants.SPRITE_BLINK_COLOR;
                else
                    color = GameConstants.SPRITE_DEFAULT_COLOR;
            }
            else if (attacking && attackTimer <= 0)
            {
                ToggleAttackMode();
                attackTimer = 0;
            }
            else if (!attacking && attackTimer < GameConstants.MOUSE_ATTACK_TIMER)
            {
                attackTimer += gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        /// <summary>
        /// move the object based on a direction
        /// </summary>
        /// <param name="direction"> the direction in which we want to move </param>
        public void Move(Vector2 direction)
        {
            // move in that direction
            drawRect.X += (int)(direction.X * GameConstants.MOUSE_DEFAULT_SPEED);
            drawRect.Y += (int)(direction.Y * GameConstants.MOUSE_DEFAULT_SPEED);

            // orient the sprite
            //if (direction != new Vector2(0,0))
            //    direction.Normalize();
            rotation = (float)Math.Atan2(direction.Y, direction.X);

            // keep the sprite in the bounds of the screen
            Clamp();
        }

        /// <summary>
        /// exposes the mouse attacking value
        /// </summary>
        public bool Attacking
        {
            get { return attacking; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Position", Position);

            // save others
            info.AddValue("JumpTimer", jumpTimer);
            info.AddValue("BoostTimer", boostTimer);
            info.AddValue("AttackTimer", attackTimer);
            info.AddValue("Attacking", Attacking);

            Console.WriteLine("User Saved...");
            Console.WriteLine("\t Position: " + position.ToString());
            Console.WriteLine("\t BoostTimer: " + boostTimer);
            Console.WriteLine("\t AttackTimer: " + attackTimer);
            Console.WriteLine("\t JumpTimer: " + jumpTimer);
            Console.WriteLine("\t Attacking: " + attacking);
        }

        public override void ReloadObject(SerializationInfo info, StreamingContext ctxt)
        {
            // load position
            base.ReloadObject(info, ctxt);

            // load others
            jumpTimer = (float)info.GetValue("JumpTimer", typeof(float));
            boostTimer = (float)info.GetValue("BoostTimer", typeof(float));
            attackTimer = (float)info.GetValue("AttackTimer", typeof(float));
            attacking = (bool)info.GetValue("Attacking", typeof(bool));

            Console.WriteLine("\t JumpTimer loaded");
            Console.WriteLine("\t BoostTimer loaded");
            Console.WriteLine("\t Attack loaded");
            Console.WriteLine("\t isAttacking loaded");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// uses the "hyperjump" when the space bar is pressed
        /// </summary>
        /// <param name="sender"> inputmanager that fired the event </param>
        /// <param name="e"> argument list, empty </param>
        private void HyperJump(object sender, EventArgs e)
        {
            // if the timer is up, jump to a new locaion and reset
            if (jumpTimer >= GameConstants.MOUSE_JUMP_START_VALUE && !attacking && Game1.gameState == GameState.Play)
            {
                Jump();
                AudioManager.Instance.PlaySound(SoundKeys.HappyMouse);
                jumpReady = false;
                jumpTimer = 0;
            }
        }

        /// <summary>
        /// delegate that tracks when a keyboard combo is activated
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event parameters </param>
        private void AttackMode(object sender, EventArgs e)
        {
            // if the timer is up, activate attack mode
            if (!attacking && attackTimer >= GameConstants.MOUSE_ATTACK_TIMER)
            {
                ToggleAttackMode();
            }
        }

        /// <summary>
        /// switches into or out of attack mode
        /// </summary>
        private void ToggleAttackMode()
        {
            if (!attacking)
            {
                attacking = true;
                drawRect.Width = GameConstants.MOUSE_ATTACK_WIDTH;
                drawRect.Height = GameConstants.MOUSE_ATTACK_HEIGHT;
            }
            else
            {
                attacking = false;
                drawRect.Width = GameConstants.MOUSE_WIDTH;
                drawRect.Height = GameConstants.MOUSE_HEIGHT;
                color = GameConstants.SPRITE_DEFAULT_COLOR;
            }
        }

        #endregion
    }
}
