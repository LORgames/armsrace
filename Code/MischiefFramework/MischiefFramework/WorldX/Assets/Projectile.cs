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
        protected Body body;

        protected Model model;

        protected Matrix postmultiplied;

        protected float speed;

        protected float lifespan;

        protected float timer = 0.0f;

        protected float angle = 0.0f;

        protected float heightOffGround;

        public int teamID = -1;

        public Projectile(float angle, int teamID) {
            this.angle = angle;
            this.teamID = teamID;
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
