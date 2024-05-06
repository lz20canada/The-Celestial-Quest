using LZFinal.Scenes.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Transactions;

namespace LZFinal
{
    public class CustomSpriteCharacter
    {
        private Game1 game; 
        private CharSpriteDirection spriteDirection = CharSpriteDirection.Right; /// Default direction is right
        private Texture2D characterSpriteSheet;
        private Dictionary<string, Rectangle[]> animationFrames;
        private int frameWidth; ///width of the sprite in pixe;ls
        private int frameHeight;///height
        private int framesPerRow;///how many sprites per row, used for getting the right cut
        private float scale = 1.0f;///scale to scale the sprite
        private string currentState; ///Each animation has a currentstate like "walking"
        private int currentFrame; ///each currentState has a currentFrame like 0,1,2 that represents the different forms 
        private int moves;///how many times the user did  a keyboard input

        private int tileSize; ///  store the tileSize
        private BlockType[,] levelBlocks; /// store levelBlocks

        private bool Jumped; ///if the user jumped with spacebar
        private bool isFalling; ///if the sprite is falling
        private float moveCooldown = 0.3f; /// Cooldown duration for movement 
        private float jumpCooldown = 0.5f; /// Cooldown duration for jumping (0.5 seconds) how long until he can jump again.
        private float moveTimer = 0f; /// Timer for tracking movement cooldown
        private float jumpTimer = 0f; /// Timer for tracking jumping cooldown
        private float fallDelay = 0f; ///delay before falling, set later 


        public Level1Scene? levelScene; /// Reference to the level scene

        ///public getter methods
        public int Moves => moves;
        public int CurrentFrame => currentFrame;
        public string CurrentState => currentState;

        ///gets the current animation of the character by checking the currentState and currentFrame. 
        public Rectangle GetCurrentAnimationFrame()
        {
            ///additional check to make sure AirTime and Walking dont have currentFrame > 0
            if (currentState == "Walking" && currentFrame > 0 || currentState == "AirTime" && currentFrame > 0)
            {
                currentFrame = 0;
            }
            return animationFrames[currentState][currentFrame];
        }

        ///Sends the spriteeffects to Level Scene. This is for adjusting the direction the character is facing.
        public SpriteEffects GetSpriteEffects()
        {
            if (spriteDirection == CharSpriteDirection.Left)
            {
                /// If character is facing Left, return FlipHorizontally
                return SpriteEffects.FlipHorizontally;
            }
            ///if character died
            else if (spriteDirection == CharSpriteDirection.Down)
            {
                return SpriteEffects.FlipVertically;

            }
            else
            {
                return SpriteEffects.None;
            }
        }
        

        ///Constructor
        public CustomSpriteCharacter(Game1 game, Texture2D texture, int frameWidth, int frameHeight, int framesPerRow, int tileSize, Level1Scene? levelScene)
        {
            this.characterSpriteSheet = texture;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.framesPerRow = framesPerRow;
            this.animationFrames = new Dictionary<string, Rectangle[]>();
            this.currentState = "Standing";
            this.currentFrame = 0;
            this.Jumped = false;
            this.jumpTimer = 0f;
            this.tileSize = tileSize; /// Store tileSize value
            this.levelScene = levelScene;
            this.levelBlocks = levelScene.LevelBlocks; ///gets the levelblocks from the level
            this.game = game;

            InitializeAnimationFrames();
        }
        ///Calls the cutting of the spritesheet using the helper method and storing it in animationFrames
        private void InitializeAnimationFrames()
        {
            /// Define the frames for each state based on their position in the sprite sheet
            animationFrames["Standing"] = new[] { GetFrame(0, 0) };
            animationFrames["AttackAnimation1"] = new[] { GetFrame(1, 0) };
            animationFrames["AttackAnimation2"] = new[] { GetFrame(2, 0) };
            animationFrames["AttackAnimation3"] = new[] { GetFrame(3, 0) };
            animationFrames["AttackAnimation4"] = new[] { GetFrame(4, 0) };
            animationFrames["AirTime"] = new[] { GetFrame(0, 1) };
            animationFrames["WalkingAnimation1"] = new[] { GetFrame(1, 1) };
            animationFrames["WalkingAnimation2"] = new[] { GetFrame(2, 1) };
            animationFrames["WalkingAnimation3"] = new[] { GetFrame(3, 1) };
            animationFrames["WalkingAnimation4"] = new[] { GetFrame(4, 1) };
            animationFrames["WalkingAnimation5"] = new[] { GetFrame(0, 2) };


            animationFrames["Walking"] = new[] {
        GetFrame(1, 1),
        GetFrame(2, 1),
        GetFrame(3, 1),
        GetFrame(4, 1),
        GetFrame(0, 2)
    };

            animationFrames["Pushing"] = new[]
            {
              GetFrame(1, 0),
             GetFrame(2, 0) ,
            GetFrame(3, 0) ,
            GetFrame(4, 0)
        };


        }

        ///Helper Method for Initializing Animation Frames
        ///Cuts the sprites out based off of their current row and col position in the animationFrames array.
        private Rectangle GetFrame(int column, int row)
        {
            /// Calculate the source rectangle of a frame based on its column and row
            return new Rectangle(column * frameWidth, row * frameHeight, frameWidth, frameHeight);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            ///if the game inputs are enabled (disabled if game over)
            if (game.IsInputEnabled)
            {
                ///calls the handle input method 
                HandleInput(keyboardState);


                /// if the user is not hitting any keys the sprite stands
                if (!keyboardState.IsKeyDown(Keys.A) && !keyboardState.IsKeyDown(Keys.D) && !keyboardState.IsKeyDown(Keys.Space))
                {
                    currentFrame = 0;
                    currentState = "Standing";
                }
                ///if they falling, do the airtime state
                if (isFalling)
                {
                    currentFrame = 0;
                    currentState = "AirTime";
                }


                /// Update movement cooldown timer
                if (moveTimer > 0)
                {
                    moveTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                /// Update jump cooldown timer
                if (jumpTimer > 0)
                {
                    jumpTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                ///if they just jumped
                if (Jumped)
                {
                    if (fallDelay >= 0.4f)///fall delay is higher if its the first jump
                    {
                        Fall(); /// Make the character fall
                        fallDelay = 0f; /// Reset the fall timer
                    }
                    else
                    {
                        /// Increment the fall timer
                        fallDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                ///if they should be falling
                else if (ShouldFall())
                {

                    /// Check if it's time to fall (every 0.25 seconds)
                    if (fallDelay >= 0.25f)
                    {
                        Fall(); /// Make the character fall
                        fallDelay = 0f; /// Reset the fall timer
                    }
                    else
                    {
                        /// Increment the fall timer
                        fallDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                }
            }
        }


        ///handles the cooldowns, takes keyboard state 
        private void HandleInput(KeyboardState keyboardState)
        {

            ///spacebar
            if (keyboardState.IsKeyDown(Keys.Space) && !Jumped && jumpTimer <= 0)
            {

                Jump();
                jumpTimer = jumpCooldown; /// Set the jump cooldown

            }
            ///"A" key
            else if (keyboardState.IsKeyDown(Keys.A) && moveTimer <= 0)
            {
                TryMoveLeft();
                IncrementAnimationFrame();
                moveTimer = moveCooldown; /// Reset the movement timer
            }
            ///"D" key
            else if (keyboardState.IsKeyDown(Keys.D) && moveTimer <= 0)
            {
                TryMoveRight();
                IncrementAnimationFrame();
                moveTimer = moveCooldown; /// Reset the movement timer

            }
            /// Placeholder for "W" (up) and "S" (down) inputs for future use
            if (keyboardState.IsKeyDown(Keys.W))
            {

            }
            if (keyboardState.IsKeyDown(Keys.S))
            {

            }
        }
        ///bool to check if the player moved left, on second glance i could make it void 
        private bool TryMoveLeft()
        {
            spriteDirection = CharSpriteDirection.Left;
            /// Loop through the levelBlocks array
            for (int y = 0; y < levelBlocks.GetLength(0); y++)
            {
                for (int x = 0; x < levelBlocks.GetLength(1); x++)
                {
                    if (levelBlocks[y, x] == BlockType.Character)
                    {
                        /// Check if the cell to the left is "Empty" and within bounds
                        if (x > 0 && levelBlocks[y, x - 1] == BlockType.Empty)
                        {
                            /// Move the character block to the left and make the current cell empty
                            levelBlocks[y, x - 1] = BlockType.Character;
                            levelBlocks[y, x] = BlockType.Empty;
                            currentState = "Walking";
                            moves++;
                            return true; /// Character successfully pushed the dirt block
                        }
                        ///if theres a crystal to the left
                        else if (x < levelBlocks.GetLength(1) - 1 && levelBlocks[y, x - 1] == BlockType.Crystal)
                        {
                            /// Replace the cell with a character
                            levelBlocks[y, x - 1] = BlockType.Character;
                            levelBlocks[y, x] = BlockType.Empty;
                            currentState = "Walking";

                            /// Play "gotcrystal" sound effect
                            this.game.CrystalObtainedSound.Play();
                            moves++;
                            return true; /// Character successfully go the crystal cell
                        }
                        ///if theresa aspike to the left
                        else if (x < levelBlocks.GetLength(1) - 1 && levelBlocks[y, x + 1] == BlockType.Spikes)
                        {
                            spriteDirection = CharSpriteDirection.Down; /// Dead character, appears upside down
                            /// Replace the cell with a character
                            levelBlocks[y, x + 1] = BlockType.Character;
                            levelBlocks[y, x] = BlockType.Empty;
                            currentState = "Walking";

                            levelScene.GameOver = true;
                            game.GameOverSound.Play();
                            moves++;
                            return true; 
                        }
                        else if (x > 1 && levelBlocks[y, x - 1] == BlockType.Dirt && levelBlocks[y, x - 2] == BlockType.Empty && !ShouldFall())
                        {
                            /// Check if the cell two spaces to the left is "Empty" (to prevent replacing non-empty blocks)
                            if (levelBlocks[y, x - 2] == BlockType.Empty)
                            {
                                ///displaces it 1 cell 
                                levelBlocks[y, x - 2] = BlockType.Dirt;
                                levelBlocks[y, x - 1] = BlockType.Character;
                                levelBlocks[y, x] = BlockType.Empty;
                                currentState = "Pushing";
                                moves++;
                                return true; /// Character successfully pushed the dirt block
                            }
                        }
                        break; /// Exit the loop once the character block is found
                    }
                }
            }
            return false; /// Character couldn't push any block
        }
        ///bool to check if the player moved right, on second glance i could make it void 
        private bool TryMoveRight()
        {
            spriteDirection = CharSpriteDirection.Right;
            /// Loop through the levelBlocks array
            for (int y = 0; y < levelBlocks.GetLength(0); y++)
            {
                for (int x = 0; x < levelBlocks.GetLength(1); x++)
                {
                    if (levelBlocks[y, x] == BlockType.Character)
                    {
                        /// Check if the cell to the right is "Empty" and within bounds
                        if (x < levelBlocks.GetLength(1) - 1 && levelBlocks[y, x + 1] == BlockType.Empty)
                        {
                            /// Move the character block to the right and make the current cell empty
                            levelBlocks[y, x + 1] = BlockType.Character;
                            levelBlocks[y, x] = BlockType.Empty;
                            currentState = "Walking";
                            moves++;
                            return true; /// Character successfully pushed the dirt block
                        }
                        /// Check if there's a crystal to the right
                        else if (x < levelBlocks.GetLength(1) - 1 && levelBlocks[y, x + 1] == BlockType.Crystal)
                        {
                            /// Replace the cell with a character
                            levelBlocks[y, x + 1] = BlockType.Character;
                            levelBlocks[y, x] = BlockType.Empty;
                            currentState = "Walking";

                            /// Play "gotcrystal" sound effect
                            this.game.CrystalObtainedSound.Play();
                            moves++;
                            return true; /// Character successfully got the crystal
                        }
                        /// Check if there's a spikes to the right
                        else if (x < levelBlocks.GetLength(1) - 1 && levelBlocks[y, x + 1] == BlockType.Spikes)
                        {
                            spriteDirection = CharSpriteDirection.Down; /// Dead character, appears upside down
                            /// Replace the cell with a character
                            levelBlocks[y, x + 1] = BlockType.Character;
                            levelBlocks[y, x] = BlockType.Empty;
                            currentState = "Walking";

                            levelScene.GameOver = true;
                            game.GameOverSound.Play();
                            /// Play "gotcrystal" sound effect
                            moves++;
                            return true; /// Character successfully got the crystal
                        }
                        ///can only push if the block to the right of the dirt is empty and also if the character should not be falling
                        else if (x < levelBlocks.GetLength(1) - 2 && levelBlocks[y, x + 1] == BlockType.Dirt && levelBlocks[y, x + 2] == BlockType.Empty && !ShouldFall())
                        {
                            /// Check if the cell two spaces to the right is "Empty" (to prevent replacing non-empty blocks)
                            if (levelBlocks[y, x + 2] == BlockType.Empty)
                            {
                                /// Displaces the dirt block one cell to the right
                                levelBlocks[y, x + 2] = BlockType.Dirt;
                                levelBlocks[y, x + 1] = BlockType.Character;
                                levelBlocks[y, x] = BlockType.Empty;
                                currentState = "Pushing";
                                moves++;
                                return true; /// Character successfully pushed the dirt block
                            }
                        }

                        break; /// Exit the loop once the character block is found
                    }
                }
            }
            return false; /// Character couldn't push any block
        }
        ///method for when the player clicks spacebar and jumps
        private void Jump()
        {
            Jumped = true;
            currentState = "AirTime";
            /// Loop through the levelBlocks array
            int characterX = -1; /// Initialize characterX outside the loop
            int characterY = -1; /// Initialize characterY outside the loop

            /// Find the current character's position
            for (int y = 0; y < levelBlocks.GetLength(0); y++)
            {
                for (int x = 0; x < levelBlocks.GetLength(1); x++)
                {
                    if (levelBlocks[y, x] == BlockType.Character)
                    {
                        characterX = x;
                        characterY = y;
                        break; /// Exit the loop once the character block is found
                    }
                }
                if (characterX != -1)
                    break; /// Exit the outer loop as well
            }

            /// Check if the cell above is "Empty" and within bounds
            if (characterY > 0 && levelBlocks[characterY - 1, characterX] == BlockType.Empty)
            {
                /// Move the character block up and make the current cell empty
                levelBlocks[characterY - 1, characterX] = BlockType.Character;
                levelBlocks[characterY, characterX] = BlockType.Empty;

                moves++;
            }

            /// Check if there's a crystal above
            else if (characterY > 0 && levelBlocks[characterY - 1, characterX] == BlockType.Crystal)
            {
                /// Replace the cell with a character, it got crystal
                levelBlocks[characterY - 1, characterX] = BlockType.Character;
                levelBlocks[characterY, characterX] = BlockType.Empty;
                currentState = "Pushing";
                moves++;
                /// Play "gotcrystal" sound effect
                this.game.CrystalObtainedSound.Play();
            }
            ///if spikes above
            else if (characterY > 0 && levelBlocks[characterY - 1, characterX] == BlockType.Spikes)
            {
                spriteDirection = CharSpriteDirection.Down; /// Dead character, appears upside down

                levelBlocks[characterY - 1, characterX] = BlockType.Character;
                levelBlocks[characterY, characterX] = BlockType.Empty;

                levelScene.GameOver = true;
                game.GameOverSound.Play();
            }



            /// Update the levelBlocks array in the level scene
            levelScene.SetLevelBlocks(levelBlocks);
        }
        ///The method that occurs when the player falls
        private void Fall()
        {
            isFalling = true;
            Jumped = false;
            currentState = "AirTime";
            int characterX = -1; /// Initialize characterX outside the loop
            int characterY = -1; /// Initialize characterY outside the loop

            /// Find the current character's position
            for (int y = 0; y < levelBlocks.GetLength(0); y++)
            {
                for (int x = 0; x < levelBlocks.GetLength(1); x++)
                {
                    if (levelBlocks[y, x] == BlockType.Character)
                    {
                        characterX = x;
                        characterY = y;
                        break; /// Exit the loop once the character block is found
                    }
                }
                if (characterX != -1)
                    break; /// Exit the outer loop as well
            }

            /// Loop through the levelBlocks array (in reverse order to simulate falling)
            for (int y = characterY + 1; y < levelBlocks.GetLength(0); y++)
            {
                /// Check if the cell below is "Empty" and within bounds
                if (levelBlocks[y, characterX] == BlockType.Empty)
                {
                    /// Move the character block down and make the current cell empty
                    levelBlocks[y, characterX] = BlockType.Character;
                    levelBlocks[y - 1, characterX] = BlockType.Empty;
                    break; ///breaking cause i want Fall() to be called multiple times with a fall cd
                }
                ///if crystal below
                else if (levelBlocks[y, characterX] == BlockType.Crystal)
                {
                    /// Replace the cell with a character
                    levelBlocks[y, characterX] = BlockType.Character;
                    levelBlocks[y - 1, characterX] = BlockType.Empty;

                    /// Play "gotcrystal" sound effect
                    this.game.CrystalObtainedSound.Play();
                    break; ///breaking cause i want Fall() to be called multiple times with a fall cd


                }
                ///If its spikes
                else if (levelBlocks[y, characterX] == BlockType.Spikes)
                {
                    spriteDirection = CharSpriteDirection.Down; ///dead char, appears upside down
                    /// Replace the cell with a character 
                    levelBlocks[y, characterX] = BlockType.Character;
                    levelBlocks[y - 1, characterX] = BlockType.Empty;
                    levelScene.GameOver = true;
                    game.GameOverSound.Play();
                    break; ///breaking cause i want Fall() to be called multiple times with a fall cd

                }

                else
                {
                    /// Stop falling if there's a non-empty block below
                    isFalling = false;
                    break;
                }
            }

            /// Update the levelBlocks array in the level scene
            levelScene.SetLevelBlocks(levelBlocks);
        }
        ///cycles through the different frames for one state. for example walking 1 walking 2 walking 3.
        ///this gives the illusion of animation
        private void IncrementAnimationFrame()
        {
            if (currentState != "Standing" && currentState != "AirTime")
            {
                /// Increment currentFrame but reset it before it exceeds the last index. 
                if (currentFrame <= animationFrames[currentState].Length - 2)
                {
                    currentFrame++;
                }
                else
                {
                    currentFrame = 0; /// Reset to the first frame
                }
            }
            else
            {
                currentFrame = 0; /// Reset frame for single-frame states
            }
        }
        ///bool to check if the character should be falling
        private bool ShouldFall()
        {

            /// Get the character's position in the levelBlocks array
            int characterX = -1; /// Initialize with an invalid value
            int characterY = -1;

            ///loop through and find the character
            for (int y = 0; y < levelBlocks.GetLength(0); y++)
            {
                for (int x = 0; x < levelBlocks.GetLength(1); x++)
                {
                    if (levelBlocks[y, x] == BlockType.Character)
                    {
                        characterX = x;
                        characterY = y;
                        break; /// Exit the loop once the character block is found
                    }
                }
            }
            ///the block below the character
            BlockType blockBelow = levelBlocks[characterY + 1, characterX];

            ///determines what blocks character can fall into
            if (blockBelow == BlockType.Empty || blockBelow == BlockType.Crystal || blockBelow == BlockType.Spikes)
            {
                return true; /// There's an Empty block below, so the character should fall
                isFalling = true;
            }

            isFalling = false;
            return false; /// There's no Empty block below, so the character should not fall
        }

        ///contains the directions the character can face
        public enum CharSpriteDirection
        {
            Left,
            Right,
            Down
        }


    }
}