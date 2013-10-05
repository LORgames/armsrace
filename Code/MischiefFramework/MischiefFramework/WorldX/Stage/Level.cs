using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core.Interfaces;
using MischiefFramework.Core;
using MischiefFramework.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MischiefFramework.Cache;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using MischiefFramework.WorldX.Assets;

namespace MischiefFramework.WorldX.Stage {
    internal class Level : IOpaque, ILight, ITransparent {
        private Model visuals;
        private Model lights;

        private Effect fx;
        private Texture tex;

        private int TOTAL_WALLS = 8; //Convex stuff..?
        private Body[] walls;

        internal static List<BaseArea> bases;

        public Level(World world) {
            bases = new List<BaseArea>();

            tex = ResourceManager.LoadAsset<Texture2D>("Meshes/Levels/overlay");

            fx = ResourceManager.LoadAsset<Effect>("Shaders/GroundShader");

            visuals = ResourceManager.LoadAsset<Model>("Meshes/Levels/Level");
            MeshHelper.ChangeEffectUsedByModel(visuals, fx, false);
            //MeshHelper.ChangeEffectUsedByModel(visuals, Renderer.Effect3D);

            lights = ResourceManager.LoadAsset<Model>("Meshes/Levels/lights");
            MeshHelper.ChangeEffectUsedByModel(lights, Renderer.EffectTransparent);

            Matrix position = Matrix.Identity;

            Camera c = new Camera(37, 37);
            c.LookAt = Vector3.Zero;
            c.Position.X = 6 * 0.612f + c.LookAt.X;
            c.Position.Y = 6 * 0.500f + c.LookAt.Y;
            c.Position.Z = 6 * -0.612f + c.LookAt.Z;
            c.GenerateMatrices();

            fx.Parameters["Texture"].SetValue(tex);
            fx.Parameters["TextureEnabled"].SetValue(true);
            fx.Parameters["CameraViewProjection"].SetValue(c.ViewProjection);
            
            //Outside walls
            BodyFactory.CreateRectangle(world, 9, 3, 0, new Vector2(8, 12.5f));
            BodyFactory.CreateRectangle(world, 9, 3, 0, new Vector2(8, -12.5f));
            BodyFactory.CreateRectangle(world, 9, 3, 0, new Vector2(-8, 12.5f));
            BodyFactory.CreateRectangle(world, 9, 3, 0, new Vector2(-8, -12.5f));
            BodyFactory.CreateRectangle(world, 3, 9, 0, new Vector2(12.5f, 8));
            BodyFactory.CreateRectangle(world, 3, 9, 0, new Vector2(-12.5f, 8));
            BodyFactory.CreateRectangle(world, 3, 9, 0, new Vector2(12.5f, -8));
            BodyFactory.CreateRectangle(world, 3, 9, 0, new Vector2(-12.5f, -8));

            //Base backs
            BodyFactory.CreateRectangle(world, 8, 2, 0, new Vector2(0, 15));
            BodyFactory.CreateRectangle(world, 8, 2, 0, new Vector2(0,-15));
            BodyFactory.CreateRectangle(world, 2, 8, 0, new Vector2( 15,0));
            BodyFactory.CreateRectangle(world, 2, 8, 0, new Vector2(-15,0));

            //Center piece...
            BodyFactory.CreateCircle(world, 1.6f, 0);
            
            //Inner walls
            walls = new Body[TOTAL_WALLS];
            Vertices verts0 = new Vertices();
            verts0.Add(new Vector2( 0.0f,-0.4f));
            verts0.Add(new Vector2( 1.6f,-0.6f));
            verts0.Add(new Vector2( 1.6f, 0.4f));
            verts0.Add(new Vector2( 0.0f, 0.6f));

            Vertices verts1 = new Vertices();
            verts1.Add(new Vector2( 0.0f,-0.4f));
            verts1.Add(new Vector2( 0.0f, 0.6f));
            verts1.Add(new Vector2(-1.6f, 0.4f));
            verts1.Add(new Vector2(-1.6f,-0.6f));
            
            for (int i = 0; i < 4; i++) {
                if (i < PlayerManager.ActivePlayers.Count) {
                    // Creates a base in one of two situations:
                    // - Base sharing is turned off
                    // - Base sharing is turned on and the team doesn't already have a base.
                    if (SettingManager._baseSharing == 0 || (SettingManager._baseSharing == 1 && !PlayerManager.ActiveTeams.Contains(PlayerManager.ActivePlayers[i].teamID))) {
                        bases.Add(new BaseArea(PlayerManager.ActiveTeams.Count, world, PlayerManager.ActivePlayers[i].teamID));
                        PlayerManager.ActivePlayers[i].baseID = bases.Count-1;
                        PlayerManager.ActiveTeams.Add(PlayerManager.ActivePlayers[i].teamID);
                    }
                }

                walls[i * 2 + 0] = BodyFactory.CreatePolygon(world, verts0, 0.0f);
                walls[i * 2 + 1] = BodyFactory.CreatePolygon(world, verts1, 0.0f);

                walls[i * 2 + 0].Position = new Vector2((float)Math.Cos(Math.PI / 2 * i) * 9.4f, (float)Math.Sin(Math.PI / 2 * i) * 9.4f);
                walls[i * 2 + 1].Position = walls[i * 2 + 0].Position;

                walls[i * 2 + 0].Rotation = (float)Math.PI / 2 * (i-1);
                walls[i * 2 + 1].Rotation = walls[i * 2 + 0].Rotation;
            }

            Renderer.Add(this);
        }

        public void RenderOpaque() {
            MeshHelper.DrawModel(Matrix.Identity, visuals);
        }

        public void RenderLight() {
            for (int i = 0; i < 4; i++) {
                Renderer.DrawPointLight(13 * (Vector3.Left * (float)Math.Cos(i * Math.PI / 2) + Vector3.Forward * (float)Math.Sin(i * Math.PI / 2)) + Vector3.Up * 0.2f, Color.Green, 5, 10);
            }

            //Nathans copy and pasted lights
            Color c = Color.Green;
            Renderer.DrawPointLight(new Vector3(-0.029f, 1.664f, 0.066f), c, 2.074f, 10);
            Renderer.DrawPointLight(new Vector3(12.8f, -4.325f, 8.956f), c, 1.505f, 10);
            Renderer.DrawPointLight(new Vector3(13.589f, 0.096f, 6.679f), c, 1.505f, 10);
            Renderer.DrawPointLight(new Vector3(16.119f, -3.956f, -1.533f), c, 1.820f, 10);
            Renderer.DrawPointLight(new Vector3(13.598f, -1.994f, -6.719f), c, 3.573f, 10);
            Renderer.DrawPointLight(new Vector3(12.504f, -3.325f, -9.769f), c, 1.439f, 10);
            Renderer.DrawPointLight(new Vector3(11.649f, -5.679f, -12.698f), c, 0.402f, 10);
            Renderer.DrawPointLight(new Vector3(9.699f, -5.301f, -12.581f), c, 2.176f, 10);
            Renderer.DrawPointLight(new Vector3(6.362f, -4.547f, -14.689f), c, 1.467f, 10);
            Renderer.DrawPointLight(new Vector3(3.086f, -2.248f, -16.328f), c, 0.487f, 10);
            Renderer.DrawPointLight(new Vector3(-5.279f, -0.688f, -14.735f), c, 0.487f, 10);
            Renderer.DrawPointLight(new Vector3(-7.239f, -0.688f, -14.735f), c, 0.487f, 10);
            Renderer.DrawPointLight(new Vector3(-9.108f, -0.688f, -14.735f), c, 0.487f, 10);
            Renderer.DrawPointLight(new Vector3(-11.739f, 0.997f, -5.304f), c, 0.781f, 10);
            Renderer.DrawPointLight(new Vector3(-13.145f, 0.997f, -5.304f), c, 0.781f, 10);
            Renderer.DrawPointLight(new Vector3(-14.231f, 2.527f, -3.336f), c, 0.918f, 10);
            Renderer.DrawPointLight(new Vector3(-16.637f, 2.085f, 1.842f), c, 0.865f, 10);
            Renderer.DrawPointLight(new Vector3(-14.187f, 3.422f, 3.214f), c, 0.485f, 10);
            Renderer.DrawPointLight(new Vector3(-11.698f, 0.579f, 4.771f), c, 1.257f, 10);
            Renderer.DrawPointLight(new Vector3(-12.695f, 0.886f, 4.771f), c, 1.231f, 10);
            Renderer.DrawPointLight(new Vector3(-12.363f, 2.991f, 6.130f), c, 0.551f, 10);
            Renderer.DrawPointLight(new Vector3(-13.022f, 2.991f, 6.130f), c, 0.551f, 10);
            Renderer.DrawPointLight(new Vector3(-12, 2.681f, 8.997f), c, 0.668f, 10);
            Renderer.DrawPointLight(new Vector3(-5.977f, 3.644f, 13.661f), c, 2.009f, 10);
            Renderer.DrawPointLight(new Vector3(-7.193f, 5.604f, 17.589f), c, 2.438f, 10);
            Renderer.DrawPointLight(new Vector3(0.233f, 5.689f, 20.280f), c, 1.307f, 10);
        }

        public void RenderTransparent() {
            //MeshHelper.DrawModel(Matrix.Identity, lights);
        }
    }
}
