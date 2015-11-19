using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKcgMultiplayerAssessment
{
    public class AnimatedSprite
    {
        //sprite texture and position
        List<Texture2D> spriteImage;

        //the source of our image within the sprite sheet to draw
        public Rectangle SourceRectangle;
        public Rectangle DestinationRectangle;


        public SpriteEffects _effect;
        public Vector2 Position;
        
        public List<Texture2D> SpriteImage
        {
            get { return spriteImage; }
            set { spriteImage = value; }
        }


        //the number of frames in the sprite sheet
        //the current fram in the animation
        //the time between frames
        public int numberOfFrames = 0;
        public int currentFrame = 0;
        public int mililsecondsBetweenFrames = 100;
        float timer = 0f;

        //the width and height of our texture
        int spriteWidth = 0;

        public int SpriteWidth
        {
            get { return spriteWidth; }
            set { spriteWidth = value; }
        }
        int spriteHeight = 0;


        public int SpriteHeight
        {
            get { return spriteHeight; }
            set { spriteHeight = value; }
        }

        public AnimatedSprite(List<Texture2D> texture, Vector2 userPosition, int framecount)
        {
            spriteImage = texture;
            Position = userPosition;
            numberOfFrames = framecount;
            spriteHeight = SpriteImage[0].Height;
            spriteWidth = SpriteImage[0].Width / framecount;
            SourceRectangle = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
        }
        public AnimatedSprite()
        { }


        public void UpdateAnimation(GameTime gametime)
        {
            timer += (float)gametime.ElapsedGameTime.Milliseconds;

            //if the timer is greater then the time between frames, then animate
            if (timer > mililsecondsBetweenFrames)
            {
                //moce to the next frame
                currentFrame++;

                //if we have exceed the number of frames
                if (currentFrame > numberOfFrames - 1)
                {
                    currentFrame = 0;
                }
                //reset our timer
                timer = 0f;
            }
            //set the source to be the current frame in our animation
            SourceRectangle = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, float scale, Vector2 origin)
        {
            spriteBatch.Draw(texture, Position, SourceRectangle, Color.White, 0f, origin, scale, _effect, 0f);

        }
    }
}
