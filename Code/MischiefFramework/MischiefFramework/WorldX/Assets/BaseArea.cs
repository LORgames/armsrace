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
            area = BodyFactory.CreateRectangle(world, 5, 5, 0);
            area.IsSensor = true;

            float angle = (float)Math.PI / 2 * baseID;
            area.Position = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

        }

    }
}
