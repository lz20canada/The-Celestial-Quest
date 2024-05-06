using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;


namespace LZFinal.Scenes.Levels
{

    public class Level1Scene : IScene
    {
        public bool ContentLoaded { get; set; }
        private Game1 game;
        private CustomSpriteFont customSpriteFont;
        private CustomSpriteFont gameOverSprite;
        private CustomSpriteFont gameComplete;
        private Button returnButton;
        private Texture2D buttonTexture;
        private Texture2D backgroundTexture;
        private Texture2D dirtTexture;
        private Texture2D crystalTexture;
        private Texture2D returnButtonTexture;
        private int highScore = 0;
        private int crystalCount;
        private CustomSpriteCharacter character;
        private Texture2D portalTexture;
        private Texture2D spikesTexture;
        private Texture2D characterSpriteSheet;
        private bool hasPlayedGameWinSound = false;

        private int tileSize = 64; /// Size of each block (e.g., dirt) 64x64

        private BlockType[,] levelBlocks;

        private int frameWidth = 43; ///adjusting this value so it properly cuts
        private int frameHeight = 64;
        private int framesPerRow = 5;


        public BlockType[,] LevelBlocks => levelBlocks; ///public getter for levelblocks



        ///tint variables
        private float tintAmount = 1f; /// Tint amount (0.0 to 1.0)
        private float tintSpeed = 2f; /// Speed of tinting
        private bool tintIncreasing = true; /// Direction of tint change
        private float gameOverDelay = 2f; /// Delay before returning to "Play" scene after game over/game win


        public bool GameOver { get; set; }
        public Level1Scene(Game1 game)
        {
            this.game = game;
        }

        public void LoadContent()
        {
            ///enable input 
            game.IsInputEnabled = true;

            ///reset the delay so it doesnt immediately go back to menu once death
            gameOverDelay = 2f;

            ///reset played game win sound
            hasPlayedGameWinSound = false;

            /// Load the background image
            backgroundTexture = game.Content.Load<Texture2D>("level1background");

            /// Load the textures and resizethem
            dirtTexture = LoadAndResizeTexture("dirt");
            crystalTexture = LoadAndResizeTexture("crystal");
            portalTexture = LoadAndResizeTexture("portal");
            spikesTexture = LoadAndResizeTexture("spikes");

            /// Load the character's sprite sheet and character
            characterSpriteSheet = game.Content.Load<Texture2D>("characterspritesheet");

            /// Initialize level layout
            InitializeLevelLayout();

            /// Should be after level is initialized
            /// Creates a character with the tileSize and levelScene args passed
            character = new CustomSpriteCharacter(game, characterSpriteSheet, frameWidth, frameHeight, framesPerRow, tileSize, this);

            Texture2D spriteFontTexture = game.Content.Load<Texture2D>("spritefonttable2");
            customSpriteFont = new CustomSpriteFont(spriteFontTexture, 20, 20, 15);

            gameOverSprite = new CustomSpriteFont(spriteFontTexture, 20, 20, 15);

            gameComplete = new CustomSpriteFont(spriteFontTexture, 20, 20, 15);

            /// Load textures for buttons
            buttonTexture = game.Content.Load<Texture2D>("button");

            /// Initialize return button properties
            InitializeReturnButton();
        }

        private Texture2D LoadAndResizeTexture(string assetName)
        {
            /// Load the original texture
            Texture2D originalTexture = game.Content.Load<Texture2D>(assetName);

            /// Resize the texture to fit a 64x64 cell
            return Button.ResizeTexture(originalTexture, tileSize, tileSize, game.GraphicsDevice);
        }

        private void InitializeReturnButton()
        {
            int screenWidth = game.GraphicsDevice.Viewport.Width;
            int screenHeight = game.GraphicsDevice.Viewport.Height;
            int returnButtonWidth = screenWidth / 15;
            int returnButtonHeight = screenHeight / 25;

            returnButtonTexture = Button.ResizeTexture(buttonTexture, returnButtonWidth, returnButtonHeight, game.GraphicsDevice);
            Vector2 returnButtonPosition = new Vector2(20, 20);
            returnButton = new Button(returnButtonTexture, customSpriteFont, "RETURN", returnButtonPosition, game);
            returnButton.OnClick += () =>
            {
                game.sceneManager.ChangeScene("Play");
                CleanupLevel();
            };
        }

        private void InitializeLevelLayout() ///this will be different for each level. I manually adding
                                             ///blocks for now
        {
            int rows = game.GraphicsDevice.Viewport.Height / tileSize; /// Number of rows that fit in the viewport height
            int cols = game.GraphicsDevice.Viewport.Width / tileSize; /// Number of columns that fit in the viewport width
            levelBlocks = new BlockType[rows, cols];

            /// Fill the bottom 4 rows with dirt blocks
            for (int y = rows - 1; y >= rows - 4; y--)
            {
                for (int x = 0; x < cols; x++)
                {
                    levelBlocks[y, x] = BlockType.Dirt;
                }
            }
            ///extra blocks
            levelBlocks[12, 5] = BlockType.Spikes;
            levelBlocks[12, 7] = BlockType.Dirt; ;
            levelBlocks[12, 18] = BlockType.Spikes;
            levelBlocks[12, 19] = BlockType.Spikes;
            levelBlocks[11, 20] = BlockType.Crystal;

            levelBlocks[11, 3] = BlockType.Dirt;
            levelBlocks[10, 4] = BlockType.Dirt;
            levelBlocks[11, 9] = BlockType.Dirt;
            levelBlocks[10, 10] = BlockType.Dirt;
            levelBlocks[9, 11] = BlockType.Dirt;
            levelBlocks[8, 13] = BlockType.Dirt;
            levelBlocks[7, 14] = BlockType.Crystal;
            levelBlocks[10, 5] = BlockType.Crystal;

            /// Find the first available spot for the character
            for (int y = rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (levelBlocks[y, x] == BlockType.Empty)
                    {
                        /// Place the character in the first available spot
                        levelBlocks[y, x] = BlockType.Character;
                        return; /// Exit the loop after placing the character
                    }
                }
            }
        }

        ///game repeatedly calls Draw. Since the LevelBlocks is being changed in CustomSpriteCharacter,
        ///Draw is different every time the character moves.
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();



            /// Calculate the tint color (transitioning towards red) for game over screen
            ///r,g,b 
            Color tintColor = new Color(1.0f, 1.0f - tintAmount, 1.0f - tintAmount, 1.0f);

            ///uses normal background if no game over
            Color backgroundColor = GameOver ? tintColor : Color.White;
            /// Update tintAmount based on tintSpeed
            if (GameOver)
            {
                tintAmount += tintSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                /// Ensure tintAmount doesn't exceed 1.0
                tintAmount = Math.Min(tintAmount, 1.0f);
            }
            else
            {
                tintAmount = 0.0f; /// Reset tintAmount if it's not game over
            }

            /// Draw the background
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), backgroundColor);
            ///set crystal count to 0 during recount
            crystalCount = 0;
            for (int y = 0; y < levelBlocks.GetLength(0); y++)
            {
                for (int x = 0; x < levelBlocks.GetLength(1); x++)
                {


                    if (levelBlocks[y, x] == BlockType.Dirt)
                    {
                        spriteBatch.Draw(dirtTexture, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                    else if (levelBlocks[y, x] == BlockType.Character)
                    {
                        /// Draw only the standing character (the first rectangle) for now
                        Rectangle sourceRectangle = character.GetCurrentAnimationFrame();

                        SpriteEffects effect = character.GetSpriteEffects();
                        spriteBatch.Draw(characterSpriteSheet, new Vector2(x * tileSize, y * tileSize + 14), sourceRectangle, Color.White, 0f, Vector2.Zero, 1.0f, effect, 0f);


                        if (GameOver)
                        {
                            /// Calculate the position for "GAME OVER" text

                            Vector2 gameOverTextPosition = new Vector2(
                                (game.GraphicsDevice.Viewport.Width) / 2,
                                (game.GraphicsDevice.Viewport.Height) / 2
                            );

                            /// Draw "GAME OVER" text in red
                            gameOverSprite.DrawString(spriteBatch, "GAME OVER", gameOverTextPosition, Color.Red);
                            gameOverSprite.SetScale(4f);

                            game.IsInputEnabled = false;
                            ///stop the Menu music
                            Exit();


                        }

                    }

                    else if (levelBlocks[y, x] == BlockType.Crystal)
                    {
                        /// Draw the "crystal" image
                        spriteBatch.Draw(crystalTexture, new Vector2(x * tileSize, y * tileSize), Color.White);
                        crystalCount++;
                    }
                    else if (levelBlocks[y, x] == BlockType.Portal)
                    {
                        /// Draw the "portal" image
                        spriteBatch.Draw(portalTexture, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                    else if (levelBlocks[y, x] == BlockType.Spikes)
                    {
                        /// Draw the "spikes" image
                        spriteBatch.Draw(spikesTexture, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                }
            }
            ///This is the check for level completion if there are no more crystals
            if (crystalCount == 0)
            {
                game.IsInputEnabled = false;
                Vector2 levelCompleteTextPosition = new Vector2(
                   (game.GraphicsDevice.Viewport.Width) / 2,
                   (game.GraphicsDevice.Viewport.Height) / 2
               );

                /// Draw "LEVEL COMPLETE" text in green
                gameComplete.DrawString(spriteBatch, "LEVEL COMPLETE", levelCompleteTextPosition, Color.Orange);
                gameComplete.SetScale(3f);

                if (!hasPlayedGameWinSound)
                {
                    game.GameWinSound.Play();
                    hasPlayedGameWinSound = true;
                }

                if (gameOverDelay > 0)
                {
                    /// Delay before changing the scene to "Play"
                    gameOverDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    
                    if (character.Moves < highScore || highScore == 0)
                    {
                        highScore = character.Moves;
                    }



                    ;
                    ///stop the Menu music
                    Exit();



                    /// Change the scene to "Play" after the delay
                    game.sceneManager.ChangeScene("Play");

                    CleanupLevel();
                }


            }

            /// Display High Score and Moves
            customSpriteFont.DrawString(spriteBatch, $"HIGH SCORE: {highScore} MOVES", new Vector2(1000, 10), Color.Orange);
            customSpriteFont.DrawString(spriteBatch, $"MOVES: {character.Moves}", new Vector2(1000, 30), Color.White);

            /// Check for game over and initiate delay if necessary
            if (GameOver)
            {

                if (gameOverDelay > 0)
                {
                    /// Delay before changing the scene to "Play"
                    gameOverDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    ///resets the game over bool and cleans up the level
                    GameOver = false;

                    /// Change the scene to "Play" after the delay
                    game.sceneManager.ChangeScene("Play");

                    CleanupLevel();

                }
            }
            /// Draw the return button
            returnButton.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState(); /// Get the current keyboard state
            MouseState mouseState = Mouse.GetState();
            returnButton.Update(gameTime, mouseState);

            if (character != null)
            {
                character.Update(gameTime, keyboardState);
            }
        }
        public void Enter()
        {
            /// Play the music when entering the menu scene
            MediaPlayer.Play(game.Level1Music);
            MediaPlayer.IsRepeating = true;
        }

        public void Exit()
        {
            /// Stop the music when exiting the menu scene
            MediaPlayer.Stop();
        }

        ///Updates the levelblocks for jump() and fall(). move right and move left use something else cause i implemented this later.
        public void SetLevelBlocks(BlockType[,] newLevelBlocks)
        {
            levelBlocks = newLevelBlocks;
        }

        ///cleans up the level
        private void CleanupLevel()
        {
            ///Restarts the level
            LoadContent();
        }
    }

    public enum BlockType
    {
        Empty,
        Dirt,
        Character,
        Crystal,
        Portal,
        Spikes
    }
}