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
    class MGBullet : Projectile, IOpaque {
        private float radius = 0.5f;

        private float damage = 1.0f;

        public MGBullet(World w, float angle, Vector2 position, int teamID)
            : base(angle, teamID) {
            speed = 20.0f;
            lifespan = 2.0f;
            heightOffGround = 1.7f;

            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gattling_Bullet");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);

            postmultiplied = Matrix.Identity;

            body = BodyFactory.CreateCircle(w, radius / 2, 1.0f, position, this);
            body.FixtureList[0].CollisionGroup = -1;
            body.BodyType = BodyType.Dynamic;
            body.IsBullet = true;
            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

            postmultiplied = Matrix.CreateScale(radius) * Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);

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
            postmultiplied = Matrix.CreateScale(radius) * Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            if (timer <= lifespan) {
                timer += dt;
            } else {
                AssetManager.RemoveAsset(this);
            }
        }
    }
}
