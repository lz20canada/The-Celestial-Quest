using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LZFinal
{
    public class CustomSpriteFont
    {
        private Texture2D spriteFontTexture;
        private Dictionary<char, Rectangle> characterMap;
        private int charWidth;
        private int charHeight;
        private int charsPerRow;
        private float scale = 1.0f; /// Default scale

        /// Public getter for scaled character width
        public int CharWidth => (int)(charWidth * scale);

        /// Public getter for scaled character height
        public int CharHeight => (int)(charHeight * scale);

        /// Public getter for scale
        public float Scale => scale;

        /// Public getter for CharacterMap
        public Dictionary<char, Rectangle> CharacterMap => characterMap;

        /// Constructor to initialize the custom sprite font
        public CustomSpriteFont(Texture2D texture, int charWidth, int charHeight, int charsPerRow)
        {
            this.spriteFontTexture = texture;
            this.charWidth = charWidth;
            this.charHeight = charHeight;
            this.charsPerRow = charsPerRow;
            this.characterMap = new Dictionary<char, Rectangle>();

            InitializeCharacterMap();
        }

        /// New method to set the scale while retaining character appearance
        public void SetScale(float scale)
        {
            this.scale = scale;
        }

        /// Initialize the character map based on the given characters
        private void InitializeCharacterMap()
        {
            ///this was manually eyed out from the spritesheet
            string characters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int index = 0;

            foreach (char character in characters)
            {
                int row = index / charsPerRow;
                int col = index % charsPerRow;
                characterMap[character] = new Rectangle(col * charWidth, row * charHeight, charWidth, charHeight);
                index++;
            }
        }

        /// Draw a string using the custom sprite font
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            Vector2 currentPosition = position;

            ///goes through each char 1 by 1 drawing them, and scaling if necessary
            foreach (char c in text)
            {
                if (characterMap.TryGetValue(c, out Rectangle sourceRect))
                {
                    ///set most of the args to 0, wanted to get to scale. scale is changed here by SetScale().
                    spriteBatch.Draw(spriteFontTexture, currentPosition, sourceRect, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    currentPosition.X += (int)(sourceRect.Width * scale); /// Move to the next character position
                }
            }
        }
    }
}