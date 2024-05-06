using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZFinal
{
    public class LevelSelectScene : IScene
    {
        public bool ContentLoaded { get; set; }
        private Game1 game;
        private List<Button> levelButtons;
        private Button returnButton;
        private Texture2D levelSelectTexture;
        private Texture2D buttonTexture;
        private CustomSpriteFont customSpriteFont;
        private Texture2D backgroundTexture;
        private Texture2D returnButtonTexture;

        public LevelSelectScene(Game1 game)
        {
            this.game = game;
        }

        public void LoadContent()
        {
            if (ContentLoaded) return;

            /// Load the background image
            backgroundTexture = game.Content.Load<Texture2D>("terrariabackground1");

            Texture2D spriteFontTexture = game.Content.Load<Texture2D>("spritefonttable2");
            customSpriteFont = new CustomSpriteFont(spriteFontTexture, 20, 20, 15);
            customSpriteFont.SetScale(2f);

            /// Load textures for buttons
            buttonTexture = game.Content.Load<Texture2D>("button");
           

            /// Create a list to store level buttons
            levelButtons = new List<Button>();

            /// Calculate the button size based on screen dimensions
            int buttonWidth = game.GraphicsDevice.Viewport.Width / 15;

            int buttonHeight = game.GraphicsDevice.Viewport.Height / 10;
            int buttonSpacing = 10;  ///button spacing

            /// Loop to create and position the level buttons
            for (int i = 1; i <= 10; i++)
            {
                /// Calculate the position of each level button with spacing
                int row = (i - 1) / 5; /// Assuming 5 buttons per row
                int col = (i - 1) % 5;
                int x = col * (buttonWidth + buttonSpacing) + (game.GraphicsDevice.Viewport.Width - 5 * (buttonWidth + buttonSpacing)) / 2;
                int y = row * (buttonHeight + buttonSpacing) + game.GraphicsDevice.Viewport.Height / 3;

                /// Resize the "button" texture for the level button to fit the text
                Texture2D resizedButtonTexture = Button.ResizeTexture(buttonTexture, buttonWidth, buttonHeight, game.GraphicsDevice);

                /// Create the level button
                Button levelButton = new Button(resizedButtonTexture, customSpriteFont, i.ToString(), new Vector2(x, y), game);

                /// Add click event for level selection
                int levelNumber = i; /// Capture the level number for the click event
                levelButton.OnClick += () =>
                {
                    string sceneName = $"Level{levelNumber}";
                    if (game.sceneManager.TryGetScene(sceneName, out IScene levelScene))
                    {
                        game.sceneManager.ChangeScene(sceneName);
                        levelScene.Enter();
                    }
                };

                /// Add the level button to your list of buttons
                levelButtons.Add(levelButton);
            }

            ///this isjust for return button
            int screenWidth = game.GraphicsDevice.Viewport.Width;
            int screenHeight = game.GraphicsDevice.Viewport.Height;
            int returnButtonWidth = screenWidth / 5;
            int returnButtonHeight = screenHeight / 10;

            returnButtonTexture = Button.ResizeTexture(buttonTexture, returnButtonWidth, returnButtonHeight, game.GraphicsDevice);

            Vector2 buttonPosition = new Vector2(20, 20);
            returnButton = new Button(returnButtonTexture, customSpriteFont, "RETURN", buttonPosition, game);
            returnButton.OnClick += () => game.ReturnToMainMenu();

            ContentLoaded = true;
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            /// Update level buttons
            foreach (var levelButton in levelButtons)
            {
                levelButton.Update(gameTime, mouseState);
            }

            /// Update the return button
            returnButton.Update(gameTime, mouseState);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            /// Draw the background
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), Color.White);

            /// Calculate the position for "LEVEL SELECT" text
            string levelSelectText = "LEVEL SELECT";
            int textWidth = levelSelectText.Length * customSpriteFont.CharWidth; /// CharWidth represents the width of a character
            int screenWidth = game.GraphicsDevice.Viewport.Width;
            Vector2 levelSelectTextPosition = new Vector2((screenWidth - textWidth) / 2, 40);

            /// Draw "LEVEL SELECT" text
            customSpriteFont.DrawString(spriteBatch, levelSelectText, levelSelectTextPosition, Color.White);

            /// Draw level buttons
            foreach (var levelButton in levelButtons)
            {
                levelButton.Draw(spriteBatch);
            }

            /// Draw the return button
            returnButton.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void Enter()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }
    }
}
