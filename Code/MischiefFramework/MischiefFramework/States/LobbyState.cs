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
        private Texture2D bg;

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

        private Texture2D playerIconTextures;
        private Vector2 PLAYER_ICON_SIZES = new Vector2(64, 64);

        public LobbyState() {
            renderTarget = new SpriteBatch(Game.device);
            font = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont");

            tableTexture.SetData<Color>(new Color[] { Color.White });

            playerIconTextures = ResourceManager.LoadAsset<Texture2D>("HUD/Player Markers");

            bg = ResourceManager.LoadAsset<Texture2D>("HUD/Team Select and instructions"); 

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
                    AudioController.RemoveAllLoops();
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

            int columnWidth = (int)(team1StringSize.X > PLAYER_ICON_SIZES.X ? team1StringSize.X : PLAYER_ICON_SIZES.X);

            tableBoxTop.Width = columnWidth * 4;
            tableBoxBottom.Width = tableBoxTop.Width;
            tableBoxLeft.Height = (int)(team1StringSize.Y + PLAYER_ICON_SIZES.Y * PlayerManager.ActivePlayers.Count);
            tableBoxRight.Height = tableBoxLeft.Height;
            tableHeaderLine.Width = tableBoxTop.Width;
            tablePlayer12Line.Height = tableBoxLeft.Height;
            tablePlayer23Line.Height = tableBoxLeft.Height;
            tablePlayer34Line.Height = tableBoxLeft.Height;

            Vector2 screenSize = Vector2.Zero;
            screenSize.X = tableBoxTop.Width > titleStringSize.X ? tableBoxTop.Width : titleStringSize.X;
            screenSize.Y = titleStringSize.Y + tableBoxLeft.Height + footerStringSize.Y;

            titlePosition.X = 910;
            titlePosition.Y = 50;

            tableBoxTop.X = 850;
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

            footerPosition.X = 910;
            footerPosition.Y = tableBoxBottom.Y;


            return false;
        }

        public bool Render(GameTime gameTime) {
            renderTarget.Begin();

            renderTarget.Draw(bg, renderTarget.GraphicsDevice.Viewport.Bounds, bg.Bounds, Color.White);

            renderTarget.DrawString(font, "Press Start to Join", titlePosition, Color.White);
            renderTarget.DrawString(font, "Team 1", team1Position, Color.White);
            renderTarget.DrawString(font, "Team 2", team2Position, Color.White);
            renderTarget.DrawString(font, "Team 3", team3Position, Color.White);
            renderTarget.DrawString(font, "Team 4", team4Position, Color.White);
            renderTarget.DrawString(font, "Press A to Lock-in", footerPosition, Color.White);

            renderTarget.Draw(tableTexture, tableBoxTop, Color.White);
            renderTarget.Draw(tableTexture, tableBoxLeft, Color.White);
            renderTarget.Draw(tableTexture, tableBoxBottom, Color.White);
            renderTarget.Draw(tableTexture, tableBoxRight, Color.White);
            renderTarget.Draw(tableTexture, tableHeaderLine, Color.White);
            renderTarget.Draw(tableTexture, tablePlayer12Line, Color.White);
            renderTarget.Draw(tableTexture, tablePlayer23Line, Color.White);
            renderTarget.Draw(tableTexture, tablePlayer34Line, Color.White);

            int columnWidth = (int)(PLAYER_ICON_SIZES.X > font.MeasureString("Team 4").X ? PLAYER_ICON_SIZES.X : font.MeasureString("Team 4").X);

            for (int i = 0; i < PlayerManager.ActivePlayers.Count; i++) {
                Rectangle src = new Rectangle(i * (int)PLAYER_ICON_SIZES.X, 0, (int)PLAYER_ICON_SIZES.X, (int)PLAYER_ICON_SIZES.Y);
                Rectangle dst = new Rectangle(tableHeaderLine.X + 1 + (PlayerManager.ActivePlayers[i].teamID * columnWidth),
                                              tableHeaderLine.Y + (i * (int)PLAYER_ICON_SIZES.Y) + 1, (int)PLAYER_ICON_SIZES.X,
                                              (int)PLAYER_ICON_SIZES.Y);
                renderTarget.Draw(playerIconTextures, dst, src, Color.White);
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
