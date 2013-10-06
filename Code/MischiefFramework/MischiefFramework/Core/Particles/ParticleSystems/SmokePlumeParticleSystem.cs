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
    class SmokePlumeParticleSystem : ParticleSystem
    {
        public SmokePlumeParticleSystem() { Initialize(); }


        protected override void InitializeSettings(ParticleSettings settings) {
            settings.TextureName = "smoke";

            settings.MaxParticles = 6000;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 1;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(0, -0.5f, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 0.5f;
            settings.MaxStartSize = 1.0f;

            settings.MinEndSize = 1.5f;
            settings.MaxEndSize = 2.0f;
        }
    }
}
