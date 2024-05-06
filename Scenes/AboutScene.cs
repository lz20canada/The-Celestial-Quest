using LZFinal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class AboutScene : IScene
{
    public bool ContentLoaded { get; set; }
    private Game1 game;
    private CustomSpriteFont customSpriteFont;
    private string aboutText = "DEVELOPED BY LEON ZHOU 8897548";
    private Vector2 aboutTextPosition;
    private Button returnButton;
    private Texture2D buttonTexture;
    private Texture2D backgroundTexture;  /// Background image

    public AboutScene(Game1 game)
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

        /// Calculate the position for the about text
        Vector2 textSize = new Vector2(aboutText.Length * customSpriteFont.CharWidth, customSpriteFont.CharHeight);
        aboutTextPosition = new Vector2((game.GraphicsDevice.Viewport.Width - textSize.X) / 2, (game.GraphicsDevice.Viewport.Height - textSize.Y) / 4);

        /// Load and resize button texture
        buttonTexture = game.Content.Load<Texture2D>("button");

        int screenWidth = game.GraphicsDevice.Viewport.Width;
        int screenHeight = game.GraphicsDevice.Viewport.Height;
        int buttonWidth = screenWidth / 5;
        int buttonHeight = screenHeight / 10;

        buttonTexture = Button.ResizeTexture(buttonTexture, buttonWidth, buttonHeight, game.GraphicsDevice);

        /// Create return button in the top left corner
        Vector2 buttonPosition = new Vector2(20, 20); /// Adjust the coordinates as needed
        returnButton = new Button(buttonTexture, customSpriteFont, "RETURN", buttonPosition, game);
        returnButton.OnClick += () => game.ReturnToMainMenu();

        ContentLoaded = true;
    }

    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();
        returnButton.Update(gameTime, mouseState);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        /// Draw the background
        spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), Color.White);

        /// Draw the about text
        customSpriteFont.DrawString(spriteBatch, aboutText, aboutTextPosition, Color.White);

        /// Draw the description
        string gameDescription = "THE CELESTIAL QUEST IS A 2D PLATFORMER. YOUR OBJECTIVE IS TO COLLECT ALL THE CRYSTALS TO COMPLETE THE LEVEL.";
        int maxWidth = game.GraphicsDevice.Viewport.Width - (int)aboutTextPosition.X * 2;

        /// Split the game description into lines that fit within the maximum width
        List<string> descriptionLines = WrapText(gameDescription, customSpriteFont, maxWidth);

        /// Calculate the position for the first line of the description
        Vector2 descriptionPosition = new Vector2(aboutTextPosition.X, aboutTextPosition.Y + customSpriteFont.CharHeight + 30);

        /// Draw each line of the description
        foreach (string line in descriptionLines)
        {
            customSpriteFont.DrawString(spriteBatch, line, descriptionPosition, Color.BlueViolet);
            descriptionPosition.Y += customSpriteFont.CharHeight; /// Move to the next line
        }



        /// Draw the return button
        returnButton.Draw(spriteBatch);

        spriteBatch.End();
    }

    /// Function to wrap text into multiple lines
    private List<string> WrapText(string text, CustomSpriteFont spriteFont, int maxWidth)
    {
        string[] words = text.Split(' ');
        List<string> lines = new List<string>();
        string currentLine = "";

        foreach (string word in words)
        {
            /// Approximate the width of the current line with the new word
            int lineLength = currentLine.Length + word.Length;
            int lineWidth = lineLength * spriteFont.CharWidth;

            if (lineWidth > maxWidth)
            {
                /// The current line exceeds the maximum width, start a new line
                lines.Add(currentLine);
                currentLine = "";
            }

            /// Add the word to the current line
            currentLine += word + " ";
        }

        /// Add the last line if it's not empty
        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine);
        }

        return lines;
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}