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
        private double rootX, rootY, sideLength;
        public double RootX
        {
            get { return rootX; }
            set
            {
                rootX = value;
                UpdateParameters();
            }
        }
        public double RootY
        {
            get { return rootY; }
            set
            {
                rootY = value;
                UpdateParameters();
            }
        }
        public double SideLength
        {
            get { return sideLength; }
            set
            {
                sideLength = value;
                UpdateParameters();
            }
        }
        public double MaxRotation { get; set; }
        public bool RotateAroundCenter { get; set; }
        public int VertexIndex { get; set; }
        public WriteableBitmap Image { get; private set; }

        private static readonly int offset = 100;
        private static readonly int minScreenTick = 50;
        private static readonly int maxScreenTick = 100;
        private static readonly Pen gridPen, axisGridPen;
        private static readonly DBrush tickTextBrush, hexagonBrush;
        private static readonly Font tickTextFont;

        private int width, height, tick, 
                    ticksFromX, ticksToX, ticksFromY, ticksToY;
        private double halfWidth, halfHeight, PlaneX, PlaneY, 
                       RootContainer, screenTick,
                       scale, animationRotation, animationScale;
        private Graphics graphics;
        private PointF[] hexagonPoints;
        private Matrix initialHexagonMatrix, toScreenMatrix;

        static TransformationManager()
        {
            gridPen = new Pen(DColor.FromArgb(255, 79, 81, 100), 1);
            axisGridPen = new Pen(DColor.FromArgb(255, 79, 81, 100), 2);
            tickTextBrush = new SolidBrush(DColor.FromArgb(255, 79, 81, 100));
            tickTextFont = new Font("Times New Roman", 14, GraphicsUnit.Pixel);
            hexagonBrush = new SolidBrush(DColor.FromArgb(255, 104, 116, 219));
        }

        public TransformationManager()
        {
            initialHexagonMatrix = new Matrix(6, 3); //матриця з вершинами шестикутника
            //матриця для перетворення координат в декартовій
            //площині в координати у просторі екрану
            toScreenMatrix = new Matrix(3, 3);
            hexagonPoints = new PointF[6];
            animationRotation = 0.0;
            animationScale = 1.0;
        }

        //промальовка сітки
        private void drawGrid()
        {
            double coord;
            for (int i = ticksFromX; i <= ticksToX; ++i)
            {
                coord = halfWidth + i * screenTick;
                if (i == 0)
                    graphics.DrawLine(axisGridPen, (float) coord, 0, (float) coord, height);
                else
                    graphics.DrawLine(gridPen, (float) coord, 0, (float) coord, height);
                graphics.DrawString((i * tick).ToString(), tickTextFont, 
                                    tickTextBrush, (float) coord, (float) halfHeight);
            }
            for (int i = ticksFromY; i <= ticksToY; ++i)
            {
                coord = halfHeight - i * screenTick;
                if (i == 0)
                    graphics.DrawLine(axisGridPen, 0, (float) coord, width, (float) coord);
                else
                {
                    graphics.DrawLine(gridPen, 0, (float) coord, width, (float) coord);
                    graphics.DrawString((i * tick).ToString(), tickTextFont,
                                        tickTextBrush, (float) halfWidth, (float) coord);
                }
            }
            //малювання трикутників для стрілок осей
            graphics.FillPolygon(tickTextBrush, new PointF[] {
                new PointF((float) halfWidth, -2),
                new PointF((float) (halfWidth + 5), 10),
                new PointF((float) (halfWidth - 6), 10)
            });
            graphics.FillPolygon(tickTextBrush, new PointF[] {
                new PointF(width + 2, (float) halfHeight),
                new PointF(width - 10, (float) (halfHeight + 5)),
                new PointF(width - 10, (float) (halfHeight - 6))
            });
        }

        private void drawHexagon()
        {
            graphics.FillPolygon(hexagonBrush, hexagonPoints);
        }

        private void saveToScreen(Matrix m)
        {
            Matrix inScreenCoords = m * toScreenMatrix;
            for(int i = 0; i < 6; ++i)
            {
                hexagonPoints[i].X = (float) inScreenCoords.At(i, 0);
                hexagonPoints[i].Y = (float) inScreenCoords.At(i, 1);
            }
        }

        private void processHexagon()
        {
            //TODO affine transformations
            saveToScreen(initialHexagonMatrix);
        }

        private void UpdateParameters()
        {
            double sideHalf = sideLength / 2;
            double side3Halves = sideHalf * 3;
            double offsetY = sideLength * (Math.Sqrt(3) / 2);
            double offsetY2 = offsetY * 2;

            //створення матриці з початковими координатами шестикутника
            initialHexagonMatrix.setCell(0, 0, rootX);
            initialHexagonMatrix.setCell(0, 1, rootY);
            initialHexagonMatrix.setCell(0, 2, 1.0);

            initialHexagonMatrix.setCell(1, 0, rootX + sideLength);
            initialHexagonMatrix.setCell(1, 1, rootY);
            initialHexagonMatrix.setCell(1, 2, 1.0);

            initialHexagonMatrix.setCell(2, 0, rootX + side3Halves);
            initialHexagonMatrix.setCell(2, 1, rootY + offsetY);
            initialHexagonMatrix.setCell(2, 2, 1.0);

            initialHexagonMatrix.setCell(3, 0, rootX + sideLength);
            initialHexagonMatrix.setCell(3, 1, rootY + offsetY2);
            initialHexagonMatrix.setCell(3, 2, 1.0);

            initialHexagonMatrix.setCell(4, 0, rootX);
            initialHexagonMatrix.setCell(4, 1, rootY + offsetY2);
            initialHexagonMatrix.setCell(4, 2, 1.0);

            initialHexagonMatrix.setCell(5, 0, rootX - sideHalf);
            initialHexagonMatrix.setCell(5, 1, rootY + offsetY);
            initialHexagonMatrix.setCell(5, 2, 1.0);
        }

        //оновлює налаштування сітки
        private bool AdjustView(int width, int height, CancellationToken? token)
        {
            this.width = width;
            this.height = height;
            halfWidth = width / 2;
            halfHeight = height / 2;

            RootContainer = (int) Math.Max(Math.Abs(rootX) + offset, Math.Abs(rootY) + offset);
            double screenMax = Math.Max(halfWidth, halfHeight);
            scale = RootContainer / screenMax;
            PlaneX = (int) (halfWidth * scale);
            PlaneY = (int) (halfHeight * scale);

            for (tick = 10; tick < RootContainer; tick += 10)
            {
                double scaledTick = tick / scale;
                if (scaledTick > minScreenTick && scaledTick < maxScreenTick)
                    break;
            }
            screenTick = tick / scale;
            ticksToX = (int) (PlaneX / tick) + 1;
            ticksFromX = -ticksToX;
            ticksToY = (int) (PlaneY / tick) + 1;
            ticksFromY = -ticksToY;

            //створення матриці для перетворення в координати екрану
            toScreenMatrix.setCell(0, 0, 1.0 / scale);
            toScreenMatrix.setCell(0, 1, 0.0);
            toScreenMatrix.setCell(0, 2, 0.0);

            toScreenMatrix.setCell(1, 0, 0.0);
            toScreenMatrix.setCell(1, 1, 1.0 / scale);
            toScreenMatrix.setCell(1, 2, 0.0);

            toScreenMatrix.setCell(2, 0, halfWidth);
            toScreenMatrix.setCell(2, 1, halfHeight);
            toScreenMatrix.setCell(2, 2, 1.0);

            return true;
        }

        //малює результат і оновлює лише налаштування фігури
        public bool GenerateTransformOnly(CancellationToken? token)
        {
            Image = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            Image.Lock();
            var bitmap = new Bitmap(Image.PixelWidth, 
                                    Image.PixelHeight, 
                                    Image.BackBufferStride,
                                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                                    Image.BackBuffer);
            graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            processHexagon();
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
