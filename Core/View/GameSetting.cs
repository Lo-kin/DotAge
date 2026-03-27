using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.View
{
    static class GameSetting
    {
        public static int ScreenWidth { get; set; } = 1280;
        public static int ScreenHeight { get; set; } = 720;
        public static Vector2 TerrBlockSize = new Vector2(32, 32);
    }
}
