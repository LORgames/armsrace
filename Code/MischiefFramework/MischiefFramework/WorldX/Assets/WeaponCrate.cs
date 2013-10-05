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
using FarseerPhysics.Dynamics.Joints;

namespace MischiefFramework.WorldX.Assets {
    public class WeaponCrate : Asset, IOpaque {

        private Model model;
        private Body body;

        private Matrix premultiplied = Matrix.Identity;

        public BlobCharacter ownedBy = null;
        public Joint joint = null;
        public bool inBase = false;

        public WeaponCrate(World world) {
            body = BodyFactory.CreateCircle(world, 0.5f, 1.0f);
            body.BodyType = BodyType.Dynamic;
            body.UserData = this;
            //body.FixtureList[0].CollisionGroup = -1;

            body.LinearDamping =  5.0f;
            body.AngularDamping = 5.0f;

            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

            model = ResourceManager.LoadAsset<Model>("Meshes/TestObjects/Crate");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);

            AssetManager.AddAsset(this);
            Renderer.Add(this);
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
            Body other;

            if (fixtureA.Body == body) {
                other = fixtureB.Body;
            } else {
                other = fixtureA.Body;
            }

            BlobCharacter charCollider;

            if (other.UserData is BlobCharacter && ownedBy != other.UserData) {
                charCollider = (BlobCharacter)other.UserData;
                if (ownedBy != null && ownedBy.player.teamID == charCollider.player.teamID && inBase) return false;

                if (!charCollider.IsCarrying()) {

                    if (ownedBy != null) {
                        body.World.RemoveJoint(joint);
                        ownedBy.Pickup(null);
                    }

                    joint = JointFactory.CreateWeldJoint(body.World, body, other, Vector2.Zero, Vector2.Zero);
                    ownedBy = charCollider;

                    charCollider.Pickup(this);

                    body.IsSensor = true;
                }

                return false;
            }

            return true;
        }

        public override void AsyncUpdate(float dt) {
            premultiplied = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY(-body.Rotation) * Matrix.CreateTranslation(body.Position.X, 0.0f, body.Position.Y);
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(premultiplied, model);
        }
    }
}
