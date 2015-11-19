using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class HealthBar
    {
        public Texture2D Container;
        public Texture2D HealthTexture;
        public Rectangle HealthDestination;
        public Rectangle HealthSource;
        public Rectangle ContainerDestination;
        
        public HealthBar(Texture2D _HealthContainer, Texture2D _HealthBar)
        {
            Container = _HealthContainer;
            HealthTexture = _HealthBar;
        }
        public void Update(Vector2 Position,int Health)
        {
            HealthDestination = new Rectangle((int)Position.X - 38, (int)Position.Y - 23, Health, HealthTexture.Height);
            HealthSource = new Rectangle(0, 0, HealthTexture.Width, HealthTexture.Height);
            ContainerDestination = new Rectangle((int)Position.X - 40, (int)Position.Y - 25, Container.Width, Container.Height);
        }
        public void Draw(SpriteBatch sp)
        {

            sp.Draw(Container, ContainerDestination, Color.White);
            sp.Draw(HealthTexture, HealthDestination, HealthSource, Color.White);
        }
    }
}
