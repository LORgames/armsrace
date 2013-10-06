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
using MischiefFramework.WorldX.Assets;

namespace MischiefFramework.WorldX.Projectiles {
    class LaserBullet : Projectile, ITransparent {
        private float width = 3.0f;
        private float height = 0.5f;

        private float damage = 10.0f;

        public LaserBullet(World w, float angle, Vector2 position, int teamID)
            : base(angle, teamID) {
            speed = 20.0f;
            lifespan = 5.0f;
            heightOffGround = 0.5f;
            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Laser Burst");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.EffectTransparent);

            postmultiplied = Matrix.Identity;

            body = BodyFactory.CreateRectangle(w, width, height, 1.0f, position, this);
            body.FixtureList[0].CollisionGroup = -1;
            body.BodyType = BodyType.Dynamic;
            body.IsBullet = true;
            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
            body.Rotation = angle;

            postmultiplied = Matrix.CreateScale(width, 1.0f, height) * Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);

            Renderer.Add(this);
            AssetManager.AddAsset(this);
        }

        private bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
            Body other;

            if (fixtureA.Body == body) {
                other = fixtureB.Body;
            } else {
                other = fixtureA.Body;
            }

            if (other.UserData is TankCharacter) {
                if ((other.UserData as TankCharacter).player.teamID == teamID) {
                    return false;
                } else {
                    (other.UserData as TankCharacter).TakeDamage(damage);
                }
            }

            AssetManager.RemoveAsset(this);

            return true;
        }

        public override void AsyncUpdate(float dt) {
            postmultiplied = Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            if (timer <= lifespan) {
                timer += dt;
            } else {
                AssetManager.RemoveAsset(this);
            }
        }

        public void RenderTransparent() {
            MeshHelper.DrawModel(postmultiplied, model);
        }
    }
}
