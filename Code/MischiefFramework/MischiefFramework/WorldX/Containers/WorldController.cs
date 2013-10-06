using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using MischiefFramework.WorldX.Stage;
using MischiefFramework.WorldX.Map;
using MischiefFramework.WorldX.Assets;
using MischiefFramework.Cache;
using MischiefFramework.Core;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.WorldX.Containers {
    internal class WorldController : IDisposable {
        internal World world;
        internal Level level;

        #if DEBUG_PHYSICS
            internal static DebugRenderer dbg;
        #endif

        private int MAX_CRATES = 10;
        private WeaponCrate[] Crates;

        // phases
        internal enum Phases {
            Phase1Ready,    // Ready countdown before phase 1 gameplay
            Phase1Play,     // Phase 1 gameplay
            Phase1Scores,   // Phase 1 scoreboard & transition to phase 2
            Phase2Ready,    // Phase 2 countdown before phase 2 gameplay
            Phase2Play,     // Phase 2 gameplay
            Phase2Scores    // Phase 2 scoreboard & menu options for rematch or main menu
        }

        internal Phases Phase = Phases.Phase1Ready;

        internal float phase1ReadyTimer = 5.0f; // in secs
        internal float phase1PlayTimer = 30.0f;
        internal float phase1ScoresTimer = 5.0f;
        internal float phase2ReadyTimer = 5.0f;

        internal bool playingPhase1Music = false;
        internal bool playingPhase2Music = false;

        public WorldController() {
            world = new FarseerPhysics.Dynamics.World(Vector2.Zero);

            #if DEBUG_PHYSICS
                dbg = new DebugRenderer(world);
            #endif

            level = new Level(world);

            new Sun();
            new FootEffects();

            foreach (GamePlayer plr in PlayerManager.ActivePlayers) {
                plr.character = new BlobCharacter(plr, world, Level.bases[plr.baseID].CenterPoint);
                //plr.character = new TankCharacter(plr, world, hasCannon:true);
            }

            Vector2 pos = Vector2.Zero;
            Crates = new WeaponCrate[MAX_CRATES];
            for (int i = 0; i < MAX_CRATES; i++) {
                if (SettingManager._spawnType == 0) { //Random
                    pos.X = Game.random.Next(16) - 8;
                    pos.Y = Game.random.Next(16) - 8;
                } else { //Center
                    pos.X = 4 * (float)Math.Cos(Math.PI * 2 / MAX_CRATES * i);
                    pos.Y = 4 * (float)Math.Sin(Math.PI * 2 / MAX_CRATES * i);
                }

                Crates[i] = new WeaponCrate(world, pos);
            }
        }

        public void Update(float dt) {
            //Do nothing?
            world.Step(dt);

            Renderer.CharacterCamera.MakeDoCameraAngleGood();

            float CAMERA_ZOOM = 60.0f;
            //Renderer.CharacterCamera.LookAt = Vector3.Zero;
            Renderer.CharacterCamera.Position.X = CAMERA_ZOOM * 0.612f + Renderer.CharacterCamera.LookAt.X;
            Renderer.CharacterCamera.Position.Y = CAMERA_ZOOM * 0.500f + Renderer.CharacterCamera.LookAt.Y;
            Renderer.CharacterCamera.Position.Z = CAMERA_ZOOM * -0.612f + Renderer.CharacterCamera.LookAt.Z;
            Renderer.CharacterCamera.GenerateMatrices();

            switch (Phase) {
                case Phases.Phase1Ready:
                    if (!playingPhase1Music) {
                        AudioController.PlayLooped("Phase1", 1.0f);
                        playingPhase1Music = true;
                    }
                    // Lock player movement etc. for countdown
                    LockAllControls(true);

                    // countdown timer
                    if (phase1ReadyTimer > 0.0f) {
                        InfoPanel.instance.SetTimer(Phase, phase1ReadyTimer);
                        phase1ReadyTimer -= dt;
                    } else {
                        Phase = Phases.Phase1Play;
                        InfoPanel.instance.SetTimer(Phase, phase1PlayTimer);
                        LockAllControls(false);
                    }
                    break;

                case Phases.Phase1Play:
                    // gameplay timer
                    if (phase1PlayTimer > 0.0f) {
                        InfoPanel.instance.SetTimer(Phase, phase1PlayTimer);
                        phase1PlayTimer -= dt;
                    } else {
                        Phase = Phases.Phase1Scores;
                        InfoPanel.instance.SetTimer(Phase, 0.0f);

                        // set player scores
                        if (Crates.Length > 0) {
                            foreach (BaseArea baseArea in Level.bases) {
                                if (SettingManager._baseSharing == 0) {
                                    PlayerManager.ActivePlayers[baseArea.baseID].score = baseArea.crates;
                                } else {
                                    foreach (GamePlayer player in PlayerManager.ActivePlayers) {
                                        if (player.teamID == baseArea.teamID) {
                                            player.score = baseArea.crates;
                                        }
                                    }
                                }
                            }

                            // clean up crates
                            foreach (WeaponCrate crate in Crates) {
                                crate.Dispose();
                            }
                            Crates = new WeaponCrate[] { };
                        }
                    }
                    break;

                case Phases.Phase1Scores:
                    // Lock player movement etc. for scoreboard
                    LockAllControls(true);

                    // TODO: Show animation/transition to phase 2
                    if (phase1ScoresTimer > 0.0f) {
                        InfoPanel.instance.SetTimer(Phase, phase1ScoresTimer);
                        phase1ScoresTimer -= dt;
                    } else {
                        Phase = Phases.Phase1Play;
                        InfoPanel.instance.SetTimer(Phase, phase2ReadyTimer);
                        LockAllControls(false);
                    }

                    // TODO: Accept input to move to phase 2 ready state
                    break;

                case Phases.Phase2Ready:
                    if (!playingPhase2Music) {
                        AudioController.RemoveAllLoops();
                        AudioController.PlayLooped("Phase2", 1.0f);
                        playingPhase2Music = true;
                    }
                    // clean up bases
                    if (Level.bases.Count > 0) {
                        foreach (BaseArea baseArea in Level.bases) {
                            AssetManager.RemoveAsset(baseArea);
                        }
                        Level.bases = new List<BaseArea>();
                    }

                    // TODO: Lock player movement etc. for countdown

                    // countdown timer
                    if (phase2ReadyTimer > 0.0f) {
                        InfoPanel.instance.SetTimer(Phase, phase2ReadyTimer);
                        phase1ReadyTimer -= dt;
                    } else {
                        Phase = Phases.Phase2Play;
                        InfoPanel.instance.SetTimer(Phase, 0.0f);
                    }
                    break;

                case Phases.Phase2Play:
                    // TODO: this?
                    Phase = Phases.Phase2Play;
                    InfoPanel.instance.SetTimer(Phase, 0.0f);
                    break;

                case Phases.Phase2Scores:
                    // TODO: this?
                    break;
            }
        }

        private void LockAllControls(bool locked) {
            foreach (GamePlayer player in PlayerManager.ActivePlayers) {
                (player.character as BlobCharacter).LockControls(locked);
            }
        }

        public void Dispose() {
            world.Clear();
            world = null;
        }
    }
}
