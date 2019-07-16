using UnityEngine;

namespace SFA
{
    public enum ImageFormat
    {
        PNG,
        JPG,
    }
    public class FrameInfo
    {
        public Texture2D tex;
        public string name;
        public Vector2 pivot;

        public FrameInfo(Texture2D tex, string name, Vector2 pivot)
        {
            this.tex = tex;
            this.name = name;
            this.pivot = pivot;
        }
    }

    public class ScreenPoint
    {
        public int x, y;

        public ScreenPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return x + ", " + y;
        }

        public static ScreenPoint operator +(ScreenPoint p1, ScreenPoint p2)
        {
            return new ScreenPoint(p1.x + p2.x, p1.y + p2.y);
        }

        public static ScreenPoint operator -(ScreenPoint p1, ScreenPoint p2)
        {
            return new ScreenPoint(p1.x - p2.x, p1.y - p2.y);
        }
    }
}
