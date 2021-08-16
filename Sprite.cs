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
    /// <summary>
    /// Interface that defines what all Sprites can do
    /// </summary>
    public interface ISprite
    {
        void Draw(SpriteBatch spriteBatch);
        void Reset(BanjoAttack game);
        void Update(BanjoAttack game);
    }

    /// <summary>
    /// Parent class for all sprites implementing the sprite interface
    /// </summary>
    public abstract class Sprite : ISprite
    {
        protected Texture2D textureSprite;
        /// <summary>
        /// Gets or sets the desire 2D texture to the sprite
        /// </summary>
        public Texture2D TextureSprite
        {
            get
            {
                return this.textureSprite;
            }
            set
            {
                this.textureSprite = value;
            }
        }

        private int xCoordinate;
        /// <summary>
        /// Gets the x coordinate of the sprite
        /// </summary>
        public int XCoordinate
        {
            get
            {
                return this.xCoordinate;
            }
        }
        /// <summary>
        /// Sets the x coordinate for the sprite according to a minimum and maximum range
        /// </summary>
        /// <param name="x">The desired x coordinate as an integer</param>
        /// <returns>false if the coordinate is out of range, sets the coordinate and returns true if otherwise</returns>
        public bool Set_X_Coordinate(int x, int min, int max)
        {
            if((x >= min) || (x <= max))
            {
                this.xCoordinate = x;
                return true;
            }

            return false;
        }

        private int yCoordinate;
        /// <summary>
        /// Gets the Y coordinate of the sprite
        /// </summary>
        public int YCoordinate
        {
            get
            {
                return this.yCoordinate;
            }
        }
        /// <summary>
        /// Sets the y coordinate for the sprite according to a minimum and maximum range
        /// </summary>
        /// <param name="y">The desired y coordinate as an integer</param>
        /// <returns>false if the coordinate is out of range, sets the coordinate and returns true if otherwise</returns>
        public bool Set_Y_Coordinate(int y, int min, int max)
        {
            if ((y >= min) || (y <= max))
            {
                this.yCoordinate = y;
                spriteRectangle.Y = this.yCoordinate;
                return true;
            }

            return false;
        }

        protected Rectangle spriteRectangle;
        /// <summary>
        /// Gets the rectangle properties of the current sprite
        /// </summary>
        public Rectangle SpriteRectangle
        {
            get
            {
                return this.spriteRectangle;
            }
        }
        /// <summary>
        /// Moves a rectangle along the given axis by the given value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveRectangle(int xSpeed, int ySpeed)
        {
            this.spriteRectangle.X += xSpeed;
            this.spriteRectangle.Y += ySpeed;            
        }
        /// <summary>
        /// Sets the rectangle for the given sprite.
        /// </summary>
        /// <param name="x">x coordinate of upper-left corner</param>
        /// <param name="y">y coordinate of upper-left corner</param>
        /// <param name="maxX">x coordinate of lower-right corner</param>
        /// <param name="maxY">y coordinate of lower-right corner</param>
        /// <returns></returns>
        public bool SetSpriteRectangle(int x, int y, int maxX, int maxY)
        {
            if ((x < 0) || (y < 0) || (maxX < 0) || (maxY < 0))
                return false;

            this.spriteRectangle = new Rectangle(x, y, maxX, maxY);
            return true;
        }

        /// <summary>
        /// Creates an instance of the Sprite class
        /// </summary>
        /// <param name="texture">The texture associated with the instance</param>
        /// <param name="x">The starting x coordinate for the sprite</param>
        /// <param name="y">The starting y coordinate of the sprite</param>
        /// <param name="maxX">Maximal allowed x, usually the rightmost game window border</param>
        /// <param name="maxY">Maximal allowed y, usually the bottom of the game window border</param>
        public Sprite(Texture2D texture, int x, int y, int maxX, int maxY)
        {
            string errorMessage = "";

            if (!Set_X_Coordinate(x, 0, maxX))
                errorMessage += "X coordinate out of Range. ";

            if (!Set_Y_Coordinate(y, 0, maxY))
                errorMessage += "Y coordinate out of Range. ";

            if (!SetSpriteRectangle(x, y, maxX, maxY))
                errorMessage += "Invalid rectangle measurements.";

            if (errorMessage != "")
                throw new Exception(errorMessage);

            this.textureSprite = texture;
        }

        public Sprite(Texture2D texture, System.IO.TextReader textIn)
        {
            this.textureSprite = texture;
            this.spriteRectangle.X = int.Parse(textIn.ReadLine());
            this.spriteRectangle.Y = int.Parse(textIn.ReadLine());
            this.spriteRectangle.Width = int.Parse(textIn.ReadLine());
            this.spriteRectangle.Height = int.Parse(textIn.ReadLine());
        }

        //Life points for the Deadly Strummer
        protected int banjoLifePoints = 2;
        public int BanjoLifePoints => banjoLifePoints;
        /// <summary>
        /// Substracts 1 life point to the Deadly Strummer
        /// </summary>
        /// <returns>The new amount of lifepoints</returns>
        public int SubstractBanjoLifePoints()
        {
            return banjoLifePoints--;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Reset(BanjoAttack game);
        public abstract void Update(BanjoAttack game);
        /// <summary>
        /// Saves the class information from a stream to a file
        /// </summary>
        /// <param name="textOut">Writing system that allows for direct writing into files</param>
        public virtual void Save(System.IO.TextWriter textOut)
        {
            textOut.WriteLine(this.spriteRectangle.X);
            textOut.WriteLine(this.spriteRectangle.Y);
            textOut.WriteLine(this.spriteRectangle.Width);
            textOut.WriteLine(this.spriteRectangle.Height);
        }
    }

    /// <summary>
    /// Represents the game's background
    /// </summary>
    class BackgroundSprite : Sprite, ISprite
    {
        public BackgroundSprite(Texture2D texture, int x, int y, int maxX, int maxY) :
            base(texture, x, y, maxX, maxY)
        {

        }

        public BackgroundSprite(Texture2D texture, System.IO.TextReader textIn) :
            base(texture, textIn)
        {

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.TextureSprite, this.SpriteRectangle, Color.White);
        }
        public override void Reset(BanjoAttack game)
        {
            //remains empty
        }
        public override void Update(BanjoAttack game)
        {
            //remains empty
        }

        /// <summary>
        /// Loads the content from a stream into the game
        /// </summary>
        /// <param name="textIn">The different parameters for the object</param>
        /// <param name="game">The game where the to load the content</param>
        /// <returns></returns>
        public static BackgroundSprite Load(System.IO.TextReader textIn, BanjoAttack game)
        {
            BackgroundSprite result = null;

            try
            {                
                result = new BackgroundSprite(
                 game.Content.Load<Texture2D>("SpaceBackground"), textIn);
            }
            catch
            {
                return null;
            }

            return result;
        }
    }

    public class MovingSprite: Sprite, ISprite
    {
        protected int speed;
        public int Speed
        {
            get
            {
                return speed;
            }
        }
        /// <summary>
        /// Sets the speed movement for the sprite
        /// </summary>
        /// <param name="speed">The desired speed</param>
        /// <returns>false if the speed value is under zero, sets the speed and returns true otherwise.</returns>
        public bool SetSpeed(int speed)
        {
            if (speed < 0)
                return false;

            this.speed = speed;
            return true;
        }
        /// <summary>
        /// Creates a new instance of a moving sprite that can be the player, an enemy or a note shot. 
        /// </summary>
        /// <param name="texture">Desired texture for the sprite</param>
        /// <param name="texture">The texture associated with the instance</param>
        /// <param name="x">The starting x coordinate for the sprite</param>
        /// <param name="y">The starting y coordinate of the sprite</param>
        /// <param name="maxX">Maximal allowed x, usually the rightmost game window border</param>
        /// <param name="maxY">Maximal allowed y, usually the bottom of the game window border</param>
        /// <param name="speed">The movement speed for the sprite</param>
        public MovingSprite(Texture2D texture, int x, int y, int maxX, int maxY, int speed) :
            base(texture, x, y, maxX, maxY)
        {
            string errorMessage = "";

            if (!SetSpeed(speed))
                errorMessage += "Speed can't be negative.";

            if (errorMessage != "")
                throw new Exception(errorMessage);
        }

        public MovingSprite(Texture2D texture, System.IO.TextReader textIn) :
            base(texture, textIn)
        {
            this.speed = int.Parse(textIn.ReadLine());
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.TextureSprite, this.SpriteRectangle, Color.White);
        }
        public override void Reset(BanjoAttack game)
        {
            //remains empty
        }
        public override void Update(BanjoAttack game)
        {
            //remains empty
        }

        /// <summary>
        /// Saves the class information from a stream to a file
        /// </summary>
        /// <param name="textOut">Writing system that allows for direct writing into files</param>
        public override void Save(System.IO.TextWriter textOut)
        {
            base.Save(textOut);
            textOut.WriteLine(this.speed);
        }        
    }

    public class PlayerSprite: MovingSprite, ISprite
    {
        private int playerLives;
        public int PlayerLives => this.playerLives;
        /// <summary>
        /// Substracts one life from the player's life points pool
        /// </summary>
        /// <returns></returns>
        public int SubtractPlayerLives()
        {
            return this.playerLives--;
        }

        //Variables and properties to manage the player's score
        private int playerScore;
        public int PlayerScore => this.playerScore;
        public int SetPlayerScore(int amount)
        {
            return this.playerScore += amount;
        }

        //variables and properties to manage the highscore between games
        private int highScore = 0;
        public int HighScore => this.highScore;
        public int SetHighScore(int score)
        {
            return this.highScore = score;
        }

        /// <summary>
        /// Creates a new instance of the player character with a standard speed of 10. 
        /// </summary>
        /// <param name="texture">Desired texture for the player</param>
        /// <param name="texture">The texture associated with the instance</param>
        /// <param name="x">The starting x coordinate for the sprite</param>
        /// <param name="y">The starting y coordinate of the sprite</param>
        /// <param name="maxX">Maximal allowed x, usually the rightmost game window border</param>
        /// <param name="maxY">Maximal allowed y, usually the bottom of the game window border</param>
        public PlayerSprite(Texture2D texture, int x, int y, int maxX, int maxY) :
            base(texture, x, y, maxX, maxY, 12)
        {
            
        }

        public PlayerSprite(Texture2D texture, System.IO.TextReader textIn) :
            base(texture, textIn)
        {
            this.playerLives = int.Parse(textIn.ReadLine());
            this.playerScore = int.Parse(textIn.ReadLine());
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void Reset(BanjoAttack game)
        {
            this.playerLives = 3;
            this.playerScore = 0;

            //At the beginning of the game we place the player in the bottom middle spot of the screen
            game.Player.SetSpriteRectangle(game.GraphicsDevice.Viewport.Width / 2 - game.Player.SpriteRectangle.Width / 2,
                                           game.GraphicsDevice.Viewport.Height - game.Player.SpriteRectangle.Height,
                                           (game.Window.ClientBounds.Width / 12) / 2,
                                           (game.Window.ClientBounds.Height / 12 + 10) / 2);
        }
        public override void Update(BanjoAttack game)
        {
            //Controls for player movement, up and down
            //In diagonal movement X and Y add up and the player would move faster, in that case we reduce each axis' speed by 50%
            if (game.KeyPressed.IsKeyDown(Keys.Up) || game.KeyPressed.IsKeyDown(Keys.W))
            {
                if(game.KeyPressed.IsKeyDown(Keys.Left) || game.KeyPressed.IsKeyDown(Keys.A) ||
                   game.KeyPressed.IsKeyDown(Keys.Right) || game.KeyPressed.IsKeyDown(Keys.D))
                {
                    this.spriteRectangle.Y -= (int)(0.7f * this.Speed);
                }
                else
                {
                    this.spriteRectangle.Y -= this.Speed;
                }                
            }
                
            if (game.KeyPressed.IsKeyDown(Keys.Down) || game.KeyPressed.IsKeyDown(Keys.S))
            {
                if (game.KeyPressed.IsKeyDown(Keys.Left) || game.KeyPressed.IsKeyDown(Keys.A) ||
                   game.KeyPressed.IsKeyDown(Keys.Right) || game.KeyPressed.IsKeyDown(Keys.D))
                {
                    this.spriteRectangle.Y += (int)(0.7f * this.Speed);
                }
                else
                {
                    this.spriteRectangle.Y += this.Speed;
                }                
            }
            //Controls for player movement, left and right
            //A diagonal movement results in approximately 50% speed to account for the extra added speed per axis
            if (game.KeyPressed.IsKeyDown(Keys.Left) || game.KeyPressed.IsKeyDown(Keys.A))
            {
                if(game.KeyPressed.IsKeyDown(Keys.Up) || game.KeyPressed.IsKeyDown(Keys.W) ||
                   game.KeyPressed.IsKeyDown(Keys.Down) || game.KeyPressed.IsKeyDown(Keys.S))
                {
                    this.spriteRectangle.X -= (int)(0.7f * this.Speed);
                }
                else
                {
                    this.spriteRectangle.X -= this.Speed;
                }                
            }
                
            if (game.KeyPressed.IsKeyDown(Keys.Right) || game.KeyPressed.IsKeyDown(Keys.D))
            {
                if (game.KeyPressed.IsKeyDown(Keys.Up) || game.KeyPressed.IsKeyDown(Keys.W) ||
                   game.KeyPressed.IsKeyDown(Keys.Down) || game.KeyPressed.IsKeyDown(Keys.S))
                {
                    this.spriteRectangle.X += (int)(0.7f * this.Speed);
                }
                else
                {
                    this.spriteRectangle.X += this.Speed;
                }
            }                

            //Set the window boundaries as limits where the player can't move further
            if (this.SpriteRectangle.Top <= game.GraphicsDevice.Viewport.Y) //Very important the <= instead of ==!!!
                this.spriteRectangle.Y = game.GraphicsDevice.Viewport.Y;
            if (this.SpriteRectangle.Bottom >= game.GraphicsDevice.Viewport.Height)
                this.spriteRectangle.Y = game.GraphicsDevice.Viewport.Height - this.SpriteRectangle.Height;
            if (this.SpriteRectangle.Left <= game.GraphicsDevice.Viewport.X)
                this.spriteRectangle.X = game.GraphicsDevice.Viewport.X;
            if (this.SpriteRectangle.Right >= game.GraphicsDevice.Viewport.Width)
                this.spriteRectangle.X = game.GraphicsDevice.Viewport.Width - this.SpriteRectangle.Width;            
        }

        /// <summary>
        /// Saves the class information from a stream to a file
        /// </summary>
        /// <param name="textOut">Writing system that allows for direct writing into files</param>
        public override void Save(System.IO.TextWriter textOut)
        {
            base.Save(textOut);
            textOut.WriteLine(this.playerLives);
            textOut.WriteLine(this.playerScore);
        }

        /// <summary>
        /// Loads the content from a stream into the game
        /// </summary>
        /// <param name="textIn">The different parameters for the object</param>
        /// <param name="game">The game where the to load the content</param>
        /// <returns></returns>
        public static PlayerSprite Load(System.IO.TextReader textIn, BanjoAttack game)
        {
            PlayerSprite result = null;

            try
            {
                result = new PlayerSprite(
                 game.Content.Load<Texture2D>("accordian"), textIn);
            }
            catch
            {
                return null;
            } 

            return result;
        }
    }

    class NoteShot: MovingSprite, ISprite
    {
        /// <summary>
        /// Creates a new instance of a note shot. 
        /// </summary>
        /// <param name="texture">Desired texture for the note</param>
        /// <param name="texture">The texture associated with the instance</param>
        /// <param name="x">The starting x coordinate for the sprite</param>
        /// <param name="y">The starting y coordinate of the sprite</param>
        /// <param name="maxX">Maximal allowed x, usually the rightmost game window border</param>
        /// <param name="maxY">Maximal allowed y, usually the bottom of the game window border</param>
        public NoteShot(Texture2D texture, int x, int y, int maxX, int maxY) :
            base(texture, x, y, maxX, maxY, 20)
        {

        }

        public NoteShot(Texture2D texture, System.IO.TextReader textIn) :
            base(texture, textIn)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void Reset(BanjoAttack game)
        {
            //remains empty
        }
        public override void Update(BanjoAttack game)
        {
            this.spriteRectangle.Y -= this.Speed;
        }

        /// <summary>
        /// Saves the class information from a stream to a file
        /// </summary>
        /// <param name="textOut">Writing system that allows for direct writing into files</param>
        public override void Save(System.IO.TextWriter textOut)
        {
            base.Save(textOut);
        }

        /// <summary>
        /// Loads the content from a stream into the game
        /// </summary>
        /// <param name="textIn">The different parameters for the object</param>
        /// <param name="game">The game where the to load the content</param>
        /// <returns></returns>
        public static NoteShot Load(System.IO.TextReader textIn, BanjoAttack game)
        {
            NoteShot result = null;

            try
            {
                result = new NoteShot(
                 game.Content.Load<Texture2D>("note"), textIn);
            }
            catch
            {
                return null;
            }

            return result;
        }
    }


    class SimpleBanjo: MovingSprite, ISprite
    {
        /// <summary>
        /// Keeps track of the movement of the banjo when in touches the sides or the bottom of the screen
        /// </summary>
        public enum MovementState
        {
            LeftToRight,
            HasTouchedSide,
            HasTouchedBottom
        }

        protected MovementState banjoMovement = MovementState.LeftToRight;
        public MovementState BanjoMovement => banjoMovement;

        /// <summary>
        /// Keeps track of the horizontal direction of the banjo movement
        /// </summary>
        public enum MovementDirection
        {
            Left,
            Right
        }

        //Random variable to decide which direction the banjo moves after spawning
        //It must be static because it should be set at the same time the enemy spawns
        static Random rndDirection = new Random();
        
        //The initial value of the direction is decided randomly
        protected MovementDirection banjoDirection =
            (MovementDirection)Enum.Parse(typeof(MovementDirection), rndDirection.Next(0,2).ToString());

        int originY; //To keep track of the original y coordinate where an enemy spawned
        public int OriginY { get; set; }

        static private int banjoScore = 10;
        static public int BanjoScore => banjoScore;


        /// <summary>
        /// Creates a new instance of a simple banjo with standard speed of 7. 
        /// </summary>
        /// <param name="texture">Desired texture for the banjo</param>
        /// <param name="texture">The texture associated with the instance</param>
        /// <param name="x">The starting x coordinate for the sprite</param>
        /// <param name="y">The starting y coordinate of the sprite</param>
        /// <param name="maxX">Maximal allowed x, usually the rightmost game window border</param>
        /// <param name="maxY">Maximal allowed y, usually the bottom of the game window border</param>
        /// <param name="speed">The movement speed for the sprite</param>
        public SimpleBanjo(Texture2D texture, int x, int y, int maxX, int maxY, int originY, int speed = 4) :
            base(texture, x, y, maxX, maxY, speed)
        {
            this.originY = originY;
        }

        public SimpleBanjo(Texture2D texture, System.IO.TextReader textIn) :
            base(texture, textIn)
        {
            this.originY = int.Parse(textIn.ReadLine());
            Enum.TryParse<MovementState>(textIn.ReadLine(), out this.banjoMovement);
            Enum.TryParse<MovementDirection>(textIn.ReadLine(), out this.banjoDirection);
        }
       

        /// <summary>
        /// The banjo moves sideways, when it reaches a side boundary, it moves down until it reaches the bottom,
        /// then back up and sideways again
        /// </summary>
        /// <param name="game">The game where the behaviour will take place</param>
        protected void MoveLeftToRight(BanjoAttack game)
        {
            switch (this.banjoMovement)
            {
                case MovementState.LeftToRight:
                    if (this.banjoDirection == MovementDirection.Right)
                        this.spriteRectangle.X += this.speed;
                    if (this.banjoDirection == MovementDirection.Left)
                        this.spriteRectangle.X -= this.speed;

                    if (this.spriteRectangle.Right >= game.GraphicsDevice.Viewport.Width)
                    {
                        this.banjoDirection = MovementDirection.Left;
                        this.banjoMovement = MovementState.HasTouchedSide;
                    }

                    if ((this.spriteRectangle.Left <= 0))
                    {
                        this.banjoDirection = MovementDirection.Right;
                        this.banjoMovement = MovementState.HasTouchedSide;
                    }
                    break;

                case MovementState.HasTouchedSide:
                    this.spriteRectangle.Y += this.speed;
                    if (this.spriteRectangle.Bottom >= game.GraphicsDevice.Viewport.Height)
                    {
                        this.banjoMovement = MovementState.HasTouchedBottom;
                        //if a banjo reaches the bottom we move to GameOver state
                        game.GameState = BanjoAttack.State.GameOver;
                    }
                    break;

                case MovementState.HasTouchedBottom:
                    this.spriteRectangle.Y -= this.speed;
                    if (this.spriteRectangle.Top <= this.originY)
                    {
                        this.banjoMovement = MovementState.LeftToRight;
                    }
                    break;
            }
        }

        /// <summary>
        /// The banjo moves toward the position of the player
        /// </summary>
        /// <param name="game">The game where the behaviour will take place</param>
        protected void MoveToPlayer(BanjoAttack game)
        {
            Vector2 banjoPosition = new Vector2(this.spriteRectangle.X, this.spriteRectangle.Y);
            Vector2 playerPosition = new Vector2(game.Player.SpriteRectangle.X, game.Player.SpriteRectangle.Y);

            Vector2 movingToPlayer = Vector2.Subtract(playerPosition, banjoPosition);
            movingToPlayer.Normalize();

            banjoPosition += (movingToPlayer * this.speed);

            this.spriteRectangle.Y = (int)banjoPosition.Y;
            this.spriteRectangle.X = (int)banjoPosition.X;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void Reset(BanjoAttack game)
        {
            //remains empty
        }
        
        public override void Update(BanjoAttack game)
        {
            this.MoveLeftToRight(game);
        }

        /// <summary>
        /// Saves the class information from a stream to a file
        /// </summary>
        /// <param name="textOut">Writing system that allows for direct writing into files</param>
        public override void Save(System.IO.TextWriter textOut)
        {
            base.Save(textOut);
            textOut.WriteLine(this.originY);
            textOut.WriteLine(this.banjoMovement);
            textOut.WriteLine(this.banjoDirection);

        }

        /// <summary>
        /// Loads the content from a stream into the game
        /// </summary>
        /// <param name="textIn">The different parameters for the object</param>
        /// <param name="game">The game where the to load the content</param>
        /// <returns></returns>
        public static SimpleBanjo Load(System.IO.TextReader textIn, BanjoAttack game)
        {
            SimpleBanjo result = null;

            try
            {
                result = new SimpleBanjo(
                 game.Content.Load<Texture2D>("PlainBanjo"), textIn);
            }
            catch
            {
                return null;
            }

            return result;
        }
    }

    class HunterBanjo: SimpleBanjo, ISprite
    {
        DateTime leftRightMoving = DateTime.Now;

        static private int banjoScore = 20;
       static public new int BanjoScore => banjoScore;

        /// <summary>
        /// Creates a new instance of the hunter banjo with standard speed of 8. 
        /// </summary>
        /// <param name="texture">Desired texture for the banjo</param>
        /// <param name="texture">The texture associated with the instance</param>
        /// <param name="x">The starting x coordinate for the sprite</param>
        /// <param name="y">The starting y coordinate of the sprite</param>
        /// <param name="maxX">Maximal allowed x, usually the rightmost game window border</param>
        /// <param name="maxY">Maximal allowed y, usually the bottom of the game window border</param>
        public HunterBanjo(Texture2D texture, int x, int y, int maxX, int maxY, int originY) :
            base(texture, x, y, maxX, maxY, originY, 5)
        {

        }
        public HunterBanjo(Texture2D texture, System.IO.TextReader textIn) :
            base(texture, textIn)
        {
            TimeSpan temp = new TimeSpan();
            TimeSpan.TryParse(textIn.ReadLine(), out temp);

            this.leftRightMoving = DateTime.Now.Subtract(temp);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void Reset(BanjoAttack game)
        {
            //remains empty
        }
        public override void Update(BanjoAttack game)
        {
            if (this.leftRightMoving.AddMilliseconds(2500).CompareTo(DateTime.Now) >= 0)
            {
                this.MoveLeftToRight(game);
            }
            else
            {
                this.MoveToPlayer(game);
            }
        }

        /// <summary>
        /// Saves the class information from a stream to a file
        /// </summary>
        /// <param name="textOut">Writing system that allows for direct writing into files</param>
        public override void Save(System.IO.TextWriter textOut)
        {
            base.Save(textOut);
            textOut.WriteLine(DateTime.Now - this.leftRightMoving);
        }

        /// <summary>
        /// Loads the content from a stream into the game
        /// </summary>
        /// <param name="textIn">The different parameters for the object</param>
        /// <param name="game">The game where the to load the content</param>
        /// <returns></returns>
        public static new HunterBanjo Load(System.IO.TextReader textIn, BanjoAttack game)
        {
            HunterBanjo result = null;

            try
            {
                result = new HunterBanjo(
                 game.Content.Load<Texture2D>("AttackerBanjo"), textIn);
            }
            catch
            {
                return null;
            }

            return result;
        }
    }

    class DeadlyStrummer: SimpleBanjo, ISprite
    {
        static private int banjoScore = 50;
        static public new int BanjoScore => banjoScore;

        //variables used when a deadly strummer instance shoots to the player
        DeadlyStrummer shot;
        Vector2 shotPosition;
        Vector2 playerPosition;
        Vector2 movingToPlayer;
        private bool hasShotOnce = false;

        /// <summary>
        /// Creates a new instance of the deadly strummer with standard speed of 10. 
        /// </summary>
        /// <param name="texture">Desired texture for the banjo</param>
        /// <param name="texture">The texture associated with the instance</param>
        /// <param name="x">The starting x coordinate for the sprite</param>
        /// <param name="y">The starting y coordinate of the sprite</param>
        /// <param name="maxX">Maximal allowed x, usually the rightmost game window border</param>
        /// <param name="maxY">Maximal allowed y, usually the bottom of the game window border</param>
        public DeadlyStrummer(Texture2D texture, int x, int y, int maxX, int maxY) :
            base(texture, x, y, maxX, maxY, 0, 7)
        {
            
        }

        public DeadlyStrummer(Texture2D texture, System.IO.TextReader textIn) :
            base(texture, textIn)
        {
            this.banjoLifePoints = int.Parse(textIn.ReadLine());
        }

        public void StrumFire(BanjoAttack game)
        {
            if (this.hasShotOnce == false)
            {
                this.shot = new DeadlyStrummer(game.Content.Load<Texture2D>("Note"),
                                         this.SpriteRectangle.X + this.SpriteRectangle.Width / 2,
                                         this.SpriteRectangle.Bottom,
                                         (game.Window.ClientBounds.Width / 30) / 2,
                                         (game.Window.ClientBounds.Height / 30) / 2);

                this.shot.SetSpeed(10);

                this.shotPosition = new Vector2(this.SpriteRectangle.X + this.SpriteRectangle.Width / 2,
                                                   this.SpriteRectangle.Bottom + shot.SpriteRectangle.Height);
                this.playerPosition = new Vector2(game.Player.SpriteRectangle.X, game.Player.SpriteRectangle.Y);



                this.movingToPlayer = Vector2.Subtract(this.playerPosition, this.shotPosition);
                this.movingToPlayer.Normalize();

                game.ShotSfx.Play();

                this.hasShotOnce = true;
            }
            else
            {
                if(this.shot != null)
                {
                    this.shotPosition += (this.movingToPlayer * this.shot.Speed);

                    this.shot.spriteRectangle.Y = (int)this.shotPosition.Y;
                    this.shot.spriteRectangle.X = (int)this.shotPosition.X;

                    if (this.shot.spriteRectangle.Intersects(game.Player.SpriteRectangle))
                    {
                        this.shot = null;
                        game.GameState = BanjoAttack.State.GameOver;
                    }

                    if (this.shot == null)
                    {
                        //do nothing
                    }
                    else
                    {
                        //erase the note
                        if (this.shot.spriteRectangle.Top >= game.GraphicsDevice.Viewport.Height)
                            this.shot = null;
                    }
                }
                else
                {
                    //do nothing
                }
                
                
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(this.shot == null)
            {
                //draw nothing
            }
            else
            {
                //draw the shot
                spriteBatch.Draw(this.shot.textureSprite, this.shot.spriteRectangle, Color.White);                
            }

            base.Draw(spriteBatch);
        }
        public override void Reset(BanjoAttack game)
        {
            //remains empty
        }
        public override void Update(BanjoAttack game)
        {
            this.MoveToPlayer(game);

            this.StrumFire(game);
        }

        /// <summary>
        /// Saves the class information from a stream to a file
        /// </summary>
        /// <param name="textOut">Writing system that allows for direct writing into files</param>
        public override void Save(System.IO.TextWriter textOut)
        {
            base.Save(textOut);
            textOut.WriteLine(this.banjoLifePoints);
        }

        /// <summary>
        /// Loads the content from a stream into the game
        /// </summary>
        /// <param name="textIn">The different parameters for the object</param>
        /// <param name="game">The game where the to load the content</param>
        /// <returns></returns>
        public static new DeadlyStrummer Load(System.IO.TextReader textIn, BanjoAttack game)
        {
            DeadlyStrummer result = null;

            try
            {
                result = new DeadlyStrummer(
                 game.Content.Load<Texture2D>("DeadlyStrummer"), textIn);
            }
            catch
            {
                return null;
            }

            return result;
        }
    }
}
