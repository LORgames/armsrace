using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.Cache;
using ZDataTypes;
using MischiefFramework.WorldX.Stage;
using MischiefFramework.Core;
using MischiefFramework.WorldX.Containers;
using MischiefFramework.WorldX.Assets;

namespace MischiefFramework.WorldX.Information {
    internal class InfoPanel : IHeadsUpDisplay {
        internal static InfoPanel instance;
        string header;
        int headerIcon;
        List<string> stats;
        List<int> upgradeIcons;
        SpriteFont headerFont;
        SpriteFont mainStatFont;
        SpriteFont statsFont;
        float DESC_COLUMN_WIDTH = 150.0f;
        SpritesheetPosition[] positions;
        Texture2D ninjaSpritesheet;

        Texture2D usernames;

        Texture2D ready;
        Texture2D set;
        Texture2D go;

        Color brownText = new Color(60, 43, 16);

        const int MAX_INDEX_OF_MAIN_STATS = 10;

        bool visible = false;

        WorldController.Phases phase;
        float timer = 0.0f;

        public InfoPanel() {
            instance = this;
            this.headerFont = ResourceManager.LoadAsset<SpriteFont>("Fonts/InfoPanelHeader");
            this.mainStatFont = ResourceManager.LoadAsset<SpriteFont>("Fonts/InfoPanelMainStat");
            this.statsFont = ResourceManager.LoadAsset<SpriteFont>("Fonts/InfoPanelStats");
            this.ready = ResourceManager.LoadAsset<Texture2D>("HUD/Ready");
            this.set = ResourceManager.LoadAsset<Texture2D>("HUD/Set");
            this.go = ResourceManager.LoadAsset<Texture2D>("HUD/Go");
            //ninjaSpritesheet = ResourceManager.LoadAsset<Texture2D>("HUD/Ninja/SpriteSheet");
            //positions = ResourceManager.LoadAsset<SpritesheetPosition[]>("HUD/Ninja/positions");

            usernames = ResourceManager.LoadAsset<Texture2D>("HUD/Player Markers");
        }

        public void SetTimer(WorldController.Phases phase, float timer) {
            this.phase = phase;
            this.timer = timer;
        }

        public void ChangeInformation(string header, int headerIcon, List<string> stats, List<int> upgradeIcons) {
            this.header = header;
            this.headerIcon = headerIcon;
            this.stats = stats;
            this.upgradeIcons = upgradeIcons;
            this.visible = true;
        }

        public void Hide() {
            visible = false;
        }

        public void Show() {
            visible = true;
        }

        public bool IsVisible() {
            return visible;
        }

        private Rectangle FindSourcePosition(string name) {
            Rectangle sourcePos = Rectangle.Empty;
            for (int i = 0; i < positions.Length; i++) {
                if (positions[i].Name == name) {
                    sourcePos.X = positions[i].X;
                    sourcePos.Y = positions[i].Y;
                    sourcePos.Width = positions[i].Width;
                    sourcePos.Height = positions[i].Height;
                }
            }
            return sourcePos;
        }

        public void RenderHeadsUpDisplay(SpriteBatch drawtome) {
            if (visible) {
                Vector2 pos = Vector2.Zero;
                /*for (int i = 0; i < PlayerManager.ActivePlayers.Count; i++) {
                    string text = string.Format("Player {0}: {1}", i + 1, Level.bases[i].crates);
                    drawtome.DrawString(headerFont, text, pos, Color.Red);
                    pos.X += headerFont.MeasureString(text).X + 10;
                }*/

                if (phase == WorldController.Phases.Phase1Ready || phase == WorldController.Phases.Phase1Play 
                    || phase == WorldController.Phases.Phase1Scores || phase == WorldController.Phases.Phase2Ready) {
                    drawtome.DrawString(headerFont, timer.ToString("00.00"), new Vector2(Game.device.Viewport.Width / 2, 0.0f), Color.Red);
                    drawtome.End();
                    if (timer <= 3f) {
                        drawtome.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                        drawtome.Draw(timer <= 1f ? go : timer <= 2f ? set : ready, drawtome.GraphicsDevice.Viewport.Bounds, ready.Bounds, new Color(new Vector4(1f, 1f, 1f, timer - (timer <= 1f ? 0f : timer <= 2f ? 1f : 2f))));
                        drawtome.End();
                    }
                    drawtome.Begin();
                }

                if (phase == WorldController.Phases.Phase1Play || phase == WorldController.Phases.Phase1Scores) {
                    foreach (MischiefFramework.WorldX.Assets.BaseArea baseArea in Level.bases) {
                        switch (baseArea.baseID) {
                            case 0:
                                pos.X = 0.0f;
                                pos.Y = Game.device.Viewport.Height - 100;
                                break;
                            case 1:
                                pos.X = Game.device.Viewport.Width - 150;
                                pos.Y = 0.0f;
                                break;
                            case 2:
                                pos.X = Game.device.Viewport.Width - 150;
                                pos.Y = Game.device.Viewport.Height - 100;
                                break;
                            case 3:
                                pos.X = 0.0f;
                                pos.Y = 0.0f;
                                break;
                        }

                        string text = string.Format("{0}: {1}", SettingManager._baseSharing == 0 ? "Player " + (baseArea.baseID + 1) : "Team " + (baseArea.teamID + 1), baseArea.crates);
                        drawtome.DrawString(headerFont, text, pos, Color.Red);
                    }
                }

                for (int i = 0; i < PlayerManager.ActivePlayers.Count; i++) {
                    GamePlayer plr = PlayerManager.ActivePlayers[i];
                    if (plr.character != null) {
                        bool drawholding = false;

                        if (plr.character is WorldX.Assets.BlobCharacter) {
                            drawholding = (plr.character as WorldX.Assets.BlobCharacter).IsCarrying();
                        }

                        Vector3 plrPos = Vector3.UnitX * plr.character.body.Position.X + Vector3.UnitZ * plr.character.body.Position.Y + Vector3.UnitY;
                        plrPos = Vector3.Transform(plrPos, Renderer.CharacterCamera.ViewProjection);

                        int width = Game.device.Viewport.Width;
                        int height = Game.device.Viewport.Height;

                        Vector2 ps = new Vector2(width * (plrPos.X/2 + 0.5f) - 32, height - ((plrPos.Y/2 + 0.5f) * height + 80)); //32 to center a bit

                        if (drawholding) {
                            ps.Y -= 48;
                            drawtome.Draw(usernames, ps, new Rectangle(256, 0, 64, 64), Color.White);
                            ps.Y += 48;
                        }

                        if (phase == WorldController.Phases.Phase2Play) {
                            string healthString = (plr.character as TankCharacter).health.ToString("0");
                            float healthRatio = (plr.character as TankCharacter).health / TankCharacter.MAXHEALTH;

                            Texture2D tex = new Texture2D(Game.device, 1, 1);
                            tex.SetData<Color>(new Color[] { Color.White });

                            drawtome.Draw(tex, new Rectangle((int)ps.X - 2, (int)ps.Y - 2, 64 + 4, 10 + 4), Color.Black);
                            drawtome.Draw(tex, new Rectangle((int)ps.X, (int)ps.Y, (int)(64 * healthRatio), 10), Color.Red);

                            ps.Y -= 72;
                            drawtome.Draw(usernames, ps, new Rectangle(64 * i, 0, 64, 64), Color.White);
                            ps.Y += 72;
                        } else {
                            drawtome.Draw(usernames, ps, new Rectangle(64 * i, 0, 64, 64), Color.White);
                        }
                    }
                }

                /*
                // Draw background
                Rectangle backgroundSourcePos = FindSourcePosition("InfoPanel");
                Vector2 backgroundDestPos = Vector2.Zero;
                backgroundDestPos.X = (Game.device.Viewport.Width - backgroundSourcePos.Width) / 2.0f;
                backgroundDestPos.Y = (Game.device.Viewport.Height - backgroundSourcePos.Height) / 2.0f;
                drawtome.Draw(ninjaSpritesheet, backgroundDestPos, backgroundSourcePos, Color.White);

                // Draw header icon
                Rectangle headerIconSourcePos = FindSourcePosition("Icon" + headerIcon.ToString());
                Vector2 headerIconDestPos = backgroundDestPos;
                headerIconDestPos.X += 48;
                headerIconDestPos.Y += 45;
                drawtome.Draw(ninjaSpritesheet, headerIconDestPos, headerIconSourcePos, Color.White);

                // Draw header text
                Vector2 headerDestPos = backgroundDestPos;
                headerDestPos.X += 130f;
                headerDestPos.Y += 60f;
                drawtome.DrawString(headerFont, header, headerDestPos, brownText);

                // Draw stats
                for (int i = 0; i < Math.Min(MAX_INDEX_OF_MAIN_STATS, stats.Count); i += 2) {
                    Vector2 statsDestPos = headerIconDestPos;
                    statsDestPos.Y += i == 0 ? 100f : 110f + mainStatFont.MeasureString(stats[0]).Y + (statsFont.MeasureString(stats[i]).Y - 4f) * (i / 2 - 1);
                    statsDestPos.X += 10f;
                    drawtome.DrawString(i == 0 ? mainStatFont : statsFont, stats[i], statsDestPos, brownText);
                    statsDestPos.X += DESC_COLUMN_WIDTH;
                    drawtome.DrawString(i == 0 ? mainStatFont : statsFont, stats[i + 1], statsDestPos, Color.White);
                }

                // Draw upgrade icons
                Vector2 upgradeIconsDestPos = backgroundDestPos + new Vector2(57, 394);
                for (int i = 0; i < upgradeIcons.Count; i++) {
                    Rectangle upgradeIconsSourcePos = FindSourcePosition("Icon" + upgradeIcons[i].ToString());
                    drawtome.Draw(ninjaSpritesheet, upgradeIconsDestPos, upgradeIconsSourcePos, Color.White);
                    upgradeIconsDestPos.X += 72;
                }

                // Draw upgrade icon (next to the upgrade text)
                Vector2 upgradeIconDestPos = backgroundDestPos + new Vector2(305f, 335f);
                Rectangle upgradeIconSourcePos = FindSourcePosition("InfoPanelUpgradeIcon");
                drawtome.Draw(ninjaSpritesheet, upgradeIconDestPos, upgradeIconSourcePos, Color.White);
                 */
            }
        }
    }
}
