using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.Core;
using MischiefFramework.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace MischiefFramework.WorldX.Stage {
    internal class Level : IOpaque {
        private Model visuals;
        private Effect fx;
        private Texture tex;

        public Level(World world) {
            tex = ResourceManager.LoadAsset<Texture2D>("Meshes/Levels/overlay");

            fx = ResourceManager.LoadAsset<Effect>("Shaders/GroundShader");

            visuals = ResourceManager.LoadAsset<Model>("Meshes/Levels/Level");
            //MeshHelper.ChangeEffectUsedByModel(visuals, fx, false);
            MeshHelper.ChangeEffectUsedByModel(visuals, Renderer.Effect3D);

            Matrix position = Matrix.Identity;

            Camera c = new Camera(35, 35);
            c.LookAt = Vector3.Zero;
            c.Position.X = 6 * 0.612f + c.LookAt.X;
            c.Position.Y = 6 * 0.500f + c.LookAt.Y;
            c.Position.Z = 6 * -0.612f + c.LookAt.Z;
            c.GenerateMatrices();

            fx.Parameters["Texture"].SetValue(tex);
            fx.Parameters["TextureEnabled"].SetValue(true);
            fx.Parameters["CameraViewProjection"].SetValue(c.ViewProjection);

            BodyFactory.CreateRectangle(world, 26, 2, 0, new Vector2(0, 12));
            BodyFactory.CreateRectangle(world, 26, 2, 0, new Vector2(0,-12));
            BodyFactory.CreateRectangle(world, 2, 26, 0, new Vector2(12, 0));
            BodyFactory.CreateRectangle(world, 2, 26, 0, new Vector2(-12, 0));
            BodyFactory.CreateCircle(world, 2, 0);

            Renderer.Add(this);
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(Matrix.Identity, visuals);
        }
    }
}
