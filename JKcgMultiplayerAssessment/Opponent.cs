using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{

    public class Opponent : AnimatedSprite
    {
        public int Direction;
        public bool Moving;
        public bool Hidden;
        public List<Collectable> CollectableList;
        public HealthBar HealthBarDisplay;
        public float Health = 100;
        public int VotesCollected;
        public List<HidingBox> HidingBoxList = new List<HidingBox>();
        public bool MessiSelected;
        public Point MiddlePoint;
        public Opponent(List<Texture2D> tx,Vector2 pos, int framecount, List<Collectable> _CollectableList, Game1 _game1) : base(tx, pos,framecount)
        {
            SpriteImage = tx;
            Position = pos;
            DestinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, tx[0].Width/3, tx[0].Height);
            CollectableList = _CollectableList;
            HealthBarDisplay = new HealthBar(_game1.HealthContainer, _game1.HealthBar);
            HidingBoxList = _game1.HidingBoxList;
            MiddlePoint = new Point((SpriteImage[0].Width/3)/2,SpriteImage[0].Height);
        }

        public bool CheckHidden()
        {
            foreach (HidingBox HidingBox in HidingBoxList)
            {
                if (HidingBox.Position.X < Position.X)
                {
                    if (HidingBox.Position.X + HidingBox.Texture.Width > Position.X + SpriteWidth)
                    {
                        if (HidingBox.Position.Y + 50 < Position.Y)
                        {
                            if (HidingBox.Position.Y + HidingBox.Texture.Height > Position.Y + SpriteHeight)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        public void PickUpVote()
        {
            for (int i = 0; i < CollectableList.Count; i++)
            {
                if (DestinationRectangle.Intersects(CollectableList[i].CollisionRectangle))
                {
                    CollectableList.RemoveAt(i);
                }
            }
        }

        public void Update(GameTime gametime)
        {
            // Get Opponents new Position from SignalR
            if (Moving == true)
            {
                UpdateAnimation(gametime);
            }
            HealthBarDisplay.Update(new Vector2(Position.X, Position.Y),(int)Health);
            PickUpVote();
            DestinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, SpriteImage[0].Width/3, SpriteImage[0].Height);
        }
        public void Draw(SpriteBatch sp)
        {
            if (CheckHidden()==false)
            {
                HealthBarDisplay.Draw(sp);
            }
            sp.Draw(SpriteImage[Direction],DestinationRectangle,SourceRectangle, Color.White);

        }
    }
}