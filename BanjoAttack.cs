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

namespace Alien_Banjo_Attack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BanjoAttack : Microsoft.Xna.Framework.Game
    {
        //General game objects to control graphics, in-game text, sprite drawing and keyboard presses
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont gameText;
        KeyboardState keyPressed; //Allow for interaction with the game with a keyboard
        KeyboardState lastKeyPressed; //Allow for creating events like pausing the game, where we need to find the exact moment the player release a key
        public KeyboardState KeyPressed
        {
            get
            {
                return keyPressed;
            }
        }

        /// <summary>
        /// Enumeration that represents the different possible gamestates
        /// </summary>
        public enum State
        {
            AttractMode,
            PlayingGame,
            GamePaused,
            GameOver
        }

        private State gameState = State.AttractMode;
        public State GameState
        {
            get { return this.gameState; }
            set { this.gameState = value; }
        }

        int counter = 0; //Keeps track of the time that we stop the game when entering the GameOver state
        const int PAUSE_TIME = 60; //maximum time that we stop the game in the GameOver state. We count 1 every update cycle, so seconds are multiples of 60

        //Reference to all the music and SFX for the game
        SoundEffect backgroundMusic;
        SoundEffectInstance music;

        SoundEffect enemySpawnSfx;
        public SoundEffect EnemySpawnSfx => this.enemySpawnSfx;

        SoundEffect shotSfx;
        public SoundEffect ShotSfx => this.shotSfx;

        SoundEffect gameOverSfx;
        SoundEffectInstance gameOver;
        bool hasPlayedOnce; //To make it play only once

        //Reference to all the items in the playfield that should be updated and drawn
        Playfield gameElements;

        //Class instance representing the background       
        BackgroundSprite background;

        //Class instance for the player character, in this case an accordian
        PlayerSprite player;
        public PlayerSprite Player => player; //Read-Only property to access the player instance outside of the BanjoAttack class


        public BanjoAttack()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            //Creates a new sprite font to provide in-game text
            this.gameText = Content.Load<SpriteFont>("GameFont");

            //We first try load a saved game
            if (Playfield.Load("Alien_Banjo_Attack_Save.txt", this) != null)
            {
                this.gameElements = Playfield.Load("Alien_Banjo_Attack_Save.txt", this);
                this.gameState = State.GamePaused;
            }

            //If there is no save game we start a new one
            if (this.gameElements == null)
            {
                this.gameElements = new Playfield();

                //If we start a new game (sprite list is empty)
                //Load and add player and background the the sprite list so they update and draw themselves
                this.gameElements.AddSprite(new BackgroundSprite(Content.Load<Texture2D>("SpaceBackground"),
                                                  0, 0,
                                                  Window.ClientBounds.Width,
                                                  Window.ClientBounds.Height));

                this.gameElements.AddSprite(new PlayerSprite(Content.Load<Texture2D>("accordian"),
                                          Window.ClientBounds.Width / 2,
                                          Window.ClientBounds.Height / 2,
                                          (Window.ClientBounds.Width / 12) / 2,
                                          (Window.ClientBounds.Height / 12 + 10)/2));
            }

            //Regardless of saved or new game, we assign the first 2 items in the list to the background and the player
            //due to how we have structured the code
            this.background = (BackgroundSprite)this.gameElements.SpriteList[0];
            this.player = (PlayerSprite)this.gameElements.SpriteList[1];

            //We load all the music and SFX tracks
            this.backgroundMusic = Content.Load<SoundEffect>("mixkit-game-level-music-689");
            this.music = this.backgroundMusic.CreateInstance();
            this.music.IsLooped = true;

            this.enemySpawnSfx = Content.Load<SoundEffect>("mixkit-small-hit-in-a-game-2072");            

            this.shotSfx = Content.Load<SoundEffect>("mixkit-tribal-dry-drum-558");

            this.gameOverSfx = Content.Load<SoundEffect>("mixkit-arcade-space-shooter-dead-notification-272");
            this.gameOver = this.gameOverSfx.CreateInstance();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }




        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Get the key pressed every frame
            this.keyPressed = Keyboard.GetState();            

            // Allows the game to and exit
            if (this.keyPressed.IsKeyDown(Keys.Escape))
                this.Exit();

            //Switch constructions that controls the game behaviour for the different states
            switch (this.gameState)
            {
                case State.AttractMode:
                    //We start playing background music
                    this.music.Play();                    

                    //We move randomly the player and make it shoot
                    this.gameElements.AttractModeScreen(this, gameTime);

                    //we call the update method in the Playfield class so that every item in the playfield updates itself in a loop
                    this.gameElements.Update(this, gameTime);


                    if (keyPressed.GetPressedKeys().Length != 0) //We change to playing state when any key is pressed
                    {
                        this.counter = 0;
                        this.gameElements.Reset(this);
                        this.hasPlayedOnce = false; //We set the bool controlling the gameOver SFX again to false so it can play
                        this.gameState = State.PlayingGame;
                    }                        
                    break;

                case State.PlayingGame:
                    //Method that allows the player to shoot missiles with the limitation of a set interval between shots
                    this.gameElements.ShootMissile(this, gameTime);

                    //Method that spawns enemies at a set time interval
                    this.gameElements.SpawnEnemies(this, gameTime);

                    //we call the update method in the Playfield class so that every item in the playfield updates itself in a loop
                    this.gameElements.Update(this, gameTime);

                    //When enemies either collide with the player or the bottom of the screen we go to GameOver state
                    //Relevant code is in the following methods
                    //Playfield.Update() and (Sprite) SimpleBanjo.MoveLeftToRight

                    //Pressing P will pause the game
                    if (this.lastKeyPressed.IsKeyDown(Keys.P) && !this.keyPressed.IsKeyDown(Keys.P))
                        this.gameState = State.GamePaused;

                    //Pressing G will save the game and quit
                    if(this.lastKeyPressed.IsKeyDown(Keys.G) && !this.keyPressed.IsKeyDown(Keys.G))
                    {
                        this.gameElements.Save("Alien_Banjo_Attack_Save.txt");
                        this.Exit();
                    }                    
                    break;

                case State.GamePaused:
                    //We pause the music
                    this.music.Pause();

                    //Pressing P will unpause the game
                    if (this.lastKeyPressed.IsKeyDown(Keys.P) && !this.keyPressed.IsKeyDown(Keys.P))
                    {
                        this.gameState = State.PlayingGame;
                        this.music.Resume();
                    }                        
                    break;

                case State.GameOver:
                    //We pause background music and play the dead SFX
                    this.music.Pause();                    
                    if(this.hasPlayedOnce == true)
                    {
                        //do nothing, this prevents the SFX from playing every frame repeatedly
                    }
                    else
                    {
                        this.gameOver.Play();
                        this.hasPlayedOnce = true;
                    }


                    //We stop the game for 60 counts of Update(), 60/60 times is update called per second = 1 seconds
                    this.counter++;
                    if(this.player.PlayerLives > 1)
                    {
                        if (this.counter >= PAUSE_TIME)
                        {
                            this.counter = 0;
                            this.player.SubtractPlayerLives();
                            this.gameState = State.PlayingGame;
                            this.hasPlayedOnce = false;
                            this.music.Resume();
                        }
                    }
                    else
                    {
                        if(this.counter >= 300) //we pause for 5 seconds to show the player the score before resetting the game
                        {                            
                            this.gameState = State.AttractMode;
                        }

                        if (this.player.PlayerScore > this.player.HighScore)
                            this.Player.SetHighScore(this.player.PlayerScore);
                    }
                    
                    break;
            }

            //We update the lastKeyPressed variable to the keyboard value of this frame
            this.lastKeyPressed = this.keyPressed;

            //We update explosion animations if there is any
            

            base.Update(gameTime);
        }

        //Strings of text that will be shown to the player at different times during the game
        string attractModeText_1 = "Press arrow keys or WASD keys to move";
        string attractModeText_2 = "Press the spacebar to shoot";
        string attractModeText_3 = "Press any key to start the game";
        string attractModeText_4 = "Press P to pause, G to save and quit the game";
        string attractModeText_5 = "Press Esc to quit the game without saving";
        string attractModeText_6 = "Current highscore is: ";
        string gamePausedText = "Game paused. Press P to resume";
        string gameOverText_1 = "You have lost a life!";
        string gameOverText_2 = "Game Over";
        string gameOverText_3 = "Your final score is: ";
        string gameOverText_4 = "You have set a new record!";
        string gameOverText_5 = "The game will restart in 5 seconds";

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();
            
            //Switch constructions that controls the drawing for the different states
            switch (this.gameState)
            {
                case State.AttractMode:
                    this.gameElements.Draw(this.spriteBatch);

                    //We show a static text explaining the controls during the attract screen
                    this.spriteBatch.DrawString(this.gameText, this.attractModeText_1,
                                                    new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                this.gameText.MeasureString(this.attractModeText_1).X / 2,
                                                                0 +
                                                                this.gameText.MeasureString(this.attractModeText_1).Y / 2),
                                                    Color.White);
                    this.spriteBatch.DrawString(this.gameText, this.attractModeText_2,
                                                    new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                this.gameText.MeasureString(this.attractModeText_2).X / 2,
                                                                0 +
                                                                3 * this.gameText.MeasureString(this.attractModeText_2).Y / 2),
                                                    Color.White);
                    this.spriteBatch.DrawString(this.gameText, this.attractModeText_4,
                                                    new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                this.gameText.MeasureString(this.attractModeText_4).X / 2,
                                                                0 +
                                                                5 * this.gameText.MeasureString(this.attractModeText_4).Y / 2),
                                                    Color.White);
                    this.spriteBatch.DrawString(this.gameText, this.attractModeText_5,
                                                    new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                this.gameText.MeasureString(this.attractModeText_5).X / 2,
                                                                0 +
                                                                7 * this.gameText.MeasureString(this.attractModeText_5).Y / 2),
                                                    Color.White);

                    //We show a blinking text inviting the player to press any key to start the game
                    if ((gameTime.TotalGameTime.Seconds % 2) == 0)
                    {
                        this.spriteBatch.DrawString(this.gameText, this.attractModeText_3,
                                                    new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                this.gameText.MeasureString(this.attractModeText_3).X / 2,
                                                                GraphicsDevice.Viewport.Height / 2 -
                                                                this.gameText.MeasureString(this.attractModeText_3).Y / 2),
                                                    Color.White);

                        this.spriteBatch.DrawString(this.gameText, this.attractModeText_6 + this.player.HighScore,
                                                    new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                this.gameText.MeasureString(this.attractModeText_6 + this.player.HighScore).X / 2,
                                                                GraphicsDevice.Viewport.Height / 2 -
                                                                3 * this.gameText.MeasureString(this.attractModeText_6 + this.player.HighScore).Y / 2),
                                                    Color.White);
                    }                        
                    break;

                case State.PlayingGame:
                    this.gameElements.Draw(this.spriteBatch);

                    //Shows the current score and lifes left of the player
                    this.spriteBatch.DrawString(this.gameText, "Score: " + this.player.PlayerScore + " Lives: " + this.player.PlayerLives,
                                                    new Vector2(10,
                                                                GraphicsDevice.Viewport.Height -
                                                                this.gameText.
                                                                MeasureString("Score: " + this.player.PlayerScore + " Lives: " + this.player.PlayerLives).Y - 5),
                                                    Color.White);
                    break;

                case State.GamePaused:
                    this.gameElements.Draw(this.spriteBatch);

                    this.spriteBatch.DrawString(this.gameText, this.gamePausedText,
                                                    new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                this.gameText.MeasureString(this.gamePausedText).X / 2,
                                                                GraphicsDevice.Viewport.Height / 2 -
                                                                this.gameText.MeasureString(this.gamePausedText).Y / 2),
                                                    Color.White);
                    break;

                case State.GameOver:
                    this.gameElements.Draw(this.spriteBatch);

                    if (this.player.PlayerLives > 1)
                    {
                        
                        this.spriteBatch.DrawString(this.gameText, this.gameOverText_1,
                                                        new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                    this.gameText.MeasureString(this.gameOverText_1).X / 2,
                                                                    GraphicsDevice.Viewport.Height / 2 -
                                                                    this.gameText.MeasureString(this.gameOverText_1).Y / 2),
                                                        Color.White);
                    }
                    else
                    {
                        this.spriteBatch.DrawString(this.gameText, this.gameOverText_2,
                                                        new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                    this.gameText.MeasureString(this.gameOverText_2).X / 2,
                                                                    GraphicsDevice.Viewport.Height / 2 -
                                                                    this.gameText.MeasureString(this.gameOverText_2).Y / 2 - 20),
                                                        Color.White);

                        this.spriteBatch.DrawString(this.gameText, this.gameOverText_3 + this.player.PlayerScore,
                                                        new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                    this.gameText.
                                                                    MeasureString(this.gameOverText_3 + this.player.PlayerScore).X / 2,
                                                                    GraphicsDevice.Viewport.Height / 2 +
                                                                    this.gameText.
                                                                    MeasureString(this.gameOverText_3 + this.player.PlayerScore).Y/ 2 - 10),
                                                        Color.White);
                        if(this.player.PlayerScore == this.player.HighScore)
                        {
                            this.spriteBatch.DrawString(this.gameText, this.gameOverText_4,
                                                        new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                    this.gameText.
                                                                    MeasureString(this.gameOverText_4).X / 2,
                                                                    GraphicsDevice.Viewport.Height / 2 +
                                                                    3 * this.gameText.
                                                                    MeasureString(this.gameOverText_4).Y / 2),
                                                        Color.White);

                            this.spriteBatch.DrawString(this.gameText, this.gameOverText_5,
                                                            new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                        this.gameText.
                                                                        MeasureString(this.gameOverText_5).X / 2,
                                                                        GraphicsDevice.Viewport.Height / 2 +
                                                                        5 * this.gameText.
                                                                        MeasureString(this.gameOverText_5).Y / 2),
                                                            Color.White);
                        }
                        else
                        {
                            this.spriteBatch.DrawString(this.gameText, this.gameOverText_5,
                                                            new Vector2(GraphicsDevice.Viewport.Width / 2 -
                                                                        this.gameText.
                                                                        MeasureString(this.gameOverText_5).X / 2,
                                                                        GraphicsDevice.Viewport.Height / 2 +
                                                                        3 * this.gameText.
                                                                        MeasureString(this.gameOverText_5).Y / 2),
                                                            Color.White);
                        }                        
                    }
                    
                    break;
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
