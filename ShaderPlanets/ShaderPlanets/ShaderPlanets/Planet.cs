﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderPlanets
{
    class Planet
    {
        #region Fields

        float period;                       // period of revolution. how long does it take to make one solar revolution?
        float rotationalVelocity;           // how quickly does the spheroid turn?
        float diameter;                     // diameter of the planet. uses arbitrary units
        float distanceFromSol;
        Planet parent;                      // parent body to orbit

        Matrix worldMatrix;

        #endregion

        #region Constructors

        public Planet(PlanetData data, Model model)
        {
            period = data.Period;
            rotationalVelocity = data.AngularPeriod;
            diameter = data.Diameter;
            distanceFromSol = data.OrbitalRadius;
        }

        #endregion

        #region Methods

        public void SetParent(Planet parent)
        {
            this.parent = parent;
        }

        #endregion

        #region Properties

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
        }

        public Vector3 Position
        {
            get
            {
                Vector3 pos;
                Vector3 scale;
                Quaternion quat;
                worldMatrix.Decompose(out scale, out quat, out pos);

                return pos;
            }
        }


        public Planet Parent
        {
            get { return parent; }
        }

        public float Scale
        {
            get { return diameter; }
        }
        
        /// <summary>
        /// draws an individual model
        /// </summary>
        /// <param name="m"></param>
        /// <param name="world"> world matrix </param>
        /// <param name="projection"> projection matrix </param>
        /// <param name="view">view matrix </param>
        /// <param name="technique"></param>
        public void DrawModel(Model body, Matrix world, Matrix projection, Matrix view, string technique, float timer)
        {
            Matrix translate;
            Matrix scale;
            Matrix localRotation;
            Matrix globalRotation;

            

            translate = Matrix.CreateTranslation(new Vector3(distanceFromSol, 0, 0));
            scale = Matrix.CreateScale(diameter);
            localRotation = Matrix.CreateRotationY(timer / rotationalVelocity);
            globalRotation = Matrix.CreateRotationY(timer / period);

            world *= scale;
            world *= localRotation;
            world *= translate;
            world *= globalRotation;

            Matrix[] transforms = new Matrix[body.Bones.Count];
            body.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in body.Meshes)
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

            worldMatrix = world;
        }
        
        #endregion
    }
}
