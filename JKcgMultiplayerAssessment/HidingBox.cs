using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class HidingBox : SimpleSprite
    {
        public int Health;
        public HidingBox(Texture2D _Texture, Vector2 _Position) : base(_Texture,_Position)
        {
            Health = 4;
        }
        public void Draw(SpriteBatch sp)
        {
            sp.Draw(Texture, DrawRectangle, Color.White);
        }
    }
}