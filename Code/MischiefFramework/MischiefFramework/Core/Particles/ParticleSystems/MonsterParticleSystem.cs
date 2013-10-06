#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace MischiefFramework.Core.Particles
{
    /// <summary>
    /// Custom particle system for creating a giant plume of long lasting smoke.
    /// </summary>
    class MonsterParticleSystem : ParticleSystem
    {
        private int MaxParticles;
        private Vector3 Gravity;
        private Color MinColor;
        private Color MaxColor;
        private float MinStartSize;
        private float MaxStartSize;
        private float MinEndSize;
        private float MaxEndSize;

        public MonsterParticleSystem(   int MaxParticles,
                                        Vector3 Gravity,
                                        Color MinColor,
                                        Color MaxColor,
                                        float MinStartSize,
                                        float MaxStartSize,
                                        float MinEndSize,
                                        float MaxEndSize)
        {
            this.MaxParticles = MaxParticles;
            this.Gravity = Gravity;
            this.MinColor = MinColor;
            this.MaxColor = MaxColor;
            this.MinStartSize = MinStartSize;
            this.MaxStartSize = MaxStartSize;
            this.MinEndSize = MinEndSize;
            this.MaxEndSize = MaxEndSize;
            Initialize();
        }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "monstersmoke";

            settings.MaxParticles = MaxParticles;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0.0f;
            settings.MaxHorizontalVelocity = 0.5f;

            settings.MinVerticalVelocity = -1.0f;
            settings.MaxVerticalVelocity = 0.75f;

            settings.Gravity = Gravity;

            settings.MinColor = MinColor;
            settings.MaxColor = MaxColor;

            settings.MinStartSize = MinStartSize;
            settings.MaxStartSize = MaxStartSize;

            settings.MinEndSize = MinEndSize;
            settings.MaxEndSize = MaxEndSize;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
