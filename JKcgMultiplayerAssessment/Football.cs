using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class Football : SimpleSprite
    {
        public Vector2 PreviousPosition;
        public float Speed;
        public Vector2 Direction;
        public List<HidingBox> HidingBoxList;
        public List<Collectable> VoteList;

        public float RotationAngle;
        public Vector2 Origin;
        public bool MyFootball;
        public Football(Texture2D _Texture,Vector2 _Position,float _Speed, Vector2 _Direction, List<HidingBox> _HidingBoxList, List<Collectable> _VoteList, bool _MyFootball) : base(_Texture,_Position)
        {
            MyFootball = _MyFootball;
            Speed = _Speed;
            Direction = _Direction;
            HidingBoxList = _HidingBoxList;
            VoteList = _VoteList;
            RotationAngle = 0f;
            Origin.X = Texture.Width / 2;
            Origin.Y = Texture.Height / 2;
            DrawRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
        public bool BoxCollide()
        {
            for(int i = 0; i < HidingBoxList.Count; i++)
            {
                if (DrawRectangle.Intersects(HidingBoxList[i].DrawRectangle))
                {
                    HidingBoxList[i].Health--;
                    if (HidingBoxList[i].Health < 1)
                    {
                        HidingBoxList.RemoveAt(i);
                    }
                    return true;
                }
            }
            return false;
        }
        public bool DestroyIfOffScreen()
        {
            if (Position.X > 2400)
                return true;
            if (Position.X <-2400)
                return true;
            if (Position.Y < -1200)
                return true;
            if (Position.Y > 1200)
                return true;
            return false;
        }
        public bool VoteCollide()
        {
            for (int i = 0; i < VoteList.Count; i++)
            {
                if (DrawRectangle.Intersects(VoteList[i].CollisionRectangle))
                {
                    VoteList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        /*public void Bounce()
        {
            foreach (HidingBox HidingBox in HidingBoxList)
            {
                if (DrawRectangle.Intersects(HidingBox.DrawRectangle))
                {
                    
                    Direction.X *= -1;
                    Direction.Y *= 1;
                    Position = PreviousPosition;
                    Position += Direction * Speed;
                }
                else
                {
                    PreviousPosition = Position;
                }
            }
        }*/
        public bool Update()
        {
            DrawRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            if (BoxCollide())
            {
                return true;
            }
            if (VoteCollide())
            {
                return true;
            }
            Position += Direction * Speed;
            RotationAngle -= 0.2f;
            return false;
        }
        public void Draw(SpriteBatch sp)
        {
            //sp.Draw(Texture, DrawRectangle, Color.White);
            sp.Draw(Texture, Position, null, Color.White, RotationAngle, Origin, 1.0f, SpriteEffects.None, -1f);
            
        }
       
    }
}