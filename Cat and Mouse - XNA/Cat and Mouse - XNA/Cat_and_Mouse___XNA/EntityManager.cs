using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cat_and_Mouse___XNA
{
    class EntityManager
    {
        #region Fields

        private static EntityManager instance;          // singleton instance of the manager
        List<MovingSprite> enemies;                     // list of enemies in the game
        List<MovingSprite> players;                     // list of players in the game
        List<Bar> bars;                                 // list of all bars used in the game

        User mouse;

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
            mouse = new User(UIManager.Instance.GetSprite(SpriteType.MOUSE), game);
            game.Components.Add(mouse);
            players.Add(mouse);


            // add the mouse jump timer
            TimerBar jumpTimer = new TimerBar(UIManager.Instance.GetSprite(SpriteType.BLANK), UIManager.Instance.GetSprite(SpriteType.BLANK), 0, GameConstants.MOUSE_JUMP_START_VALUE, mouse, game);
            game.Components.Add(jumpTimer);
            bars.Add(jumpTimer);
            
            // add some cats
            for (int i = 0; i < GameConstants.TOTAL_ENEMIES; i++)
            {
                Agent cat = new Agent(UIManager.Instance.GetSprite(SpriteType.CAT), game);
                game.Components.Add(cat);
                enemies.Add(cat);
            }

            // place each object in scene
            PlaceObjects();
        }

        /// <summary>
        /// resets the game for subsequent plays
        /// </summary>
        public void ResetGame()
        {
            mouse.JumpTimerValue = 0;
            PlaceObjects();
        }

        public void Update(KeyboardState state, GameTime gameTime)
        {
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
                    UIManager.Instance.EndGame(Winner.Cats, Game1.timer);
                }
            }

            // update all health/timerbars
            foreach (Bar bar in bars)
            {
                bar.Update();
            }
        }

        #endregion

        #region Private/Protected Members

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

        #endregion
    }
}
