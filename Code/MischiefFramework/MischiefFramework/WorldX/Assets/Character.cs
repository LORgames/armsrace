using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using MischiefFramework.Core;
using MischiefFramework.Cache;
using MischiefFramework.Core.Interfaces;
using Microsoft.Xna.Framework;
using MischiefFramework.Core.Helpers;
using FarseerPhysics.Factories;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.WorldX.Assets {
    public class Character : Asset {
        protected Body body;
        protected PlayerInput Input;

        private const float SPEED = 10f;

        internal GamePlayer player;

        public Character(GamePlayer player, World w) {
            this.player = player;
            AssetManager.AddAsset(this);
            Input = player.Input;
        }

        public override void Update(float dt) {
            float x = -Input.GetX();
            float y = -Input.GetY();

            if (x != 0 || y != 0) {
                body.Rotation = (float)(Math.Atan2(x, y) - Math.PI / 4);
                body.LinearVelocity = Vector2.UnitX * (float)Math.Cos(body.Rotation) * SPEED + Vector2.UnitY * (float)Math.Sin(body.Rotation) * SPEED;
            } else {
                body.AngularVelocity = 0;
                body.LinearVelocity = Vector2.Zero;
            }
        }

        public Vector2 GetPosition() {
            return body.Position;
        }
    }
}
