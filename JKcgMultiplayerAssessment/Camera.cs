using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class Camera
    {
        public Matrix transform;
        Viewport view;
        Vector2 centre;

        public Camera(Viewport newView)
        {
            view = newView;
        }
        public void Update (GameTime gameTime, Player player1, int ViewWidth, int ViewHeight)
        {
            centre = new Vector2((player1.Position.X + (player1.SpriteWidth / 2) - (ViewWidth/2)), player1.Position.Y + (player1.SpriteHeight / 2) - (ViewHeight/2));
            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                Matrix.CreateTranslation(new Vector3(-centre.X, -centre.Y, 0));
        }
    }
}
