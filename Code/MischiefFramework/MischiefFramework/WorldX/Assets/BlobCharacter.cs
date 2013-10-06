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
using Microsoft.Xna.Framework.Audio;

namespace MischiefFramework.WorldX.Assets {
    public class BlobCharacter : Character, IOpaque {
        private Model model;
        private SkinningData skinData;
        private AnimationPlayer animPlayer;

        private Matrix premultiplied;
        private Matrix postmultiplied;

        public bool isAttacking = false;
        public bool isMoving = false;

        private Model accessories;

        private float attackTimeout = 0.0f;
        private const float ATTACK_TIME = 0.2f;

        private SoundEffectInstance moveSound;

        public BlobCharacter(GamePlayer player, World w, Vector2 pos) : base(player, w) {
            moveSound = AudioController.GetSoundEffect("Slime_Movement");
            moveSound.IsLooped = true;

            body = BodyFactory.CreateCircle(w, 0.5f, 1.0f, pos, this);
            body.BodyType = BodyType.Dynamic;
            body.UserData = this;
            body.FixtureList[0].CollisionGroup = -1;

            model = ResourceManager.LoadAsset<Model>("Meshes/Character/Blob Phase one Animation");
            MeshHelper.ChangeEffectUsedByModel(model, Renderer.EffectAnimated);
            
            // Look up our custom skinning information.
            skinData = model.Tag as SkinningData;

            if (skinData == null) throw new InvalidOperationException("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            animPlayer = new AnimationPlayer(skinData);
            animPlayer.StartClip(skinData.AnimationClips["Idle"]);

            premultiplied = Matrix.Identity;
            postmultiplied = Matrix.Identity;
            
            Renderer.Add(this);

            switch (player.teamID) {
                case 0: //Bow
                    accessories = ResourceManager.LoadAsset<Model>("Meshes/Character/Accessories/Bow");
                    break;
                case 1: // Eye patch
                    accessories = ResourceManager.LoadAsset<Model>("Meshes/Character/Accessories/eye patch");
                    break;
                case 2: // Mustache
                    accessories = ResourceManager.LoadAsset<Model>("Meshes/Character/Accessories/Mustache");
                    break;
                default: // Top hat
                    accessories = ResourceManager.LoadAsset<Model>("Meshes/Character/Accessories/Top hat");
                    break;
            }

            MeshHelper.ChangeEffectUsedByModel(accessories, Renderer.Effect3D);
        }

        public override void AsyncUpdate(float dt) {
            float moveSpeedSQ = body.LinearVelocity.LengthSquared();

            if (!isAttacking) {
                float aimX = player.Input.AimX();
                float aimY = player.Input.AimY();

                if ((aimX != 0 || aimY != 0) && !IsCarrying()) {
                    animPlayer.StartClip(skinData.AnimationClips["Roll"]);
                    isAttacking = true;
                    attackTimeout = ATTACK_TIME;
                    //LockControls(true);
                }

                if (moveSpeedSQ > 1) {
                    if (!isMoving) {
                        isMoving = true;
                        animPlayer.StartClip(skinData.AnimationClips["Walk"]);
                        moveSound.Play();
                    }
                } else {
                    if (isMoving) {
                        isMoving = false;
                        animPlayer.StartClip(skinData.AnimationClips["Idle"]);
                        moveSound.Pause();
                    }
                }
            } else {
                attackTimeout -= dt;
                if (attackTimeout <= 0) {
                    //LockControls(false);
                    if (moveSpeedSQ > 1) {
                        isMoving = true;
                        animPlayer.StartClip(skinData.AnimationClips["Walk"]);
                    } else {
                        isMoving = false;
                        animPlayer.StartClip(skinData.AnimationClips["Idle"]);
                    }
                    isAttacking = false;
                }
            }

            animPlayer.Update(TimeSpan.FromSeconds(dt), true, Matrix.Identity);
            postmultiplied = Matrix.CreateScale(carrying==null?0.5f:0.75f) * Matrix.CreateRotationY(-body.Rotation - (float)Math.PI / 2) * Matrix.CreateTranslation(body.Position.X, 0f, body.Position.Y);
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
            MeshHelper.DrawModel(bones[2]*postmultiplied, accessories);
        }

        private WeaponCrate carrying = null;

        internal bool IsCarrying() {
            return (carrying != null);
        }

        internal void Pickup(WeaponCrate weaponCrate) {
            if (IsCarrying() && weaponCrate != null) throw new Exception("Cannot pick up while holding a crate!");
            carrying = weaponCrate;
            AudioController.PlayOnce("Slime_Attack");
        }
    }
}
