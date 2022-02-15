using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubicus
{
    public class Chunk
    {
        private List<Cube> cubes = new List<Cube>();
        private List<Cube> drawingCube = new List<Cube>();
        public List<Chunk> neigborChunks;

        public Vector2i position;

        private Vector3i size = new Vector3i(16, 20, 16);

        public Chunk(int x, int y)
        {
            Shader shader = new Shader(@"Assets\Shaders\Shader.vert", @"Assets\Shaders\Shader.frag");
            position = new Vector2i(x, y);

            for (int j = 0; j < size.Y; j++)
            {
                for (int k = 0; k < size.Z; k++)
                {
                    for (int i = 0; i < size.X; i++)
                    {
                        cubes.Add(new Cube(new Vector3i(i, j, k), shader, @"Assets\Textures\stone.png"));
                    }
                }
            }

            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < size.Z; k++)
                {
                    for (int i = 0; i < size.X; i++)
                    {
                        cubes[i + k * size.Z + j * (size.Y * size.Z)].air = false;
                    }
                }
            }
        }

        public bool CubeHasNeighbor(Vector3i pos)
        {
            // y * size.y + z * size.z + x
            int x = pos.X;
            int y = pos.Y;
            int z = pos.Z;

            if (x - 1 < 0 || x + 2 > size.X || y - 1 < 0 || y + 2 > size.Y || z - 1 < 0 || z + 2 > size.Z)
                return true;

            return false;
        }

        public void Draw(Matrix4 projection, Matrix4 view, Vector3 cameraPos)
        {
            Vector2i pos = new Vector2i(position.X * size.X, position.Y * size.Z);

            foreach (Cube item in cubes)
            {
                item.Draw(projection, view, pos, cameraPos);
            }
        }
    }
}
