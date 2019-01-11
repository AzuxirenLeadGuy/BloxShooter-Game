using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
namespace BloxShooter.SharedFiles
{
    public enum GameState : byte { Ready, Play, Pause, GameOver }
    struct GameScreen : IScreen
    {
        /// <summary>The current State of the Screen </summary>
        internal GameState State;
        /// <summary>All the players, including the PC </summary>
        internal Blox[] Players;
        /// <summary>A copy of the original Non-PC players to use on reloading the game</summary>
        internal Blox[] PlayersCopy;
        /// <summary>The Stack of Bullets</summary>
        internal Node<Bullet> List;
        /// <summary>Rectangle of the screen. </summary>
        internal Rectangle RegionWall;
        ///<summary>Count of Players. 0-indexing i.e starts from 0 to n-1</summary>
        byte PlayerCount;
        /// <summary>Displays message on the Screen </summary>
        string ScreenMessage;
        /// <summary>The rectangle to fit the ScreenMessage</summary>
        internal Rectangle TextRect;
        /// <summary>The Time Storing Variable</summary>
        int Timer;
        /// <summary>Lock Timeout variable for the Default input</summary>
        int LockInput;
        /// <summary>The Constructor that must be used</summary>
        public GameScreen(Blox[] x)
        {
            Timer = LockInput = 0;
            List = null;
            State = GameState.Ready;
            PlayersCopy = new Blox[x.Length];
            for (int i = 0; i < x.Length; i++) PlayersCopy[i] = x[i];
            Players = x;
            PlayerCount = 0;
            TextRect = new Rectangle(Global.Width / 2 - 300, Global.Height / 2 - 70, 600, 140);
            State = GameState.Ready;
            ScreenMessage = "";
            RegionWall = new Rectangle(0, 0, Global.Width, Global.Height);
            LoadContent();
        }
        /// <summary>Reloads the variables value</summary>
        public void LoadContent()
        {
            Players = new Blox[PlayersCopy.Length];
            PlayerCount = (byte)(PlayersCopy.Length - 1);
            for (int i = 0; i <= PlayerCount; i++)
            {
                Players[i] = PlayersCopy[i];
                Players[i].Health = 100;
            }
            List = null;
            ScreenMessage = "3";
            State = GameState.Ready;
            switch (PlayerCount)
            {
                case 3:
                    Players[3].x = new Vector2(Global.Width - Players[3].Dest.Width, Global.Height - Players[3].Dest.Height);
                    goto case 2;
                case 2:
                    Players[2].x = new Vector2(0, Global.Height - Players[2].Dest.Height);
                    goto case 1;
                case 1:
                    Players[1].x = new Vector2(Global.Width - Players[1].Dest.Width, 0);
                    goto case 0;
                case 0:
                    Players[0].x = new Vector2(0, 0);
                    break;
            }
        }
        /// <summary>Draw the screen</summary>
        public void Draw(GameTime gt)
        {
            int i;
            Global.thisgame.GraphicsDevice.Clear(Color.White);
            switch (State)
            {
                case GameState.Pause:
                case GameState.Ready:
                case GameState.GameOver:
                    Global.font1.Write(TextRect, ScreenMessage, Color.Black);
                    break;
                case GameState.Play:
                    var x = List;
                    while (x != null) { x.item.Draw(); x = x.next; }
                    for (i = 0; i <= PlayerCount; i++) { Players[i].Draw(); }
                    break;
            }
        }
        /// <summary>Updates the Screen</summary> 
        public void Update(GameTime gameTime)
        {
            switch (State)
            {
                case GameState.Play:
                    if (!Global.thisgame.IsActive) { State = GameState.Pause; ScreenMessage = "Paused. Press Start to Resume.\nPress Select to Exit"; }
                    if (PlayerCount == 0)
                    {
                        ScreenMessage = $"Game Over. Team " + (Players[0].c == Color.Red ? "Red" : "Blue") + " Won \nPress Enter/Start to Restart\nPress Escape/Select to Exit.";
                        State = GameState.GameOver;
                    }
                    UpdateGamePlay(gameTime);
                    break;
                case GameState.Ready:
                    Timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (Timer >= 1000)
                    {
                        Timer = 0;
                        if (ScreenMessage.Equals("3")) { ScreenMessage = "2"; }
                        else if (ScreenMessage.Equals("2")) { ScreenMessage = "1"; }
                        else { State = GameState.Play; }
                    }
                    break;
                case GameState.Pause:
                    if (LockInput < 2000) { LockInput += gameTime.ElapsedGameTime.Milliseconds; }
                    else if (Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.Start)) { State = GameState.Ready; ScreenMessage = "3"; }
                    else if (Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.Back))
                    {
                        Global.thisgame.isLoading = true;
                        MenuScreen.State = MenuScreenState.PressStart;
                        Global.ScreenLoad(new MenuScreen());
                    }
                    break;
                case GameState.GameOver:
                    if (Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.Start)) LoadContent();
                    else if (Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.Back))
                    {
                        Global.thisgame.isLoading = true;
                        MenuScreen.State = MenuScreenState.PressStart;
                        Global.ScreenLoad(new MenuScreen());
                    }
                    break;
            }
        }
        /// <summary>Updating the screen on GameState.Gameplay</summary>
        public void UpdateGamePlay(GameTime gt)
        {
            int i, j;
            for (i = 0; i <= PlayerCount; i++)
            {
                if (Players[i].Input > 4) {HandleInputNonPC(i,Players[i].Input);}
                else { Players[i].HandleInput(); }
                if (Players[i].keys.Get(4)) { State = GameState.Pause; ScreenMessage = "Paused. Press Start to Resume.\nPress Select to Exit"; LockInput = 0; }
                //else if (Players[i].keys.Get(5)) { State = GameState.Select; }
                if (Players[i].Shoot(gt.ElapsedGameTime.Milliseconds))
                {
                    Vector2 ix = new Vector2(Players[i].Dest.Center.X, Players[i].Dest.Center.Y);
                    Players[i].RightStick.Y *= -1;
                    Players[i].RightStick.Normalize();
                    ix += Players[i].RightStick * Players[i].Stat;
                    var iv = Players[i].RightStick * Players[i].Stat / 4;
                    if (List != null)
                        List = List.InsertBefore(new Bullet(ix, iv, (byte)(Players[i].Stat / 5)));
                    else
                        List = new Node<Bullet>(new Bullet(ix, iv, (byte)(Players[i].Stat / 5)));
                }
                if (!RegionWall.Contains(Players[i].Dest))
                {
                    if (Players[i].Dest.Top < 0 || Players[i].Dest.Bottom > Global.Height)
                        Players[i].v.Y *= -2;
                    else
                        Players[i].v.X *= -2;
                }
                Players[i].UpdatePos();
                for (j = 0; j < i; j++)
                {
                    if (Players[i].Collide(Players[j]))
                    {
                        Blox.HandleCollision(ref Players[j], ref Players[i]);
                    }
                }
                if (Players[i].Health <= 0) { Players[i] = Players[PlayerCount--]; }//Remove Player as He is Dead.
            }
            var x = List;
            while (x != null)
            {
                if (!RegionWall.Contains(x.item.Dest))
                {
                    if (x == List)
                        List = x = List.next;
                    else x = x.Delete();
                    goto ex;
                }
                for (i = 0; i <= PlayerCount; i++)
                {
                    if (x.item.Collide(Players[i]))
                    {
                        Players[i].Health--;
                        if (x == List)
                            List = x = List.next;
                        else x = x.Delete();
                        goto ex;
                    }
                }
                x.item.UpdatePos();
                x = x.next;
            ex:;
            }
        }
        /// <summary>
        /// Gives input to non-PC player Blox
        /// </summary>
        /// <param name="playerindex">The index of the Blox</param>
        /// <param name="Level">The degree of mistakes/Idleness</param>
        void HandleInputNonPC(int playerindex,int Level)
        {
            Level-=3;
            Level%=5;
        }
    }
}