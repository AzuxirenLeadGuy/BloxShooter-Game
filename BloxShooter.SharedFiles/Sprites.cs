using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace BloxShooter.SharedFiles
{
    ///<summary>SpriteSheet Manager. 
    ///
    ///Only use Draw() and Update() once initialized</summary>
    public struct SpriteSheet
    {
        ///<summary>SpriteSheet Image</summary>
        internal Texture2D Sheet;
        ///<summary>Source Rectangle Maps the image on the source Spritesheet</summary>
        internal Rectangle Source;
        ///<summary>Destination Rectangle is on the Screen</summary>
        internal Rectangle Dest;
        ///<summary>Keeps track of Frames on the SpriteSheet</summary>
        internal int[] FrameX,FrameY,Next;
        ///<summary>The Count of Frames</summary>
        int Frames;
        ///<summary>The Current</summary>
        int CurrentFrame;
        ///<summary>The Constructor that *must* be used</summary>
        ///<param name="sh">Sheet Image Texture</param>
        ///<param name="fw">Frame-width</param>
        ///<param name="fh">Frame-height</param>
        ///<param name="lx">Last Frame-X count (Starts from 0)</param>
        ///<param name="ly">Last Frame-Y count (Starts from 0)</param>
        public SpriteSheet(Texture2D sh,int fw,int fh,int lx,int ly)
        {
            Sheet=sh;
            int FramesPerLine=sh.Width/fw;
            Source=new Rectangle(0,0,fw,fh);
            Dest=new Rectangle(0,0,fw,fh);
            Frames=(FramesPerLine*ly)+lx+1;
            FrameX=new int[Frames];
            FrameY=new int[Frames];
            Next=new int[Frames];
            CurrentFrame=0;
            int i,j;
            for(i=0;i<ly;i++)
            {
                for(j=0;j<FramesPerLine;j++)
                {
                    FrameX[CurrentFrame]=j*fw;
                    FrameY[CurrentFrame]=i*fh;
                    Next[CurrentFrame]=CurrentFrame+1;
                    CurrentFrame++;
                }
            }
            for(j=0;j<=lx;j++)
            {
                FrameX[CurrentFrame]=j*fw;
                FrameY[CurrentFrame]=ly*fh;
                Next[CurrentFrame]=CurrentFrame+1;
                CurrentFrame++;
            }
            Next[Frames-1]=0;
            CurrentFrame=0;
        }
        ///<summary>Sets the Current Animation frame at f</summary>
        public void SetFrame(int f)=>CurrentFrame=f;
        ///<summary>The Draw function of the SpriteSheet</summary>
        internal void Draw(){Sheet.Draw(Dest,Source,Color.White);}
        ///<summary>The Draw function of the SpriteSheet</summary>
        internal void Draw(Rectangle dest)
        {Sheet.Draw(dest,Source,Color.White);}
        ///<summary>The Update Function of SpriteSheet
        ///
        ///Not Calling Update "Pauses" the Animation.</summary>
        internal void Update()
        {
            Source.X=FrameX[CurrentFrame];
            Source.Y=FrameY[CurrentFrame];
            CurrentFrame=Next[CurrentFrame];
        }
        internal void Copy(SpriteSheet source)
        {
            Sheet=source.Sheet;
            Next=source.Next;
            FrameX=source.FrameX;
            FrameY=source.FrameY;
            Frames=source.Frames;
            Source=source.Source;
            Dest=source.Dest;
        }
    }
}