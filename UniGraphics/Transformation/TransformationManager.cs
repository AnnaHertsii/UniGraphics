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

        private static readonly double offsetCoef = 1.1;
        private static readonly int minScreenTick = 50;
        private static readonly int maxScreenTick = 100;
        private static readonly double rotationSpeed = 60.0;
        private static readonly double scalingSpeed = 0.5;
        private static readonly Pen gridPen, axisGridPen;
        private static readonly DBrush tickTextBrush, hexagonBrush;
        private static readonly Font tickTextFont;

        private int width, height, tick, 
                    ticksFromX, ticksToX, ticksFromY, ticksToY;
        private double halfWidth, halfHeight, PlaneX, PlaneY, 
                       rootContainer, screenTick,
                       scale, animationRotation, animationScale;
        private Graphics graphics;
        private PointF[] hexagonPoints;
        private Matrix initialHexagonMatrix, toScreenMatrix,
                       rotationMatrix, scalingMatrix, 
                       planeMoveMatrix, PlaneMoveBackMatrix;

        static TransformationManager()
        {
            gridPen = new Pen(DColor.FromArgb(255, 79, 81, 100), 1);
            axisGridPen = new Pen(DColor.FromArgb(255, 79, 81, 100), 2);
            tickTextBrush = new SolidBrush(DColor.FromArgb(255, 79, 81, 100));
            tickTextFont = new Font("Times New Roman", 14, GraphicsUnit.Pixel);
            hexagonBrush = new SolidBrush(DColor.FromArgb(255, 104, 116, 219));
        }

        public TransformationManager(double rootX, double rootY, double sideLength)
        {
            initialHexagonMatrix = new Matrix(6, 3); //матриця з вершинами шестикутника
            //матриця для перетворення координат в декартовій
            //площині в координати у просторі екрану
            toScreenMatrix = new Matrix(3, 3);
            hexagonPoints = new PointF[6];
            animationRotation = 0.0;
            animationScale = 1.0;
            this.rootX = rootX;
            this.rootY = rootY;
            this.sideLength = sideLength;
            InitTransformMatrices();
            UpdateParameters();
        }

        //виконує початкове налаштування матриць перетворень
        private void InitTransformMatrices()
        {
            rotationMatrix = new Matrix(3, 3);
            scalingMatrix = new Matrix(3, 3);
            planeMoveMatrix = new Matrix(3, 3);
            PlaneMoveBackMatrix = new Matrix(3, 3);

            rotationMatrix.Set(0, 0, 1.0);
            rotationMatrix.Set(1, 1, 1.0);
            rotationMatrix.Set(2, 2, 1.0);

            scalingMatrix = rotationMatrix.DeepCopy();

            planeMoveMatrix = rotationMatrix.DeepCopy();

            PlaneMoveBackMatrix = rotationMatrix.DeepCopy();
        }

        //промальовка сітки
        private void DrawGrid()
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

        //малює шестикутник
        private void DrawHexagon()
        {
            graphics.FillPolygon(hexagonBrush, hexagonPoints);
        }

        //переводить координати вказаної матриці в 
        //масив з точками, які є в просторі екрану
        private void SaveToScreen(Matrix m)
        {
            Matrix inScreenCoords = m * toScreenMatrix;
            for(int i = 0; i < 6; ++i)
            {
                hexagonPoints[i].X = (float) inScreenCoords.At(i, 0);
                hexagonPoints[i].Y = (float) inScreenCoords.At(i, 1);
            }
        }

        //виконує множення матриць афінних перетворень
        private void ProcessHexagon()
        {
            Matrix result = initialHexagonMatrix *
                            planeMoveMatrix *
                            scalingMatrix *
                            rotationMatrix *
                            PlaneMoveBackMatrix;
            SaveToScreen(result);
        }

        //оновлює параметри шестикутника
        private void UpdateParameters()
        {
            double sideHalf = sideLength / 2;
            double side3Halves = sideHalf * 3;
            double offsetY = sideLength * (Math.Sqrt(3) / 2);
            double offsetY2 = offsetY * 2;

            //створення матриці з початковими координатами шестикутника
            initialHexagonMatrix.Set(0, 0, rootX);
            initialHexagonMatrix.Set(0, 1, rootY);
            initialHexagonMatrix.Set(0, 2, 1.0);

            initialHexagonMatrix.Set(1, 0, rootX + sideLength);
            initialHexagonMatrix.Set(1, 1, rootY);
            initialHexagonMatrix.Set(1, 2, 1.0);

            initialHexagonMatrix.Set(2, 0, rootX + side3Halves);
            initialHexagonMatrix.Set(2, 1, rootY + offsetY);
            initialHexagonMatrix.Set(2, 2, 1.0);

            initialHexagonMatrix.Set(3, 0, rootX + sideLength);
            initialHexagonMatrix.Set(3, 1, rootY + offsetY2);
            initialHexagonMatrix.Set(3, 2, 1.0);

            initialHexagonMatrix.Set(4, 0, rootX);
            initialHexagonMatrix.Set(4, 1, rootY + offsetY2);
            initialHexagonMatrix.Set(4, 2, 1.0);

            initialHexagonMatrix.Set(5, 0, rootX - sideHalf);
            initialHexagonMatrix.Set(5, 1, rootY + offsetY);
            initialHexagonMatrix.Set(5, 2, 1.0);

            //оновлення матриць зміщення координатної площини для анімації
            planeMoveMatrix.Set(2, 0, -RootX);
            planeMoveMatrix.Set(2, 1, -RootY);

            PlaneMoveBackMatrix.Set(2, 0, RootX);
            PlaneMoveBackMatrix.Set(2, 1, RootY);
        }

        //оновлює налаштування сітки
        private bool AdjustView(int width, int height, CancellationToken? token)
        {
            this.width = width;
            this.height = height;
            halfWidth = width / 2;
            halfHeight = height / 2;
            double rootAbsX = Math.Abs(rootX),
                   rootAbsY = Math.Abs(rootY);

            rootContainer = (Math.Max(rootAbsX, rootAbsY) + sideLength * 2) * offsetCoef;
            double containerScreenLenght = (rootAbsX > rootAbsY) ? halfWidth : halfHeight;
            scale = rootContainer / containerScreenLenght;
            PlaneX = (int) (halfWidth * scale);
            PlaneY = (int) (halfHeight * scale);

            if (CheckEnd(token))
                return false;

            for (tick = 10; tick < rootContainer; tick += 10)
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

            if (CheckEnd(token))
                return false;

            //створення матриці для перетворення в координати екрану
            toScreenMatrix.Set(0, 0, 1.0 / scale);
            toScreenMatrix.Set(0, 1, 0.0);
            toScreenMatrix.Set(0, 2, 0.0);

            toScreenMatrix.Set(1, 0, 0.0);
            toScreenMatrix.Set(1, 1, 1.0 / scale);
            toScreenMatrix.Set(1, 2, 0.0);

            toScreenMatrix.Set(2, 0, halfWidth);
            toScreenMatrix.Set(2, 1, halfHeight);
            toScreenMatrix.Set(2, 2, 1.0);

            return true;
        }

        //виконує зміну параметрів (поворот і масштаб) в часі
        public bool HandleTimeFlow(double deltaTime)
        {
            bool finished = false;
            animationRotation += rotationSpeed * deltaTime;
            animationScale += scalingSpeed * deltaTime;
            if (animationRotation > MaxRotation)
            {
                animationRotation = MaxRotation;
                finished = true;
            }

            scalingMatrix.Set(0, 0, animationScale);
            scalingMatrix.Set(1, 1, animationScale);

            double cos = Math.Cos(Deg2Rad(animationRotation));
            double sin = Math.Sin(Deg2Rad(animationRotation));
            rotationMatrix.Set(0, 0, cos);
            rotationMatrix.Set(0, 1, sin);
            rotationMatrix.Set(1, 0, -sin);
            rotationMatrix.Set(1, 1, cos);

            return finished;
        }

        //малює результат і оновлює лише налаштування фігури
        public bool UpdateTransform(CancellationToken? token)
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

            ProcessHexagon();

            if (CheckEnd(token, bitmap, graphics))
                return false;

            DrawGrid();

            if (CheckEnd(token, bitmap, graphics))
                return false;

            DrawHexagon();

            CleanUp(bitmap, graphics);
            return true;
        }

        //малює результат і оновлює налаштування сітки та фігури
        public bool FullGenerate(int width, int height, CancellationToken? token)
        {
            if (!AdjustView(width, height, token))
                return false;
            return UpdateTransform(token);
        }

        //звільняє ресурси і розблоковує зображення
        private void CleanUp(Bitmap bitmap, Graphics graphics)
        {
            graphics.Dispose();
            bitmap.Dispose();
            Image.AddDirtyRect(new Int32Rect(0, 0, width, height));
            Image.Unlock();
        }

        //робить перевірку чи було запитано зупинку виконання потоку
        //і за потреби виконує звільнення ресурсів
        private bool CheckEnd(CancellationToken? token, Bitmap bitmap, Graphics graphics)
        {
            if (token != null && token.Value.IsCancellationRequested)
            {
                CleanUp(bitmap, graphics);
                return true;
            }
            return false;
        }

        //робить перевірку чи було запитано зупинку виконання потоку
        private bool CheckEnd(CancellationToken? token)
        {
            return token != null && token.Value.IsCancellationRequested;
        }

        public static double Deg2Rad(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
