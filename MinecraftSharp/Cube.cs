using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubicus
{
    public class Cube
    {
        public Vector3i position;
        public bool air;

        private Shader shader;
        private Texture texture;
        private List<ArrayObject> faces = new List<ArrayObject>();

        public Cube(Vector3i pos, Shader shader, string texturePath)
        {
            this.shader = shader;
            position = pos;

            texture = Texture.LoadFromFile(texturePath);
            texture.Use(TextureUnit.Texture0);

            CreateStuff();

            air = true;
        }

        private float[] frontVertex = new float[]
        {
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f, 0.0f, 0.0f, 1.0f, // 0
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f, 0.0f, 1.0f, // 1
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f, // 2
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f, // 3
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f, 0.0f, 1.0f, // 4
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f, 0.0f, 1.0f, // 5
        };
        
        private float[] backVertex = new float[]
        {
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f, 0.0f, -1.0f, // 6
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f, 0.0f, -1.0f, // 7
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f, 0.0f, -1.0f, // 8
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f, 0.0f, -1.0f, // 9
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f, 0.0f, -1.0f, // 10
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f, 0.0f, -1.0f, // 11
        };

        private float[] rightVertex = new float[]
        {
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 1.0f, 0.0f, 0.0f, // 12
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 1.0f, 0.0f, 0.0f, // 13
            0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 1.0f, 0.0f, 0.0f, // 14
            0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 1.0f, 0.0f, 0.0f, // 15
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 1.0f, 0.0f, 0.0f, // 16
            0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // 17
        };

        private float[] leftVertex = new float[]
        {
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, -1.0f, 0.0f, 0.0f, // 18
            -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, -1.0f, 0.0f, 0.0f, // 19
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, -1.0f, 0.0f, 0.0f, // 20
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, -1.0f, 0.0f, 0.0f, // 21
            -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, -1.0f, 0.0f, 0.0f, // 22
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f, -1.0f, 0.0f, 0.0f, // 23
        };

        private float[] upVertex = new float[]
        {
           -0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f, 1.0f, 0.0f, // 24
            0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // 25
           -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // 26
           -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // 27
            0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // 28
            0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 0.0f, 1.0f, 0.0f, // 29
        };

        private float[] downVertex = new float[]
        {
            0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f, -1.0f, 0.0f, // 30
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f, -1.0f, 0.0f, // 31
           -0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 0.0f, -1.0f, 0.0f, // 32
           -0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 0.0f, -1.0f, 0.0f, // 33
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f, -1.0f, 0.0f, // 34
           -0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f, -1.0f, 0.0f, // 35
        };

        private void CreateStuff()
        {
            AddFace(frontVertex);
            AddFace(backVertex);
            AddFace(rightVertex);
            AddFace(leftVertex);
            AddFace(upVertex);
            AddFace(downVertex);
        }

        private void AddFace(float[] vertex)
        {
            faces.Add(CreateVAO(vertex));
        }

        private ArrayObject CreateVAO(float[] data)
        {
            ArrayObject vao = new ArrayObject();
            BufferObject vbo = new BufferObject(BufferType.ArrayBuffer);

            vbo.SetData(data, BufferHint.StaticDraw);

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

            return vao;
        }

        public void Draw(Matrix4 projection, Matrix4 view, Vector2i chunkPos, Vector3 cameraPos)
        {
            if (air) return;

            Matrix4 model = Matrix4.Identity;

            model = model * Matrix4.CreateTranslation(position);
            model = model * Matrix4.CreateTranslation(new Vector3(chunkPos.X, 0, chunkPos.Y));

            texture.Use(TextureUnit.Texture0);

            shader.ActiveProgram();

            shader.SetUniformMat4("proj", projection);
            shader.SetUniformMat4("view", view);
            shader.SetUniformMat4("model", model);

            for (int i = 0; i < 6; i++)
            {
                faces[i].Activate();
                faces[i].Draw(0, 6);
            }

            shader.DeactiveProgram();
        }
    }
}
