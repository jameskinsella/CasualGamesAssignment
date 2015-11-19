using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class Quadrant
    {
        public Vector2 Position;
        public Rectangle PositionRect;
        public float Safety = 0;
        public bool DangerSquare = false;
        public int XPosition;
        public int YPosition;
        public bool Visited = false;
        public Quadrant(Vector2 _Position, int QuadrantWidth,int QuadrantHeight, int _XPosition, int _YPosition)
        {
            Position = _Position;
            PositionRect = new Rectangle((int)Position.X, (int)Position.Y, QuadrantWidth, QuadrantHeight);
            XPosition = _XPosition;
            YPosition = _YPosition;
        }
        
    }
}
