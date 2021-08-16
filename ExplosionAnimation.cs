using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alien_Banjo_Attack
{
    public class ExplosionAnimation
    {
        int frameCounter;
        int totalAnimationTime = 0;
        float switchFrame;

        bool active;

        Vector2 position, amountofFrames, currentFrame = Vector2.Zero;

        Texture2D Image;

        Rectangle sourceRect;

        public int TotalAnimationTime
        {
            get
            {
                return this.totalAnimationTime;
            }
        }

        public Vector2 CurrentFrame
        {
            get
            {
                return this.currentFrame;
            }
            set
            {
                this.currentFrame = value;
            }
        }

        public Rectangle SourceRectangle
        {
            get
            {
                return this.sourceRect;
            }
            set
            {
                this.sourceRect = value;
            }
        }

        public int FrameCounter
        {
            get
            {
                return this.frameCounter;
            }
        }
        public float SwitchFrame
        {
            get
            {
                return this.switchFrame;
            }
            set
            {
                this.switchFrame = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        public int FrameWidth
        {
            get
            {
                return this.Image.Width / (int)this.amountofFrames.X;
            }
        }

        public int FrameHeight
        {
            get
            {
                return this.Image.Height / (int)this.amountofFrames.Y;
            }
        }

        public bool Active
        {
            get
            {
                return this.active;
            }
            set
            {
                this.active = value;
            }
        }

        public Texture2D AnimationImage
        {
            set
            {
                this.Image = value;
            }
        }

        public ExplosionAnimation(Texture2D spriteSheet, Vector2 position, Vector2 Frames)
        {
            this.active = true;
            this.switchFrame = 50;
            this.Image = spriteSheet;
            this.position = position;
            this.amountofFrames = Frames;
        }
       

        public void Update(GameTime gameTime)
        {            
            if (this.active == true)
            {
                this.frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                this.totalAnimationTime += this.frameCounter;
            }                
            else
                this.frameCounter = 0;

            if (this.frameCounter >= this.switchFrame)
            {
                this.frameCounter = 0;
                this.currentFrame.X += this.FrameWidth;
                if (this.currentFrame.X >= this.Image.Width)
                {
                    this.currentFrame.X = 0;
                    this.currentFrame.Y += this.FrameHeight;
                }  
            }

            if (this.totalAnimationTime >= 850)
                this.active = false;

            this.sourceRect = new Rectangle((int)this.currentFrame.X, (int)this.currentFrame.Y, this.FrameWidth, this.FrameHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(this.active == true)
                spriteBatch.Draw(this.Image, this.position, this.sourceRect, Color.White);
        }
    }
}
