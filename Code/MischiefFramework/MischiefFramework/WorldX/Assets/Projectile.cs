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
    class Projectile : Asset {
        internal Body body;

        internal Model model;

        internal Matrix postmultiplied;

        internal const float SPEED = 20.0f;

        internal const float LIFESPAN = 2.0f;

        internal float timer = 0.0f;

        internal float angle = 0.0f;

        internal float radius = 0.5f;

        internal float heightOffGround = 1.7f;

        public Projectile(float angle) {
            this.angle = angle;
        }

        public override void Dispose() {
            body.Dispose();
            Renderer.Remove(this);
        }

        public override void AsyncUpdate(float dt) {
            postmultiplied = Matrix.CreateScale(radius) * Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * SPEED;

            if (timer <= LIFESPAN) {
                timer += dt;
            } else {
                AssetManager.RemoveAsset(this);
            }
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(postmultiplied, model);
        }
    }
}
