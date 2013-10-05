using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;

namespace MischiefFramework.WorldX.Assets {
    public class BaseArea {

        Body area;

        public BaseArea(int baseID, World world) {
            area = BodyFactory.CreateRectangle(world, 5, 5, 0);
            area.IsSensor = true;
            
        }

    }
}
