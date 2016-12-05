using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NoiseGPU
{
    class Ball : DrawableGameComponent
    {
        private string modelFile;
        private string effectFile;
        private string techniqueName;
        private float radius;
        private float rotation;
        private float speed;

        private Game game;
        private Model model;
        private Effect perlinNoiseEffect;
        private Camera camera;
        PerlinNoise noiseEngine;
        Texture2D permTexture2d;
        Texture2D permGradTexture;

        public Vector3 Position { get; set; }
        public float Scale { get; set; }
        public Vector3 Velocity { get; set; }
        public bool IsMoving { get; set; }

        public BoundingSphere BoundingSphere { get; set; }

        public Ball(Game game, Camera camera, string modelFile, string effectFile, string techniqueName, 
                    Vector3 position, float scale, Vector3 velocity) : base(game)
        {
            this.camera = camera;
            this.modelFile = modelFile;
            this.effectFile = effectFile;
            this.techniqueName = techniqueName;
            this.game = game;
            Position = position;
            Scale = scale;
            noiseEngine = new PerlinNoise();
            noiseEngine.InitNoiseFunctions(3435, ((Game1)game).graphics.GraphicsDevice);
            radius = position.Y;
            speed = 5.0f;
            Velocity = velocity;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            model = game.Content.Load<Model>(modelFile);
            perlinNoiseEffect = game.Content.Load<Effect>(effectFile);

            permTexture2d = noiseEngine.GeneratePermTexture2d();
            permGradTexture = noiseEngine.GeneratePermGradTexture();

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = perlinNoiseEffect;
                    part.Effect.Parameters["permTexture2d"].SetValue(permTexture2d);
                    part.Effect.Parameters["permGradTexture"].SetValue(permGradTexture);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            Velocity.Normalize();
            Position += speed * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            rotation += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // add table friction


            // check for moving
            if (Velocity.Length() <= 0)
                IsMoving = false;
            else
                IsMoving = true;

            // TODO: Create the bounding sphere here as it must be recalculated after movement
            BoundingSphere = new BoundingSphere(Position, radius * .6f);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix rotateX = Matrix.CreateRotationX(rotation);
            Matrix rotateY;
            if ((Velocity.Z > 0 && Velocity.X > 0) || (Velocity.Z < 0 && Velocity.X < 0))
            {
                rotateY = Matrix.CreateRotationY((float)Math.Atan2(Velocity.Z, Velocity.X));
            }
            else // if ((Velocity.Z > 0 && Velocity.X < 0) || (Velocity.Z < 0 && Velocity.X > 0))
            {
                rotateY = Matrix.CreateRotationY(MathHelper.Pi + (float)Math.Atan2(Velocity.Z, Velocity.X));
            }

            Matrix translateZ = Matrix.CreateTranslation(Position);
            Matrix scale = Matrix.CreateScale(Scale);

            Matrix world = scale * rotateX * rotateY * translateZ;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = perlinNoiseEffect.Techniques[techniqueName];
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(camera.View);
                    effect.Parameters["Projection"].SetValue(camera.Projection);
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
