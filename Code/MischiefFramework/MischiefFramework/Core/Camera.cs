using Microsoft.Xna.Framework;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.Core {
    internal class Camera {
        internal Matrix View;
        internal Matrix Projection;
        internal Matrix ViewProjection;

        internal Vector3 Position;
        internal Vector3 LookAt;
        internal Vector3 Up;

        internal BoundingFrustum Frustum;

        private float near, far, oldz;

        internal Camera(float w, float h, float near = 0.1f, float far = 180f) {
            this.near = near;
            this.far = far;

            View = Matrix.Identity;
            Projection = Matrix.Identity;
            ViewProjection = Matrix.Identity;

            Position = Vector3.Forward;
            LookAt = Vector3.Zero;
            Up = Vector3.Up;

            //TODO: Store these values rather than hardcode them
            Matrix.CreateOrthographic(w, h, near, far, out Projection);

            Frustum = new BoundingFrustum(Matrix.Identity);

            GenerateMatrices();
        }

        internal void GenerateMatrices() {
            Matrix.CreateLookAt(ref Position, ref LookAt, ref Up, out View);
            Matrix.Multiply(ref View, ref Projection, out ViewProjection);
            Frustum.Matrix = ViewProjection;
        }

        public void MakeDoCameraAngleGood() {
            float totalX = 0.0f, totalY = 0.0f;
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (GamePlayer player in PlayerManager.ActivePlayers) {
                float x = player.character.GetPosition().X;
                float y = player.character.GetPosition().Y;
                totalX += x;
                totalY += y;
                minX = x < minX ? x : minX;
                maxX = x > maxX ? x : maxX;
                minY = y < minY ? y : minY;
                maxY = y > maxY ? y : maxY;
            }

            LookAt.X = MathHelper.Lerp(LookAt.X, (maxX + minX) / 2, 0.5f);
            LookAt.Y = 0;
            LookAt.Z = MathHelper.Lerp(LookAt.Z, (maxY + minY) / 2, 0.5f);

            float r = (float)Game.device.Viewport.Width / (float)Game.device.Viewport.Height;

            float z = (maxX - minX) > (maxY - minY) ? (maxX - minX) : (maxY - minY);

            z += 20;

            z = MathHelper.Lerp(oldz, z, 0.5f);
            oldz = z;

            Matrix.CreateOrthographic(z, (z/r), near, far, out Projection);
        }
    }
}
