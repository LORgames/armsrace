using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.Cache;
using MischiefFramework.WorldX.Assets;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Core;
using MischiefFramework.Core.Helpers;

namespace MischiefFramework.WorldX.Weapons {
    public class RocketLauncher : IWeapon {
        private const float FIRE_DELAY = 6.0f;
        private float currentDelay = 0.0f;

        private TankCharacter tank;

        public RocketLauncher(TankCharacter myTank) {
            tank = myTank;

            //Model model = ResourceManager.LoadAsset<Model>("Meshes/Weapons/Gatling Gun");
            //MeshHelper.ChangeEffectUsedByModel(model, Renderer.Effect3D);
        }

        public void Update(float dt) {
            if (currentDelay > 0) currentDelay -= dt;
        }

        public void TryFire() {
            
        }

        public void RenderOpaque() {
            
        }
    }
}
