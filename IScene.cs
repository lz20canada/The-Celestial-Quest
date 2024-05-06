using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZFinal
{
    public interface IScene
    {
        bool ContentLoaded { get; set; }
        public void LoadContent();
        public void Update(GameTime gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public void Enter();
        public void Exit();
    }
}
