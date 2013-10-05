using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MischiefFramework.WorldX.Assets {
    public class BaseArea {
        Body area;

        public BaseArea(int baseID, World world) {
            area = BodyFactory.CreateRectangle(world, 6, 6, 0);
            area.IsSensor = true;
            area.OnCollision += new OnCollisionEventHandler(area_OnCollision);

            float angle = (float)Math.PI / 2 * baseID;
            area.Position = 14f * (Vector2.UnitX * (float)Math.Cos(angle) + Vector2.UnitY * (float)Math.Sin(angle));
        }

        bool area_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact) {
            Body other;

            if (fixtureA.Body == area) {
                other = fixtureB.Body;
            } else {
                other = fixtureA.Body;
            }

            if (other is WeaponCrate) {
                //((WeaponCrate)other).ow
            }

            return true;
        }
    }
}
