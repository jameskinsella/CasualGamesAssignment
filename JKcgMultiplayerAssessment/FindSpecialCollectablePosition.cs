using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class FindSpecialCollectablePosition
    {
        //public Texture2D Tex;
        public Point Position;
        Player Player1;
        Opponent Player2;
        public int Speed;
        //public int Health;
        public int AmountOfQuadrants = 625;
        int QuadrantAmountX;
        int QuadrantAmountY;
        public Vector2 CurrentGridPosition;
        public List<Quadrant> QuadrantList = new List<Quadrant>();
        public Dictionary<Vector2, int> GridPosition = new Dictionary<Vector2, int>();
        public List<int> PossibleMoveList = new List<int>();
        public Vector2 NextPosition;
        //public Rectangle DrawRectangle;
        public FindSpecialCollectablePosition(Point _Position, Player _Player1, Opponent _Player2, int PitchWidth, int PitchHeight)
        {
            Player1 = _Player1;
            Player2 = _Player2;
            Position = _Position;
            //Health = 10;
            Speed = 12;


            QuadrantAmountX = (int)Math.Sqrt(AmountOfQuadrants);
            QuadrantAmountY = (int)Math.Sqrt(AmountOfQuadrants);

            int QuadrantWidth = (PitchWidth / QuadrantAmountX);
            int QuadrantHeight = (PitchHeight / QuadrantAmountY);

            int QuadrantX = -(PitchWidth / 2) + (QuadrantWidth / 2);
            int QuadrantY = -(PitchHeight / 2) + (QuadrantHeight / 2);
            int QuadListValue = 0;
            //Set Quadrant Positions
            for (int j = 0; j < QuadrantAmountY; j++)
            {
                for (int i = 0; i < QuadrantAmountX; i++)
                {
                    QuadrantList.Add(new Quadrant(new Vector2(QuadrantX, QuadrantY), QuadrantWidth, QuadrantHeight, i, j));
                    GridPosition.Add(new Vector2(i, j), QuadListValue);
                    QuadListValue++;
                    QuadrantX += QuadrantWidth;
                }
                QuadrantX = -(PitchWidth / 2) + (QuadrantWidth / 2);
                QuadrantY += QuadrantHeight;
            }
            //DrawRectangle = new Rectangle(Position.X, Position.Y, Tex.Width, Tex.Height);
        }

        public void FindPlayersQuadrantPosition()
        {
            foreach (Quadrant Quadrant in QuadrantList)
            {
                Quadrant.DangerSquare = false;
                if (Quadrant.PositionRect.Contains(Player1.Position.ToPoint()))
                {
                    Quadrant.DangerSquare = true;
                }
                if (Quadrant.PositionRect.Contains(Player2.Position.ToPoint()))
                {
                    Quadrant.DangerSquare = true;
                }
                QuadrantList[GridPosition[new Vector2(0, 0)]].DangerSquare = true;
                QuadrantList[GridPosition[new Vector2(QuadrantAmountX - 1, 0)]].DangerSquare = true;
                QuadrantList[GridPosition[new Vector2(0, QuadrantAmountY - 1)]].DangerSquare = true;
                QuadrantList[GridPosition[new Vector2(QuadrantAmountX - 1, QuadrantAmountY - 1)]].DangerSquare = true;
            }

        }
        public void ResetQuadrants()
        {
            foreach (Quadrant Quadrant in QuadrantList)
            {
                Quadrant.Safety = 0;
            }
        }


        public void SetQuadrantsSafety()
        {
            foreach (Quadrant Quadrant in QuadrantList)
            {
                if (Quadrant.DangerSquare)
                {
                    foreach (Quadrant Quadrants in QuadrantList)
                    {
                        Quadrants.Visited = false;
                    }
                    CurrentGridPosition = new Vector2(Quadrant.XPosition, Quadrant.YPosition);
                    int x = 0;
                    int yOriginal = 7;
                    int y = yOriginal;
                    int yIncrement = -1;
                    int xIncrement = 1;
                    int iMultiplier = 1;
                    for (int k = 0; k < 4; k++)
                    {
                        if (k == 1)
                        {
                            xIncrement *= -1;
                        }
                        if (k == 2)
                        {
                            yIncrement *= -1;
                            iMultiplier *= -1;
                        }
                        if (k == 3)
                        {
                            xIncrement *= -1;
                        }
                        for (int j = 0; j < yOriginal; j++)
                        {

                            for (int i = 0; i < Math.Abs(x) + 1; i++)
                            {
                                int SafetyValue;
                                int Biggernum;

                                if (Math.Abs(i) > Math.Abs(y))
                                {
                                    Biggernum = Math.Abs(i);

                                }
                                else if (Math.Abs(y) > Math.Abs(i))
                                {
                                    Biggernum = Math.Abs(y);
                                }
                                else
                                {
                                    Biggernum = ((Math.Abs(i) + Math.Abs(y)));
                                }
                                SafetyValue = Math.Abs(i) + Math.Abs(y);
                                SafetyValue = SafetyValue + Biggernum;

                                if (GridPosition.ContainsKey(new Vector2(CurrentGridPosition.X + (i * iMultiplier), CurrentGridPosition.Y + y)))
                                {
                                    if (QuadrantList[GridPosition[new Vector2(CurrentGridPosition.X + (i * iMultiplier), CurrentGridPosition.Y + y)]].Visited == false)
                                    {
                                        QuadrantList[GridPosition[new Vector2(CurrentGridPosition.X + (i * iMultiplier), CurrentGridPosition.Y + y)]].Safety += 40 - SafetyValue;
                                        QuadrantList[GridPosition[new Vector2(CurrentGridPosition.X + (i * iMultiplier), CurrentGridPosition.Y + y)]].Visited = true;
                                    }
                                }
                            }
                            x += xIncrement;
                            y += yIncrement;
                        }
                    }

                }
            }
        }

        public Point Move()
        {
            foreach (Quadrant Quadrant in QuadrantList)
            {
                if (Quadrant.PositionRect.Contains(Position))
                {
                    CurrentGridPosition = new Vector2(Quadrant.XPosition, Quadrant.YPosition);
                }
            }
            for (int j = -3; j <= 3; j++)
            {
                for (int i = -3; i <= 3; i++)
                {
                    if (GridPosition.ContainsKey(new Vector2(CurrentGridPosition.X + i, CurrentGridPosition.Y + j)))
                    {
                        if (i != 0 || j != 0)
                            PossibleMoveList.Add((GridPosition[new Vector2(CurrentGridPosition.X + i, CurrentGridPosition.Y + j)]));
                    }
                }
            }
            int BestPath = 1000;
            ShuffleList(PossibleMoveList);
            for (int i = 0; i < PossibleMoveList.Count; i++)
            {
                if (QuadrantList[PossibleMoveList[i]].Safety < BestPath)
                {
                    BestPath = (int)QuadrantList[PossibleMoveList[i]].Safety;
                    NextPosition = QuadrantList[PossibleMoveList[i]].Position;
                }
            }
            if (NextPosition.X < Position.X)
                Position.X -= Speed;
            if (NextPosition.X > Position.X)
                Position.X += Speed;
            if (NextPosition.Y < Position.Y)
                Position.Y -= Speed;
            if (NextPosition.Y > Position.Y)
                Position.Y += Speed;
            PossibleMoveList.Clear();
            return Position;
        }
        public void ShuffleList(List<int> ToBeShuffled)
        {
            Random rnd = new Random();
            for (int i = 0; i < ToBeShuffled.Count; i++)
            {
                int temp;
                int rndtemp = rnd.Next(0, ToBeShuffled.Count);
                temp = ToBeShuffled[i];
                ToBeShuffled[i] = ToBeShuffled[rndtemp];
                ToBeShuffled[rndtemp] = temp;
            }
        }

        public Point Update()
        {
            ResetQuadrants();
            FindPlayersQuadrantPosition();
            SetQuadrantsSafety();
            Move();
            return Position;
            /*DrawRectangle = new Rectangle(Position.X, Position.Y, Tex.Width, Tex.Height);
            if (DrawRectangle.Intersects(Player1.DestinationRectangle))
            {
                Player1.game1.proxy.Invoke("GameOver", Player1.PlayerID, false);
            }
            for (int i = 0; i < Player1.FootballList.Count; i++)
            {
                if (DrawRectangle.Intersects(Player1.FootballList[i].DrawRectangle))
                {
                    Player1.FootballList.RemoveAt(i);
                    Speed -= 1;
                }
            }*/
        }
        /*public void Draw(SpriteBatch sp)
        {
            DrawRectangle = new Rectangle(Position.X, Position.Y, Tex.Width, Tex.Height);
            sp.Draw(Tex, DrawRectangle, Color.Black);
        }*/
    }
}