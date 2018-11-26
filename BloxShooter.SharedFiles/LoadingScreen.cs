using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace BloxShooter.SharedFiles
{
    ///<summary>The Loading Screen to display while changing screens. To be used with Default Constructor.</summary>
    public struct LoadingScreen : IScreen
    {
        ///<summary>The Loading Spritesheet</summary>
        internal SpriteSheet Loading;
        ///<summary>The Draw Method. Makes Logo on Black Screen</summary>
        public void Draw(GameTime gt)
        {
            Global.thisgame.GraphicsDevice.Clear(Color.Gray);
            Loading.Draw();
        }
        ///<summary>Loads the file</summary>
        public void LoadContent()
        {
            var w = Global.Width;
            var h = Global.Height;
            int LU=Global.Height/10;
            Loading=new SpriteSheet(Global.thisgame.Content.Load<Texture2D>(@"Load2"),367,379,9,6);
            w = w / 2; h = h / 2;
            Loading.Dest=new Rectangle(w-LU*2,h-LU*2,LU*4,LU*4);
        }
        ///<summary>Updates the Loading Spritesheet</summary>
        public void Update(GameTime gt)=>Loading.Update();
    }
}