using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using MischiefFramework.Cache;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Core.Helpers;
using MischiefFramework.Core;
using FarseerPhysics.Factories;

namespace MischiefFramework.WorldX.Assets {
    class LaserBullet : Projectile, IOpaque {
        private float width = 3.0f;
        private float height = 0.5f;

        public LaserBullet(World w, float angle, Vector2 position)
            : base(angle) {
            speed = 10.0f;
            lifespan = 2.0f;
            heightOffGround = 0.5f;
            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gattling_Bullet");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);

            postmultiplied = Matrix.Identity;

            body = BodyFactory.CreateRectangle(w, width, height, 1.0f, position, this);
            body.BodyType = BodyType.Dynamic;
            body.IsBullet = true;
            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
            body.Rotation = angle;

            postmultiplied = Matrix.CreateScale(width, 1.0f, height) * Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);

            Renderer.Add(this);
            AssetManager.AddAsset(this);
        }

        private bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
            //AssetManager.RemoveAsset(this);
            return false;
        }

        public override void AsyncUpdate(float dt) {
            postmultiplied = Matrix.CreateScale(width, 1.0f, height) * Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            if (timer <= lifespan) {
                timer += dt;
            } else {
                AssetManager.RemoveAsset(this);
            }
        }
    }
}
