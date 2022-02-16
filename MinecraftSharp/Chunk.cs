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
        public List<Chunk> neigborChunks;

        public Vector2i position;

        private Vector3i size = new Vector3i(16, 5, 16);

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

            for (int j = 0; j < size.Y; j++)
            {
                for (int k = 0; k < size.Z; k++)
                {
                    for (int i = 0; i < size.X; i++)
                    {
                        cubes[i + k * size.Z + j * (size.X * size.Z)].air = false;
                    }
                }
            }

            foreach (Cube item in cubes)
            {
                item.visibleFaces = CubeHasNeighbor(item.position);
            }
        }

        public Faces CubeHasNeighbor(Vector3i pos)
        {
            Faces ret = Faces.Null;

            int x = pos.X;
            int y = pos.Y;
            int z = pos.Z;
            int s = size.X * size.Y * size.Z;

            if (x - 1 < 0 || x - 1 + z * size.Z + y * size.X * size.Z > 0 && cubes[x - 1 + z * size.Z + y * size.X * size.Z].air) ret |= Faces.Left;
            if (x + 1 >= size.X || x + 1 + z * size.Z + y * size.X * size.Z < s && cubes[x + 1 + z * size.Z + y * size.X * size.Z].air) ret |= Faces.Right;

            if (z - 1 < 0 || x + z - 1 * size.Z + y * size.X * size.Z > 0 && cubes[x + z - 1 * size.Z + y * size.X * size.Z].air) ret |= Faces.Back;
            if (z + 1 >= size.Z || x + z + 1 * size.Z + y * size.X * size.Z < s && cubes[x + z + 1 * size.Z + y * size.X * size.Z].air) ret |= Faces.Front;

            if (y - 1 < 0 || x + z * size.Z + y - 1 * size.X * size.Z > 0 && cubes[x + z * size.Z + y - 1 * size.X * size.Z].air) ret |= Faces.Down;
            if (y + 1 >= size.Y || x + z * size.Z + y + 1 * size.X * size.Z < s && cubes[x + z * size.Z + y + 1 * size.X * size.Z].air) ret |= Faces.Up;

            return ret;
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
