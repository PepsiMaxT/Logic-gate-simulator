using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farming
{
    public enum GateType
    {
        OR,
        AND,
        NOT,
        XOR,
    };

    public class Gate
    {
        public const int scale = 4;
        Vector2 position;
        public Rectangle rectangle;
        GateType gateType;
        public Texture2D texture;

        public Gate(Vector2 position, GateType gateType, Texture2D[] textureList)
        {
            this.position = position;
            this.gateType = gateType;
            this.texture = textureList[(int)gateType];
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width * scale, texture.Height * scale);
        }
        public Gate(Rectangle rectangle, GateType gateType, Texture2D[] textureList)
        {
            this.position = new Vector2(rectangle.X, rectangle.Y);
            this.gateType = gateType;
            this.texture = textureList[(int)gateType];
            this.rectangle = rectangle;
        }
    }
}
