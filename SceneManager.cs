using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZFinal
{
    public class SceneManager
    {
        private Dictionary<string, IScene> scenes = new Dictionary<string, IScene>();
        private IScene currentScene;

        /// Add a scene to the scene manager.
        public void AddScene(string name, IScene scene)
        {
            scenes[name] = scene;
        }

        /// Change the currently active scene.
        public void ChangeScene(string name)
        {
            if (scenes.TryGetValue(name, out IScene scene))
            {
                /// Call LoadContent here if it hasn't been loaded already
                if (!scene.ContentLoaded)
                {
                    scene.LoadContent();
                    scene.ContentLoaded = true; /// You might need to add this property to your IScene interface
                }

                currentScene = scene;
            }
        }

        /// Update the current scene.
        public void Update(GameTime gameTime)
        {
            currentScene?.Update(gameTime);
        }

        /// Draw the current scene using the provided SpriteBatch.
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            currentScene?.Draw(gameTime, spriteBatch);
        }

        /// Try to get a scene by its name.
        public bool TryGetScene(string name, out IScene scene)
        {
            return scenes.TryGetValue(name, out scene);
        }
    }
}