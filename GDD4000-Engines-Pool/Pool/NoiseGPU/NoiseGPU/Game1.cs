using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace NoiseGPU
{
    public enum GameState { Placement, Shooting, Simulating, GameOver }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        Ball[] balls;
        Box floor;
        Box cue;
        Box[] walls;
        Box[] pockets;
        Vector3[] positions;

        // ball positioning
        float spacing = 6.5f;

        // physics constants
        float wallDamp = .8f;
        float feltFriction = .6f;

        // scoring
        int score = 0;
        int highScore = 0;
        bool ballSunk;

        // pool table
        float tableWidth = 100f;
        float tableLength = 200f;
        float tableThickness = 1f;
        float tableHeight = 5f;

        // keyboard states
        KeyboardState prevState;
        KeyboardState currState;

        public static GameState gameState = GameState.Placement;
        SpriteFont myFont;

        // cue settings
        int chargeTimer = 0;
        float cueAngle = 0;
        bool charging = false;
        float power;

        Random rand;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1080;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            rand = new Random();

            //camera = new Camera(this, new Vector3(0f, 100f, 10f), new Vector3(0f, 0f, 0f), Vector3.Up,
            //                    45.0f, graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);

            camera = new Camera(this, new Vector3(0f, 75f, 175f), new Vector3(0f, 0f, 0f), Vector3.Up,
                                45.0f, graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);

            // calculate starting position of balls in triangle formation
            positions = new Vector3[10];
            for (int i = 0; i < positions.Length; i++)
            {
                // first row
                if (i < 4)
                {
                    float pos = i * spacing - spacing * 1.5f;
                    positions[i] = new Vector3(pos, 4.75f, -40);
                }
                // second row
                else if (i < 7)
                {
                    float pos = (i - 4) * spacing - spacing;
                    positions[i] = new Vector3(pos, 4.75f, -35);
                }
                // third row
                else if (i < 9)
                {
                    float pos = (i - 7) * spacing - spacing * .5f;
                    positions[i] = new Vector3(pos, 4.75f, -30);
                }
                // last row
                else
                {
                    positions[i] = new Vector3(0, 4.75f, -25);
                }
            }

            // set up balls in game
            ResetGame();
            
            // Pool Table
            
            floor = new Box(this, camera, new Vector3(tableWidth, tableThickness, tableLength), 
                new Vector3(-tableWidth * 0.5f, tableThickness * 0.5f, -tableLength * 0.5f), Color.Green, new Vector3(0, 1, 0));

            walls = new Box[4];
            walls[0] = new Box(this, camera, new Vector3(tableThickness, tableHeight, tableLength), 
                new Vector3(tableWidth * 0.5f, 0.5f, -tableLength * 0.5f), Color.Blue, new Vector3(-1, 0, 0)); // Right wall
            walls[1] = new Box(this, camera, new Vector3(tableWidth, tableHeight, tableThickness), 
                new Vector3(-50f, 0.5f, 100f), Color.Red, new Vector3(0, 0, 1)); // Bottom wall
            walls[2] = new Box(this, camera, new Vector3(tableThickness, tableHeight, tableLength), 
                new Vector3(-50f, 0.5f, -100f), Color.Blue, new Vector3(1, 0, 0)); // Left wall
            walls[3] = new Box(this, camera, new Vector3(tableWidth, tableHeight, tableThickness), 
                new Vector3(-50f, 0.5f, -100f), Color.Red, new Vector3(0, 0, 1)); // Top wall

            // pocket values
            float pocketSize = 8f;
            float leftPosition = -(tableWidth / 2 + pocketSize) + tableThickness * 2;
            float rightPosition = tableWidth / 2 - tableThickness;
            float bottomPosition = tableLength / 2 - pocketSize;
            float topPosition = -(tableLength / 2);

            pockets = new Box[6];
            pockets[0] = new Box(this, camera, new Vector3(pocketSize, tableHeight + 2, pocketSize), new Vector3(leftPosition, 0, topPosition), Color.Black, new Vector3(0, 0, 0));   // top left 
            pockets[1] = new Box(this, camera, new Vector3(pocketSize, tableHeight + 2, pocketSize), new Vector3(rightPosition, 0, topPosition), Color.Black, new Vector3(0, 0, 0));   // top right 
            pockets[2] = new Box(this, camera, new Vector3(pocketSize, tableHeight + 2, pocketSize), new Vector3(leftPosition, 0, 0), Color.Black, new Vector3(0, 0, 0));   // mid left
            pockets[3] = new Box(this, camera, new Vector3(pocketSize, tableHeight + 2, pocketSize), new Vector3(rightPosition, 0, 0), Color.Black, new Vector3(0, 0, 0));   // mid right 
            pockets[4] = new Box(this, camera, new Vector3(pocketSize, tableHeight + 2, pocketSize), new Vector3(leftPosition, 0, bottomPosition), Color.Black, new Vector3(0, 0, 0));   // bottom left
            pockets[5] = new Box(this, camera, new Vector3(pocketSize, tableHeight + 2, pocketSize), new Vector3(rightPosition, 0, bottomPosition), Color.Black, new Vector3(0, 0, 0));   // bottom right

            Components.Add(camera);

            Components.Add(floor);
            foreach (Box wall in walls)
            {
                Components.Add(wall);
            }

            foreach (Box pocket in pockets)
            {
                Components.Add(pocket);
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            myFont = Content.Load<SpriteFont>("myFont");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            currState = Keyboard.GetState();


            camera.LookAt = balls[10].Position;

            // game state machine
            // placing cue ball
            if (gameState == GameState.Placement)
            {
                // get movement vector
                Vector2 move = new Vector2(0, 0);
                if (currState.IsKeyDown(Keys.Up))
                {
                    move += new Vector2(0, -1);
                }
                if (currState.IsKeyDown(Keys.Down))
                {
                    move += new Vector2(0, 1);
                }
                if (currState.IsKeyDown(Keys.Left))
                {
                    move += new Vector2(-1, 0);
                }
                if (currState.IsKeyDown(Keys.Right))
                {
                    move += new Vector2(1, 0);
                }
                balls[10].Position += new Vector3(move.X, 0, move.Y);

                // clamp cue ball
                if (balls[10].Position.X < -tableWidth / 2 + balls[10].BoundingSphere.Radius + tableThickness)
                    balls[10].Position = new Vector3(-tableWidth / 2 + balls[10].BoundingSphere.Radius + tableThickness, balls[10].Position.Y, balls[10].Position.Z);
                if (balls[10].Position.X > tableWidth / 2 - balls[10].BoundingSphere.Radius)
                    balls[10].Position = new Vector3(tableWidth / 2 - balls[10].BoundingSphere.Radius, balls[10].Position.Y, balls[10].Position.Z);
                if (balls[10].Position.Z > tableLength / 2 - balls[10].BoundingSphere.Radius - tableThickness)
                    balls[10].Position = new Vector3(balls[10].Position.X, balls[10].Position.Y, tableLength / 2 - balls[10].BoundingSphere.Radius - tableThickness);
                if (balls[10].Position.Z < tableLength / 4 + balls[10].BoundingSphere.Radius)
                    balls[10].Position = new Vector3(balls[10].Position.X, balls[10].Position.Y, tableLength / 4 + balls[10].BoundingSphere.Radius);

                camera.Position = new Vector3(balls[10].Position.X, camera.Position.Y, balls[10].Position.Z + 100);

                if (currState.IsKeyDown(Keys.Space))
                {
                    gameState = GameState.Shooting;
                    chargeTimer = 0;
                    cueAngle = 0;
                    charging = false;
                    CreateCue();
                }
            }
            // aiming cue
            else if (gameState == GameState.Shooting)
            {
                // rotate cue
                if (currState.IsKeyDown(Keys.A))
                {
                    cueAngle -= 1f;
                }
                if (currState.IsKeyDown(Keys.D))
                {
                    cueAngle += 1f;
                }

                // clamp if first time shooting
                if (!ballSunk)
                {
                    // clamp cue
                    if (cueAngle > 90)
                        cueAngle = 90;
                    if (cueAngle < -90)
                        cueAngle = -90;
                }

                cue.Translate = Vector3.Transform(cue.Translate, Matrix.CreateRotationY(MathHelper.ToRadians(cueAngle)));

                camera.Position = new Vector3((float)(100 * Math.Sin(MathHelper.ToRadians(cueAngle))) + balls[10].Position.X, camera.Position.Y, (float)(100 * Math.Cos(MathHelper.ToRadians(cueAngle))) + balls[10].Position.Z);

                // begin charging shot
                if (currState.IsKeyDown(Keys.Space) && !prevState.IsKeyDown(Keys.Space))
                {
                    charging = true;

                    Console.WriteLine("Begin Charge");
                }
                // charging shot
                else if (currState.IsKeyDown(Keys.Space) && prevState.IsKeyDown(Keys.Space))
                {
                    if (chargeTimer < 5000)
                    {
                        chargeTimer += gameTime.ElapsedGameTime.Milliseconds;

                        power = 3f * chargeTimer / 1000;
                        if (power > 15)
                            power = 15;
                    }

                    Console.WriteLine("Charging:" + charging);
                }
                // fire shot
                else if (!currState.IsKeyDown(Keys.Space) && prevState.IsKeyDown(Keys.Space))
                {
                    if (charging)
                    {

                        balls[10].Velocity += new Vector3((float)(power * Math.Sin((MathHelper.ToRadians(180 + cueAngle)))), 0, (float)(power * Math.Cos((MathHelper.ToRadians(180 + cueAngle)))));
                        ballSunk = false;
                        gameState = GameState.Simulating;

                        Console.WriteLine("Fire!" + power);
                    }
                }
            }
            // resolving physics and collisions
            else if (gameState == GameState.Simulating)
            {
                foreach (Box wall in walls)
                {
                    foreach (Ball ball in balls)
                    {
                        // TODO: Put your collision handling with the wall code here
                        if (ball.BoundingSphere.Intersects(wall.BoundingBox))
                        {
                            // bounce off walls
                            ball.Velocity = (-2 * Vector3.Dot(ball.Velocity, wall.Normal) * wall.Normal + ball.Velocity);

                            Console.WriteLine("Ball/wall collision");
                        }
                    }
                }

                foreach (Box pocket in pockets)
                {
                    foreach (Ball ball in balls)
                    {
                        if (ball.BoundingSphere.Intersects(pocket.BoundingBox))
                        {
                            // delete ball (well, make it invisible and move it out of sight
                            Console.WriteLine("Ball/pocket collision");

                            ball.Visible = false;
                            ball.Position = new Vector3(0, -20, 0);

                            // increase score
                            score += 1;
                            if (score > highScore)
                                highScore = score;
                            ballSunk = true;
                        }
                    }
                }

                for (int i = 0; i < balls.Length; i++)
                {
                    for (int j = i + 1; j < balls.Length; j++)
                    {
                        if (balls[i] != balls[j])
                        {
                            // TODO: Put your collision handling with another ball code here
                            if (balls[i].BoundingSphere.Intersects(balls[j].BoundingSphere))
                            {
                                Console.WriteLine("Ball/ball collision");

                                // reset balls to no longer be intersecting
                                Vector3 difference = balls[j].Position - balls[i].Position;    // get vector between 2 ball centers

                                // calculate vector components
                                difference.Normalize();
                                float a1 = Vector3.Dot(balls[i].Velocity, difference);
                                float a2 = Vector3.Dot(balls[j].Velocity, difference);

                                // using radius as mass
                                float p = (2 * (a1 - a2)) / (balls[i].BoundingSphere.Radius + balls[j].BoundingSphere.Radius);

                                balls[i].Velocity = balls[i].Velocity - p * balls[j].BoundingSphere.Radius * difference;
                                balls[j].Velocity = balls[j].Velocity + p * balls[i].BoundingSphere.Radius * difference;

                                balls[i].SweetSpot = false;
                                balls[j].SweetSpot = false;
                            }
                        }
                    }
                }

                bool check = false;

                // check to see if balls are unmoving
                foreach (Ball ball in balls)
                {
                    if (ball.IsMoving == true)
                    {
                        check = true;
                    }
                }

                if (check && ballSunk)
                {
                    gameState = GameState.Shooting;
                    chargeTimer = 0;
                    cueAngle = 0;
                    charging = false;
                }
                else if (check && !ballSunk)
                {
                    gameState = GameState.GameOver;
                    chargeTimer = 5000;
                }
            }
            // gameover
            else
            {
                chargeTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (chargeTimer <= 0)
                {
                    ResetGame();
                    gameState = GameState.Placement;
                }
            }
















            

            prevState = currState;

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (gameState == GameState.Shooting)
                spriteBatch.DrawString(myFont, "Power: " + power, new Vector2(10, 10), Color.White);

            if (gameState == GameState.GameOver)
            {
                spriteBatch.DrawString(myFont, "Score: " + score, new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(myFont, "High Score: " + highScore, new Vector2(10, 10), Color.White);
            }

            spriteBatch.End();

            // reset depth for models draw correctly
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

        protected void ResetGame()
        {
            // reset all balls
            // Pool Balls
            balls = new Ball[11];

            // TODO: Change the initial locations for each ball so that they don't start at the
            // same point.  
            balls[0] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Wood",
                            positions[0], 0.1f, new Vector3(0, 0, 0));
            balls[1] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Turbulence",
                            positions[1], 0.1f, new Vector3(0, 0, 0));
            balls[2] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Scribbles",
                            positions[2], 0.1f, new Vector3(0, 0, 0));
            balls[3] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Cloud",
                            positions[3], 0.1f, new Vector3(0, 0, 0));

            balls[4] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Contours",
                            positions[4], 0.1f, new Vector3(0, 0, 0));
            balls[5] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Marble",
                            positions[5], 0.1f, new Vector3(0, 0, 0));
            balls[6] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Discard",
                            positions[6], 0.1f, new Vector3(0, 0, 0));

            balls[7] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Wood",
                            positions[7], 0.1f, new Vector3(0, 0, 0));
            balls[8] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise_Banding",
                            positions[8], 0.1f, new Vector3(0, 0, 0));

            balls[9] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "PerlinNoise",
                            positions[9], 0.1f, new Vector3(0, 0, 0));

            balls[10] = new Ball(this, camera, @"Models/sphere", @"Effects/PerlinNoiseEffect", "CueBall",
                            new Vector3(0, 4.75f, 50), 0.1f, new Vector3(0, 0, 0));
            balls[10].SweetSpot = true;

            foreach (Ball ball in balls)
            {
                Components.Add(ball);
            }
        }

        protected void CreateCue()
        {
            cue = new Box(this, camera, new Vector3(50, 1, 1), new Vector3(0, 0, 0), Color.Beige, new Vector3(0, 0, 0));
            Components.Add(cue);
        }

        protected void RemoveCue()
        {
            Components.Remove(cue);
        }
    }
}
