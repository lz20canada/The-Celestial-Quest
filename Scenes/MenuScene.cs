using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System.Linq;

///Name: Leon Zhou
///Student ID: 8897548
///Prog 2370 Final Project
///Due date: december 10th, 2023.
///Professor: Sabbir Ahmed


namespace LZFinal
{
    public class MenuScene : IScene
    {

        public bool ContentLoaded { get; set; }
        private Game1 game;
        private CustomSpriteFont customSpriteFont;
        private Texture2D titleTexture;
        private Vector2 titlePosition;
        private Rectangle titleDestination;
        private Texture2D spriteFontTexture;
        private List<Button> buttons;
        private Texture2D buttonTexture;
        private Texture2D backgroundTexture;
        

        private float tintAmount = 0.0f; /// Tint amount (0.0 to 1.0)
        private float tintSpeed = 0.2f; /// Speed of tinting
        private bool tintIncreasing = true; /// Direction of tint change

        public MenuScene(Game1 game)
        {
            this.game = game;
            buttons = new List<Button>();
        }

        public void LoadContent()
        {
            if (ContentLoaded) return;

            backgroundTexture = game.Content.Load<Texture2D>("terrariabackground1");
            /// Load the texture for the custom sprite font
            spriteFontTexture = game.Content.Load<Texture2D>("spritefonttable2");
            customSpriteFont = new CustomSpriteFont(spriteFontTexture, 20, 20, 15);
            customSpriteFont.SetScale(2f);

            /// Load the title image
            titleTexture = game.Content.Load<Texture2D>("titlename");

            /// Resize the title image
            int titleWidth = game.GraphicsDevice.Viewport.Width / 2;
            int titleHeight = game.GraphicsDevice.Viewport.Height / 4;
            titleDestination = new Rectangle(
                (game.GraphicsDevice.Viewport.Width - titleWidth) / 2, /// Centered horizontally
                20, ///padding
                titleWidth,
                titleHeight);

            /// Load button texture
            buttonTexture = game.Content.Load<Texture2D>("button");

            

            /// Initialize buttons
            InitializeButtons();

            ContentLoaded = true;
        }

        
        private void InitializeButtons()
        {
            int screenWidth = game.GraphicsDevice.Viewport.Width;
            int screenHeight = game.GraphicsDevice.Viewport.Height;
            int buttonWidth = screenWidth / 5;
            int buttonHeight = screenHeight / 10;
            int spacing = buttonHeight / 2;

            /// Resize the button texture
            buttonTexture = Button.ResizeTexture(buttonTexture, buttonWidth, buttonHeight, game.GraphicsDevice);

            /// Calculate button positions
            Vector2 playButtonPosition = new Vector2((screenWidth - buttonWidth) / 2, screenHeight / 2 - buttonHeight - spacing);
            Vector2 aboutButtonPosition = new Vector2((screenWidth - buttonWidth) / 2, screenHeight / 2);
            Vector2 helpButtonPosition = new Vector2((screenWidth - buttonWidth) / 2, screenHeight / 2 + buttonHeight + spacing);

            /// Add buttons
            buttons.Add(new Button(buttonTexture, customSpriteFont, "PLAY", playButtonPosition, game));
            buttons.Add(new Button(buttonTexture, customSpriteFont, "ABOUT", aboutButtonPosition, game));
            buttons.Add(new Button(buttonTexture, customSpriteFont, "HELP", helpButtonPosition, game));

            Button aboutButton = buttons.FirstOrDefault(b => b.Text == "ABOUT");
            Button helpButton = buttons.FirstOrDefault(b => b.Text == "HELP");
            Button playButton = buttons.FirstOrDefault(b => b.Text == "PLAY");

            aboutButton.OnClick += () =>
            {
                Exit(); /// Stop the menu music 
                game.sceneManager.ChangeScene("About");
            };

            helpButton.OnClick += () =>
            {
                Exit(); /// Stop the menu music 
                game.sceneManager.ChangeScene("Help");
            };

            playButton.OnClick += () =>
            {
                Exit(); /// Stop the menu music
                game.sceneManager.ChangeScene("Play");
            }; ;

        }

        ///notably updates the tint amount for the background color change
        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            foreach (var button in buttons)
            {
                button.Update(gameTime, mouseState);
            }


            /// Update the tint effect
            ///if the tint is increasing check if the tint amount exceeded 1.0f, if it did go back down
            if (tintIncreasing)
            {
                tintAmount += tintSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (tintAmount > 1.0f)
                {
                    tintAmount = 1.0f;
                    tintIncreasing = false;
                }
            }
            ///goes back down to 0 in rgb values. decreases tint amount
            else
            {
                tintAmount -= tintSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (tintAmount < 0.0f)
                {
                    tintAmount = 0.0f;
                    tintIncreasing = true;
                }
            }

        }
        ///dynamically draws every 0.1 something seconds
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            /// Calculate the tint color (transitioning towards red)
            Color tintColor = new Color(1.0f, 1.0f - tintAmount, 1.0f - tintAmount, 1.0f);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), tintColor);

            /// Draw the title image and buttons as before

            /// Draw the title image
            spriteBatch.Draw(titleTexture, titleDestination, Color.White);

            /// Calculate the position for the additional text
            Vector2 additionalTextSize = new Vector2("F11 to Toggle Fullscreen Mode".Length * customSpriteFont.CharWidth, customSpriteFont.CharHeight);
            Vector2 additionalTextPosition = new Vector2(
                (game.GraphicsDevice.Viewport.Width - additionalTextSize.X) / 2,
                titleDestination.Bottom + 10 /// Adjust the vertical position as needed
            );

            /// Draw the additional text
            customSpriteFont.DrawString(spriteBatch, "F11 - TOGGLE FULLSCREEN MODE", additionalTextPosition, Color.Red);
            
            /// Draw buttons
            foreach (var button in buttons)
            {
                button.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        

        public void Enter()
        {
            /// Play the music when entering the menu scene
            MediaPlayer.Play(game.MenuMusic);
            MediaPlayer.IsRepeating = true;
        }

        public void Exit()
        {
            /// Stop the music when exiting the menu scene
            MediaPlayer.Stop();
        }
    }
}