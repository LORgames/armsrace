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
    public class BaseArea {
        public Body area;
        public int baseID;
        public int basePos;
        public int teamID;

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
        }

        void area_OnSeparation(Fixture fixtureA, Fixture fixtureB) {
            Body other;

            if (fixtureA.Body == area) {
                other = fixtureB.Body;
            } else {
                other = fixtureA.Body;
            }

            if (other.UserData is WeaponCrate) {
                if ((other.UserData as WeaponCrate).joint != null) {
                    if ((other.UserData as WeaponCrate).inBase) {
                        if ((other.UserData as WeaponCrate).ownedBy.player.teamID == teamID) {
                            crates--;
                        }
                    }
                    (other.UserData as WeaponCrate).inBase = false;
                }
            }
        }

        bool area_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
            Body other;

            if (fixtureA.Body == area) {
                other = fixtureB.Body;
            } else {
                other = fixtureA.Body;
            }

            if (other.UserData is WeaponCrate) {
                if ((other.UserData as WeaponCrate).ownedBy != null && (other.UserData as WeaponCrate).ownedBy.player.teamID == teamID) {
                    if ((other.UserData as WeaponCrate).joint != null) {
                        area.World.RemoveJoint((other.UserData as WeaponCrate).joint);
                        (other.UserData as WeaponCrate).ownedBy.Pickup(null);
                        (other.UserData as WeaponCrate).ownedBy = (BlobCharacter)PlayerManager.ActivePlayers[baseID].character;
                        crates++;
                    }
                    (other.UserData as WeaponCrate).inBase = true;
                }
            }

            return true;
        }
    }
}
