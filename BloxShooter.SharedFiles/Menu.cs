using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace BloxShooter.SharedFiles
{
    ///<summary>A menu List containing various options</summary>
    public struct Menu
    {
        internal int Input;
        internal bool Locked;
        internal int Timer;
        internal int LockTime;
        internal IOption[] options;
        internal byte Counter;
        internal byte OptionCount;
        internal Rectangle Bounds;
        internal Rectangle SelectionDest;
        internal Texture2D Selection;
        internal int Gap;
        internal Color TextColor, SelectColor, ActionColor;
        /// <summary>
        /// Menu for Monogame
        /// </summary>
        /// <param name="opt">Conut of Options</param>
        /// <param name="b">Bounds of menu in Game-Pixels</param>
        /// <param name="ii">Default Input for this Menu</param>
        public Menu(byte opt, Rectangle b, int ii)
        {
            options = new IOption[opt];
            OptionCount = opt;
            Counter = 0;
            Bounds = b;
            Gap = 0;
            Timer = 0;
            LockTime = 100;
            Locked = false;
            Input = ii;
            TextColor = Color.White;
            SelectColor = Color.White;
            ActionColor = Color.Yellow;
            Selection = Global.thisgame.Content.Load<Texture2D>("OptionShade");
            SelectionDest = new Rectangle(0, 0, 0, 0);
        }
        public void LoadContent()
        {
            Gap = Bounds.Height / OptionCount;
            SelectionDest = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Gap);
            float xx;
            Vector2 yy;
            for (int i = 0; i < OptionCount; i++)
            {
                SelectionDest.FitText(Global.font1, options[i].Text, out xx, out yy);
                options[i].Scale = xx;
                options[i].TextPos = yy;
                SelectionDest.Y += Gap;
            }
            //Recalculate SelectionDest,Bounds,Gap,Position
        }
        internal void Update(GameTime gt)
        {
            if (!Locked)
            {
                if (Global.Gi[Input].IsButtonDown(Buttons.DPadDown)) { Counter++; Counter = (byte)(Counter % (OptionCount + 1)); Locked = true; }
                else if (Global.Gi[Input].IsButtonDown(Buttons.DPadUp)) { Counter--; Counter = (byte)(Counter % (OptionCount + 1)); Locked = true; }
                if (Global.Gi[Input].ThumbSticks.Left.Y < -0.030f) { Counter++; Counter = (byte)(Counter % (OptionCount + 1)); Locked = true; }
                else if (Global.Gi[Input].ThumbSticks.Left.Y > 0.30f) { Counter--; Counter = (byte)(Counter % (OptionCount + 1)); Locked = true; }
                if (Counter != OptionCount)
                {
                    options[Counter].Update(true, Input, ref Locked);
                    if (options[Counter].state == OptState.Released)
                        options[Counter].x();
                }
                SelectionDest.Y = Bounds.Y + (Gap * Counter);
            }
            else
            {
                Timer += gt.ElapsedGameTime.Milliseconds;
                if (Timer > LockTime) { Timer = 0; Locked = false; }
            }
        }
        internal void Draw()
        {
            Selection.Draw(Bounds, Color.DarkBlue);
            for (int i = 0; i < OptionCount; i++) { options[i].Draw(TextColor); }
            if (Counter != OptionCount)
            {
                Selection.Draw(SelectionDest, (options[Counter].state == OptState.Clicked ? ActionColor : SelectColor)*0.6f);
            }
        }
    }
    public enum OptState : byte { InActive, MouseOver, Clicked, Released }
    public interface IOption
    {
        string Text { get; }
        OptState state { set; get; }
        void Update(bool CounterOn, int Input, ref bool locked);
        void Draw(Color cc);
        System.Action x { get; }
        float Scale { get; set; }
        Vector2 TextPos { get; set; }
    }
    public struct Button : IOption
    {
        internal string OptionText;
        internal OptState CurrnetState;
        System.Action action;
        internal float TextScale;
        internal Vector2 TextVector;
        public Vector2 TextPos { get => TextVector; set => TextVector = value; }
        public float Scale { get => TextScale; set => TextScale = value; }
        public OptState state { get => CurrnetState; set => CurrnetState = value; }
        public System.Action x => action;
        public string Text => OptionText;
        public Button(string s, System.Action a = null)
        {
            TextVector = Vector2.Zero;
            TextScale = 0;
            action = a;
            CurrnetState = OptState.InActive;
            OptionText = s;
        }
        public void Draw(Color cc) => Global.thisgame.spriteBatch.DrawString
        (Global.font1, OptionText, TextVector, cc, 0, Vector2.Zero, TextScale, SpriteEffects.None, 1);
        public void Update(bool CounterOn, int Input, ref bool Locked)
        {
            if (!CounterOn) { CurrnetState = OptState.InActive; }
            else if (Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.A)) { CurrnetState = OptState.Clicked; }
            else if (CurrnetState == OptState.Clicked) { CurrnetState = OptState.Released; }
            else { CurrnetState = OptState.MouseOver; }
        }
    }
    public struct Slider : IOption
    {
        internal byte MinValue;
        internal byte MaxValue;
        public byte Current;
        internal bool Active;
        internal string OptionText;
        internal OptState State;
        public System.Action x => action;
        System.Action action;
        internal float TextScale;
        internal Vector2 TextVector;
        public string Text => OptionText;
        public Vector2 TextPos { get => TextVector; set => TextVector = value; }
        public float Scale { get => TextScale; set => TextScale = value; }
        public OptState state { get => State; set => State = value; }
        public Slider(string Opt, byte min = 0, byte max = 255, System.Action a = null)
        {
            MinValue = min;
            MaxValue = max;
            TextVector = Vector2.Zero;
            OptionText = Opt;
            Active = false;
            Current = min;
            action = a;
            TextScale = 0;
            State = OptState.InActive;
        }
        public void Update(bool CounterOn, int Input, ref bool Locked)
        {
            if (!CounterOn) { State = OptState.InActive; }
            else if
            (Locked == false &&
            (
                Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.DPadLeft) ||
                Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.DPadRight) ||
                Global.Gi[Global.DefaultInput].ThumbSticks.Left.X >= 0.3f ||
                Global.Gi[Global.DefaultInput].ThumbSticks.Left.X <= -0.3f
            )
            )
            {
                State = OptState.Clicked;
                if (Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.DPadLeft) && Current > MinValue) Current--;
                else if (Global.Gi[Global.DefaultInput].IsButtonDown(Buttons.DPadRight) && Current < MaxValue) Current++;
                if (Global.Gi[Global.DefaultInput].ThumbSticks.Left.X <= -0.3f && Current > MinValue) Current--;
                else if (Global.Gi[Global.DefaultInput].ThumbSticks.Left.X >= 0.3f && Current < MaxValue) Current++;
                Locked = true;
            }
            else if (State == OptState.Clicked) { State = OptState.Released; }
            else { State = OptState.MouseOver; }
        }//Take Input, Reflect Current
        public void Draw(Color TextColor) => Global.thisgame.spriteBatch.DrawString(Global.font1, OptionText + " << " + Current.ToString() + " >> ",
        TextVector, TextColor, 0, Vector2.Zero, TextScale, SpriteEffects.None, 1.00f);//Draw
    }
}
