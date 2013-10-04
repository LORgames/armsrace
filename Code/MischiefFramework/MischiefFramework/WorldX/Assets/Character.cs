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

namespace MischiefFramework.WorldX.Assets {
    public class Character : Asset, IOpaque {
        private Body body;

        private Model model;
        private SkinningData skinData;
        private AnimationPlayer animPlayer;

        private Matrix premultiplied;
        private Matrix postmultiplied;

        private PlayerInput input;

        private const float SPEED = 10f;
        
        public Character(PlayerInput input, World w) {
            model = ResourceManager.LoadAsset<Model>("Meshes/Character/Blob Phase one");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.EffectAnimated);
            
            // Look up our custom skinning information.
            skinData = model.Tag as SkinningData;

            if (skinData == null) throw new InvalidOperationException("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            animPlayer = new AnimationPlayer(skinData);
            animPlayer.StartClip(skinData.AnimationClips["Walk"]);

            premultiplied =  Matrix.CreateTranslation(0, 1, 0);
            postmultiplied = Matrix.Identity;

            body = BodyFactory.CreateCircle(w, 1.0f, 1.0f, new Vector2(5, 5), this);
            body.BodyType = BodyType.Dynamic;

            Renderer.Add(this);
            AssetManager.AddAsset(this);

            this.input = input;
        }

        public override void AsyncUpdate(float dt) {
            float x = -input.GetX();
            float y = -input.GetY();

            if (x != 0 || y != 0) {
                body.Rotation = (float)(Math.Atan2(x, y) + Math.PI / 4);
                body.LinearVelocity = Vector2.UnitX * (float)Math.Cos(body.Rotation) * SPEED + Vector2.UnitY * (float)Math.Sin(body.Rotation) * SPEED;
            } else {
                body.AngularVelocity = 0;
                body.LinearVelocity = Vector2.Zero;
            }

            animPlayer.Update(TimeSpan.FromSeconds(dt), true, Matrix.Identity);
            postmultiplied = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY(-body.Rotation - (float)Math.PI/2) * Matrix.CreateTranslation(body.Position.X, 0.90f, body.Position.Y);
        }

        public void RenderOpaque() {
            Matrix[] mats = new Matrix[model.Bones.Count];
            Matrix[] bones = animPlayer.GetSkinTransforms();

            foreach (ModelMesh mesh in model.Meshes) {
                foreach (Effect effect in mesh.Effects) {
                    effect.Parameters["Bones"].SetValue(bones);
                }
            }

            MeshHelper.DrawModel(postmultiplied, model);
        }
    }
}
