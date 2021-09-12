using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathematics
{
    public class SparseMatrix<T>
    {
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public long MaxSize { get; private set; }
        public long Count { get { return _cells.Count; } }

        private Dictionary<long, T> _cells = new Dictionary<long, T>();

        private Dictionary<int, Dictionary<int, T>> _rows =
            new Dictionary<int, Dictionary<int, T>>();

        private Dictionary<int, Dictionary<int, T>> _columns =
            new Dictionary<int, Dictionary<int, T>>();

        public SparseMatrix(int c, int r)
        {
            this.Columns = c;
            this.Rows = r;
            this.MaxSize = c * r;
        }

        public bool IsCellEmpty(int row, int col)
        {
            long index = row * Columns + col;
            return _cells.ContainsKey(index);
        }

        public static SparseMatrix<T> operator *(SparseMatrix<T> a, SparseMatrix<T> b)
        {

            int rad = a.Rows;
            int kol = b.Columns;

            if (a.Columns != b.Rows)
                throw new Exception("Not compatible matrices");

            SparseMatrix<T> result = new SparseMatrix<T>(rad, kol);

            for (int i = 0; i < rad; i++)
            {
                for (int j = 0; j < kol; j++)
                {
                    for (int k = 0; k < a.Columns; k++)
                    {      
                        if ((!a.IsCellEmpty(i,k))&&(!b.IsCellEmpty(k,j)))
                            result[i, j] += (dynamic)a[i, k] * (dynamic)b[k, j];
                    }
                }
            }
            return result;
        }

        public T this[int row, int col]
        {
            get
            {
                long index = row * Columns + col;
                _cells.TryGetValue(index, out T result);
                return result;
            }
            set
            {
                long index = row * Columns + col;
                _cells[index] = value;

                UpdateValue(col, row, _columns, value);
                UpdateValue(row, col, _rows, value);
            }
        }

        private void UpdateValue(int index1, int index2,
            Dictionary<int, Dictionary<int, T>> parent, T value)
        {
            if (!parent.TryGetValue(index1, out Dictionary<int, T> dict))
            {
                parent[index2] = dict = new Dictionary<int, T>();
            }
            dict[index2] = value;
        }
    }
}
