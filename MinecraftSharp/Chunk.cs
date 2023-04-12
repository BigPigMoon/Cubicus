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

        public Vector3i size = new Vector3i(16, 256, 16);

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
                        cubes.Add(new Cube(new Vector3i(i, j, k), 1));
                        cubes[i + k * size.Z + j * (size.X * size.Z)].air = false;
                    }
                }
            }

            SetCubeDrawingFaces();
            UpdateChunkData();
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

            var down = x + z * size.Z + (y - 1) * size.X * size.Z;
            var up = x + z * size.Z + (y + 1) * size.X * size.Z;
            var left = x + 1 + z * size.Z + y * size.X * size.Z;
            var right = x - 1 + z * size.Z + y * size.X * size.Z;
            var front = x + (z - 1) * size.Z + y * size.X * size.Z;
            var back = x + (z + 1) * size.Z + y * size.X * size.Z;

            if (x - 1 < 0 || right > 0 && cubes[right].air) ret |= Faces.Left;
            if (x + 1 >= size.X || left < s && cubes[left].air) ret |= Faces.Right;
            if (z - 1 < 0 || front > 0 && cubes[front].air) ret |= Faces.Back;
            if (z + 1 >= size.Z || back < s && cubes[back].air) ret |= Faces.Front;
            if (y - 1 < 0 || down > s || down > 0 && cubes[down].air) ret |= Faces.Down;
            if (y + 1 >= size.Y || up < s && cubes[up].air) ret |= Faces.Up;

            //ret = Faces.Front | Faces.Back | Faces.Right | Faces.Left | Faces.Up | Faces.Down;

            return ret;
        }

        public void UpdateChunkData()
        {
            List<float[]> data = new List<float[]>();

            vertexCount = 0;

            foreach (Cube item in drawingCubes)
            {
                if (item.visibleFaces.HasFlag(Faces.Front))
                    data.Add(AddVertex(Faces.Front, item.frontVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Back))
                    data.Add(AddVertex(Faces.Back, item.backVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Left))
                    data.Add(AddVertex(Faces.Left, item.leftVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Right))
                    data.Add(AddVertex(Faces.Right, item.rightVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Up))
                    data.Add(AddVertex(Faces.Up, item.upVertex, item.position));
                if (item.visibleFaces.HasFlag(Faces.Down))
                    data.Add(AddVertex(Faces.Down, item.downVertex, item.position));
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

        private float[] AddVertex(Faces face, float[] vertex, Vector3 pos)
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

        public void Draw(Matrix4 projection, Matrix4 view)
        {
            Matrix4 model = Matrix4.Identity;

            model = model * Matrix4.CreateTranslation(new Vector3(position.X * size.X, 0, position.Y * size.Z));

            texture.Use(TextureUnit.Texture0);

            shader.ActiveProgram();

            shader.SetUniformMat4("proj", projection);
            shader.SetUniformMat4("view", view);
            shader.SetUniformMat4("model", model);
            shader.SetUniformVec3("selectedCube", new Vector3(0, 0, 0));
            shader.SetUniformInt("isSelectedCube", 1);


            vao.Activate();
            vao.Draw(0, vertexCount);

            shader.DeactiveProgram();
        }
    }
}
