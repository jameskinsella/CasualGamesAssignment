using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    
    public class Ronaldo
    {
        public int Speed = 6;
        public int FootballsRemaining = 20;
        public float ShotPower = 18;
        public Texture2D FootballTex;
        public List<Texture2D> Tex;
        public List<Texture2D> ShootingTex;
        public Ronaldo(Game1 _game1)
        {
            Tex = _game1.RonaldoTex;
            ShootingTex = _game1.RonaldoShootingTex;
            FootballTex = _game1.FootballTex;
        }
    }
}