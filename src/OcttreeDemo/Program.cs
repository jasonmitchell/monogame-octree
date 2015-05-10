using System;

namespace OctreeDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (OctreeDemoGame game = new OctreeDemoGame())
            {
                game.Run();
            }
        }
    }
}

