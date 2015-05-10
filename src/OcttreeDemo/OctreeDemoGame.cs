using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OctreeDemo
{
    public class OctreeDemoGame : Game
    {
        private const int PropagationThreshold = 2;
        private const int MaxDepth = -1;
        private const int SphereRadius = 40;

        private static readonly Random Random = new Random();
        private static readonly Vector3 TreePosition = new Vector3(0, 0, -3000);
        private static readonly Vector3 TreeDimensions = new Vector3(3000, 1000, 3000);

        private GraphicsDeviceManager graphics;
        private Camera camera;
        private OctreeNode tree;

        private KeyboardState prevState = Keyboard.GetState(PlayerIndex.One);

        public OctreeDemoGame()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            DebugShapeRenderer.Initialize(GraphicsDevice);
      
            CreateCamera();

            tree = OctreeNode.CreateTree(PropagationThreshold, MaxDepth, TreePosition, TreeDimensions);

            base.Initialize();
        }

        private void CreateCamera()
        {
            camera = new Camera();
            camera.CreateViewMatrix(new Vector3(1500, 2500, 1500), new Vector3(1500, 0, -1500), Vector3.Up);
            camera.CreateProjectionMatrix(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 50000f);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState(PlayerIndex.One);

            if(state.IsKeyDown(Keys.Space) && prevState.IsKeyUp(Keys.Space))
            {
                int x = Random.Next((int) TreeDimensions.X - SphereRadius);
                int y = Random.Next((int) TreeDimensions.Y - SphereRadius);
                int z = Random.Next((int) TreePosition.Z + SphereRadius, (int) (TreePosition.Z + TreeDimensions.Z) - SphereRadius);

                tree.Insert(new BoundingSphere(new Vector3(x, y, z), SphereRadius));
            }

            prevState = state;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            DebugShapeRenderer.Draw(gameTime, camera.ViewMatrix, camera.ProjectionMatrix);
        }
    }
}
