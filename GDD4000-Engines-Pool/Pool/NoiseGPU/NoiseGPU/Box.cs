using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NoiseGPU
{
    class Box : DrawableGameComponent
    {
        private Game game;
        private Camera camera;
        private VertexPositionColor[] boxVertices;
        private VertexBuffer vertexBuffer;
        private Color color;
        private BasicEffect effect;

        public Vector3 Translate { get; set; }
        public Vector3 Scale { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public Vector3 Normal { get; set; }
        public bool IsPocket { get; set; }
        
        public Box(Game game, Camera camera, Vector3 scale, Vector3 translate, Color color, Vector3 normal) : base(game)
        {
            this.camera = camera;
            this.game = game;
            Scale = scale;
            Translate = translate;
            this.color = color;
            Normal = normal;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Vector3[] vertices = new Vector3[8];
            vertices[0] = new Vector3(0, 0, 0);   // Backface bottom left
            vertices[1] = new Vector3(1, 0, 0);   // Backface bottom right
            vertices[2] = new Vector3(1, 0, 1);   // Frontface bottom right
            vertices[3] = new Vector3(0, 0, 1);   // Frontface bottom left
            vertices[4] = new Vector3(0, 1, 0);   // Backface upper left
            vertices[5] = new Vector3(1, 1, 0);   // Backface upper right
            vertices[6] = new Vector3(1, 1, 1);   // Frontface upper right
            vertices[7] = new Vector3(0, 1, 1);   // Frontface upper left

            boxVertices = new VertexPositionColor[14];
            boxVertices[0] = new VertexPositionColor(vertices[7], color);
            boxVertices[1] = new VertexPositionColor(vertices[6], color);
            boxVertices[2] = new VertexPositionColor(vertices[3], color);
            boxVertices[3] = new VertexPositionColor(vertices[2], color);
            boxVertices[4] = new VertexPositionColor(vertices[1], color);
            boxVertices[5] = new VertexPositionColor(vertices[6], color);
            boxVertices[6] = new VertexPositionColor(vertices[5], color);
            boxVertices[7] = new VertexPositionColor(vertices[7], color);
            boxVertices[8] = new VertexPositionColor(vertices[4], color);
            boxVertices[9] = new VertexPositionColor(vertices[3], color);
            boxVertices[10] = new VertexPositionColor(vertices[0], color);
            boxVertices[11] = new VertexPositionColor(vertices[1], color);
            boxVertices[12] = new VertexPositionColor(vertices[4], color);
            boxVertices[13] = new VertexPositionColor(vertices[5], color);


            //Set vertex data in VertexBuffer
            vertexBuffer = new VertexBuffer(((Game1)game).GraphicsDevice,
                                    typeof(VertexPositionColor),
                                    boxVertices.Length,
                                    BufferUsage.None);
            vertexBuffer.SetData(boxVertices);

            effect = new BasicEffect(((Game1)game).GraphicsDevice);

            // primitive color
            ((BasicEffect)effect).AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            ((BasicEffect)effect).DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            ((BasicEffect)effect).SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            ((BasicEffect)effect).SpecularPower = 5.0f;
            ((BasicEffect)effect).Alpha = 1.0f;

            effect.VertexColorEnabled = true;

            ((Game1)game).GraphicsDevice.SetVertexBuffer(vertexBuffer);

            // TODO: Create a bounding box here, since the walls and floor don't move,
            // it can be precomputed.  HINT: you need to use the min/max vertices from
            // the above cube definition and transform them based on the scale
            // and translation of the box

            Vector3 min = Vector3.Transform(vertices[0], Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Translate));
            Vector3 max = Vector3.Transform(vertices[6], Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Translate));

            BoundingBox = new BoundingBox(min, max);
        }

        public override void Draw(GameTime gameTime)
        {
            //set effect parameters 
            ((BasicEffect)effect).World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Translate);
            ((BasicEffect)effect).View = camera.View;
            ((BasicEffect)effect).Projection = camera.Projection;
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                ((Game1)game).GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                    (PrimitiveType.TriangleStrip, boxVertices, 0, 12);
            }

            base.Draw(gameTime);
        }
    }
}
