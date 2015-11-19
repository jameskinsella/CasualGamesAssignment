using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{

    public class OpponentFootball : SimpleSprite
    {
        public OpponentFootball(Texture2D _Texture, Vector2 _Position) : base(_Texture,_Position)
        {
        }
        public void Update()
        {
            // get location of enemy footballs from SignalR
            DrawRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
        public void Draw(SpriteBatch sp)
        {
            sp.Draw(Texture, DrawRectangle, Color.White);
        }
    }
}
