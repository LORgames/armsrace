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
    class MonsterEyeParticleSystem : ParticleSystem
    {
        public MonsterEyeParticleSystem() { Initialize(); }


        protected override void InitializeSettings(ParticleSettings settings) {
            settings.TextureName = "densesmoke";

            settings.MaxParticles = 2500;

            settings.Duration = TimeSpan.FromSeconds(0.5);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0.0f;
            settings.MaxHorizontalVelocity = 0.5f;

            settings.MinVerticalVelocity = -1.0f;
            settings.MaxVerticalVelocity = 0.75f;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 1, 0);

            settings.MinColor = new Color(100, 0, 0, 100);
            settings.MaxColor = new Color(200, 0, 0, 150);

            settings.MinStartSize = 0.1f;
            settings.MaxStartSize = 0.2f;

            settings.MinEndSize = 0.1f;
            settings.MaxEndSize = 0.4f;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
