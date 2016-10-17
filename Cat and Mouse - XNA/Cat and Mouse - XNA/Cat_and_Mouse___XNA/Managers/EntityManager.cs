using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cat_and_Mouse___XNA
{
    public class TimerArgs : EventArgs
    {
        public int Timer { get; set; }
        public TimerArgs(int timer)
        {
            Timer = timer;
        }
    }

    class EntityManager
    {
        #region Fields

        private static EntityManager instance;          // singleton instance of the manager
        List<MovingSprite> enemies;                     // list of enemies in the game
        List<MovingSprite> players;                     // list of players in the game
        List<Bar> bars;                                 // list of all bars used in the game

        User mouse;

        float elapsedMilliseconds;                      // time since last frame. derived from gameTime and used for event calculations

        public static EventHandler MouseWin;
        public static EventHandler CatsWin;

        #endregion

        #region Constructors

        /// <summary>
        /// private constructor for the manager. Initializes most fields to empty values
        /// </summary>
        private EntityManager()
        {
            // blank lists
            enemies = new List<MovingSprite>();
            players = new List<MovingSprite>();
            bars = new List<Bar>();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// accesses the singleton instance of the manager
        /// </summary>
        public static EntityManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new EntityManager();

                return instance;
            }
        }

        /// <summary>
        /// fills all data with startup values. brings the game entities to a starting state
        /// </summary>
        public void Initialize(Game game)
        {
            // add the mouse
            mouse = new User(GraphicsManager.Instance.GetSprite(SpriteType.MOUSE), game);
            game.Components.Add(mouse);
            players.Add(mouse);

            // add the mouse jump timer
            TimerBar jumpTimer = new TimerBar(GraphicsManager.Instance.GetSprite(SpriteType.BLANK), GraphicsManager.Instance.GetSprite(SpriteType.BLANK), 0, GameConstants.MOUSE_JUMP_START_VALUE,
                GameConstants.JUMP_TIMER_BACKGROUND_COLOR, GameConstants.JUMP_TIMER_FOREGROUND_COLOR, mouse, game);
            game.Components.Add(jumpTimer);
            bars.Add(jumpTimer);

            // add the mouse boost timer
            TimerBar boostTimer = new BoostTimerBar(GraphicsManager.Instance.GetSprite(SpriteType.BLANK), GraphicsManager.Instance.GetSprite(SpriteType.BLANK), 0, GameConstants.MOUSE_BOOST_TIMER,
                GameConstants.BOOST_TIMER_BACKGROUND_COLOR, GameConstants.BOOST_TIMER_FOREGROUND_COLOR, mouse, game);
            game.Components.Add(boostTimer);
            bars.Add(boostTimer);

            // add the mouse attack timer
            TimerBar attackTimer = new AttackTimerBar(GraphicsManager.Instance.GetSprite(SpriteType.BLANK), GraphicsManager.Instance.GetSprite(SpriteType.BLANK), 0, GameConstants.MOUSE_ATTACK_TIMER,
                GameConstants.ATTACK_TIMER_BACKGROUND_COLOR, GameConstants.ATTACK_TIMER_FOREGROUND_COLOR, mouse, game);
            game.Components.Add(attackTimer);
            bars.Add(attackTimer);         
             
            // add some cats
            for (int i = 0; i < GameConstants.TOTAL_ENEMIES; i++)
            {
                Agent cat = new Agent(GraphicsManager.Instance.GetSprite(SpriteType.CAT), game);
                game.Components.Add(cat);
                enemies.Add(cat);
            }

            // register movementkeys event
            InputManager.Instance.MoveKeysPressed += new EventHandler<MovementArgs>(MoveMouse);

            // 
            elapsedMilliseconds = 0;

            // place each object in scene
            PlaceObjects();
        }

        /// <summary>
        /// resets the game for subsequent plays
        /// </summary>
        public void ResetGame()
        {
            mouse.JumpTimerValue = 0;
            mouse.BoostTimerValue = 0;
            mouse.AttackTimerValue = 0;
            PlaceObjects();
        }

        /// <summary>
        /// updates the state of the entity manager
        /// </summary>
        /// <param name="state"> the keyboard state from the current frame </param>
        /// <param name="gameTime"> GameTime object from Game1 </param>
        public void Update(KeyboardState state, GameTime gameTime)
        {
            // update time
            elapsedMilliseconds = gameTime.ElapsedGameTime.Milliseconds;

            // update the players
            foreach (MovingSprite player in players)
            {
                player.Update(state, gameTime);
            }

            // update all enemy agents
            foreach (MovingSprite cat in enemies)
            {
                cat.Update(mouse, gameTime);

                // check collisions
                if (mouse.DrawRectangle.Intersects(cat.DrawRectangle))
                {
                    if (!mouse.Attacking)
                        GraphicsManager.Instance.EndGame(Winner.Cats, Game1.timer);

                    else
                    {
                        cat.JumpToFarCorner();
                    }
                }
            }

            // update all health/timerbars
            for (int i = 0; i < bars.Count; i++)
            {
                bars[i].Update(i);
            }
        }

        #endregion

        #region Private/Protected Members

        private void OnCatsWin()
        {
            // fire event
            if (CatsWin != null)
            {
                CatsWin(this, EventArgs.Empty);
            }
        }

        protected void OnMouseWin()
        {
            // fire event
            if (MouseWin != null)
            {
                MouseWin(this, EventArgs.Empty);
            }
        }


        /// <summary>
        /// sets all entities within the scene
        /// </summary>
        protected void PlaceObjects()
        {
            // place the mouse in the center of the screen
            mouse.SetPosition(GameConstants.WINDOW_WIDTH / 2, GameConstants.WINDOW_HEIGHT / 2);

            bool validSpawn;

            // place all enemies within the scene
            foreach (Agent agent in enemies)
            {
                do
                {
                    validSpawn = true;

                    // pick a location for the agent
                    for (int i = 0; i < MovingSprite.rand.Next(0, 3); i++)
                    {
                        agent.Jump();
                    }

                    // check proximity of allies
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (agent != enemies[i])
                        {
                            if (Vector2.Distance(enemies[i].Position, agent.Position) < GameConstants.CAT_NEIGHBOR_THRESHOLD)
                            {
                                validSpawn = false;
                            }
                        }
                    }
                } while (!validSpawn && Vector2.Distance(mouse.Position, agent.Position) < GameConstants.CAT_PLAYER_THRESHOLD);
            }
        }

        /// <summary>
        /// moves the mouse when the movementkeys event is fired
        /// </summary>
        /// <param name="sender"> object that triggered the event </param>
        /// <param name="e"> movement vector received from the event </param>
        public void MoveMouse(object sender, MovementArgs e)
        {
            Vector2 direction = e.Direction;
            
            direction.X *= elapsedMilliseconds;
            direction.Y *= elapsedMilliseconds;

            if (InputManager.Instance.IsShiftPressed && mouse.BoostTimerValue > 0 && !mouse.Attacking)
            {
                direction.X *= GameConstants.MOUSE_SHIFT_MODIFIER;
                direction.Y *= GameConstants.MOUSE_SHIFT_MODIFIER;
            }

            if (mouse.Attacking)
            {
                direction.X *= GameConstants.MOUSE_ATTACK_SPEED_MODIFIER;
                direction.Y *= GameConstants.MOUSE_ATTACK_SPEED_MODIFIER;
            }

            mouse.Move(direction);
        }

        #endregion
    }
}
