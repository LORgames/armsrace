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

        public Level(World world) {
            visuals = ResourceManager.LoadAsset<Model>("Meshes/Levels/Level");
            MeshHelper.ChangeEffectUsedByModel(visuals, Renderer.Effect3D);

            const float CAMERA_ZOOM = 50.0f;
            Renderer.CharacterCamera.LookAt = Vector3.Zero;
            Renderer.CharacterCamera.Position.X = CAMERA_ZOOM * 0.612f + Renderer.CharacterCamera.LookAt.X;
            Renderer.CharacterCamera.Position.Y = CAMERA_ZOOM * 0.500f + Renderer.CharacterCamera.LookAt.Y;
            Renderer.CharacterCamera.Position.Z = CAMERA_ZOOM * 0.612f + Renderer.CharacterCamera.LookAt.Z;
            Renderer.CharacterCamera.GenerateMatrices();

            Renderer.Add(this);

            BodyFactory.CreateRectangle(world, 26, 1, 0, new Vector2(0, 12));
            BodyFactory.CreateRectangle(world, 26, 1, 0, new Vector2(0,-12));
            BodyFactory.CreateRectangle(world, 1, 26, 0, new Vector2(12, 0));
            BodyFactory.CreateRectangle(world, 1, 26, 0, new Vector2(-12, 0));
            BodyFactory.CreateCircle(world, 1, 0);
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(Matrix.Identity, visuals);
        }
    }
}
