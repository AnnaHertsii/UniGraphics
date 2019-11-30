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
        
        void setCell(int row, int column, double value)
        {
            items[row, column] = value;
        }

        double at(int row, int column)
        {
            return items[row, column];
        }

        Matrix deepCopy()
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
                    for (int k = 0; k < second.Rows; ++k)
                        result.setCell(i, j, first.at(i, k) * second.at(k, j));
            return result;
        }
    }
}
