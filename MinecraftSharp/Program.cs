using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

namespace Cubicus // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nativeWinSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(1600, 900),
                WindowBorder = WindowBorder.Resizable,
                WindowState = WindowState.Normal,


                // Flags = ContextFlags.ForwardCompatible,
                Flags = ContextFlags.Default,
                APIVersion = new Version(3, 3),
                Profile = ContextProfile.Compatability,
                // Profile = ContextProfile.Core,
                API = ContextAPI.OpenGL,

                IsFullscreen = true,
                NumberOfSamples = 0
            };


            using (Game game = new Game(GameWindowSettings.Default, nativeWinSettings))
            {
                game.Run();
            }
        }
    }
}
