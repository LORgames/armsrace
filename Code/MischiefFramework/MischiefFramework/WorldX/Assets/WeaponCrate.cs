using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Cache;
using MischiefFramework.Core.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using MischiefFramework.Core.Helpers;
using MischiefFramework.Core;
using Microsoft.Xna.Framework;

namespace MischiefFramework.WorldX.Assets {
    public class WeaponCrate : Asset, IOpaque {

        private Model model;
        private Body body;

        private Matrix premultiplied = Matrix.Identity;

        public WeaponCrate(World world) {
            body = BodyFactory.CreateRectangle(world, 1, 1, 1.0f);
            body.BodyType = BodyType.Dynamic;

            model = ResourceManager.LoadAsset<Model>("Meshes/TestObjects/Cube");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);

            AssetManager.AddAsset(this);
            Renderer.Add(this);
        }

        public override void AsyncUpdate(float dt) {
            premultiplied = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY(-body.Rotation) * Matrix.CreateTranslation(body.Position.X, 1, body.Position.Y);
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(premultiplied, model);
        }
    }
}
