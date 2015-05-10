using Microsoft.Xna.Framework;

namespace OctreeDemo
{
    public class Camera
    {
        protected Vector3 Position;
        protected Vector3 Target;
        protected Vector3 Up;

        public void CreateProjectionMatrix(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public void CreateViewMatrix(Vector3 position, Vector3 target, Vector3 up)
        {
            Position = position;
            Target = target;
            Up = up;

            ViewMatrix = Matrix.CreateLookAt(position, target, up);
        }

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }
    }
}
