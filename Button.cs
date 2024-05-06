using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZFinal
{
    public class Button
    {
        /// Properties
        public Vector2 Position { get; private set; } /// The position of the button.
        public string Text { get; private set; } /// The text displayed on the button.
        public Rectangle Bounds { get; private set; } /// The bounding rectangle of the button.
        public Action OnClick { get; set; } /// A delegate representing the action to perform when the button is clicked.

        /// Fields
        private Texture2D texture; /// The texture (image) used for the button.
        private CustomSpriteFont customFont; /// A custom font for rendering text.
        private Color hoverColor = Color.Orange; /// The color of the button when hovered over.
        private bool isHovered; /// A flag indicating if the mouse pointer is hovering over the button.
        private int CharWidth; /// The width of a character in the font.
        private int CharHeight; /// The height of a character in the font.
        private float originalY; /// The original Y-coordinate of the button.
        private float floatAmplitude = 5.0f; /// The amplitude of a floating effect.
        private float floatSpeed = 2.0f; /// The speed of the floating effect.
        private Game1 game; /// A reference to the main game class.
        private Vector2 textOffset; /// Offset for centering text when scaled.

        /// Constructor
        public Button(Texture2D texture, CustomSpriteFont customFont, string text, Vector2 position, Game1 game)
        {
            this.texture = texture;
            this.customFont = customFont;
            this.game = game;
            Text = text;
            Position = position;
            Bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            originalY = position.Y;

            /// Set the action for the OnClick event (plays a button click sound)
            OnClick += () => this.game.ButtonClickSound.Play();
        }

        /// Update method
        public void Update(GameTime gameTime, MouseState mouseState)
        {

            //if the rectangle contains the position where the mouse is, isHovered is true
            isHovered = Bounds.Contains(mouseState.X, mouseState.Y);

            //if its hovering and the user clicks the left button start the onclick
            if (isHovered && mouseState.LeftButton == ButtonState.Pressed)
            {
                /// Invoke the OnClick action when the button is clicked.
                OnClick?.Invoke();
            }

            /// Perform a floating effect on the button's Y-coordinate.
            /// scales the time based on the floatspeed set. 
            float time = (float)gameTime.TotalGameTime.TotalSeconds * floatSpeed;

            //makes the time value oscillate. aka Math.Sin makes the value of time a wave function.
            //this makes the speed look like the waves of a sin function, decelerating and accelerating
            //without Math.sin it would just move up and down without 

            float delta = floatAmplitude * (float)Math.Sin(time);
            Position = new Vector2(Position.X, originalY + delta);
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Bounds.Width, Bounds.Height);
        }

        /// Draw method
        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = isHovered ? hoverColor : Color.White;
            spriteBatch.Draw(texture, Bounds, isHovered ? hoverColor : Color.White);

            /// Calculate the total width of the text (including the scaled width)
            int textWidth = (int)(CalculateTextWidth(Text, customFont) * customFont.Scale);

            /// Center the text on the button (adjusting for scale)
            Vector2 textPosition = new Vector2(
                Bounds.Center.X - textWidth / 2 + textOffset.X,
                Bounds.Center.Y - customFont.CharHeight / 2 + textOffset.Y);

            /// Draw the text on the button.
            customFont.DrawString(spriteBatch, Text, textPosition, Color.White);
        }

        /// Helper method to calculate the total width of the text, takes the width of each character 
        ///in the sprite sheet, adds them up to a total
        private int CalculateTextWidth(string text, CustomSpriteFont customFont)
        {
            int totalWidth = 0;
            foreach (char c in text)
            {
                if (customFont.CharacterMap.TryGetValue(c, out Rectangle charRect))
                {
                    totalWidth += charRect.Width;
                }
            }
            return totalWidth;
        }



        /// Static method to resize a Texture2D. width and height represent the new desired dimensions <summary>
        /// can be called without needing to instantiate the button. Used not only for buttons essentially.
        /// </summary>
        /// <param name="originalTexture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public static Texture2D ResizeTexture(Texture2D originalTexture, int width, int height, GraphicsDevice graphicsDevice)
        {
            /// Create a new empty texture with the desired dimensions
            Texture2D resizedTexture = new Texture2D(graphicsDevice, width, height);

            /// Data for resizing [pixel * pixel] Images are an array of colors.
            /// initializes a data array which size is equal to the desired width * height.
            Color[] data = new Color[width * height];
            Color[] originalData = new Color[originalTexture.Width * originalTexture.Height];
            //gets the color data from the original texture and stores it in the originalData array.
            originalTexture.GetData(originalData);

            /// Scale factor
            /// ratio of original texture over the new texture. aka 1.2 times rowsize / height, or 0.8 times more rowsize / height (0.8 times smaller)
            float rowRatio = ((float)originalTexture.Height) / height;
            float colRatio = ((float)originalTexture.Width) / width;

            ///for every pixel height in the desired image
            for (int row = 0; row < height; row++)
            {
                //for every pixel width in the desired image
                for (int col = 0; col < width; col++)
                {
                    //int because pixels must be whole numbers
                    //lets say col and row are both 1 in the loop, original texture width is 100.
                    //i want the width to be 30.
                    //after calculations that means data[1 * 30 + 1] = originaldata[3*100+3]
                    //or data[31] = originaldata[303]
                    //In this method, for each pixel in the resized (smaller) image,
                    //the algorithm selects the nearest corresponding pixel in the original (larger) image.


                    int originalCol = (int)(col * colRatio);
                    int originalRow = (int)(row * rowRatio);
                    // needs to be row * width + col so it can go like data[1], data[2], data[3]. since its a 1D array. the height is the number of rows that will be checked.
                    //the width is the number of columns that will be checked. row * width + col 
                    //

                    data[row * width + col] = originalData[originalRow * originalTexture.Width + originalCol];
                }
            }

            resizedTexture.SetData(data);
            return resizedTexture;
        }
    }
}