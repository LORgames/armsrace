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
        
        public Character(PlayerInput input, World w) {
            model = ResourceManager.LoadAsset<Model>("Meshes/Character/Blob Phase one");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.EffectAnimated);

            // Look up our custom skinning information.
            skinData = model.Tag as SkinningData;

            if (skinData == null)
                throw new InvalidOperationException("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            animPlayer = new AnimationPlayer(skinData);
            animPlayer.StartClip(skinData.AnimationClips["Walk"]);

            premultiplied = Matrix.CreateTranslation(0, 1, 0);
            postmultiplied = Matrix.Identity;

            body = BodyFactory.CreateCircle(w, 2.5f, 1.0f);
            body.Position = new Vector2(5, 5);

            Renderer.Add(this);
            AssetManager.AddAsset(this);
        }

        public override void AsyncUpdate(float dt) {

            //input.GetX();

            animPlayer.Update(TimeSpan.FromSeconds(dt), true, Matrix.Identity);
            postmultiplied = premultiplied * Matrix.CreateRotationY(body.Rotation) * Matrix.CreateTranslation(body.Position.X, 0, body.Position.Y);
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
