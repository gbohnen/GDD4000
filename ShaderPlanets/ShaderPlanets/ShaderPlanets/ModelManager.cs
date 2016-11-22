using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderPlanets
{
    public enum Planets { Sol = 0, Mercury = 1, Venus = 2, Earth = 3, Luna = 9, Mars = 4, Jupiter = 5, Saturn = 6, Uranus = 7, Neptune = 8, Global }

    class ModelManager
    {
        #region Fields

        private static ModelManager instance;                               // singleton instance of the manager
        Dictionary<Planets, Planet> entityCollection;                       // dictionary for all of the entities we need to manage
        Model planetModel;                                                  // base model for each planet

        #endregion

        #region Singleton-related Members

        private ModelManager()
        {
        }

        public static ModelManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ModelManager();

                return instance;
            }
        }

        #endregion

        #region Methods
            
        public void Initialize(Model model)
        {
            entityCollection = new Dictionary<Planets, Planet>();
            planetModel = model;

            // for all entities, be they sol, planet, or otherwisea
            foreach (Planets key in Enum.GetValues(typeof(Planets)))
            {
                if (key != Planets.Global)
                {
                    // add a body for each key in the enumeration
                    entityCollection.Add(key, new Planet(key, model));

                    // set parents, edge cases first
                    // sol orbits nothing
                    if (key == Planets.Sol)
                    { }
                    // luna orbits earth
                    else if (key == Planets.Luna)
                        entityCollection[key].SetParent(entityCollection[Planets.Earth]);
                    // all else orbits sol
                    else
                        entityCollection[key].SetParent(entityCollection[Planets.Sol]);
                }
            }
        }

        /// <summary>
        /// draws all entities into the gameworld
        /// </summary>
        /// <param name="world"> world matrix </param>
        /// <param name="projection"> projection matrix </param>
        /// <param name="view">view matrix </param>
        public void DrawPlanets(Matrix world, Matrix projection, Matrix view, float timer)
        {
            // draw planets
            for (int i = 0; i < entityCollection.Count; i++)
            {
                entityCollection[(Planets)i].DrawModel(planetModel, world, projection, view, ((Planets)i).ToString(), timer);
            }

            // draw saturns rings
            DrawRing(entityCollection[Planets.Saturn].Scale * 2, entityCollection[Planets.Saturn], world, projection, view, "SaturnRings", timer);

            // uranus rings
            DrawRing(entityCollection[Planets.Uranus].Scale * 2, entityCollection[Planets.Uranus], world, projection, view, "UranusRings", timer);

            // draw moons 
            DrawMoon(Planets.Luna, entityCollection[Planets.Earth], world, projection, view, "Mercury", timer);

            // draw asteroids 
            //DrawAsteroidBelt(world, projection, view, timer);
        }

        public Vector3 GetNewCameraPosition(Planets planet)
        {
            Vector3 translation;
            Vector3 scale;
            Quaternion ugh;

            entityCollection[planet].WorldMatrix.Decompose(out scale, out ugh, out translation);

            return translation;
        }

        public float GetCameraDistance(Planets planet)
        {
            if (planet == Planets.Global)
                return PlanetaryConstants.CAMERA_DISTANCE;
            else
                return PlanetaryConstants.CAMERA_DISTANCE / 20 * entityCollection[planet].Scale;
        }

        public void DrawAsteroidBelt(Matrix world, Matrix projection, Matrix view, float timer)
        {
            Matrix translate;
            Matrix scale;
            Matrix localRotation;
            Matrix globalRotation;

            translate = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            scale = Matrix.CreateScale(new Vector3(PlanetaryConstants.ASTEROID_BELT_SCALE, .00001f, PlanetaryConstants.ASTEROID_BELT_SCALE));

            localRotation = Matrix.CreateRotationY(timer / PlanetaryConstants.ASTEROID_ROTATION);

            world *= scale;
            world *= localRotation;
            world *= translate;

            Matrix[] transforms = new Matrix[planetModel.Bones.Count];
            planetModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in planetModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = Game1.perlinNoiseEffect.Techniques["AsteroidBelt"];
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
        }

        public void DrawRing(float radius, Planet parent, Matrix world, Matrix projection, Matrix view, string technique, float timer)
        {
            Matrix translate;
            Matrix scale;
            Matrix localRotation;
            Matrix globalRotation;

            translate = Matrix.CreateTranslation(new Vector3(parent.DistanceFromSol, 0, 0));
            scale = Matrix.CreateScale(new Vector3(radius, .0000000000000001f, radius));

            localRotation = Matrix.CreateRotationY(timer / parent.RotationalVelocity);

            localRotation *= Matrix.CreateRotationZ(MathHelper.ToRadians(parent.AxialTilt));

            globalRotation = Matrix.CreateRotationY(timer / parent.Period);

            world *= scale;
            world *= localRotation;
            world *= translate;
            world *= globalRotation;

            Matrix[] transforms = new Matrix[planetModel.Bones.Count];
            planetModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in planetModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = Game1.perlinNoiseEffect.Techniques[technique];
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
        }

        public void DrawMoon(Planets planet, Planet parent, Matrix world, Matrix projection, Matrix view, string technique, float timer)
        {
            PlanetData data = PlanetaryConstants.GetPlanet(planet);

            Matrix translate;
            Matrix scale;
            Matrix localRotation;
            Matrix globalRotation;

            // create local values
            translate = Matrix.CreateTranslation(new Vector3(parent.Scale * 2, 0, 0));
            scale = Matrix.CreateScale(data.Diameter);
            globalRotation = Matrix.CreateRotationY(timer / data.Period);
            localRotation = Matrix.CreateRotationZ(MathHelper.ToRadians(parent.AxialTilt));
            
            world *= scale;
            world *= localRotation;
            world *= translate;
            world *= globalRotation;

            // match parent values
            translate = Matrix.CreateTranslation(new Vector3(parent.DistanceFromSol, 0, 0));
            globalRotation = Matrix.CreateRotationY(timer / parent.Period);
            
            // apply values
            world *= translate;
            world *= globalRotation;

            Matrix[] transforms = new Matrix[planetModel.Bones.Count];
            planetModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in planetModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = Game1.perlinNoiseEffect.Techniques[technique];
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
        }

        #endregion
    }
}
