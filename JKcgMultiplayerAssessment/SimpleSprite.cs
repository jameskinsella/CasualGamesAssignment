using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class SimpleSprite
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle DrawRectangle;
        public Rectangle CollisionRectangle;

        public SimpleSprite(Texture2D _Texture, Vector2 _Position)
        {
            Texture = _Texture;
            Position = _Position;
            DrawRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
    }
}