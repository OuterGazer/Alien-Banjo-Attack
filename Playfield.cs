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
    class Playfield
    {
        private List<Sprite> spriteList;
        public List<Sprite> SpriteList
        {
            get
            {
                return spriteList;
            }
        }
        public bool AddSprite(Sprite gameElement)
        {
            this.spriteList.Add(gameElement);
            return true;
        }

        public bool RemoveSprite(Sprite gameElement)
        {
            if (gameElement.GetType() != typeof(SimpleBanjo) &&
                gameElement.GetType() != typeof(HunterBanjo) &&
                gameElement.GetType() != typeof(DeadlyStrummer) &&
                gameElement.GetType() != typeof(NoteShot))
                return false;

            this.spriteList.Remove(gameElement);
            return true;
        }

        private List<ExplosionAnimation> explosionList;
        public List<ExplosionAnimation> ExplosionList
        {
            get
            {
                return this.explosionList;
            }
        }
        public bool AddExplosion(ExplosionAnimation explosion)
        {
            this.explosionList.Add(explosion);
            return true;
        }

        /// <summary>
        /// Creates an instance of the Playfield class to keep track of all the items and behaviours on the playfield
        /// </summary>
        public Playfield()
        {
            this.spriteList = new List<Sprite>();
            this.explosionList = new List<ExplosionAnimation>();
        }

        float shootingCounter; //Variable to control the amount of shots per second so the player doesn't shoot every frame
        public const int SHOOTING_THRESHOLD = 80; //100 miliseconds, to shoot aproximately 10 times per second

        //Variables for spawning enemies
        int enemySpawntime; //The original spawn time an enemy appeared

        int enemySpawnThreshold = 1000; //Constant value in miliseconds that controls how often enemies spawn 
        Random rndSpawn = new Random(); //A random number will decide which enemy to spawn and also where

        //Keeps track if an enemy rectangle has intersected with a note rectangle;
        bool enemyWasDestroyed;

        int attractModeSpeedX = 7;
        int attractModeSpeedY = 7;

        public void AttractModeScreen(BanjoAttack game, GameTime gameTime)
        {
            game.Player.MoveRectangle(attractModeSpeedX, attractModeSpeedY);

            if (gameTime.TotalGameTime.TotalMilliseconds >= (this.shootingCounter + SHOOTING_THRESHOLD))
            {
                this.AddSprite(new NoteShot(game.Content.Load<Texture2D>("Note"),
                                          game.Player.SpriteRectangle.X + game.Player.SpriteRectangle.Width / 4,
                                          game.Player.SpriteRectangle.Top,
                                          (game.Window.ClientBounds.Width / 30) / 2,
                                          (game.Window.ClientBounds.Height / 30) / 2));

                this.shootingCounter = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (game.Player.SpriteRectangle.Right >= game.GraphicsDevice.Viewport.Width)
                attractModeSpeedX *= -1;

            if (game.Player.SpriteRectangle.Left <= 0)
                attractModeSpeedX *= -1;

            if (game.Player.SpriteRectangle.Bottom >= game.GraphicsDevice.Viewport.Height)
                attractModeSpeedY *= -1;

            if (game.Player.SpriteRectangle.Top <= game.GraphicsDevice.Viewport.Height / 2)
                attractModeSpeedY *= -1;
        }                
            
        /// <summary>
        /// Spawns a note shot from the player and adds it to the list
        /// </summary>
        /// <param name="gameTime">The specific time snapshot of the game when the method is called</param>
        public void ShootMissile(BanjoAttack game, GameTime gameTime)
        {
            //We compare the GameTime snapshot of the last shot and allow another shot only when a certain time has passed
            if (gameTime.TotalGameTime.TotalMilliseconds >= (this.shootingCounter + SHOOTING_THRESHOLD))
            {
                if (game.KeyPressed.IsKeyDown(Keys.Space) || game.KeyPressed.IsKeyDown(Keys.RightShift))
                {
                    this.AddSprite(new NoteShot(game.Content.Load<Texture2D>("Note"),
                                          game.Player.SpriteRectangle.X + game.Player.SpriteRectangle.Width / 4,
                                          game.Player.SpriteRectangle.Top,
                                          (game.Window.ClientBounds.Width / 30) / 2,
                                          (game.Window.ClientBounds.Height / 30) / 2));

                    this.shootingCounter = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    game.ShotSfx.Play();
                }
            }
        }

        /// <summary>
        /// Creates enemie banjos and adds them to the sprite list.
        /// </summary>
        public void CreateEnemy(BanjoAttack game, int x, int y)
        {
            
            switch (this.rndSpawn.Next(1, 31))
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    this.AddSprite(new SimpleBanjo(game.Content.Load<Texture2D>("PlainBanjo"),
                                                x, y,
                                                (game.Window.ClientBounds.Width / 20) / 2,
                                                (game.Window.ClientBounds.Height / 5 + 15) / 2, y));
                    break;

                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                    this.AddSprite(new HunterBanjo(game.Content.Load<Texture2D>("AttackerBanjo"),
                                                x, y,
                                                (game.Window.ClientBounds.Width / 20) / 2,
                                                (game.Window.ClientBounds.Height / 5 + 15) / 2, y));
                    break;

                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                    this.AddSprite(new DeadlyStrummer(game.Content.Load<Texture2D>("DeadlyStrummer"),
                                                x, y,
                                                (game.Window.ClientBounds.Width / 20) / 2,
                                                (game.Window.ClientBounds.Height / 5 + 15) / 2));
                    break;
            }

            game.EnemySpawnSfx.Play();
        }

        /// <summary>
        /// Spawns random enemies on the screen at random starting places.
        /// </summary>
        /// <param name="gameTime"></param>
        public void SpawnEnemies(BanjoAttack game, GameTime gameTime)
        {
            //We randomly spawn enemies every second
            if (gameTime.TotalGameTime.TotalMilliseconds >= (this.enemySpawntime + this.enemySpawnThreshold))
            {
                //Enemies can spawn throught the whole window width but will be limited to the upper fourth of it
                //To avoid that half or more of an enemy sprite spawns outside the right bound we subtract approximately half of the enemy's width to the generator
                this.CreateEnemy(game,
                                 this.rndSpawn.Next(0, (game.GraphicsDevice.Viewport.Width - (game.Window.ClientBounds.Width / 20) / 2)),
                                 this.rndSpawn.Next(0, game.GraphicsDevice.Viewport.Height / 4));

                this.enemySpawntime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }

            if ((int)gameTime.TotalGameTime.TotalMilliseconds % 5000 == 0)
                this.enemySpawnThreshold -= 10; //every 5 seconds we reduce the time for enemy spawn in 10 miliseconds
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite item in this.spriteList)
                item.Draw(spriteBatch);
            
            if(this.explosionList.Count != 0)
            {
                foreach (ExplosionAnimation item in this.explosionList)
                    item.Draw(spriteBatch);
            }            
        }

        public void Reset(BanjoAttack game)
        {
            this.spriteList.RemoveRange(2, this.spriteList.Count - 2); //We remove everything from the sprite list except the background and he player
            foreach (Sprite item in this.spriteList)
                item.Reset(game);
        }

        public void Update(BanjoAttack game, GameTime gameTime)
        {
            //Each frame we check whether note shots collide with enemies or enemies collide with the player and we remove
            //respectively the enemy sprites and the note shots from the list
            foreach (Sprite item in this.SpriteList)
            {
                if (item.GetType() == typeof(NoteShot))
                {
                    foreach (Sprite s in this.SpriteList)
                    {
                        if (item.SpriteRectangle.Intersects(s.SpriteRectangle))
                        {
                            if (s.GetType() == typeof(SimpleBanjo))
                            {
                                this.enemyWasDestroyed = true;
                                game.Player.SetPlayerScore(SimpleBanjo.BanjoScore);
                                this.AddExplosion(new ExplosionAnimation(game.Content.Load<Texture2D>("ExplosionAnimation"),
                                                                     new Vector2(s.SpriteRectangle.X + s.SpriteRectangle.Width / 2,
                                                                                 s.SpriteRectangle.Y + s.SpriteRectangle.Height / 2),
                                                                     new Vector2(5, 4)));
                                this.RemoveSprite(s);
                                break;
                            }
                            if (s.GetType() == typeof(HunterBanjo))
                            {
                                this.enemyWasDestroyed = true;
                                game.Player.SetPlayerScore(HunterBanjo.BanjoScore);
                                this.AddExplosion(new ExplosionAnimation(game.Content.Load<Texture2D>("ExplosionAnimation"),
                                                                     new Vector2(s.SpriteRectangle.X + s.SpriteRectangle.Width / 2,
                                                                                 s.SpriteRectangle.Y + s.SpriteRectangle.Height / 2),
                                                                     new Vector2(5, 4)));
                                this.RemoveSprite(s);
                                break;
                            }
                            if (s.GetType() == typeof(DeadlyStrummer))
                            {
                                if (s.BanjoLifePoints > 0)
                                    s.SubstractBanjoLifePoints();

                                if(s.BanjoLifePoints == 0)
                                {
                                    this.enemyWasDestroyed = true;
                                    game.Player.SetPlayerScore(DeadlyStrummer.BanjoScore);
                                    this.AddExplosion(new ExplosionAnimation(game.Content.Load<Texture2D>("ExplosionAnimation"),
                                                                     new Vector2(s.SpriteRectangle.X + s.SpriteRectangle.Width / 2,
                                                                                 s.SpriteRectangle.Y + s.SpriteRectangle.Height / 2),
                                                                     new Vector2(5, 4)));
                                    this.RemoveSprite(s);
                                    break;
                                }                                
                            }
                        }
                    }

                    if (this.enemyWasDestroyed == true)
                    {
                        this.RemoveSprite(item);
                        this.enemyWasDestroyed = false;                        
                        break;
                    }

                    //If the note missile leaves the screen we destroy it
                    if (item.SpriteRectangle.Bottom < 0)
                    {
                        this.RemoveSprite(item);
                        break;
                    }                        
                }

                if (item.GetType() == typeof(SimpleBanjo) ||
                    item.GetType() == typeof(HunterBanjo) ||
                    item.GetType() == typeof(DeadlyStrummer))
                {
                    if (item.SpriteRectangle.Intersects(game.Player.SpriteRectangle))
                    {
                        this.RemoveSprite(item);
                        this.AddExplosion(new ExplosionAnimation(game.Content.Load<Texture2D>("ExplosionAnimation"),
                                                                     new Vector2(item.SpriteRectangle.X + item.SpriteRectangle.Width / 2,
                                                                                 item.SpriteRectangle.Y + item.SpriteRectangle.Height / 2),
                                                                     new Vector2(5, 4)));
                        //When a banjo collides with the player we go to game over
                        game.GameState = BanjoAttack.State.GameOver;
                        break;
                    }
                }
            }

            //Here we tell every single sprite and explosion animation to update itself by calling their individual update methods
            foreach (Sprite item in this.spriteList)
                item.Update(game);

            if(this.explosionList.Count != 0)
            {
                foreach (ExplosionAnimation item in this.explosionList)
                {
                    if (item.Active == false)
                    {
                        this.explosionList.Remove(item);
                        break;
                    }

                    item.Update(gameTime);
                }
            }
        }

        public void Save(System.IO.TextWriter textOut)
        {
            textOut.WriteLine(this.spriteList.Count);

            foreach(Sprite item in this.SpriteList)
            {
                textOut.WriteLine(item.GetType().Name);
                item.Save(textOut);
            }
        }

        /// <summary>
        /// Opens a stream to saves the class information to a file
        /// </summary>
        /// <param name="filename">Filepath where the data should be saved to</param>
        public bool Save(string filename)
        {
            System.IO.TextWriter textOut = null;

            try
            {
                textOut = new System.IO.StreamWriter(filename);
                this.Save(textOut);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (textOut != null)
                    textOut.Close();
            }

            return true;
        }

        public static Playfield Load(System.IO.TextReader textIn, BanjoAttack game)
        {
            Playfield result = new Playfield();

            try
            {
                int count = int.Parse(textIn.ReadLine());

                for(int i = 0; i < count; i++)
                {
                    string className = textIn.ReadLine();

                    switch (className)
                    {
                        case "BackgroundSprite":
                            result.AddSprite(BackgroundSprite.Load(textIn, game));
                            break;

                        case "PlayerSprite":
                            result.AddSprite(PlayerSprite.Load(textIn, game));
                            break;

                        case "NoteShot":
                            result.AddSprite(NoteShot.Load(textIn, game));
                            break;

                        case "SimpleBanjo":
                            result.AddSprite(SimpleBanjo.Load(textIn, game));
                            break;

                        case "HunterBanjo":
                            result.AddSprite(HunterBanjo.Load(textIn, game));
                            break;

                        case "DeadlyStrummer":
                            result.AddSprite(DeadlyStrummer.Load(textIn, game));
                            break;

                        default:
                            return null;
                    }                    
                }
            }
            catch
            {
                return null;
            }

            return result;
        }

        public static Playfield Load(string filename, BanjoAttack game)
        {
            Playfield result = null;
            //System.IO.Stream stream = null;
            System.IO.TextReader textIn = null;
            try
            {
                textIn = new System.IO.StreamReader(filename);
                //stream = TitleContainer.OpenStream(filename);
                result = Playfield.Load(textIn, game);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (textIn != null)
                    textIn.Close();
            }

            return result;
        }
    }
}
