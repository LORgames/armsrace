﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;
using MischiefFramework.Core;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.States {
    class CreditsState : IState {
        private struct CreditStruct {
            public string line;
            public bool isHeader;
            public Vector2 position;
            public bool isShortListed;

            public CreditStruct(int arg) {
                line = "";
                isHeader = false;
                position = Vector2.Zero;
                isShortListed = false;
            }
        }

        //Drawables :)
        private SpriteBatch spritebatch;
        private SpriteFont headerFont;
        private SpriteFont defaultFont;
        private float backgroundTiming = 5.0f;
        private float currentTime = 0.0f;
        private int currentBackground = 0;
        private List<Texture2D> backgrounds = new List<Texture2D>();
        private List<string> backgroundStrings = new List<string>();
        private List<CreditStruct> creditsList = new List<CreditStruct>();
        private bool getStartHeld = true;
        private bool rollingLong = true;
        private float longOffset = 0.0f;
        private float shortOffset = 0.0f;

        // Strings
        private const string CREDITS_SCREEN_SHORT = "Art\n\tNathan \"Sleep it Off\" Perry\n\tRyan \"Cigar Lover\" Furlong\n\tYing \"Make it Cute\" Luo\n\nProgramming\n\tMiles \"AFK\" Till\n\tPaul \"Magic Numbers\" Fox\n\tSamuel \"Bug Smasher\" Surtees\n\nPowered By\n\tFarseer Physics\n\tXNA Game Studio\n\nSanity Restoration\n\tCOFFEE!!!!!!!!\n\tSubway (Thanks Katie XD)";
        private const string CREDITS_SCREEN_LONG = "Music\n\t\tSONG NAME\n\t\tBy PERSON from URL";

        public CreditsState(GraphicsDevice device) {
            spritebatch = new SpriteBatch(device);
            headerFont = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont24");
            defaultFont = ResourceManager.LoadAsset<SpriteFont>("Fonts/MenuFont");

            //Add Background images
            //backgrounds.Add(ResourceManager.LoadAsset<Texture2D>("Menus/Credits/Old_Build_1"));

            //Add background text
            //backgroundStrings.Add("Old School-Admiring Grass");

            string credits = "";
            for (int j = 0; j < 2; j++) {
                if (j == 0) {
                    credits = CREDITS_SCREEN_SHORT;
                } else {
                    credits = CREDITS_SCREEN_LONG;
                }
                credits = credits.Replace("\\n", "\n");
                credits = credits.Replace("\\t", "\t");
                CreditStruct temp = new CreditStruct();
                temp.isHeader = true;
                temp.position.Y = Game.device.Viewport.Height;
                if (j == 0)
                    temp.isShortListed = true;
                bool newSection = true;
                for (int i = 0; i < credits.Length; i++) {
                    if (credits[i] != '\n' && credits[i] != '\t') {
                        temp.line += credits[i];
                    } else {
                        SetPositionOfCredit(ref temp, newSection);
                        newSection = false;
                        creditsList.Add(temp);
                        temp = new CreditStruct();
                        if (j == 0)
                            temp.isShortListed = true;
                        if (credits[i] == '\n' && credits[i + 1] == '\n' && credits[i + 2] == '\t') {
                            newSection = true;
                            i++;
                            i++;
                        } else if (credits[i] == '\n' && credits[i + 1] == '\n') {
                            temp.isHeader = true;
                            newSection = true;
                            i++;
                        } else if (credits[i] == '\n' && credits[i + 1] == '\t' && credits[i + 2] == '\t') {
                            i++;
                            i++;
                        } else if (credits[i] == '\n' && credits[i + 1] == '\t') {
                            i++;
                        } else if (credits[i] == '\n') {
                            temp.isHeader = true;
                        }
                    }
                }
                SetPositionOfCredit(ref temp, newSection);
                creditsList.Add(temp);
                if (j == 0) {
                    float shortHeight = (creditsList[creditsList.Count - 1].position.Y + defaultFont.MeasureString(creditsList[creditsList.Count - 1].line).Y) - creditsList[0].position.Y;
                    float shortMiddle = (Game.device.Viewport.Height - shortHeight) / 2;
                    shortOffset = Game.device.Viewport.Height - shortMiddle;
                }
            }
        }

        private void SetPositionOfCredit(ref CreditStruct credit, bool newSection) {
            if (credit.isHeader) {
                credit.position.X = (Game.device.Viewport.Width - headerFont.MeasureString(credit.line).X) / 2;
            } else {
                credit.position.X = (Game.device.Viewport.Width - defaultFont.MeasureString(credit.line).X) / 2;
            }
            if (creditsList.Count != 0) {
                if (credit.isHeader) {
                    if (newSection)
                        credit.position.Y = creditsList[creditsList.Count - 1].position.Y + headerFont.MeasureString(credit.line).Y * 2;
                    else
                        credit.position.Y = creditsList[creditsList.Count - 1].position.Y + headerFont.MeasureString(credit.line).Y;
                } else {
                    if (creditsList[creditsList.Count - 1].isHeader)
                        credit.position.Y = creditsList[creditsList.Count - 1].position.Y + headerFont.MeasureString(credit.line).Y;
                    else if (newSection)
                        credit.position.Y = creditsList[creditsList.Count - 1].position.Y + defaultFont.MeasureString(credit.line).Y * 2;
                    else
                        credit.position.Y = creditsList[creditsList.Count - 1].position.Y + defaultFont.MeasureString(credit.line).Y;
                }
            }
        }

        public bool Update(GameTime gameTime) {
            foreach (GamePlayer player in PlayerManager.ActivePlayers) {
                if (player == null) continue;

                if (!player.Input.GetAny())
                    getStartHeld = false;

                if (player.Input.GetY() < -0.5f) {
                    longOffset += 4.0f;
                } else if (player.Input.GetY() > 0.5f) {
                    longOffset -= 2.0f;
                } else {
                    longOffset += 2.0f;
                }

                if (player.Input.GetAny() && !getStartHeld) {
                    if (rollingLong) {
                        rollingLong = false;
                    } else {
                        StateManager.Pop();
                        getStartHeld = true;
                        rollingLong = true;
                        longOffset = 0.0f;
                    }
                    getStartHeld = true;
                }
            }
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime >= backgroundTiming) {
                currentTime = 0;
                currentBackground++;
                if (currentBackground >= backgrounds.Count)
                    currentBackground = 0;
            }
            return false;
        }

        public bool Render(GameTime gameTime) {
            CreditStruct finalCredit = creditsList[creditsList.Count - 1];
            if (finalCredit.isHeader) {
                if (Game.device.Viewport.Y > (finalCredit.position.Y + headerFont.MeasureString(finalCredit.line).Y - longOffset)) {
                    rollingLong = false;
                }
            } else {
                if (Game.device.Viewport.Y > (finalCredit.position.Y + defaultFont.MeasureString(finalCredit.line).Y - longOffset)) {
                    rollingLong = false;
                }
            }


            spritebatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            if (backgrounds.Count > 0) {
                Vector2 backgroundPos = new Vector2((Game.device.Viewport.TitleSafeArea.Width - backgrounds[currentBackground].Bounds.Width) / 2, 0);
                float alpha = currentTime / backgroundTiming;
                if (alpha < 0.5)
                    alpha *= 2;
                else
                    alpha = 1 - (alpha - 0.5f) * 2f;
                Color color = new Color(255, 255, 255, (byte)(alpha * 255));
                spritebatch.Draw(backgrounds[currentBackground], backgroundPos, color);
                string[] strings = backgroundStrings[currentBackground].Split('-');
                Vector2 stringPos = Vector2.Zero;
                spritebatch.DrawString(defaultFont, strings[0], stringPos, color);
                stringPos.Y += defaultFont.MeasureString(strings[0]).Y;
                for (int i = 0; i < strings[1].Length; i++) {
                    spritebatch.DrawString(defaultFont, strings[1][i].ToString(), stringPos, color);
                    stringPos.Y += defaultFont.MeasureString(strings[1][i].ToString()).Y - 15;
                }
            }

            if (rollingLong) {
                Vector2 temp = Vector2.Zero;
                foreach (CreditStruct credit in creditsList) {
                    temp.X = credit.position.X;
                    temp.Y = credit.position.Y - longOffset;
                    if (credit.isHeader)
                        spritebatch.DrawString(headerFont, credit.line, temp, Color.White);
                    else
                        spritebatch.DrawString(defaultFont, credit.line, temp, Color.White);
                }
            } else {
                foreach (CreditStruct credit in creditsList) {
                    Vector2 temp = Vector2.Zero;
                    if (credit.isShortListed) {
                        temp.X = credit.position.X;
                        temp.Y = credit.position.Y - shortOffset;
                        if (credit.isHeader)
                            spritebatch.DrawString(headerFont, credit.line, temp, Color.White);
                        else
                            spritebatch.DrawString(defaultFont, credit.line, temp, Color.White);
                    }
                }
            }
            spritebatch.End();
            return false;
        }

        public bool OnRemove() {
            spritebatch.Dispose();

            return true;
        }
    }
}
