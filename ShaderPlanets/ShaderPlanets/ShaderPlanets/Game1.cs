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

        ScrollingBackground background;

        Model sphere;

        Vector3 cameraPosition;
        Matrix projection, view;

        public static Effect perlinNoiseEffect;

        Texture2D permTexture2d;
        Texture2D permGradTexture;

        PerlinNoise noiseEngine = new PerlinNoise();

        Planets target = Planets.Global;

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

            // load contents necessary for shaders
            sphere = Content.Load<Model>(@"sphere");

            perlinNoiseEffect = Content.Load<Effect>(@"PerlinNoiseEffect");

            permTexture2d = noiseEngine.GeneratePermTexture2d();
            permGradTexture = noiseEngine.GeneratePermGradTexture();

            // add textures/effect to model
            foreach (ModelMesh mesh in sphere.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = perlinNoiseEffect;
                    part.Effect.Parameters["permTexture2d"].SetValue(permTexture2d);
                    part.Effect.Parameters["permGradTexture"].SetValue(permGradTexture);
                }
            }

            // background members
            background = new ScrollingBackground();
            Texture2D backgroundTex = Content.Load<Texture2D>("background");
            background.Load(GraphicsDevice, backgroundTex);



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

            // pass values to shader effect
            perlinNoiseEffect.Parameters["Offset"].SetValue(timer);
            perlinNoiseEffect.Parameters["SOL_TURB"].SetValue((float)Math.Sin(timer) / 5 + .5f);

            perlinNoiseEffect.Parameters["TEST"].SetValue(timer / 10);

            KeyboardState state = Keyboard.GetState();

            // camera controlas
            CheckState(state);
            SetCamera();

            // updates background position
            background.Update(timer);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // draw background
            spriteBatch.Begin();
            background.Draw(spriteBatch);
            spriteBatch.End();

            // reset depth for models draw correctly
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // logic for drawing models
            timer += gameTime.ElapsedGameTime.Milliseconds / 5000.0f;

            Matrix translate;
            Matrix rotate = Matrix.CreateRotationY((float)timer);

            // TODO: Add your drawing code here

            // draw models
            translate = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            ModelManager.Instance.DrawPlanets(translate, projection, view, timer);

            base.Draw(gameTime);
        }

        protected void CheckState(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.NumPad1))
                target = Planets.Mercury;
            else if (state.IsKeyDown(Keys.NumPad2))
                target = Planets.Venus;
            else if (state.IsKeyDown(Keys.NumPad3))
                target = Planets.Earth;
            else if (state.IsKeyDown(Keys.NumPad4))
                target = Planets.Mars;
            else if (state.IsKeyDown(Keys.NumPad5))
                target = Planets.Jupiter;
            else if (state.IsKeyDown(Keys.NumPad6))
                target = Planets.Saturn;
            else if (state.IsKeyDown(Keys.NumPad7))
                target = Planets.Uranus;
            else if (state.IsKeyDown(Keys.NumPad8))
                target = Planets.Neptune;
            else if (state.IsKeyDown(Keys.NumPad9))
                target = Planets.Global;
            else if (state.IsKeyDown(Keys.NumPad0))
                target = Planets.Sol;
            else if (state.IsKeyDown(Keys.OemTilde))
                target = Planets.Global;
        }

        protected void SetCamera()
        {
            // Rotating camera
            float distance = ModelManager.Instance.GetCameraDistance(target);
            cameraPosition = new Vector3(0f, distance, distance);

            if (target == Planets.Global)
            {
                view = Matrix.CreateLookAt(cameraPosition, new Vector3(0f, 5f, 0f), Vector3.Up);
            }
            else
            {
                Vector3 targetPos = ModelManager.Instance.GetNewCameraPosition(target);

                cameraPosition.X = targetPos.X;
                cameraPosition.Z += targetPos.Z;
                view = Matrix.CreateLookAt(cameraPosition, ModelManager.Instance.GetNewCameraPosition(target), Vector3.Up);
            }
        }
    }
}
