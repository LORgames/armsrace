using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;
using Microsoft.Xna.Framework;
using MischiefFramework.Core;

namespace MischiefFramework.States {
    class LobbyState : IState {
        private SpriteBatch renderTarget;
        private SpriteFont font;

        private const float MAX_HELD_TIME = 0.5f;
        private List<float> getXHeldTimes = new List<float>();
        private List<float> getYHeldTimes = new List<float>();

        private Vector2 titlePosition = Vector2.Zero;
        private Vector2 team1Position = new Vector2(0, 100);
        private Vector2 team2Position = new Vector2(100, 100);
        private Vector2 team3Position = new Vector2(200, 100);
        private Vector2 team4Position = new Vector2(300, 100);
        private Vector2 footerPosition = new Vector2(0, 600);

        private Rectangle tableBoxTop = new Rectangle(0, 100, 400, 1);
        private Rectangle tableBoxLeft = new Rectangle(0, 100, 1, 600);
        private Rectangle tableBoxBottom = new Rectangle(0, 700, 400, 1);
        private Rectangle tableBoxRight = new Rectangle(400, 100, 1, 600);
        private Rectangle tableHeaderLine = new Rectangle(0, 200, 400, 1);
        private Rectangle tablePlayer12Line = new Rectangle(100, 100, 1, 600);
        private Rectangle tablePlayer23Line = new Rectangle(200, 100, 1, 600);
        private Rectangle tablePlayer34Line = new Rectangle(300, 100, 1, 600);
        private Texture2D tableTexture = new Texture2D(Game.device, 1, 1);

        private List<Texture2D> playerIconTextures = new List<Texture2D>();

        public LobbyState(GraphicsDevice device) {
            renderTarget = new SpriteBatch(device);
            font = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont");

            tableTexture.SetData<Color>(new Color[] { Color.White });

            for (int i = 0; i < PlayerManager.MAX_ACTIVE; i++) {
                List<Color> playerTextureColor = new List<Color>();
                playerIconTextures.Add(new Texture2D(Game.device, 50, 50));
                for (int j = 0; j < playerIconTextures[i].Height * playerIconTextures[i].Width; j++) {
                    playerTextureColor.Add(i == 0 ? Color.Blue : i == 1 ? Color.Red : i == 2 ? Color.Green : Color.Yellow);
                }
                playerIconTextures[i].SetData<Color>(playerTextureColor.ToArray());
            }

            Update(new GameTime());

            Game.players.Activate();
        }

        public bool Update(GameTime gameTime) {
            // Update Input and Teams
            for (int i = 0; i < PlayerManager.ActivePlayers.Count; i++) {
                if (getXHeldTimes.Count < (i + 1)) {
                    getXHeldTimes.Add(MAX_HELD_TIME);
                    getYHeldTimes.Add(MAX_HELD_TIME);
                }

                float xHeld = Math.Abs(PlayerManager.ActivePlayers[i].Input.GetX()) > 0.5 ? PlayerManager.ActivePlayers[i].Input.GetX() : 0.0f;
                float yHeld = Math.Abs(PlayerManager.ActivePlayers[i].Input.GetY()) > 0.5 ? PlayerManager.ActivePlayers[i].Input.GetY() : 0.0f;

                if (Math.Abs(yHeld) <= float.Epsilon || getYHeldTimes[i] >= MAX_HELD_TIME)
                    getYHeldTimes[i] = 0.0f;
                if (Math.Abs(xHeld) <= float.Epsilon || getYHeldTimes[i] >= MAX_HELD_TIME)
                    getXHeldTimes[i] = 0.0f;

                if (xHeld != 0) {
                    if (getXHeldTimes[i] == 0f) {
                        if (xHeld > 0.5) {
                            PlayerManager.ActivePlayers[i].teamID++;
                            PlayerManager.ActivePlayers[i].teamID %= 4;
                        } else if (xHeld < -0.5) {
                            PlayerManager.ActivePlayers[i].teamID += 3;
                            PlayerManager.ActivePlayers[i].teamID %= 4;
                        }
                    }
                    getXHeldTimes[i] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (PlayerManager.ActivePlayers[i].Input.GetJump()) {
                    StateManager.Pop();
                    StateManager.Push(new PlayingState());
                }
            }

            // Update sizes and positions
            Viewport vp = Game.device.Viewport;

            Vector2 titleStringSize = font.MeasureString("Press Start to Join");
            Vector2 team1StringSize = font.MeasureString("Team 1");
            Vector2 team2StringSize = font.MeasureString("Team 2");
            Vector2 team3StringSize = font.MeasureString("Team 3");
            Vector2 team4StringSize = font.MeasureString("Team 4");
            Vector2 footerStringSize = font.MeasureString("Press A (Controller) or Space (Keyboard) to Lock-in");
            //Vector2 playerIconSize = new Vector2(50f, 50f);

            int columnWidth = (int)(team1StringSize.X > playerIconTextures[0].Width ? team1StringSize.X : playerIconTextures[0].Width);

            tableBoxTop.Width = columnWidth * 4;
            tableBoxBottom.Width = tableBoxTop.Width;
            tableBoxLeft.Height = (int)(team1StringSize.Y + playerIconTextures[0].Height * PlayerManager.ActivePlayers.Count);
            tableBoxRight.Height = tableBoxLeft.Height;
            tableHeaderLine.Width = tableBoxTop.Width;
            tablePlayer12Line.Height = tableBoxLeft.Height;
            tablePlayer23Line.Height = tableBoxLeft.Height;
            tablePlayer34Line.Height = tableBoxLeft.Height;

            Vector2 screenSize = Vector2.Zero;
            screenSize.X = tableBoxTop.Width > titleStringSize.X ? tableBoxTop.Width : titleStringSize.X;
            screenSize.Y = titleStringSize.Y + tableBoxLeft.Height + footerStringSize.Y;

            titlePosition.X = (vp.Width - titleStringSize.X) / 2;
            titlePosition.Y = (vp.Height - screenSize.Y) / 2;

            tableBoxTop.X = (vp.Width - tableBoxTop.Width) / 2;
            tableBoxTop.Y = (int)(titlePosition.Y + titleStringSize.Y);

            tableHeaderLine.X = tableBoxTop.X;
            tableHeaderLine.Y = (int)(tableBoxTop.Y + team1StringSize.Y);

            tableBoxLeft.X = tableBoxTop.X;
            tableBoxLeft.Y = tableBoxTop.Y;

            tableBoxBottom.X = tableBoxLeft.X;
            tableBoxBottom.Y = tableBoxLeft.Y + tableBoxLeft.Height + tableBoxTop.Height;

            tableBoxRight.X = tableBoxTop.X + tableBoxTop.Width;
            tableBoxRight.Y = tableBoxTop.Y;

            tablePlayer12Line.X = tableBoxLeft.X + columnWidth;
            tablePlayer12Line.Y = tableBoxLeft.Y;

            tablePlayer23Line.X = tablePlayer12Line.X + columnWidth;
            tablePlayer23Line.Y = tablePlayer12Line.Y;

            tablePlayer34Line.X = tablePlayer23Line.X + columnWidth;
            tablePlayer34Line.Y = tablePlayer23Line.Y;

            team1Position.X = tableBoxLeft.X;
            team1Position.Y = tableBoxLeft.Y;

            team2Position.X = tablePlayer12Line.X;
            team2Position.Y = tablePlayer12Line.Y;

            team3Position.X = tablePlayer23Line.X;
            team3Position.Y = tablePlayer23Line.Y;

            team4Position.X = tablePlayer34Line.X;
            team4Position.Y = tablePlayer34Line.Y;

            footerPosition.X = (vp.Width - footerStringSize.X) / 2;
            footerPosition.Y = tableBoxBottom.Y;


            return false;
        }

        public bool Render(GameTime gameTime) {
            renderTarget.Begin();

            renderTarget.DrawString(font, "Press Start to Join", titlePosition, Color.White);
            renderTarget.DrawString(font, "Team 1", team1Position, Color.White);
            renderTarget.DrawString(font, "Team 2", team2Position, Color.White);
            renderTarget.DrawString(font, "Team 3", team3Position, Color.White);
            renderTarget.DrawString(font, "Team 4", team4Position, Color.White);
            renderTarget.DrawString(font, "Press A (Controller) or Space (Keyboard) to Lock-in", footerPosition, Color.White);

            renderTarget.Draw(tableTexture, tableBoxTop, Color.White);
            renderTarget.Draw(tableTexture, tableBoxLeft, Color.White);
            renderTarget.Draw(tableTexture, tableBoxBottom, Color.White);
            renderTarget.Draw(tableTexture, tableBoxRight, Color.White);
            renderTarget.Draw(tableTexture, tableHeaderLine, Color.White);
            renderTarget.Draw(tableTexture, tablePlayer12Line, Color.White);
            renderTarget.Draw(tableTexture, tablePlayer23Line, Color.White);
            renderTarget.Draw(tableTexture, tablePlayer34Line, Color.White);

            int columnWidth = (int)(playerIconTextures[0].Width > font.MeasureString("Team 4").X ? playerIconTextures[0].Width : font.MeasureString("Team 4").X);

            for (int i = 0; i < PlayerManager.ActivePlayers.Count; i++) {
                renderTarget.Draw(playerIconTextures[i], new Rectangle(tableHeaderLine.X + 1 + (PlayerManager.ActivePlayers[i].teamID * columnWidth),
                                                                 tableHeaderLine.Y + (i * playerIconTextures[i].Height) + 1, playerIconTextures[i].Width,
                                                                 playerIconTextures[i].Height), Color.White);
            }

            renderTarget.End();

            return false;
        }

        public bool OnRemove() {
            Game.players.Deactivate();
            renderTarget.Dispose();
            return true; //All cleaned up
        }
    }
}
