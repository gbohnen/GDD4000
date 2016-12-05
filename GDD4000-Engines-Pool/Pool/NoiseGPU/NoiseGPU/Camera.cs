using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NoiseGPU
{
    class Camera : GameComponent
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 LookAt { get; set; }

        public Vector3 Up { get; set; }

        public Camera(Game game, Vector3 position, Vector3 lookat, Vector3 up,
            float angularWidth, float aspectRatio, float minDist, float maxDist) : base(game)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(angularWidth),
                        aspectRatio, minDist, maxDist);
            Position = position;
            LookAt = lookat;
            Up = up;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.A))
            {
                Position += new Vector3(-1.0f, 0.0f, 0.0f);
            }
            else if (state.IsKeyDown(Keys.D))
            {
                Position += new Vector3(1.0f, 0.0f, 0.0f);
            }
            if (state.IsKeyDown(Keys.W))
            {
                Position += new Vector3(0.0f, 1.0f, 0.0f);
            }
            else if (state.IsKeyDown(Keys.S))
            {
                Position += new Vector3(0.0f, -1.0f, 0.0f);
            }
            if (state.IsKeyDown(Keys.OemPlus))
            {
                Position += new Vector3(0.0f, 0.0f, -1.0f);
            }
            else if (state.IsKeyDown(Keys.OemMinus))
            {
                Position += new Vector3(0.0f, 0.0f, 1.0f);
            }
            View = Matrix.CreateLookAt(Position, LookAt, Up);
        }
    }
}
