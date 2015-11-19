using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class Messi
    {
        public int Speed = 8;
        public int FootballsRemaining = 20;
        public float ShotPower = 14;
        public Texture2D FootballTex;
        public List<Texture2D> Tex;
        public List<Texture2D> ShootingTex;
        public Messi(Game1 _game1)
        {
            Tex = _game1.MessiTex;
            ShootingTex = _game1.MessiShootingTex;
            FootballTex = _game1.FootballTex;
        }
    }
}