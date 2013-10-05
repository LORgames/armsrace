using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Cache;
using MischiefFramework.Core.Interfaces;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MischiefFramework.Core;
using MischiefFramework.Core.Helpers;
using FarseerPhysics.Factories;

namespace MischiefFramework.WorldX.Assets {
    class Projectile : Asset, IOpaque {
        private Body body;

        private Model model_projectile;

        private Matrix postmultiplied;

        private const float SPEED = 10f;

        private const float LIFESPAN = 10.0f;

        private float timer = 0.0f;

        private float angle;

        public Projectile(World w, float angle) {
            this.angle = angle;

            model_projectile = ResourceManager.LoadAsset<Model>("Meshes/TestObjects/ball");
            MeshHelper.ChangeEffectUsedByModel(model_projectile, Renderer.Effect3D);

            postmultiplied = Matrix.Identity;

            body = BodyFactory.CreateCircle(w, 0.5f, 1.0f, new Vector2(5, 5), this);
            body.BodyType = BodyType.Dynamic;

            Renderer.Add(this);
            AssetManager.AddAsset(this);
        }

        public override void Dispose() {
            body.Dispose();

            Renderer.Remove(this);
        }

        public override void AsyncUpdate(float dt) {
            postmultiplied = Matrix.CreateTranslation(body.Position.X, 1, body.Position.Y);
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            if (timer <= LIFESPAN) {
                timer += dt;
            } else {
                AssetManager.RemoveAsset(this);
            }
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(postmultiplied, model_projectile);
        }
    }
}
