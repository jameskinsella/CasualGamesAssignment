using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class Player : AnimatedSprite
    {
        #region PlayerVariables
        public int SpriteDirection = 0;
        public bool Shooting = false;
        public int ShootDirection;
        public bool Moving;
        public string PlayerID;
        public float Speed;
        public float ShotPower;
        public float ShotPowerMultiplier=.4f;
        public float ShotPowerBarDisplay;
        public float ShotPowerVariance = .04f;
        public int FootballsRemaining;
        public float Health = 100;
        public int VotesCollected = 0;
        public bool Hidden = false;
        public int LeftLimit = -1492;
        public int RightLimit = 1468;
        public int UpLimit = -742;
        public int DownLimit = 642;
        public bool CanShoot = true;
        public int ReloadTime = 40;
        public int ReloadTimeCounter = 0;

        #endregion PlayerVariables
        #region Lists and Textures
        List<Star> StarList = new List<Star>();
        public List<Football> FootballList = new List<Football>();
        public List<Football> OpponentFootballList = new List<Football>();
        public List<HidingBox> HidingBoxList;
        public List<Collectable> CollectableList = new List<Collectable>();
        public List<Texture2D> PlayerShooting;
        public Texture2D FootballTex;
        Texture2D StarTex;
        #endregion Lists and Texures
        #region Mouse and Keyboard
        public KeyboardState keyState;
        public MouseState CurMouseState = Mouse.GetState();
        public MouseState PrevMouseState;
        #endregion
        #region OtherVariables
        public Vector2 MiddlePoint;
        public Vector2 FootballOrigin = new Vector2(15,98);
        public int ViewPortWidth;
        public int ViewPortHeight;
        public HealthBar HealthBarDisplay;
        public PowerBar PowerBarDisplay;

        #endregion
        public Game1 game1;
        
        public Player(List<Texture2D> _Texture, Vector2 _Position, int _Framecount,string PlayerType,string _PlayerID,Game1 _game1)
            : base(_Texture, _Position, _Framecount)
        {
            game1 = _game1;
            CollectableList = game1.CollectableList;
            HidingBoxList = game1.HidingBoxList;
            ViewPortWidth = game1.GraphicsDevice.Viewport.Width;
            ViewPortHeight = game1.GraphicsDevice.Viewport.Height;
            HealthBarDisplay = new HealthBar(game1.HealthContainer, game1.HealthBar);
            PowerBarDisplay = new PowerBar(game1.PowerContainer, game1.PowerBar);
            StarTex = game1.StarTex;
            FootballTex = game1.FootballTex;
            PlayerID = _PlayerID;
            if (PlayerType == "Messi")
            {
                PlayerShooting = game1.messi.ShootingTex;
                FootballsRemaining = game1.messi.FootballsRemaining;
                ShotPower = game1.messi.ShotPower;
                Speed = game1.messi.Speed;
            }
            else
            {
                PlayerShooting = game1.ronaldo.ShootingTex;
                FootballsRemaining = game1.ronaldo.FootballsRemaining;
                ShotPower = game1.ronaldo.ShotPower;
                Speed = game1.ronaldo.Speed;
            }
            MiddlePoint = new Vector2((_Texture[0].Width / 3)/2, _Texture[0].Height / 2);
        }
        #region MiscMethods
        
        public void CreateStars()
        {
            StarList.Add(new Star(StarTex, Position + FootballOrigin, new Vector2(1, -1), 1, ShotPowerBarDisplay / 100));
            StarList.Add(new Star(StarTex, Position + FootballOrigin, new Vector2(-1, -1), 1, ShotPowerBarDisplay / 100));
            StarList.Add(new Star(StarTex, Position + FootballOrigin, new Vector2(-1, 1), 1, ShotPowerBarDisplay / 100));
            StarList.Add(new Star(StarTex, Position + FootballOrigin, new Vector2(1, 1), 1, ShotPowerBarDisplay / 100));
            StarList.Add(new Star(StarTex, Position + FootballOrigin, new Vector2(1, 0), 1, ShotPowerBarDisplay / 100));
            StarList.Add(new Star(StarTex, Position + FootballOrigin, new Vector2(-1, 0), 1, ShotPowerBarDisplay / 100));
            StarList.Add(new Star(StarTex, Position + FootballOrigin, new Vector2(0, 1), 1, ShotPowerBarDisplay / 100));
        }
        public void Move()
        {
            Moving = false;
            if (keyState.IsKeyDown(Keys.W))
            {
                if (Position.Y > UpLimit + Speed)
                {
                    Position.Y -= Speed;
                    SpriteDirection = 1;
                    Moving = true;
                }
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                if (Position.Y < DownLimit - Speed)
                {
                    Position.Y += Speed;
                    SpriteDirection = 0;
                    Moving = true;
                }

            }
            if (keyState.IsKeyDown(Keys.A))
            {
                if (Position.X > LeftLimit + Speed)
                {
                    Position.X -= Speed;
                    Moving = true;
                    SpriteDirection = 3;
                }


            }

            if (keyState.IsKeyDown(Keys.D))
            {
                if (Position.X < RightLimit - Speed)
                {
                    Position.X += Speed;
                    Moving = true;
                    SpriteDirection = 2;
                }

            }

        }
        public void PickUpVote()
        {
            for (int i = 0; i < CollectableList.Count; i++)
            {
                if (DestinationRectangle.Intersects(CollectableList[i].CollisionRectangle))
                {
                    CollectableList.RemoveAt(i);
                    VotesCollected += 1;
                    game1.VoteCollected.Play();
                }
            }
        }
        public void FindMousePosition(Vector2 Target, Vector2 Origin)
        {
            Vector2 Direction = (Target - new Vector2(ViewPortWidth / 2, ViewPortHeight / 2)) + Origin - new Vector2(10, 50);

            int VertValue = Math.Abs((int)Direction.Y - (int)Position.Y);
            int HorizValue = Math.Abs((int)Direction.X - (int)Position.X);
            int Highestnumber = Math.Max(VertValue, HorizValue);
            if (HorizValue == Highestnumber)
            {
                if (Position.X > Direction.X)
                {
                    ShootDirection = 3;
                    SpriteDirection = 3;
                }
                else
                {
                    ShootDirection = 2;
                    SpriteDirection = 2;
                }
            }
            else
            {
                if (Position.Y > Direction.Y)
                {
                    ShootDirection = 1;
                    SpriteDirection = 1;
                }
                else
                {
                    ShootDirection = 0;
                    SpriteDirection = 0;
                }
            }
        }
        public void Shoot(Vector2 Target, Vector2 Origin)
        {
            Vector2 Movement = (Target - new Vector2(ViewPortWidth / 2, ViewPortHeight / 2) + new Vector2(Position.X + (SpriteWidth / 2), Position.Y + (SpriteHeight / 2)) - Origin);
            if (Movement != Vector2.Zero)
            {
                Movement.Normalize();
                FootballList.Add(new Football(FootballTex, Position + FootballOrigin, ShotPower * ShotPowerMultiplier, Movement, HidingBoxList, CollectableList, true));
                game1.BallKicked.Play();
                game1.proxy.Invoke("CreateBalls", PlayerID, Position + FootballOrigin, Movement, ShotPower * ShotPowerMultiplier);
                CreateStars();
            }
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
        public void CheckIfHit()
        {
            for (int i = 0; i < OpponentFootballList.Count; i++)
            {
                if (DestinationRectangle.Intersects(OpponentFootballList[i].DrawRectangle))
                {
                    Health -= OpponentFootballList[i].Speed;
                    game1.PlayerHit.Play();
                    OpponentFootballList.RemoveAt(i);
                }
            }
        }
        public void UpdatePowerBar()
        {
            ShotPowerMultiplier += ShotPowerVariance;
            if (ShotPowerMultiplier > 1f)
            {
                ShotPowerMultiplier = 1f;
                ShotPowerVariance *= -1;
            }
            else if (ShotPowerMultiplier < .4f)
            {
                ShotPowerMultiplier = .6f;
                ShotPowerVariance *= -1;
            }

            ShotPowerBarDisplay = ShotPowerMultiplier * 100;
            PowerBarDisplay.Update(Position, (int)ShotPowerBarDisplay);
        }
        #endregion MiscMethods

        public void Update(GameTime gametime)
        {
            ReloadTimeCounter++;
            if (ReloadTimeCounter > ReloadTime)
            {
                CanShoot = true;
            }
            CheckIfHit();
            PickUpVote();
            #region Shoot
            CurMouseState = Mouse.GetState();
            if (CanShoot)
            {
                if (CurMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (FootballsRemaining > 0)
                    {
                        if (Hidden != true)
                        {
                            Shooting = true;
                            FindMousePosition(new Vector2(CurMouseState.X, CurMouseState.Y), Position);
                            UpdatePowerBar();
                        }
                    }
                }
                if (CurMouseState.LeftButton != PrevMouseState.LeftButton)
                {
                    if (CurMouseState.LeftButton == ButtonState.Released)
                    {
                        if (FootballsRemaining > 0)
                        {
                            if (Shooting)
                            {

                                Shoot(new Vector2(CurMouseState.X, CurMouseState.Y), Position + FootballOrigin);
                                FootballsRemaining -= 1;
                                Shooting = false;
                                CanShoot = false;
                                ReloadTimeCounter = 0;
                            }
                        }

                    }
                }
            }
            
            
            PrevMouseState = CurMouseState;
            #endregion Shoot
            #region UpdateOthers
            for (int i = 0; i < OpponentFootballList.Count; i++)
            {
                if (OpponentFootballList[i].Update())
                {
                    OpponentFootballList.RemoveAt(i);
                }
            }

            for (int i = 0; i < FootballList.Count; i++)
            {
                if (FootballList[i].Update())
                {
                    FootballList.RemoveAt(i);
                }
            }
            for (int i = 0; i < StarList.Count; i++)
            {
                if (StarList[i].Update())
                {
                    StarList.RemoveAt(i);
                }
            }
            #endregion
            #region Move/Animate/Hide
            keyState = Keyboard.GetState();

            if (Shooting != true)
            {
                Move();
                HealthBarDisplay.Update(Position, (int)Health);
            }

            if (Moving == true)
            {
                UpdateAnimation(gametime);
            }
            if (CheckHidden())
            {
                Hidden = true;
            }
            else
            {
                Hidden = false;
            }
            #endregion
            for (int i = 0; i < FootballList.Count; i++)
            {
                if (FootballList[i].DestroyIfOffScreen())
                {
                    FootballList.RemoveAt(i);
                }
            }
            DestinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, SpriteImage[0].Width/3, SpriteImage[0].Height);
        }
        public void Draw(SpriteBatch sp, SpriteFont font)
        {
            
            if (Shooting)
            {
                sp.Draw(PlayerShooting[ShootDirection], DestinationRectangle, Color.White);
                PowerBarDisplay.Draw(sp);
            }
            else
            {
                sp.Draw(SpriteImage[SpriteDirection], DestinationRectangle, SourceRectangle, Color.White);
            }
            sp.DrawString(font, PlayerID, new Vector2(Position.X - 20, Position.Y - 50), Color.White);


            foreach (Football Football in FootballList)
            {
                Football.Draw(sp);
            }
            for (int i = 0; i < OpponentFootballList.Count; i++)
            {
                OpponentFootballList[i].Draw(sp);
            }
            foreach (Star Star in StarList)
            {
                Star.Draw(sp);
            }
            if (Hidden != true)
            {
                HealthBarDisplay.Draw(sp);
            }
        }
    }
}