using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
namespace BloxShooter.SharedFiles
{
    internal interface IScreen
    {
        void LoadContent();
        void Update(GameTime gt);
        void Draw(GameTime gt);
    }
    /// <summary>The Common Extension methods and Global variables used</summary>
    public static class Global
    {
        public static Main thisgame;
        public static int Width, Height;
        public static SpriteFont font1, font2;
        internal static int DefaultInput=0;
        internal static GamePadState[] Gi;
        internal static Texture2D Square=null;
        internal static Thread ScreenLoader;
        ///<summary>Loads Screen while keeping Loading Splash Screen. PLEASE MAKE isLoading=true before calling </summary>
        internal static void ScreenLoad(IScreen screen)
        {
            ScreenLoader=new Thread(TaskPerform);
            ScreenLoader.Start();
            void TaskPerform()
            {
                screen.LoadContent();
                thisgame.CurrentScreen=screen;
                thisgame.isLoading=false;
            }
        }
        ///<summary>Return true on Collision</summary>
        public static bool Collide(this GameObject a, GameObject b) => a.Des.Intersects(b.Des);
        /// <summary>Draws a part of Texture2D on the Dest rectangle on the Screen </summary>
        /// <param name="tex">The Texture2D used</param>
        /// <param name="source">The Rectagle Mapping on the Texture2D/image</param>
        /// <param name="Dest">The Rectangle on The Screen</param>
        /// <param name="c">Tint Color[White by default]</param>
        public static void Draw(this Texture2D tex, Rectangle source, Rectangle Dest,Color? c=null) => thisgame.spriteBatch.Draw(tex, source, Dest, c??Color.White);
        /// <summary>Draws the whole Texture2D on the Screen </summary>        
        /// <param name="tex">The Texture2D to draw</param>
        /// <param name="Dest">The Dest rectangle</param>
        /// <param name="c"></param>
        public static void Draw(this Texture2D tex,Rectangle Dest,Color? c=null) => thisgame.spriteBatch.Draw(tex, Dest, c??Color.White);
        /// <summary>Draws the whole Texture2D at the given Vector2 on the Screen </summary>        
        /// <param name="tex">The Texture2D image to Draw</param>
        /// <param name="Dest">The Destination Rectangle</param>
        /// <param name="c">Tint Color[White by default]</param>
        public static void Draw(this Texture2D tex, Vector2 Dest, Color? c = null) => thisgame.spriteBatch.Draw(tex, Dest, c ?? Color.White);
        /// <summary>Writes the text within the rectangle on the Screen </summary>
        /// <param name="font">The SpriteFont to use</param>
        /// <param name="Dest">The Rectangle to fit the text</param>
        /// <param name="Message">The Message to write on the screen</param>
        /// <param name="c">The Color to use [White By default]</param>
        public static void Write(this SpriteFont font, Rectangle Dest, string Message, Color? c=null)
        {
            Vector2 size = font.MeasureString(Message);
            float xScale = (Dest.Width / size.X);
            float yScale = (Dest.Height / size.Y);
            float scale = Math.Min(xScale, yScale);
            int strWidth = (int)Math.Round(size.X * scale);
            int strHeight = (int)Math.Round(size.Y * scale);
            Vector2 position = new Vector2();
            position.X = (((Dest.Width - strWidth) / 2) + Dest.X);
            position.Y = (((Dest.Height - strHeight) / 2) + Dest.Y);
            float rotation = 0.0f;
            Vector2 spriteOrigin = new Vector2(0, 0);
            float spriteLayer = 0.0f; // all the way in the front
            SpriteEffects spriteEffects = SpriteEffects.None;
            Color cc= c ?? Color.White;// Draw the string to the sprite batch!
            thisgame.spriteBatch.DrawString(font, Message, position, cc, rotation, spriteOrigin, scale, spriteEffects, spriteLayer);
        }
        public static void FitText(this Rectangle Dest,SpriteFont font,string Message,out float scale,out Vector2 position)
        {
            Vector2 size = font.MeasureString(Message);
            float xScale = (Dest.Width / size.X);
            float yScale = (Dest.Height / size.Y);
            // Taking the smaller scaling value will result in the text always fitting in the boundaires.
            scale = Math.Min(xScale, yScale);
            // Figure out the location to absolutely-center it in the boundaries rectangle.
            int strWidth = (int)Math.Round(size.X * scale);
            int strHeight = (int)Math.Round(size.Y * scale);
            position = new Vector2();
            position.X = (((Dest.Width - strWidth) / 2) + Dest.X);
            position.Y = (((Dest.Height - strHeight) / 2) + Dest.Y);
        }
    }
    /// <summary>This is the main type for your game.</summary>
    public class Main : Game
    {
        ///<summary>Graphics Interface</summary>
        internal GraphicsDeviceManager graphics;
        ///<summary>Sprite drawing</summary>
        internal SpriteBatch spriteBatch;
        ///<summary>show Loading bar</summary>
        internal IScreen PauseScreen;
        ///<summary>The actual Screen</summary>
        internal IScreen CurrentScreen;
        ///<summary>When true, show Loading Screen</summary>
        internal bool isLoading;
        int i;
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Global.Gi = new GamePadState[4];
            PauseScreen = new LoadingScreen();
            CurrentScreen = new LogoScreen();
            isLoading = false;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Global.Width = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            Global.Height = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = Global.Width;
            graphics.PreferredBackBufferHeight = Global.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Global.thisgame = this;
            Global.Square = new Texture2D(GraphicsDevice, 1, 1);
            Global.Square.SetData<Color>(new Color[] { Color.White });
            base.Initialize();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Global.Width = (ushort)Window.ClientBounds.Width;
            Global.Height = (ushort)Window.ClientBounds.Height;
            Global.font1 = Content.Load<SpriteFont>("font1");
            Global.font2 = Content.Load<SpriteFont>("font2");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            PauseScreen.LoadContent();
            CurrentScreen.LoadContent();
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent(){if(Global.ScreenLoader!=null&&Global.ScreenLoader.IsAlive)Global.ScreenLoader.Abort();}
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            for (i = 0; i < 4; i++) { Global.Gi[i] = GamePad.GetState(i); }
            if (isLoading) PauseScreen.Update(gameTime);
            else CurrentScreen.Update(gameTime);
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (isLoading) PauseScreen.Draw(gameTime);
            else CurrentScreen.Draw(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
