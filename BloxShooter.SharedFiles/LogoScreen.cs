using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace BloxShooter.SharedFiles
{
    public struct LogoScreen : IScreen
    {
        Texture2D[] tests;
        Rectangle[] dest;
        int cases;
        float timer,timelimit,alpha;
        public void Draw(GameTime gt)
        {
            Global.thisgame.GraphicsDevice.Clear(Color.White);
            switch(cases/3)
            {
                case 0:tests[0].Draw(dest[0],Color.White*alpha); break;
                case 1:tests[1].Draw(dest[1],Color.White*alpha);break;
                case 2:tests[2].Draw(dest[2],Color.White*alpha);break;
                case 3:
                    cases+=4;
                    Global.thisgame.isLoading=true;
                    Global.ScreenLoad(new MenuScreen(true));
                break;
                default:break;
            }
        }
        public void LoadContent()
        {
            int w = Global.Width, h = Global.Height;
            cases=0;
            tests=new Texture2D[3];
            timelimit=2000;
            timer=0;
            tests[0]=Global.thisgame.Content.Load<Texture2D>("Xifosware");
            tests[1]=Global.thisgame.Content.Load<Texture2D>("MG");
            tests[2]=Global.thisgame.Content.Load<Texture2D>("GReq");
            dest=new Rectangle[3];
            for(int i=0;i<3;i++)
            {
                dest[i]=new Rectangle(0,0,tests[i].Width,tests[i].Height);
                dest[i].X=(Global.Width-dest[i].Width)>>1;
                dest[i].Y=(Global.Height-dest[i].Height)>>1;
            }
        }
        public void Update(GameTime gt)
        {
            switch(cases%3)
            {
                case 0:alpha+=0.01f;if(alpha>0.99f){cases++;};break;
                case 2:alpha-=0.01f;if(alpha<=0){cases++;};break;
                case 1:
                    timer+=gt.ElapsedGameTime.Milliseconds;
                    if(timer>timelimit){timer=0;cases++;}
                    break;
            }
        }
    }
}