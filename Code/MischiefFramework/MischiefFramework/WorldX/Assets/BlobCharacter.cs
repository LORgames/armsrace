using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using MischiefFramework.Cache;
using MischiefFramework.Core;
using MischiefFramework.Core.Helpers;
using FarseerPhysics.Factories;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.WorldX.Assets {
    public class BlobCharacter : Character, IOpaque {
        private Model model;
        private SkinningData skinData;
        private AnimationPlayer animPlayer;

        private Matrix premultiplied;
        private Matrix postmultiplied;

        public BlobCharacter(GamePlayer player, World w) : base(player, w) {
            body = BodyFactory.CreateCircle(w, 0.5f, 1.0f, new Vector2(5, 5), this);
            body.BodyType = BodyType.Dynamic;

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
            
            Renderer.Add(this);
        }

        public override void AsyncUpdate(float dt) {
            animPlayer.Update(TimeSpan.FromSeconds(dt), true, Matrix.Identity);
            postmultiplied = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY(-body.Rotation - (float)Math.PI / 2) * Matrix.CreateTranslation(body.Position.X, 0.90f, body.Position.Y);
        }

        public void  RenderOpaque() {
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
