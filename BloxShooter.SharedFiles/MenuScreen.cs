using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace BloxShooter.SharedFiles
{
    public enum MenuScreenState : byte { PressStart, FadeOut, MenuLoaded, TakeInput }
    public struct MenuScreen : IScreen
    {
        float Alpha;
        Texture2D GameSplash;
        Menu GameMenu;
        public static MenuScreenState State;
        Rectangle SplashDest, fontDest;
        int P2input;
        public MenuScreen(bool x)
        {
            GameSplash = Global.thisgame.Content.Load<Texture2D>("l2");
            Alpha = 1.00f;
            var asp = (float)GameSplash.Width / (float)GameSplash.Height;
            int sh = (int)System.Math.Round(0.6 * Global.Height);
            int sw = (int)System.Math.Round(sh * asp);
            int sx = (Global.Width - sw) >> 1;
            int sy = (int)System.Math.Round(0.1 * Global.Height);
            int fy = (int)System.Math.Round(0.8 * Global.Height);
            int fx = (int)System.Math.Round(0.2 * Global.Width);
            int fw = (int)System.Math.Round(0.6 * Global.Width);
            int fh = sy << 1;
            SplashDest = new Rectangle(0, 0, Global.Width, Global.Height);
            fontDest = new Rectangle(fx, fy, fw, fh);
            Alpha = 1.0f;
            P2input = 7;
            GameMenu = default(Menu);
        }
        public void Draw(GameTime gt)
        {
            Global.thisgame.GraphicsDevice.Clear(Color.Black);
            switch(State)
            {
                case MenuScreenState.PressStart:
                    GameSplash.Draw(SplashDest);
                    Global.font2.Write(fontDest, "Press Enter or Start", Color.LightBlue);
                    break;
                case MenuScreenState.FadeOut:
                    GameSplash.Draw(SplashDest, Color.White * Alpha);
                    break;
                case MenuScreenState.MenuLoaded:
                    GameMenu.Draw();
                    break;
                case MenuScreenState.TakeInput:
                    Global.font2.Write(new Rectangle(0, Global.Height / 2, Global.Width, 40), $"Player 2 , Please Press Start.", Color.White);
                    break;
            }
        }
        internal void LoadMenu()
        {
            int x = Global.Width * 2 / 10, y = Global.Height * 2 / 10, l = Global.Width * 6 / 10, b = Global.Height * 3 / 10;
            P2input = 7;
            GameMenu = new Menu(3, new Rectangle(x, y, l, b), Global.DefaultInput);
            GameMenu.options = new IOption[3];
            GameMenu.options[0] = new Button("Play v/s PC", PlayvsPC);
            GameMenu.options[1] = new Button("Play 2P", () => State = MenuScreenState.TakeInput);
            GameMenu.options[2] = new Button("Exit", Global.thisgame.Exit);
            GameMenu.LoadContent();
        }
        internal void PlayvsPC() 
        {
            var ps=new Blox[2];
            ps[0]=new Blox(Color.Blue,Global.DefaultInput);
            ps[1]=new Blox(Color.Red,5);
            Global.thisgame.isLoading=true;
            Global.ScreenLoad(new GameScreen(ps));
        }
        internal void PlayvsHuman()
        {
            var ps = new Blox[2];
            ps[0] = new Blox(Color.Blue, Global.DefaultInput);
            ps[1] = new Blox(Color.Red, P2input);
            Global.thisgame.isLoading = true;
            Global.ScreenLoad(new GameScreen(ps));
        }
        public void LoadContent()
        {
            GameSplash = Global.thisgame.Content.Load<Texture2D>("l2");
            Alpha = 1.00f;
            var asp = (float)GameSplash.Width / (float)GameSplash.Height;
            int sh = (int)System.Math.Round(0.6 * Global.Height);
            int sw = (int)System.Math.Round(sh * asp);
            int sx = (Global.Width - sw) >> 1;
            int sy = (int)System.Math.Round(0.1 * Global.Height);
            int fy = (int)System.Math.Round(0.8 * Global.Height);
            int fx = (int)System.Math.Round(0.2 * Global.Width);
            int fw = (int)System.Math.Round(0.6 * Global.Width);
            int fh = sy << 1;
            SplashDest = new Rectangle(0, 0, Global.Width, Global.Height);
            fontDest = new Rectangle(fx, fy, fw, fh);
        }
        public void Update(GameTime gt)
        {
            switch(State)
            {
                case MenuScreenState.PressStart:
                    for(int i = 0; i < 4; i++) { if (Global.Gi[i].IsButtonDown(Buttons.Start)) { Global.DefaultInput = i;LoadMenu(); State = MenuScreenState.FadeOut; goto Exc; } }
                    Exc:break;
                case MenuScreenState.FadeOut:
                    Alpha -= 0.04f;
                    if (Alpha <= 0.0f) { State = MenuScreenState.MenuLoaded; }
                    break;
                case MenuScreenState.MenuLoaded:
                    GameMenu.Update(gt);
                    break;
                case MenuScreenState.TakeInput:
                    for(int i=0;i<4;i++)
                    {
                        if (Global.Gi[i].IsButtonDown(Buttons.Start)&&i!=Global.DefaultInput)
                        {
                            P2input = i;
                        }
                    }
                    if (P2input <= 4){PlayvsHuman();}
                    break;
            }
        }
    }
}