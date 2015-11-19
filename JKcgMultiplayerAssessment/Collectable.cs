using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class Collectable : SimpleSprite
    {
        public Collectable(Texture2D _Texture, Vector2 _Position) : base(_Texture,_Position)
        {
            Texture = _Texture;
            Position = _Position;
            DrawRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            CollisionRectangle = new Rectangle((int)Position.X+10, (int)Position.Y+10, _Texture.Width-10, _Texture.Height-10);
        }
        public void Draw(SpriteBatch sp)
        {
            sp.Draw(Texture, DrawRectangle, Color.White);
        }
    }
}
