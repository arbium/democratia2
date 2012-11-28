using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace AxonInformer
{
    public class Informer
    {
        public int FontSize = 18;
        public string FontFamily = "Arial";

        public Bitmap Bitmap { get; set; }
        internal Graphics Graphic { get; set; }
        internal FontStyle FontStyle = FontStyle.Bold;
        internal GraphicsUnit FontUnit = GraphicsUnit.Pixel;

        public Informer(string fileName)
        {
            Bitmap = new Bitmap(fileName);
            Graphic = Graphics.FromImage(Bitmap);
        }

        public void DrawText(string imageText, Color color, float x, float y)
        {
            var font = new Font(FontFamily, FontSize, FontStyle, FontUnit);

            if (Graphic == null)
            {
                // тут надо определить размер и 

                // Задаем цвет фона.
                //this.G.Clear(Color.White);
                
            }
            else
            {
                // Задаем параметры анти-алиасинга
                Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                Graphic.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Пишем (рисуем) текст
                Graphic.DrawString(imageText, font, new SolidBrush(color), x, y);
                Graphic.Flush();
            }
            
            //return this._Bitmap;
        }

        public void SaveToPng(string fileName)
        {
            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);

            Bitmap.Save(fileName, ImageFormat.Png);
        }
    }
}
