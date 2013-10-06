using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Particles;
using MischiefFramework.Cache;
using MischiefFramework.WorldX.Information;
using MischiefFramework.Core;
using Microsoft.Xna.Framework;
using MischiefFramework.WorldX.Assets;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;

namespace MischiefFramework.WorldX.Containers {
    internal class PhaseTransitionora : IOpaque {
        private static PhaseTransitionora _me = null;
        public static PhaseTransitionora GET() {
            if (_me == null) _me = new PhaseTransitionora();
            return _me;
        }

        private bool _cleanedUp = false;
        private bool _createdMechs = false;
        private bool _soundPlayed = false;

        private Model crateModel;
        public List<Vector3> CratePositions = new List<Vector3>();
        public List<Vector3> CrateDestinations = new List<Vector3>();
        public List<Vector3> CrateRealPosition = new List<Vector3>();
        public List<float> CrateRotations = new List<float>();
        public float LERP_TIME = 3.0f;

        public PhaseTransitionora() {
            Renderer.Add(this);

            crateModel = ResourceManager.LoadAsset<Model>("Meshes/TestObjects/Crate");
            MeshHelper.ChangeEffectUsedByModel(crateModel, Renderer.Effect3D);
        }

        public void Dispose() {
            _me = null;
            Renderer.Remove(this);
        }

        public void Update(float dt, float remainingTime, WorldController w) {
            if (!_cleanedUp) {
                // clean up crates
                foreach (WeaponCrate crate in w.Crates) {
                    Vector2 pos2 = crate.body.Position;
                    Vector3 pos3 = new Vector3(pos2.X, 0, pos2.Y);

                    if(crate.inBase) {
                        foreach(GamePlayer plr in PlayerManager.ActivePlayers) {
                            if (SettingManager._baseSharing == 1) {
                                if (plr.teamID == crate.baseIn.teamID) {
                                    CratePositions.Add(pos3);
                                    CrateRealPosition.Add(pos3);
                                    CrateDestinations.Add(new Vector3(plr.character.body.Position.X, 0, plr.character.body.Position.Y));
                                    CrateRotations.Add(plr.character.body.Rotation);
                                } else if (plr.baseID == crate.baseIn.baseID) {
                                    CratePositions.Add(pos3);
                                    CrateRealPosition.Add(pos3);
                                    CrateDestinations.Add(new Vector3(plr.character.body.Position.X, 0, plr.character.body.Position.Y));
                                    CrateRotations.Add(plr.character.body.Rotation);
                                }
                            }
                        }
                    }

                    crate.Dispose();
                }
                w.Crates = new WeaponCrate[] { };

                _cleanedUp = true;
            }

            LERP_TIME -= dt;
            int i = CratePositions.Count;
            while (--i > -1) {
                CrateRealPosition[i] = Vector3.Lerp(CrateDestinations[i], CratePositions[i], LERP_TIME / 3.0f);
            }

            if (remainingTime > 2) {
                if (!_soundPlayed) {
                    AudioController.PlayOnce("Mech_Sound");
                    _soundPlayed = true;
                }
                foreach (GamePlayer plr in PlayerManager.ActivePlayers) {
                    Renderer.PuffyWhiteSmoke.AddParticle(new Vector3(plr.character.body.Position.X, 0, plr.character.body.Position.Y), new Vector3((float)Game.random.NextDouble() / 2, 2, (float)Game.random.NextDouble() / 2));
                    Renderer.PuffyWhiteSmoke.AddParticle(new Vector3(plr.character.body.Position.X, 0, plr.character.body.Position.Y), new Vector3((float)Game.random.NextDouble() / 2, 2, (float)Game.random.NextDouble() / 2));
                    Renderer.PuffyWhiteSmoke.AddParticle(new Vector3(plr.character.body.Position.X, 0, plr.character.body.Position.Y), new Vector3((float)Game.random.NextDouble() / 2, 2, (float)Game.random.NextDouble() / 2));
                }
            } else {
                if (!_createdMechs) {
                    foreach (GamePlayer player in PlayerManager.ActivePlayers) {
                        Vector2 pos = player.character.GetPosition();
                        AssetManager.RemoveAsset(player.character);
                        bool hasMG = true;
                        bool hasLaser = player.score > 0;
                        bool hasCannon = player.score > 1;
                        bool hasRocket = player.score > 2;
                        player.character = new TankCharacter(player, w.world, pos, hasMG, hasLaser, hasCannon, hasRocket);
                    }
                    _createdMechs = true;
                }
            }
        }

        public void RenderOpaque() {
            if (LERP_TIME > 0) {
                int i = CratePositions.Count;
                while (--i > -1) {
                    MeshHelper.DrawModel(Matrix.CreateScale(0.5f) * Matrix.CreateRotationY(CrateRotations[i]) * Matrix.CreateTranslation(CrateRealPosition[i]), crateModel);
                }
            }
        }
    }
}
