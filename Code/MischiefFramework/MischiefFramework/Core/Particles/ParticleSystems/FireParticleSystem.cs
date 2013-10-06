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
    class FireParticleSystem : ParticleSystem {
        public FireParticleSystem() { Initialize(); }


        protected override void InitializeSettings(ParticleSettings settings) {
            settings.TextureName = "fire";

            settings.MaxParticles = 120;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0.0f;
            settings.MaxHorizontalVelocity = 0.5f;

            settings.MinVerticalVelocity = -1.0f;
            settings.MaxVerticalVelocity = 0.50f;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 2, 0);

            settings.MinColor = new Color(255, 255, 255, 100);
            settings.MaxColor = new Color(255, 255, 255, 150);

            settings.MinStartSize = 0.5f;
            settings.MaxStartSize = 1.0f;

            settings.MinEndSize = 2.0f;
            settings.MaxEndSize = 3.0f;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
