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

        internal float speed;

        internal float lifespan;

        internal float timer = 0.0f;

        internal float angle = 0.0f;

        internal float heightOffGround;

        public Projectile(float angle) {
            this.angle = angle;
        }

        public override void Dispose() {
            body.Dispose();
            Renderer.Remove(this);
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(postmultiplied, model);
        }
    }
}
