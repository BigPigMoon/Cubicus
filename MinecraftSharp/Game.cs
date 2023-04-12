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
        private float updateFPS;
        private Vector2 size;
        private Vector2 lastMousePos;
        private bool polyVisible = true;

        private Camera camera;
        private Cube cube;

        Matrix4 projection;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            NameExampleWindow = "Cubicus";
            Title = NameExampleWindow;

            size = nativeWindowSettings.Size;

            lastMousePos.X = size.X / 2;
            lastMousePos.Y = size.Y / 2;

            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.Vendor));
            Console.WriteLine(GL.GetString(StringName.Renderer));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));

            VSync = VSyncMode.On;
            CursorGrabbed = true;
        }

        public string NameExampleWindow { private set; get; }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.2f, 0.2f, 0.2f);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(new Vector3(0, 4.0f, 0), new Vector3(0.0f, 1.0f, 0.0f), -90.0f, 0.0f);

            projection = Matrix4.CreatePerspectiveFieldOfView(camera.zoom, size.X / (float)size.Y, 0.01f, 1000.0f);

            cube = new Cube(new Vector3i(0, 0, 0), 1);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            deltaTime = (float)args.Time;

            if (updateFPS > 1)
            {
                updateFPS = 0;
                Title = (1.0f / deltaTime).ToString();
            }

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

            updateFPS += deltaTime;

            base.OnUpdateFrame(args);
        }

        private void CameraMove(KeyboardState key)
        {
            // TODO: move this to camera scprit
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

            Matrix4 view = camera.GetViewMatrix();

            cube.Draw(projection, view);

            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
