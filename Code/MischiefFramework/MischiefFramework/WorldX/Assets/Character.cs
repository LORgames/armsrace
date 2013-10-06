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
        public Body body;
        public PlayerInput Input;

        protected float SPEED = 10f;

        internal GamePlayer player;

        private float moveSpeedMod = 1.0f;

        public Character(GamePlayer player, World w) {
            if (SettingManager._moveSpeed == 0) {
                moveSpeedMod = 0.75f;
            } else if (SettingManager._moveSpeed == 1) {
                moveSpeedMod = 1.0f;
            } else {
                moveSpeedMod = 1.25f;
            }

            this.player = player;
            AssetManager.AddAsset(this);
            Input = player.Input;
        }

        public override void Update(float dt) {
            if (!_locked) {
                float x = -Input.GetX();
                float y = -Input.GetY();

                if (x != 0 || y != 0) {
                    body.Rotation = (float)(Math.Atan2(x, y) - Math.PI / 4);
                    body.LinearVelocity = Vector2.UnitX * (float)Math.Cos(body.Rotation) * SPEED * moveSpeedMod + Vector2.UnitY * (float)Math.Sin(body.Rotation) * SPEED * moveSpeedMod;
                } else {
                    body.AngularVelocity = 0;
                    body.LinearVelocity = Vector2.Zero;
                }
            } else {
                body.LinearVelocity = _prething;
            }
        }

        public override void Dispose() {
            body.Dispose();
            Renderer.Remove(this);
        }

        public Vector2 GetPosition() {
            return body.Position;
        }

        protected bool _locked = false;
        private Vector2 _prething = Vector2.Zero;
        public void LockControls(bool locked) {
            _locked = locked;
            _prething = Vector2.Zero;
            body.AngularVelocity = 0;
            body.LinearVelocity = Vector2.Zero;
        }
    }
}
