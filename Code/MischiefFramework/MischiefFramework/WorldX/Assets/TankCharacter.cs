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
    public class TankCharacter : Character, IOpaque {
        private Model model_tank;
        private Model model_turret;

        private Matrix postmultiplied_tank;
        private Matrix postmultiplied_turret;

        private float turretAngle = 0;

        private const float SPEED = 10f;

        public TankCharacter(GamePlayer player, World w) : base(player, w) {
            model_tank = ResourceManager.LoadAsset<Model>("Meshes/Character/TankBlob");
            MeshHelper.ChangeEffectUsedByModel(model_tank, Renderer.Effect3D);

            model_turret = ResourceManager.LoadAsset<Model>("Meshes/Character/TankTurret");
            MeshHelper.ChangeEffectUsedByModel(model_turret, Renderer.Effect3D);

            postmultiplied_tank = Matrix.Identity;
            postmultiplied_turret = Matrix.Identity;

            body = BodyFactory.CreateRectangle(w, 2.0f, 2.0f, 1.0f, new Vector2(5, 5), this);
            body.BodyType = BodyType.Dynamic;

            Renderer.Add(this);
            AssetManager.AddAsset(this);
        }

        public override void Update(float dt) {
            base.AsyncUpdate(dt);

            float x1 = -Input.AimX();
            float y1 = -Input.AimY();

            if (x1 != 0 && y1 != 0) {
                turretAngle = (float)Math.Atan2(x1, y1);
            }

            postmultiplied_tank = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-body.Rotation - (float)Math.PI / 2) * Matrix.CreateTranslation(body.Position.X, 1f, body.Position.Y);
            postmultiplied_turret = Matrix.CreateScale(0.4f) * Matrix.CreateRotationY(-turretAngle - (float)Math.PI / 4 * 3) * Matrix.CreateTranslation(body.Position.X, 1f, body.Position.Y);
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(postmultiplied_tank, model_tank);
            MeshHelper.DrawModel(postmultiplied_turret, model_turret);
        }
    }
}
