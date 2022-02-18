using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubicus
{
    [Flags]
    public enum Faces
    {
        Null =  0b_0000_0000,
        Front = 0b_0000_0001,
        Back =  0b_0000_0010,
        Right = 0b_0000_0100,
        Left =  0b_0000_1000,
        Up =    0b_0001_0000,
        Down =  0b_0010_0000,
    };

    public class Cube
    {
        public Vector3i position;
        public bool air;
        public Faces visibleFaces;
        private Vector2 minTexPos;
        private Vector2 maxTexPos;

        public Cube(Vector3i pos, byte textureUnitID)
        {
            position = pos;

            int x = textureUnitID & 0b_1111;
            int y = textureUnitID >> 4;

            float size = 16.0f;

            minTexPos = new Vector2(x / size, y / size);
            maxTexPos = new Vector2((x + 1) / size, (y + 1) / size);

            air = true;

            frontVertex = new float[]
            {
                 0.5f,  0.5f,  0.5f,  maxTexPos.X, minTexPos.Y,  0.0f, 0.0f, 1.0f, // 0
                 0.5f, -0.5f,  0.5f,  maxTexPos.X, maxTexPos.Y,  0.0f, 0.0f, 1.0f, // 1
                -0.5f,  0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  0.0f, 0.0f, 1.0f, // 2
                -0.5f,  0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  0.0f, 0.0f, 1.0f, // 3
                 0.5f, -0.5f,  0.5f,  maxTexPos.X, maxTexPos.Y,  0.0f, 0.0f, 1.0f, // 4
                -0.5f, -0.5f,  0.5f,  minTexPos.X, maxTexPos.Y,  0.0f, 0.0f, 1.0f, // 5
            };

            backVertex = new float[]
            {
                 0.5f,  0.5f, -0.5f,  maxTexPos.X, minTexPos.Y,  0.0f, 0.0f, -1.0f, // 6
                -0.5f,  0.5f, -0.5f,  minTexPos.X, minTexPos.Y,  0.0f, 0.0f, -1.0f, // 7
                 0.5f, -0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  0.0f, 0.0f, -1.0f, // 8
                 0.5f, -0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  0.0f, 0.0f, -1.0f, // 9
                -0.5f,  0.5f, -0.5f,  minTexPos.X, minTexPos.Y,  0.0f, 0.0f, -1.0f, // 10
                -0.5f, -0.5f, -0.5f,  minTexPos.X, maxTexPos.Y,  0.0f, 0.0f, -1.0f, // 11
            };

            rightVertex = new float[]
            {
                0.5f,  0.5f, -0.5f,  maxTexPos.X, minTexPos.Y,  1.0f, 0.0f, 0.0f, // 12
                0.5f, -0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  1.0f, 0.0f, 0.0f, // 13
                0.5f,  0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  1.0f, 0.0f, 0.0f, // 14
                0.5f,  0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  1.0f, 0.0f, 0.0f, // 15
                0.5f, -0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  1.0f, 0.0f, 0.0f, // 16
                0.5f, -0.5f,  0.5f,  minTexPos.X, maxTexPos.Y,  1.0f, 0.0f, 0.0f, // 17
            };

            leftVertex = new float[]
            {
                -0.5f, -0.5f,  0.5f,  minTexPos.X, maxTexPos.Y,  -1.0f, 0.0f, 0.0f, // 18
                -0.5f, -0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  -1.0f, 0.0f, 0.0f, // 19
                -0.5f,  0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  -1.0f, 0.0f, 0.0f, // 20
                -0.5f,  0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  -1.0f, 0.0f, 0.0f, // 21
                -0.5f, -0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  -1.0f, 0.0f, 0.0f, // 22
                -0.5f,  0.5f, -0.5f,  maxTexPos.X, minTexPos.Y,  -1.0f, 0.0f, 0.0f, // 23
            };

            upVertex = new float[]
            {
               -0.5f,  0.5f, -0.5f,  maxTexPos.X, minTexPos.Y,  0.0f, 1.0f, 0.0f, // 24
                0.5f,  0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  0.0f, 1.0f, 0.0f, // 25
               -0.5f,  0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  0.0f, 1.0f, 0.0f, // 26
               -0.5f,  0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  0.0f, 1.0f, 0.0f, // 27
                0.5f,  0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  0.0f, 1.0f, 0.0f, // 28
                0.5f,  0.5f,  0.5f,  minTexPos.X, maxTexPos.Y,  0.0f, 1.0f, 0.0f, // 29
            };

            downVertex = new float[]
            {
                0.5f, -0.5f,  0.5f,  minTexPos.X, maxTexPos.Y,  0.0f, -1.0f, 0.0f, // 30
                0.5f, -0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  0.0f, -1.0f, 0.0f, // 31
               -0.5f, -0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  0.0f, -1.0f, 0.0f, // 32
               -0.5f, -0.5f,  0.5f,  minTexPos.X, minTexPos.Y,  0.0f, -1.0f, 0.0f, // 33
                0.5f, -0.5f, -0.5f,  maxTexPos.X, maxTexPos.Y,  0.0f, -1.0f, 0.0f, // 34
               -0.5f, -0.5f, -0.5f,  maxTexPos.X, minTexPos.Y,  0.0f, -1.0f, 0.0f, // 35
            };
        }

        public float[] frontVertex;

        public float[] backVertex;

        public float[] rightVertex;

        public float[] leftVertex;

        public float[] upVertex;

        public float[] downVertex;
    }
}
