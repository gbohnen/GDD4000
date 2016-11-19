using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ShaderPlanets
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model sphere;

        Vector3 cameraPosition;
        Matrix projection, view;

        Effect perlinNoiseEffect;

        Texture2D permTexture2d;
        Texture2D permGradTexture;

        PerlinNoise noiseEngine = new PerlinNoise();

        float timer = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1500;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                aspectRatio, 1.0f, 10000.0f);

            noiseEngine.InitNoiseFunctions(3435, graphics.GraphicsDevice);

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

            sphere = Content.Load<Model>(@"sphere");

            perlinNoiseEffect = Content.Load<Effect>(@"PerlinNoiseEffect");

            permTexture2d = noiseEngine.GeneratePermTexture2d();
            permGradTexture = noiseEngine.GeneratePermGradTexture();

            foreach (ModelMesh mesh in sphere.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = perlinNoiseEffect;
                    part.Effect.Parameters["permTexture2d"].SetValue(permTexture2d);
                    part.Effect.Parameters["permGradTexture"].SetValue(permGradTexture);
                }

                Console.WriteLine(mesh.BoundingSphere.Radius);
            }



            ModelManager.Instance.Initialize(sphere);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            // TODO: Add your update logic here

            // Rotating camera
            float distance = PlanetaryConstants.CAMERA_DISTANCE;
            cameraPosition = new Vector3(0f, distance, distance);
            //view = Matrix.CreateLookAt(new Vector3(distance * (float)Math.Sin(timer), distance, distance * (float)Math.Cos(timer)),
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0f, 5f, 0f), Vector3.Up);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            timer += gameTime.ElapsedGameTime.Milliseconds / 5000.0f;

            Matrix translate;
            Matrix rotate = Matrix.CreateRotationY((float)timer);

            // TODO: Add your drawing code here

            translate = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            ModelManager.Instance.DrawPlanets(rotate * translate, projection, view, timer);

            base.Draw(gameTime);
        }
    }
}
