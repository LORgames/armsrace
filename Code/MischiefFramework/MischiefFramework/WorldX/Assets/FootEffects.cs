using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;
using MischiefFramework.Core.Helpers;
using MischiefFramework.Core;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.WorldX.Information;
using Microsoft.Xna.Framework;

namespace MischiefFramework.WorldX.Assets {
    public class FootEffects : ITransparent, IShadowLight {
        private RenderTarget2D RT;

        private Model worldQuad;
        private Effect fx;

        private Texture2D[] gooTextures;

        private bool cleared = false;

        public FootEffects() {
            fx = Renderer.EffectTransparent.Clone();

            worldQuad = ResourceManager.LoadAsset<Model>("Meshes/Levels/plane");
            MeshHelper.ChangeEffectUsedByModel(worldQuad, fx, false);

            RT = new RenderTarget2D(Game.device, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            fx.Parameters["TextureEnabled"].SetValue(true);

            Renderer.Add(this);

            gooTextures = new Texture2D[8];

            for (int i = 1; i < 9; i++) {
                gooTextures[i - 1] = ResourceManager.LoadAsset<Texture2D>("Meshes/Levels/Goo/Slime Decal " + i);
            }
        }

        public void RenderTransparent() {
            fx.Parameters["Texture"].SetValue(RT);

            MeshHelper.DrawModel(Matrix.CreateTranslation(0, 0.01f, 0), worldQuad);
            //worldQuad
        }

        public void RenderShadow() {
            Game.device.SetRenderTarget(RT);

            if (!cleared) {
                cleared = true;
                Game.device.Clear(Color.Transparent);
            }

            MeshHelper.RenderCamera = new Camera(1, 1);

            SpriteBatch sb = new SpriteBatch(Game.device);

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            Rectangle r;

            foreach (GamePlayer plr in PlayerManager.ActivePlayers) {
                if (plr.character != null) {

                    float XS = 15.489f;
                    float YS = 16.374f;

                    //System.Diagnostics.Debug.WriteLine("TRIALLING PLAYER THING!");
                    r.X = (int)((XS - plr.character.body.Position.Y) * (1024/XS));
                    r.Y = (int)((YS + plr.character.body.Position.X) * (1024/YS));
                    r.Width = 64;
                    r.Height = 64;

                    sb.Draw(gooTextures[Game.random.Next(8)], r, null, new Color(1, 1, 1, 0.05f), (float)Game.random.NextDouble()*6.28f, new Vector2(64, 64), SpriteEffects.None, 0f);

                    //TODO FIX THIS @SAM @MILES
                    //System.Diagnostics.Debug.WriteLine(r + " " + plr.character.body.Position);
                }
            }

            sb.End();
        }

        public void RenderLight() { /* DO NOTHING HERE */ }
    }
}
