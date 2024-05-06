using LZFinal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

public class HelpScene : IScene
{
    public bool ContentLoaded { get; set; }
    private Game1 game;
    private CustomSpriteFont customSpriteFont;
    private string helpText = "CONTROLS";
    private Vector2 helpTextPosition;
    private Button returnButton;
    private Texture2D buttonTexture;
    private Texture2D backgroundTexture;  /// Background image
    private Texture2D wasdTexture;
    private Texture2D spacebarTexture;
    private Texture2D mouseTexture;
    private Texture2D characterPushingTexture;
    private Texture2D dirtTexture;

    public HelpScene(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        if (ContentLoaded) return;

        /// Load the background image
        backgroundTexture = game.Content.Load<Texture2D>("terrariabackground1");

        ///load fonts
        Texture2D spriteFontTexture = game.Content.Load<Texture2D>("spritefonttable2");
        customSpriteFont = new CustomSpriteFont(spriteFontTexture, 20, 20, 15);
        ///make font 2x bigger
        customSpriteFont.SetScale(2f);

        

        /// Calculate the position for the help text
        Vector2 textSize = new Vector2(helpText.Length * customSpriteFont.CharWidth, customSpriteFont.CharHeight);
        helpTextPosition = new Vector2((game.GraphicsDevice.Viewport.Width - textSize.X) / 2, (game.GraphicsDevice.Viewport.Height - textSize.Y) / 5);

        /// Load and resize button texture
        buttonTexture = game.Content.Load<Texture2D>("button");

        ///Load wasd and spacebar
        wasdTexture = game.Content.Load<Texture2D>("wasd");
        spacebarTexture = game.Content.Load<Texture2D>("spacebar");

        ///Load character image
        characterPushingTexture = game.Content.Load<Texture2D>("push");

        ///Load dirt image
        dirtTexture = game.Content.Load<Texture2D>("dirt");

        
        /// Load the "mouse" image
        mouseTexture = game.Content.Load<Texture2D>("mouse");

        int screenWidth = game.GraphicsDevice.Viewport.Width;
        int screenHeight = game.GraphicsDevice.Viewport.Height;
        int buttonWidth = screenWidth / 5;
        int buttonHeight = screenHeight / 10;

        buttonTexture = Button.ResizeTexture(buttonTexture, buttonWidth, buttonHeight, game.GraphicsDevice);

        /// Define the new size for the images 
        int imageWidth = buttonWidth + 30; 
        int imageHeight = buttonHeight + 30;
        wasdTexture = Button.ResizeTexture(wasdTexture, imageWidth + 10, imageHeight + 30, game.GraphicsDevice);
        spacebarTexture = Button.ResizeTexture(spacebarTexture, imageWidth, imageHeight, game.GraphicsDevice);

        /// Define the new size for the "mouse" image
        int mouseWidth = imageWidth + 100;
        int mouseHeight = imageHeight + 100;
        mouseTexture = Button.ResizeTexture(mouseTexture, mouseWidth, mouseHeight, game.GraphicsDevice);

        ///Define the new size for the "dirt image
        dirtTexture = Button.ResizeTexture(dirtTexture, dirtTexture.Width + 50, dirtTexture.Height + 50, game.GraphicsDevice);


        /// Create return button in the top left corner
        Vector2 buttonPosition = new Vector2(20, 20); 
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

        /// Draw "CONTROLS" text and calculate its center
        customSpriteFont.DrawString(spriteBatch, helpText, helpTextPosition, Color.White);
        Vector2 controlsTextCenter = new Vector2(helpTextPosition.X + (helpText.Length * customSpriteFont.CharWidth) / 2, helpTextPosition.Y + customSpriteFont.CharHeight);

        /// Calculate the position for "Aiming + Shooting" text relative to the center of "CONTROLS"
        Vector2 aimingTextPosition = new Vector2(controlsTextCenter.X - 300, controlsTextCenter.Y + customSpriteFont.CharHeight + 45);
        customSpriteFont.DrawString(spriteBatch, "AIMING/SHOOTING", aimingTextPosition, Color.OrangeRed);

        /// Calculate the position for "Pushing" text relative to the center of "CONTROLS"
        Vector2 pushingTextPosition = new Vector2(controlsTextCenter.X + 400, aimingTextPosition.Y + customSpriteFont.CharHeight - 40);
        customSpriteFont.DrawString(spriteBatch, "PUSH-A/D KEY", pushingTextPosition, Color.OrangeRed);


        /// Calculate the position for the character texture 
        Vector2 characterTexturePosition = new Vector2(pushingTextPosition.X, pushingTextPosition.Y + 80);
        /// Draw the character texture
        spriteBatch.Draw(characterPushingTexture, characterTexturePosition, Color.White);

        /// Calculate the position for the dirt texture 
        Vector2 dirtTexturePosition = new Vector2(pushingTextPosition.X + 150, pushingTextPosition.Y + 80);
        /// Draw the dirt 
        spriteBatch.Draw(dirtTexture, dirtTexturePosition, Color.White);




        /// Draw "Movement" text
        Vector2 movementTextPosition = new Vector2(60, game.GraphicsDevice.Viewport.Height / 3 - 20);
        customSpriteFont.DrawString(spriteBatch, "MOVEMENT/JUMP", movementTextPosition, Color.OrangeRed);

        /// Draw "WASD" image
        Vector2 wasdImagePosition = new Vector2(90, game.GraphicsDevice.Viewport.Height / 2 - 50);
        spriteBatch.Draw(wasdTexture, wasdImagePosition, Color.White);

        /// Draw "Spacebar" image, its relative to wasd
        Vector2 spacebarImagePosition = new Vector2(90, game.GraphicsDevice.Viewport.Height / 2 + wasdTexture.Height);
        spriteBatch.Draw(spacebarTexture, spacebarImagePosition, Color.White);

        /// color for tinting (whiter and brighter)(rgb + transparency)
        Color tint = new Color(240, 240, 240, 255);

        /// Calculate the position for the "mouse" image relative to the "Aiming + Shooting" text 
        Vector2 mouseImagePosition = new Vector2(aimingTextPosition.X, aimingTextPosition.Y + customSpriteFont.CharHeight + 20);
        spriteBatch.Draw(mouseTexture, mouseImagePosition, tint);

        /// Draw the return button
        returnButton.Draw(spriteBatch);

        spriteBatch.End();
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