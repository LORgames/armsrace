#region File Description
//-----------------------------------------------------------------------------
// FireParticleSystem.cs
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

namespace MischiefFramework.Core.Particles {
    /// <summary>
    /// Custom particle system for creating a flame effect.
    /// </summary>
    class SeedParticleSystem : ParticleSystem {
        public SeedParticleSystem() { Initialize(); }


        protected override void InitializeSettings(ParticleSettings settings) {
            settings.TextureName = "green";

            settings.MaxParticles = 20;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0.0f;
            settings.MaxHorizontalVelocity = 0.5f;

            settings.MinVerticalVelocity = -1.0f;
            settings.MaxVerticalVelocity = 0.50f;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 0.5f, 0);

            settings.MinColor = new Color(100, 100, 100, 50);
            settings.MaxColor = new Color(150, 150, 150, 100);

            settings.MinStartSize = 0.1f;
            settings.MaxStartSize = 0.2f;

            settings.MinEndSize = 0.5f;
            settings.MaxEndSize = 1.0f;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
