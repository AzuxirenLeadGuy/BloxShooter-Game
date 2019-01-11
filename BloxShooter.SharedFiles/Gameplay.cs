using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace BloxShooter.SharedFiles
{
    public enum BloxState : byte { Normal, AttackCharge, Attack, Lock, Parry, HealTap, HealUntap, Modifier }
    ///<summary>Defines basic object physics for game</summary>
    public interface GameObject
    {
    ///<summary>The Destination to Draw</summary>
        Rectangle Des{get;}
    ///<summary>The Update for the Position</summary>
        void UpdatePos();
    }
    public struct Blox : GameObject
    {
    ///<summary>Position Vector;</summary>
        internal Vector2 x;
    ///<summary>Velocity Vector</summary>
        internal Vector2 v;
    ///<summary>Acceleration Vector</summary>
        internal Vector2 a;
    ///<summary>Color of the Texture</summary>
        internal Color c;
    ///<summary>Bitmap to draw</summary>
        internal Texture2D img;
        /// <summary>
        /// {Size, Attack, Friction, Defend, ShootLockDelay}
        /// </summary>
        internal sbyte Stat;
    ///<summary>Health Point of Player</summary>
        internal byte Health;
    ///<summary>The Input id of the input device</summary>
        internal int Input;
    ///<summary>if true, shooting is locked</summary>
        internal bool ShootLock;
    ///<summary>The timer for maintaining shootlock</summary>
        internal int ShootTimer;
    ///<summary>The Vibration Amplitude</summary>
        internal Vector2 Vibrate;
    ///<summary>Native input [X,Y,A,B,Start,Select,LB,RB]</summary>
        internal System.Collections.BitArray keys;
    ///<summary>Acceleration Input</summary>
        internal Vector2 LeftStick;
    ///<summary>Shooting Input</summary>
        internal Vector2 RightStick;
    ///<summary>Trigger input</summary>
        internal float LTrigger,RTrigger;
        internal Rectangle Dest;
    ///<summary>The Constructor to use</summary>
        public Blox(Color cc,int inp=5)
        {
            img=Global.Square;
            Stat = 50;
            c = cc;
            Input = inp;
            ShootLock = false;
            ShootTimer=0;
            Vibrate=Vector2.Zero;
            keys=new System.Collections.BitArray(8);
            keys.SetAll(false);
            LeftStick=RightStick=x=v=a=Vector2.Zero;
            LTrigger=RTrigger=0;
            Health=100;
            Dest=new Rectangle(0,0,Stat,Stat);
        }
        /// <summary>The Method to execute if Collision is true</summary>
        public static void HandleCollision(ref Blox a,ref Blox b)
        {
            Vector2.Add(ref a.v, ref b.v, out a.v);
            Vector2.Subtract(ref a.v, ref b.v, out b.v);
            Vector2.Subtract(ref a.v, ref b.v, out a.v);
            float Mesure =  Vector2.Distance(a.x, b.x) / (a.Stat + b.Stat);
            var Center = Vector2.Add(a.x, b.x) / 2;
            a.v += Vector2.Multiply(Vector2.Subtract(a.x, Center), Mesure);
            b.v += Vector2.Multiply(Vector2.Subtract(b.x, Center), Mesure);
            a.Health--;b.Health--;
        }
    ///<summary>Initialize the Native input Keys</summary>
        public void HandleInput()
        {
            if(Input<4)//GamePad
            {
                LTrigger = Global.Gi[Input].Triggers.Left;
                RTrigger = Global.Gi[Input].Triggers.Right;
                LeftStick = Global.Gi[Input].ThumbSticks.Left;
                RightStick = Global.Gi[Input].ThumbSticks.Right;
                keys.Set(4, Global.Gi[Input].IsButtonDown(Buttons.Start));
                keys.Set(5, Global.Gi[Input].IsButtonDown(Buttons.Back));
                keys.Set(0,Global.Gi[Input].IsButtonDown(Buttons.X));
                keys.Set(1,Global.Gi[Input].IsButtonDown(Buttons.Y));
                keys.Set(2,Global.Gi[Input].IsButtonDown(Buttons.A));
                keys.Set(3,Global.Gi[Input].IsButtonDown(Buttons.B));
                keys.Set(6,Global.Gi[Input].IsButtonDown(Buttons.LeftShoulder));
                keys.Set(7,Global.Gi[Input].IsButtonDown(Buttons.RightShoulder));
            }
        }
        /// <summary>Updates position of the Blox</summary>
        public void UpdatePos()
        {
            a = LeftStick;
            a.Y *= -1;
            var dv=Vector2.Add(a,-v*friction);
            Vector2.Add(ref v,ref dv,out v);
            Vector2.Add(ref x,ref v,out x);
            Dest.X = (int)x.X;
            Dest.Y = (int)x.Y;
        }
        /// <summary>Returns if it is valid to shoot a bullet</summary>
        public bool Shoot(int ms)
        {
            if (ShootLock)
            {
                ShootTimer += ms;
                if (ShootTimer > ShootTime) { ShootLock = false; }
                return false;
            }
            else if (RightStick.Length() >= 0.3f)
            {
                ShootTimer = 0;
                ShootLock = true;
                return true;
            }
            else return false;
        }
        public Rectangle Des=>Dest;
        internal int ShootTime=>Stat<<1;
        internal static byte MinStat=>50;
        internal static byte MaxStat=>200;
        internal float friction => ((float)Stat)/500.0f;
        internal void Draw()
        {
            img.Draw(Dest,c);
            Global.font2.Write(Dest,Health.ToString(),Color.Black);
        }
    }
    public struct Bullet : GameObject
    {
    ///<summary>Position Vector;</summary>
        internal Vector2 x;
    ///<summary>Velocity Vector</summary>
        internal Vector2 v;
    ///<summary>Color of the Texture</summary>
        internal Color c;
    ///<summary>Bitmap to draw</summary>
        internal Texture2D img;
        internal byte Stat;
        internal Rectangle Dest;
        public byte StatVal => Stat;
        public Rectangle Des=>Dest;
        public Vector2 Position => x;
        public Vector2 Velocity => v;
        public Bullet(Vector2 ix,Vector2 iv,byte s)
        {
            v=iv;
            x=ix;
            c=Color.Black;
            img=Global.Square;
            Stat=s;
            Dest=new Rectangle((int)x.X,(int)x.Y,Stat,Stat);
        }
        public void UpdatePos()
        {
            Vector2.Add(ref v,ref x,out x);
            Dest.X = (int)x.X;
            Dest.Y = (int)x.Y;
        }
        public void Draw()=>img.Draw(Dest,c);
    }
}