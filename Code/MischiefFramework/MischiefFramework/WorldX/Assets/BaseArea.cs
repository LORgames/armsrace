using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using MischiefFramework.Cache;

namespace MischiefFramework.WorldX.Assets {
    public class BaseArea {
        Body area;
        int baseID;

        public BaseArea(int baseID, World world) {
            area = BodyFactory.CreateRectangle(world, 6, 6, 0);
            area.IsSensor = true;
            area.OnCollision += new OnCollisionEventHandler(area_OnCollision);
            area.OnSeparation += new OnSeparationEventHandler(area_OnSeparation);

            float angle = (float)Math.PI / 2 * baseID;
            area.Position = 14f * (Vector2.UnitX * (float)Math.Cos(angle) + Vector2.UnitY * (float)Math.Sin(angle));

            this.baseID = baseID;
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
                if ((other.UserData as WeaponCrate).ownedBy != null &&
                    (SettingManager._baseSharing == 1 ? (other.UserData as WeaponCrate).ownedBy.player.teamID == baseID : (other.UserData as WeaponCrate).ownedBy.player.playerID == baseID)) {
                    if ((other.UserData as WeaponCrate).joint != null) {
                        area.World.RemoveJoint((other.UserData as WeaponCrate).joint);
                        (other.UserData as WeaponCrate).ownedBy.Pickup(null);
                    }
                    (other.UserData as WeaponCrate).inBase = true;
                }
            }

            return true;
        }
    }
}
