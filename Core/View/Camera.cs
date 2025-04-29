using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DotAge.Core.View
{
    public class Camera2D
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Point ScreenSize { get; set; }
        public Matrix View;
        public float Zoom { get; set; }
        public Vector2 Origin { get; set; }
        public Camera2D(string name, Vector2 position, float zoom)
        {
            Name = name;
            Position = position;
            Zoom = zoom;
            Origin = new Vector2(0, 0);
        }
        public Camera2D()
        {
            Name = "DefaultCamera";
            Position = new Vector2(0, 0);
            Zoom = 1.0f;
            Origin = new Vector2(0, 0);
        }
        public void Move(Vector2 delta)
        {
            Position += delta;
            View.Translation = new Vector3(Position, 0);
        }
        public void ChangeZoom(float amount)
        {
            Zoom += amount;
        }
    }
}
