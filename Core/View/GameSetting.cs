using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DotAge.Core.View
{
    static class GameSetting
    {
        public static int ScreenWidth { get; set; } = 1280;
        public static int ScreenHeight { get; set; } = 720;
        public static Vector2 ScreenSize { get { return new Vector2(ScreenWidth, ScreenHeight); } }

        public static float ChunkWidth { get; set; } = 1024f;
        public static float ChunkHeight { get; set; } = 1024f;
        public static Vector2 ChunkSize { get { return new Vector2(ChunkWidth, ChunkHeight); } }
    }
}
