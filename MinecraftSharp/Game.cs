using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubicus
{
    public class Game : GameWindow
    {
        private float deltaTime;
        private Vector2 size;
        private Vector2 lastMousePos;
        private bool polyVisible = true;

        private Camera camera;
        private Chunk[] world;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            NameExampleWindow = "Index Buffer Object";
            Title = NameExampleWindow;

            size = nativeWindowSettings.Size;

            lastMousePos.X = size.X / 2;
            lastMousePos.Y = size.Y / 2;

            //MousePosition = lastMousePos;

            //Console.WriteLine(GL.GetString(StringName.Version));
            //Console.WriteLine(GL.GetString(StringName.Vendor));
            //Console.WriteLine(GL.GetString(StringName.Renderer));
            //Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));

            VSync = VSyncMode.On;
            CursorGrabbed = true;
        }

        public string NameExampleWindow { private set; get; }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            // GL.PolygonMode(MaterialFace.Back, PolygonMode.Point);

            camera = new Camera(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f, 0.0f);

            int amount = 1;

            world = new Chunk[amount * amount];

            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    world[i * amount + j] = new Chunk(i, j);
                }
            }

            for (int i = 0; i < world.Length; i++)
            {
                List<Chunk> neigbor = new List<Chunk>();
                int x = world[i].position.X;
                int y = world[i].position.Y;

                if (y - 1 >= 0) neigbor.Add(world[i - 1]);
                if (y + 1 < amount) neigbor.Add(world[i + 1]);
                if (x - 1 >= 0) neigbor.Add(world[i - amount]);
                if (x + 1 < amount) neigbor.Add(world[i + amount]);

                if (x - 1 >= 0 && y - 1 >= 0) neigbor.Add(world[i - 1 - amount]);
                if (x - 1 >= 0 && y + 1 < amount) neigbor.Add(world[i + 1 - amount]);
                if (x + 1 < amount && y - 1 >= 0) neigbor.Add(world[i - 1 + amount]);
                if (x + 1 < amount && y + 1 < amount) neigbor.Add(world[i + 1 + amount]);

                world[i].neigborChunks = neigbor;
            }      
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            deltaTime = (float)args.Time;

            Title = (1.0f / deltaTime).ToString();

            var key = KeyboardState;

            if (key.IsKeyDown(Keys.Escape))
                Close();

            if (key.IsKeyPressed(Keys.F3))
            {
                CursorGrabbed = !CursorGrabbed;
            }    

            if (key.IsKeyPressed(Keys.F4))
            {
                if (polyVisible)
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Line);
                else
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);
                polyVisible = !polyVisible;
            }    

            CameraMove(key);

            if (CursorGrabbed)
                MouseMovement();

            base.OnUpdateFrame(args);
        }

        private void CameraMove(KeyboardState key)
        {
            if (key.IsKeyDown(Keys.W))
                camera.ProcessKeyboard(Camera.CameraMovement.Forward, deltaTime);
            if (key.IsKeyDown(Keys.A))
                camera.ProcessKeyboard(Camera.CameraMovement.Left, deltaTime);
            if (key.IsKeyDown(Keys.S))
                camera.ProcessKeyboard(Camera.CameraMovement.Backward, deltaTime);
            if (key.IsKeyDown(Keys.D))
                camera.ProcessKeyboard(Camera.CameraMovement.Right, deltaTime);
            if (key.IsKeyDown(Keys.Space))
                camera.ProcessKeyboard(Camera.CameraMovement.Up, deltaTime);
            if (key.IsKeyDown(Keys.LeftShift))
                camera.ProcessKeyboard(Camera.CameraMovement.Down, deltaTime);
        }

        private void MouseMovement()
        {
            Vector2 mousePos = MousePosition;

            float xoffset = mousePos.X - lastMousePos.X;
            float yoffset = lastMousePos.Y - mousePos.Y;

            lastMousePos = mousePos;

            camera.ProcessMouseMovement(xoffset, yoffset);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, size.X / (float) size.Y, 0.01f, 1000.0f);
            Matrix4 view = camera.GetViewMatrix();

            for (int i = 0; i < world.Length; i++)
            {
                world[i].Draw(projection, view, camera.position);
            }

            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
