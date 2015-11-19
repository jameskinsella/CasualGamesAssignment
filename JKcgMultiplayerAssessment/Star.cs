using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class Star : SimpleSprite
    {
        public Vector2 Direction;
        public int Speed;
        public float Scale;
        public float Rotation;
        public int RotationDirection;
        public Vector2 Origin;
        public Star(Texture2D _Texture, Vector2 _Position,Vector2 _Direction, int _RotationDirection,float _Scale) : base(_Texture,_Position)
        {
            Direction = _Direction;
            Speed = 4;
            Rotation = 0;
            RotationDirection = _RotationDirection;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            
            Direction.Normalize();
            Scale = _Scale*.8f;
        }
        public bool Update()
        {
            Rotation += 0.5f;
            Scale -= .01f;
            Position += Direction*Speed;
            if (Scale < 0.01)
            {
                return true;
            }
            return false;
        }
        public void Draw(SpriteBatch sp)
        {
            sp.Draw(Texture, Position, new Rectangle(0,0,Texture.Width,Texture.Height), Color.White, Rotation, Origin, Scale, SpriteEffects.None, .1f);
            //sp.Draw(Texture, DrawRectangle, Color.White);
        }
    }
}
