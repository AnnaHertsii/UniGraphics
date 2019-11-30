using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using DBrush = System.Drawing.Brush;

namespace UniGraphics.Transformation
{
    public class TransformationManager
    {
        public double RootX { get; set; }
        public double RootY { get; set; }
        public double SideLength { get; set; }
        public double Rotation { get; set; }
        public bool RotateAroundCenter { get; set; }
        public int VertexIndex { get; set; }
        public WriteableBitmap Image { get; private set; }

        private static readonly int offset = 100;
        private static readonly int minScreenTick = 50;
        private static readonly int maxScreenTick = 100;
        private static readonly Pen gridPen, axisGridPen;
        private static readonly DBrush tickTextBrush;
        private static readonly Font tickTextFont;

        private int PlaneX, PlaneY, RootContainer, 
                    width, height, halfWidth, halfHeight,
                    tick, screenTick, ticksFromX, ticksToX, ticksFromY, ticksToY;
        private double scale;
        private Graphics graphics;

        static TransformationManager()
        {
            gridPen = new Pen(DColor.FromArgb(255, 79, 81, 100), 1);
            axisGridPen = new Pen(DColor.FromArgb(255, 79, 81, 100), 2);
            tickTextBrush = new SolidBrush(DColor.FromArgb(255, 79, 81, 100));
            tickTextFont = new Font("Times New Roman", 14, GraphicsUnit.Pixel);
        }

        //промальовка сітки
        private void drawGrid()
        {
            int coord;
            for (int i = ticksFromX; i <= ticksToX; ++i)
            {
                coord = halfWidth + i * screenTick;
                if (i == 0)
                    graphics.DrawLine(axisGridPen, coord, 0, coord, height);
                else
                    graphics.DrawLine(gridPen, coord, 0, coord, height);
                graphics.DrawString((i * tick).ToString(), tickTextFont, 
                                    tickTextBrush, coord, halfHeight);
            }
            for (int i = ticksFromY; i <= ticksToY; ++i)
            {
                coord = halfHeight - i * screenTick;
                if (i == 0)
                    graphics.DrawLine(axisGridPen, 0, coord, width, coord);
                else
                {
                    graphics.DrawLine(gridPen, 0, coord, width, coord);
                    graphics.DrawString((i * tick).ToString(), tickTextFont,
                                        tickTextBrush, halfWidth, coord);
                }
            }
            //малювання трикутників для стрілок осей
            graphics.FillPolygon(tickTextBrush, new PointF[] {
                new PointF(halfWidth, -2),
                new PointF(halfWidth + 5, 10),
                new PointF(halfWidth - 6, 10)
            });
            graphics.FillPolygon(tickTextBrush, new PointF[] {
                new PointF(width + 2, halfHeight),
                new PointF(width - 10, halfHeight + 5),
                new PointF(width - 10, halfHeight - 6)
            });
        }

        private void drawHexagon()
        {

        }

        //оновлює налаштування сітки
        private bool AdjustView(int width, int height, CancellationToken? token)
        {
            this.width = width;
            this.height = height;
            halfWidth = width / 2;
            halfHeight = height / 2;
            Image = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            RootContainer = (int) Math.Max(Math.Abs(RootX) + offset, Math.Abs(RootY) + offset);
            int screenMax = Math.Max(halfWidth, halfHeight);
            scale = RootContainer / (double)screenMax;
            PlaneX = (int) (halfWidth * scale);
            PlaneY = (int) (halfHeight * scale);

            for (tick = 10; tick < RootContainer; tick += 10)
            {
                double scaledTick = tick / scale;
                if (scaledTick > minScreenTick && scaledTick < maxScreenTick)
                    break;
            }
            screenTick = (int)(tick / scale);
            ticksToX = (PlaneX / tick) + 1;
            ticksFromX = -ticksToX;
            ticksToY = (PlaneY / tick) + 1;
            ticksFromY = -ticksToY;

            //Image.DrawEllipseCentered((int)(halfWidth + RootX / scale), (int)(halfHeight + RootY / scale), 2, 2, Colors.Red);
            return true;
        }

        //малює результат і оновлює лише налаштування фігури
        public bool GenerateTransformOnly(CancellationToken? token)
        {
            Image.Lock();
            var bitmap = new Bitmap(Image.PixelWidth, 
                                    Image.PixelHeight, 
                                    Image.BackBufferStride,
                                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                                    Image.BackBuffer);
            graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            //TODO
            drawGrid();
            drawHexagon();

            graphics.Dispose();
            bitmap.Dispose();
            Image.AddDirtyRect(new Int32Rect(0, 0, width, height));
            Image.Unlock();
            return true;
        }

        //малює результат і оновлює налаштування сітки та фігури
        public bool FullGenerate(int width, int height, CancellationToken? token)
        {
            if (!AdjustView(width, height, token))
                return false;
            return GenerateTransformOnly(token);
        }
    }
}
