using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class PowerBar
    {

        public Texture2D Container;
        public Texture2D PowerTexture;
        public Rectangle PowerDestination;
        public Rectangle PowerSource;
        public Rectangle ContainerDestination;
        public PowerBar(Texture2D _PowerContainer, Texture2D _PowerBar)
        {
            Container = _PowerContainer;
            PowerTexture = _PowerBar;
        }
        public void Update(Vector2 Position,int ShotPower)
        {
            PowerDestination = new Rectangle((int)Position.X - 38, (int)Position.Y + 125, (int)ShotPower, PowerTexture.Height);
            PowerSource = new Rectangle(0, 0, PowerTexture.Width, PowerTexture.Height);
            ContainerDestination = new Rectangle((int)Position.X - 40, (int)Position.Y + 123, Container.Width, Container.Height);
        }
        public void Draw(SpriteBatch sp)
        {

            sp.Draw(Container, ContainerDestination, Color.White);
            sp.Draw(PowerTexture, PowerDestination, PowerSource, Color.White);
        }
    }
}
