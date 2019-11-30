namespace UniGraphics.Transformation
{
    public class Matrix
    {
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        private double[,] items;
        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            items = new double[rows, columns];
        }

        public void setCell(int row, int column, double value)
        {
            items[row, column] = value;
        }

        public double At(int row, int column)
        {
            return items[row, column];
        }

        public Matrix deepCopy()
        {
            Matrix newMatrix = new Matrix(Rows, Columns);
            for (int i = 0; i < Rows; ++i)
                for (int j = 0; j < Columns; ++j)
                    newMatrix.setCell(i, j, items[i, j]);
            return newMatrix;
        }

        public static Matrix operator * (Matrix first, Matrix second)
        {
            Matrix result = new Matrix(first.Rows, second.Columns);
            for (int i = 0; i < first.Rows; ++i)
                for (int j = 0; j < second.Columns; ++j)
                {
                    double cellValue = 0.0;
                    for (int k = 0; k < second.Rows; ++k)
                        cellValue += first.At(i, k) * second.At(k, j);
                    result.setCell(i, j, cellValue);
                }
            return result;
        }
    }
}
