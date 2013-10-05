using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using MischiefFramework.Cache;
using MischiefFramework.Core;

namespace MischiefFramework.WorldX.Assets {
    public class BaseArea : Asset {
        internal Body area;
        internal int baseID;
        internal int basePos;
        internal int teamID;

        public Vector2 CenterPoint = Vector2.Zero;

        internal int crates = 0;

        public BaseArea(int baseID, World world, int teamID) {
            area = BodyFactory.CreateRectangle(world, 6, 6, 0);
            area.IsSensor = true;
            area.OnCollision += new OnCollisionEventHandler(area_OnCollision);
            area.OnSeparation += new OnSeparationEventHandler(area_OnSeparation);

            if (baseID == 0) {
                basePos = 0;
            } else if (baseID == 1) {
                basePos = 2;
            } else if (baseID == 2) {
                basePos = 3;
            } else {
                basePos = 1;
            }

            float angle = (float)Math.PI / 2 * basePos;
            area.Position = 14f * (Vector2.UnitX * (float)Math.Cos(angle) + Vector2.UnitY * (float)Math.Sin(angle));

            CenterPoint.X = 14f * (float)Math.Cos(angle);
            CenterPoint.Y = 14f * (float)Math.Sin(angle);

            this.baseID = baseID;
            this.teamID = teamID;

            AssetManager.AddAsset(this);
        }

        public override void AsyncUpdate(float dt) {
            crates = 0;
            if (area.ContactList != null) {
                FarseerPhysics.Dynamics.Contacts.ContactEdge ce = area.ContactList;

                while (ce != null) {
                    if (ce.Other.UserData is WeaponCrate) {
                        crates++;
                    }
                    ce = ce.Next;
                }
            }
        }

        public override void Dispose() {
            area.Dispose();
        }

        void area_OnSeparation(Fixture fixtureA, Fixture fixtureB) {
            Body other; if (fixtureA.Body == area) { other = fixtureB.Body; } else { other = fixtureA.Body; }

            if (other.UserData is WeaponCrate) {
                if ((other.UserData as WeaponCrate).isHeld) {
                    (other.UserData as WeaponCrate).inBase = false;
                    (other.UserData as WeaponCrate).baseIn = null;
                } else {
                    throw new Exception("Stop knocking crates around!");
                }
            }
        }

        bool area_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
            Body other; if (fixtureA.Body == area) { other = fixtureB.Body; } else { other = fixtureA.Body; }

            if (other.UserData is WeaponCrate) {
                if ((other.UserData as WeaponCrate).isHeld && (other.UserData as WeaponCrate).ownedBy.player.teamID == teamID) {
                    (other.UserData as WeaponCrate).Drop();
                    (other.UserData as WeaponCrate).baseIn = this;
                    (other.UserData as WeaponCrate).inBase = true;
                }
            }

            return true;
        }
    }
}
