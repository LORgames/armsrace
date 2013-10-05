using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.WorldX.Assets;
using MischiefFramework.Core.Interfaces;

namespace MischiefFramework.WorldX.Weapons {
    internal interface IWeapon : IOpaque {
        void TryFire();
        void Update(float dt);
    }
}
