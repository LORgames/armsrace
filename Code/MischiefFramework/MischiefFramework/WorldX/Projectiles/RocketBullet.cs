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
using FarseerPhysics.Dynamics.Contacts;

namespace MischiefFramework.WorldX.Projectiles {
    class RocketBullet : Projectile, IOpaque {
        private float width = 1.0f;
        private float height = 0.5f;

        private float damage = 20.0f;

        private float targetRadius = 5.0f;
        private Body targetCircle;
        private Vector2 targetPos = Vector2.Zero;

        private const float TURNSPEED = 1.0f;

        public RocketBullet(World w, float angle, Vector2 position, int teamID)
            : base(angle, teamID) {
                Console.Out.WriteLine("**********************************************************");
            speed = 6.0f;
            lifespan = 6.0f;
            heightOffGround = 0.5f;
            model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Rocket");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);

            postmultiplied = Matrix.Identity;

            body = BodyFactory.CreateRectangle(w, width, height, 1.0f, position, this);
            body.FixtureList[0].CollisionGroup = -1;
            body.BodyType = BodyType.Dynamic;
            body.IsBullet = true;
            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
            body.Rotation = angle;

            targetCircle = BodyFactory.CreateCircle(w, targetRadius, 1.0f, position, this);
            targetCircle.FixtureList[0].CollisionGroup = -1;
            targetCircle.IsBullet = true;
            targetCircle.IsSensor = true;

            postmultiplied = Matrix.CreateScale(width, 1.0f, height) * Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);

            Renderer.Add(this);
            AssetManager.AddAsset(this);
        }

        public override void Dispose() {
            AudioController.PlayOnce("Rocket_Boom");
            //TODO: Explosion Effect

            body.Dispose();
            targetCircle.Dispose();
            Renderer.Remove(this);
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
            bool found = false;
            if (targetCircle.ContactList != null) {
                ContactEdge ce = targetCircle.ContactList;
                while (ce.Next != null) {
                    if (ce.Other.UserData is TankCharacter && (ce.Other.UserData as TankCharacter).player.teamID != teamID) {
                        found = true;
                        targetPos = (ce.Other.UserData as TankCharacter).GetPosition();
                    }
                    ce = ce.Next;
                }
            }

            if (found) {
                float targetAngle = (float)Math.Atan2(targetPos.Y - body.Position.Y, targetPos.X - body.Position.X);

                float stupidAngle = targetAngle - angle;
                stupidAngle = (float)((stupidAngle + Math.PI) % (Math.PI * 2) - Math.PI);

                float oldAngle = angle;
                angle = stupidAngle * dt * TURNSPEED + angle;

                //angle = MathHelper.Lerp(angle, targetAngle + stupidAngle, TURNSPEED * dt);
                
                Console.Out.WriteLine("OLDANGLE: {0}; TARGETANGLE: {1}; NEWANGLE: {2}; STUPIDANGLE: {3}", oldAngle, targetAngle, angle, stupidAngle);
            }

            targetCircle.Position = body.Position;

            postmultiplied = Matrix.CreateScale(width, 1.0f, height) * Matrix.CreateRotationY((float)Math.PI / -2 - angle) * Matrix.CreateTranslation(body.Position.X, heightOffGround, body.Position.Y);
            body.Rotation = angle;
            body.LinearVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

            if (timer <= lifespan) {
                timer += dt;
            } else {
                AssetManager.RemoveAsset(this);
            }
        }
    }
}
