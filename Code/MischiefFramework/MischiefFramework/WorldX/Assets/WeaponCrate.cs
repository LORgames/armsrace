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
    public class WeaponCrate : IOpaque {
        private Model model;
        private Body body;

        private Matrix premultiplied = Matrix.Identity;

        public Joint joint = null;

        public BlobCharacter ownedBy = null;

        public bool isHeld = false;
        public bool inBase = false;

        public BaseArea baseIn;

        public WeaponCrate(World world, Vector2 position) {
            body = BodyFactory.CreateCircle(world, 0.5f, 1.0f, position);
            body.BodyType = BodyType.Dynamic;
            body.UserData = this;

            body.LinearDamping =  5.0f;
            body.AngularDamping = 5.0f;

            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

            model = ResourceManager.LoadAsset<Model>("Meshes/TestObjects/Crate");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);

            Renderer.Add(this);
        }

        private bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
            Body other;
            if (fixtureA.Body == body) { other = fixtureB.Body; } else { other = fixtureA.Body; }

            if (other.UserData is BlobCharacter) {
                BlobCharacter charCollider = (BlobCharacter)other.UserData;

                if (ownedBy == charCollider) {
                    return false;
                } else if (!charCollider.IsCarrying()) {
                    if (isHeld && charCollider.isAttacking) {
                        Drop();
                        Pickup(charCollider);
                    } else if (inBase && baseIn.teamID == charCollider.player.teamID) {
                        return false;
                    } else if (!isHeld) {
                        Pickup(charCollider);
                    }
                } else {
                    return false;
                }
            }

            return true;
        }

        private void joint_Broke(Joint arg1, float arg2) {
            body.IsSensor = false;
        }

        public void Drop() {
            body.World.RemoveJoint(joint);
            ownedBy.Pickup(null);
            ownedBy = null;
            isHeld = false;
        }

        public void Dispose() {
            body.Dispose();
            if (ownedBy != null) {
                ownedBy.Pickup(null);
                ownedBy = null;
            }
            Renderer.Remove(this);
        }

        public void Pickup(BlobCharacter newOwner) {
            if (isHeld) throw new Exception("Cannot be owned while its being held!");
            if (ownedBy != null) throw new Exception("Cannot be owned while someone else already owns it!");
            if (newOwner.IsCarrying()) throw new Exception("Cannot be held by someone already holding something!");

            joint = JointFactory.CreateWeldJoint(body.World, body, newOwner.body, Vector2.Zero, Vector2.Zero);
            joint.Broke += new Action<Joint, float>(joint_Broke);

            isHeld = true;
            ownedBy = newOwner;

            body.IsSensor = true;
            newOwner.Pickup(this);
        }

        public void RenderOpaque() {
            premultiplied = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY(-body.Rotation) * Matrix.CreateTranslation(body.Position.X, 0.0f, body.Position.Y);
            MeshHelper.DrawModel(premultiplied, model);
        }
    }
}
