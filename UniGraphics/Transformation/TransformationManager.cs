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
using System.Linq;

namespace UniGraphics.Transformation 
{
    public class TransformationManager
    {
        private double rootX, rootY, sideLength;
        bool rotateAroundCenter;
        int vertexIndex;
        public double RootX
        {
            get { return rootX; }
            set
            {
                rootX = value;
                UpdateParameters();
                Rollback();
            }
        }
        public double RootY
        {
            get { return rootY; }
            set
            {
                rootY = -value;
                UpdateParameters();
                Rollback();
            }
        }
        public double SideLength
        {
            get { return sideLength; }
            set
            {
                sideLength = value;
                CalculateVertexOffsets();
                UpdateRotationPivot();
                UpdateParameters();
                Rollback();
            }
        }
        public bool RotateAroundCenter
        {
            get { return rotateAroundCenter; }
            set
            {
                rotateAroundCenter = value;
                UpdateRotationPivot();
                Rollback();
            }
        }
        public int VertexIndex
        {
            get { return vertexIndex; }
            set
            {
                vertexIndex = value;
                UpdateRotationPivot();
                Rollback();
            }
        }

        public double MaxRotation { get; set; }
        public WriteableBitmap Image { get; private set; }

        private static readonly double offsetCoef = 1.1;
        private static readonly double rotationSpeed = 60.0;
        private static readonly double scalingSpeed = 0.5;
        private static readonly int pivotRadius = 10;
        private static readonly Pen gridPen, axisGridPen;
        private static readonly DBrush tickTextBrush, hexagonBrush, pivotBrush;
        private static readonly Font tickTextFont;

        private int width, height, 
                    ticksFromX, ticksToX, ticksFromY, ticksToY;
        private double halfWidth, halfHeight, PlaneX, PlaneY,
                       screenTick, tick,
                       scale, animationRotation, animationScale,
                       sideHalf, side3Halves, centerOffsetY, centerOffsetY2;
        private Graphics graphics;
        private PointF[] hexagonPoints;
        private double[,] vertexOffsets;
        private Matrix initialHexagonMatrix, toScreenMatrix,
                       rotationMatrix, scalingMatrix, 
                       planeMoveMatrix, planeMoveBackMatrix, 
                       pivotOffsetMatrix, pivotOffsetBackMatrix;

        static TransformationManager()
        {
            gridPen = new Pen(DColor.FromArgb(255, 79, 81, 100), 1);
            axisGridPen = new Pen(DColor.FromArgb(255, 79, 81, 100), 2);
            tickTextBrush = new SolidBrush(DColor.FromArgb(255, 79, 81, 100));
            tickTextFont = new Font("Times New Roman", 14, GraphicsUnit.Pixel);
            hexagonBrush = new SolidBrush(DColor.FromArgb(255, 104, 116, 219));
            pivotBrush = new SolidBrush(DColor.FromArgb(255, 255, 0, 0));
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
            this.rootY = -rootY;
            this.sideLength = sideLength;
            InitTransformMatrices();
            CalculateVertexOffsets();
            UpdateParameters();
            UpdateRotationPivot();
        }

        //виконує початкове налаштування матриць перетворень
        private void InitTransformMatrices()
        {
            rotationMatrix = new Matrix(3, 3);
            scalingMatrix = new Matrix(3, 3);
            planeMoveMatrix = new Matrix(3, 3);
            planeMoveBackMatrix = new Matrix(3, 3);
            pivotOffsetMatrix = new Matrix(3, 3);

            rotationMatrix.Set(0, 0, 1.0);
            rotationMatrix.Set(1, 1, 1.0);
            rotationMatrix.Set(2, 2, 1.0);

            scalingMatrix = rotationMatrix.DeepCopy();

            planeMoveMatrix = rotationMatrix.DeepCopy();

            planeMoveBackMatrix = rotationMatrix.DeepCopy();

            pivotOffsetMatrix = rotationMatrix.DeepCopy();

            pivotOffsetBackMatrix = rotationMatrix.DeepCopy();
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
            graphics.DrawString("X", tickTextFont, tickTextBrush, width - 20, (float)(halfHeight - 20));
            graphics.DrawString("Y", tickTextFont, tickTextBrush, (float)(halfWidth - 20), 5.0f);
        }

        //малює шестикутник
        private void DrawHexagon()
        {
            graphics.FillPolygon(hexagonBrush, hexagonPoints);
            PointF pivot;
            if (rotateAroundCenter)
                pivot = new PointF((hexagonPoints[0].X + hexagonPoints[3].X) / 2,
                                   (hexagonPoints[0].Y + hexagonPoints[3].Y) / 2);
            else
                pivot = hexagonPoints[vertexIndex - 1];
            int radiushalf = pivotRadius / 2;
            graphics.FillEllipse(pivotBrush, pivot.X - radiushalf, 
                                                pivot.Y - radiushalf,
                                                pivotRadius, pivotRadius);
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
            Matrix modification = planeMoveMatrix *
                                  pivotOffsetMatrix *
                                  scalingMatrix *
                                  rotationMatrix *
                                  pivotOffsetBackMatrix *
                                  planeMoveBackMatrix;
            Matrix result = initialHexagonMatrix * modification;
            SaveToScreen(result);
        }

        //розраховує числа, які потрібні для розрахунку позицій вершин
        private void CalculateVertexOffsets()
        {
            sideHalf = sideLength / 2;
            side3Halves = sideHalf * 3;
            centerOffsetY = sideLength * (Math.Sqrt(3) / 2);
            centerOffsetY2 = centerOffsetY * 2;

            vertexOffsets = new double[6, 2]
            {
                { 0.0, 0.0 },
                { sideLength, 0.0 },
                { side3Halves, centerOffsetY },
                { sideLength, 2 * centerOffsetY },
                { 0.0, 2 * centerOffsetY },
                { -sideHalf, centerOffsetY }
            };
        }

        //оновлює параметри шестикутника
        private void UpdateParameters()
        {
            //створення матриці з початковими координатами шестикутника
            initialHexagonMatrix.Set(0, 0, rootX);
            initialHexagonMatrix.Set(0, 1, rootY);
            initialHexagonMatrix.Set(0, 2, 1.0);

            initialHexagonMatrix.Set(1, 0, rootX + sideLength);
            initialHexagonMatrix.Set(1, 1, rootY);
            initialHexagonMatrix.Set(1, 2, 1.0);

            initialHexagonMatrix.Set(2, 0, rootX + side3Halves);
            initialHexagonMatrix.Set(2, 1, rootY + centerOffsetY);
            initialHexagonMatrix.Set(2, 2, 1.0);

            initialHexagonMatrix.Set(3, 0, rootX + sideLength);
            initialHexagonMatrix.Set(3, 1, rootY + centerOffsetY2);
            initialHexagonMatrix.Set(3, 2, 1.0);

            initialHexagonMatrix.Set(4, 0, rootX);
            initialHexagonMatrix.Set(4, 1, rootY + centerOffsetY2);
            initialHexagonMatrix.Set(4, 2, 1.0);

            initialHexagonMatrix.Set(5, 0, rootX - sideHalf);
            initialHexagonMatrix.Set(5, 1, rootY + centerOffsetY);
            initialHexagonMatrix.Set(5, 2, 1.0);

            //оновлення матриць зміщення координатної площини для анімації
            planeMoveMatrix.Set(2, 0, -rootX);
            planeMoveMatrix.Set(2, 1, -rootY);

            planeMoveBackMatrix.Set(2, 0, rootX);
            planeMoveBackMatrix.Set(2, 1, rootY);
        }

        //оновлює матрицю, що визначає точку, довкола якої відбувається поворот
        private void UpdateRotationPivot()
        {
            if(rotateAroundCenter)
            {
                pivotOffsetMatrix.Set(2, 0, -sideHalf);
                pivotOffsetMatrix.Set(2, 1, -centerOffsetY);

                pivotOffsetBackMatrix.Set(2, 0, sideHalf);
                pivotOffsetBackMatrix.Set(2, 1, centerOffsetY);
            }
            else
            {
                if (vertexIndex < 1)
                    return;
                pivotOffsetMatrix.Set(2, 0, -vertexOffsets[vertexIndex - 1, 0]);
                pivotOffsetMatrix.Set(2, 1, -vertexOffsets[vertexIndex - 1, 1]);

                pivotOffsetBackMatrix.Set(2, 0, vertexOffsets[vertexIndex - 1, 0]);
                pivotOffsetBackMatrix.Set(2, 1, vertexOffsets[vertexIndex - 1, 1]);
            }
        }

        //оновлює налаштування сітки
        private bool AdjustView(int width, int height, CancellationToken? token)
        {
            this.width = width;
            this.height = height;
            halfWidth = width / 2;
            halfHeight = height / 2;

            return AdjustView(token);
        }

        //рахує довжину одиничного відрізку
        private static double CalculateTick(double container)
        {
            int stepCount = 10;
            double roughStep = container / (stepCount - 1);

            double[] goodSteps = { 1.0, 2.0, 5.0, 10.0 };
            double stepPower = Math.Pow(10, -Math.Floor(Math.Log10(roughStep)));
            double normalizedStep = roughStep * stepPower;
            double goodNormalizedStep = goodSteps.First(step => step >= normalizedStep);
            return goodNormalizedStep / stepPower;
        }

        //шукає масштаб для того, щоб вмістити фігуру, а також "контейтер"
        private void CalculateContainerAndScale(out double rootContainer,
                                                       out double scale)
        {
            double figureFromX = rootX - sideHalf;
            double figureToX = rootX + side3Halves;
            double figureToY = rootY + centerOffsetY2;
            double maxX = Math.Max(Math.Abs(figureFromX), Math.Abs(figureToX));
            double maxY = Math.Max(Math.Abs(rootY), Math.Abs(figureToY));
            double maxCoord = Math.Max(maxX, maxY);
            rootContainer = (maxCoord + sideLength * 2) * offsetCoef;
            double containerScreenLength = (maxX > maxY) ? halfWidth : halfHeight;
            scale = rootContainer / containerScreenLength;
        }

        //оновлює налаштування сітки
        private bool AdjustView(CancellationToken? token)
        {
            double rootContainer;
            CalculateContainerAndScale(out rootContainer, out scale);

            PlaneX = halfWidth * scale;
            PlaneY = halfHeight * scale;

            if (CheckEnd(token))
                return false;

            tick = CalculateTick(rootContainer);

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
                animationScale = (MaxRotation / rotationSpeed) * scalingSpeed + 1.0;
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
        public bool Generate(int width, int height, CancellationToken? token)
        {
            if (!AdjustView(width, height, token))
                return false;
            return UpdateTransform(token);
        }

        public bool GenerateWithSameResolution(CancellationToken? token)
        {
            if (!AdjustView(token))
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

        public void Rollback()
        {
            animationRotation = 0.0;
            animationScale = 1.0;

            scalingMatrix.Set(0, 0, 1.0);
            scalingMatrix.Set(1, 1, 1.0);

            rotationMatrix.Set(0, 0, 1.0);
            rotationMatrix.Set(0, 1, 0.0);
            rotationMatrix.Set(1, 0, 0.0);
            rotationMatrix.Set(1, 1, 1.0);
        }
    }
}
