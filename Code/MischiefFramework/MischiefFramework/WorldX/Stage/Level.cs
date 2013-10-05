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
    internal class Level : IOpaque, ILight {
        private Model visuals;
        private Effect fx;
        private Texture tex;

        private int TOTAL_WALLS = 8; //Convex stuff..?
        private Body[] walls;

        private List<BaseArea> bases = new List<BaseArea>();

        public Level(World world) {
            tex = ResourceManager.LoadAsset<Texture2D>("Meshes/Levels/overlay");

            fx = ResourceManager.LoadAsset<Effect>("Shaders/GroundShader");

            visuals = ResourceManager.LoadAsset<Model>("Meshes/Levels/Level");
            MeshHelper.ChangeEffectUsedByModel(visuals, fx, false);
            //MeshHelper.ChangeEffectUsedByModel(visuals, Renderer.Effect3D);

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
                bases.Add(new BaseArea(i, world));

                walls[i * 2 + 0] = BodyFactory.CreatePolygon(world, verts0, 0.0f);
                walls[i * 2 + 1] = BodyFactory.CreatePolygon(world, verts1, 0.0f);

                walls[i * 2 + 0].Position = new Vector2((float)Math.Cos(Math.PI / 2 * i) * (i%2==0?9f:7f), (float)Math.Sin(Math.PI / 2 * i) * 10);
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
        }
    }
}
