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
        public List<Cube> cubes { private set; get; }
        private List<Cube> drawingCubes = new List<Cube>();
        public List<Chunk> neigborChunks = new List<Chunk>();

        private float[] chunkVertex;
        private ArrayObject vao = new ArrayObject();
        private Shader shader;
        private int vertexCount = 0;
        private Texture texture;

        public Vector2i position;

        public Vector3i size = new Vector3i(16, 128, 16);

        public Chunk(int x, int y)
        {
            cubes = new List<Cube>();
            shader = new Shader(@"Assets\Shaders\Shader.vert", @"Assets\Shaders\Shader.frag");
            texture = Texture.LoadFromFile(@"Assets\Textures\texture.png");
            position = new Vector2i(x, y);

            for (int j = 0; j < size.Y; j++)
            {
                for (int k = 0; k < size.Z; k++)
                {
                    for (int i = 0; i < size.X; i++)
                    {
                        cubes.Add(new Cube(new Vector3i(i, j, k), 1));
                        cubes[i + k * size.Z + j * (size.X * size.Z)].air = true;
                    }
                }
            }

            for (int j = 0; j < 32; j++)
            {
                for (int k = 0; k < size.Z; k++)
                {
                    for (int i = 0; i < size.X; i++)
                    {
                        cubes[i + k * size.Z + j * (size.X * size.Z)].air = false;
                    }
                }
            }
        }

        public void SetCubeDrawingFaces()
        {
            foreach (Cube item in cubes)
            {
                if (!item.air)
                {
                    item.visibleFaces = CubeHasNeighbor(item.position);
                    if (item.visibleFaces != Faces.Null) drawingCubes.Add(item);
                }
            }
        }

        public Faces CubeHasNeighbor(Vector3i pos)
        {
            Faces ret = Faces.Null;

            int x = pos.X;
            int y = pos.Y;
            int z = pos.Z;
            int s = size.X * size.Y * size.Z;

            Chunk neighbor = GetNeighborChunk(position.X - 1, position.Y);
            if ((x - 1 < 0 && neighbor == null) || (x - 1 < 0 && neighbor.cubes[x + size.X - 1 + z * size.Z + y * size.X * size.Z].air) ||
                (x - 1 + z * size.Z + y * size.X * size.Z > 0 && cubes[x - 1 + z * size.Z + y * size.X * size.Z].air)) ret |= Faces.Left;

            neighbor = GetNeighborChunk(position.X + 1, position.Y);
            if ((x + 1 >= size.X && neighbor == null) || (x + 1 >= size.X && neighbor.cubes[x - size.X + 1 + z * size.Z + y * size.X * size.Z].air) ||
                (x + 1 + z * size.Z + y * size.X * size.Z < s && cubes[x + 1 + z * size.Z + y * size.X * size.Z].air)) ret |= Faces.Right;

            neighbor = GetNeighborChunk(position.X, position.Y - 1);
            if ((z - 1 < 0 && neighbor == null) || (z - 1 < 0 && neighbor.cubes[x + (z + size.Z - 1) * size.Z + y * size.X * size.Z].air) ||
                (x + (z - 1) * size.Z + y * size.X * size.Z > 0 && cubes[x + (z - 1) * size.Z + y * size.X * size.Z].air)) ret |= Faces.Back;

            neighbor = GetNeighborChunk(position.X, position.Y + 1);
            if ((z + 1 >= size.Z && neighbor == null) || (z + 1 >= size.Z && neighbor.cubes[x + (z - size.Z + 1) * size.Z + y * size.X * size.Z].air) ||
                x + (z + 1) * size.Z + y * size.X * size.Z < s && cubes[x + (z + 1) * size.Z + y * size.X * size.Z].air) ret |= Faces.Front;

            if (y - 1 < 0 || x + z * size.Z + (y - 1) * size.X * size.Z > 0 && cubes[x + z * size.Z + (y - 1) * size.X * size.Z].air) ret |= Faces.Down;
            if (y + 1 >= size.Y || x + z * size.Z + (y + 1) * size.X * size.Z < s && cubes[x + z * size.Z + (y + 1) * size.X * size.Z].air) ret |= Faces.Up;


            return ret;
        }

        private Chunk GetNeighborChunk(int x, int y)
        {
            foreach (Chunk item in neigborChunks)
                if (item.position.X == x && item.position.Y == y)
                    return item;

            return null;
        }

        public void UpdateChunkData()
        {
            List<float[]> data = new List<float[]>();

            vertexCount = 0;

            foreach (Cube item in drawingCubes)
            {
                if (item.visibleFaces.HasFlag(Faces.Front))
                    data.Add(addVertex(Faces.Front, item.frontVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Back))
                    data.Add(addVertex(Faces.Back, item.backVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Left))
                    data.Add(addVertex(Faces.Left, item.leftVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Right))
                    data.Add(addVertex(Faces.Right, item.rightVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Up))
                    data.Add(addVertex(Faces.Up, item.upVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Down))
                    data.Add(addVertex(Faces.Down, item.downVertex, item.position));
            }

            List<float> vertex = new List<float>();

            foreach (float[] item in data)
            {
                foreach (float i in item)
                    vertex.Add(i);
            }

            chunkVertex = vertex.ToArray();

            CreateVAO();
        }

        private void CreateVAO()
        { 
            BufferObject vbo = new BufferObject(BufferType.ArrayBuffer);

            vbo.SetData(chunkVertex, BufferHint.StaticDraw);

            int VertexArray = shader.GetAttribProgram("aPosition");
            int ColorArray = shader.GetAttribProgram("aTexCoord");
            int NormalArray = shader.GetAttribProgram("aNormal");

            vao.Activate();

            vao.AttachBuffer(vbo);

            vao.AttribPointer(VertexArray, 3, AttribType.Float, 8 * sizeof(float), 0);
            vao.AttribPointer(ColorArray, 2, AttribType.Float, 8 * sizeof(float), 3 * sizeof(float));
            vao.AttribPointer(NormalArray, 3, AttribType.Float, 8 * sizeof(float), 5 * sizeof(float));

            vao.Deactivate();
            vao.DisableAttribAll();
        }

        private float[] addVertex(Faces face, float[] vertex, Vector3 pos)
        {
            List<float> data = new List<float>();

            for (int i = 0; i < 6; i++)
            {
                vertex[i * 8] += pos.X;
                vertex[i * 8 + 1] += pos.Y;
                vertex[i * 8 + 2] += pos.Z;
            }

            foreach (float i in vertex)
                data.Add(i);

            vertexCount += 6;

            return data.ToArray();
        }

        public void Draw(Matrix4 projection, Matrix4 view, Vector3 cameraPos)
        {
            Matrix4 model = Matrix4.Identity;

            //model = model * Matrix4.CreateTranslation(position);
            model = model * Matrix4.CreateTranslation(new Vector3(position.X * size.X, 0, position.Y * size.Z));

            texture.Use(TextureUnit.Texture0);

            shader.ActiveProgram();

            shader.SetUniformMat4("proj", projection);
            shader.SetUniformMat4("view", view);
            shader.SetUniformMat4("model", model);

            vao.Activate();
            vao.Draw(0, vertexCount);

            shader.DeactiveProgram();
        }
    }
}
