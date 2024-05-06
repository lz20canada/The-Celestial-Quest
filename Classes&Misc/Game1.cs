using LZFinal.Scenes.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using static System.Formats.Asn1.AsnWriter;

namespace LZFinal
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        ///renders 2d images
        private SpriteBatch _spriteBatch;

        ///needs to be public to be changed across other classes
        public SceneManager sceneManager;

        /// Background music
        private Song menuMusic;

        ///Level 1 Music
        private Song level1Music;

        ///button click sound
        private SoundEffect buttonClickSound;

        ///crystal obtain sound
        private SoundEffect crystalObtainedSound;

        ///game over sound effect
        private SoundEffect gameOverSound;

        ///game win sound effect
        private SoundEffect gameWinSound;


        /// Property to access the sound effect
        public SoundEffect ButtonClickSound => buttonClickSound;

        public SoundEffect CrystalObtainedSound => crystalObtainedSound;

        public SoundEffect GameOverSound => gameOverSound;

        public SoundEffect GameWinSound => gameWinSound;

        ///disables or enables input

        public bool IsInputEnabled { get; set; } = true;


        public Song MenuMusic => menuMusic; ///getter to access the music

        public Song Level1Music =>  level1Music; 
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            sceneManager = new SceneManager();

            /// Set fullscreen mode

            /// Set a common screen resolution that's divisible by 64x64 tiles. important for 
            ///later level design
            _graphics.PreferredBackBufferWidth = 1920; /// Width is 1920 pixels
            _graphics.PreferredBackBufferHeight = 1024; /// Height is 1024 pixels
            _graphics.IsFullScreen = false; /// Set to false for windowed mode
           


        }

        protected override void Initialize()
        {
            /// TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            ///load menu music
            menuMusic = Content.Load<Song>("menumusic");

            ///load level1 music
            level1Music = Content.Load<Song>("level1music");

            /// Load the button click sound effect
            buttonClickSound = Content.Load<SoundEffect>("ButtonClick");

            ///load the crystal obtained sound effect
            crystalObtainedSound = Content.Load<SoundEffect>("gotcrystal");

            ///load the game over sound effect
            gameOverSound = Content.Load<SoundEffect>("gameover");

            ///load the game win sound effect
            gameWinSound = Content.Load<SoundEffect>("win");



            /// Initialize and add the menu scene
            var menuScene = new MenuScene(this);
            sceneManager.AddScene("Menu", menuScene);

            /// Initialize and add the play scene
            var playScene = new LevelSelectScene(this);
            sceneManager.AddScene("Play", playScene);

            /// Initialize and add the about scene
            var aboutScene = new AboutScene(this);
            sceneManager.AddScene("About", aboutScene);

            /// Initialize and add the help scene
            var helpScene = new HelpScene(this);
            sceneManager.AddScene("Help", helpScene);

            ///Initialize and add the level scenes

            IScene[] levels = new IScene[]
{
    new Level1Scene(this),
    /*new Level2Scene(this),
    new Level3Scene(this),
    new Level4Scene(this),
    new Level5Scene(this),
    new Level6Scene(this),
    new Level7Scene(this),
    new Level8Scene(this),
    new Level9Scene(this),
    new Level10Scene(this),*/
};

            /// Add the level scenes to the SceneManager
            for (int i = 0; i < levels.Length; i++)
            {
                sceneManager.AddScene($"Level{i + 1}", levels[i]);
            }

            ///start the menu scene first

            sceneManager.ChangeScene("Menu");
            menuScene.Enter(); /// Start playing music as soon as the menu is loaded

            




        }

        protected override void Update(GameTime gameTime)
        {

            /// Check for input to toggle fullscreen
            if (Keyboard.GetState().IsKeyDown(Keys.F11)) /// f11 to fullscreen
            {
                /// Toggle fullscreen mode
                _graphics.ToggleFullScreen();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            /// Update the current scene
            sceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /// Draw the current scene
            sceneManager.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
        ///
        public void ReturnToMainMenu()
        {
            /// Get the MenuScene from the scene manager
            if (sceneManager.TryGetScene("Menu", out IScene menuScene))
            {
                /// Stop any currently playing music
                MediaPlayer.Stop();

                /// Change to the Menu scene
                sceneManager.ChangeScene("Menu");

                /// Start the menu music
                menuScene.Enter();
            }
        }
    }
}