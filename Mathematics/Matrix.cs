using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace Mathematics
{
    [Serializable]
    [KnownType(typeof(Complex))]
    public class Matrix<T> : ISerializable
    {
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        private T[,] Items { get; set; }

        protected Matrix(SerializationInfo info, StreamingContext context)
        {
            string ff = (string)info.GetValue("0_0", typeof(string));
            string[] y = ff.Split('_');
            Rows = int.Parse(y[0]);
            Columns = int.Parse(y[1]);
            Items = JsonConvert.DeserializeObject<T[,]>((string)info.GetValue("Items", typeof(string)));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("0_0", Rows.ToString() + "_" + Columns.ToString());
            info.AddValue("Items", JsonConvert.SerializeObject(Items, Formatting.Indented));
        }

        public Matrix()
        {
            if (!HasMathemathicsSupport())
                throw new Exception(typeof(T).ToString() + " do not implement mathematical operators");
        }

        public Matrix(int row, int col)
        {
            if (!HasMathemathicsSupport())
                throw new Exception(typeof(T).ToString() + " do not implement mathematical operators");

            Rows = row;
            Columns = col;

            Items = new T[row, col];
        }

        public ref T this[int row, int column] => ref Items[row, column];

        bool HasMathemathicsSupport()
        {
            var c = Expression.Constant(default(T), typeof(T));
            try
            {
                Expression.Multiply(c, c);
                Expression.Add(c, c);
                Expression.Subtract(c, c);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b)
        {
            int rad = a.Rows;
            int kol = b.Columns;

            if (a.Columns != b.Rows)
                throw new Exception("Not compatible matrices");

            Matrix<T> result = new Matrix<T>(rad, kol);

            for (int i = 0; i < rad; i++)
            {
                for (int j = 0; j < kol; j++)
                {
                    for (int k = 0; k < a.Columns; k++)
                    {
                        result[i, j] += (dynamic)a[i, k] * (dynamic)b[k, j];
                    }
                }
            }
            return result;
        }

        public static Matrix<T> operator *(T a, Matrix<T> b)
        {
            int rad = b.Rows;
            int kol = b.Columns;

            Matrix<T> result = new Matrix<T>(rad, kol);

            for (int i = 0; i < rad; i++)
            {
                for (int j = 0; j < kol; j++)
                {
                    result[i, j] += (dynamic)a * (dynamic)b[i, j];
                }
            }
            return result;
        }

        public static Matrix<T> operator *(Matrix<T> b, T a)
        {

            int rad = b.Rows;
            int kol = b.Columns;

            Matrix<T> result = new Matrix<T>(rad, kol);

            for (int i = 0; i < rad; i++)
            {
                for (int j = 0; j < kol; j++)
                {
                    result[i, j] += (dynamic)a * (dynamic)b[i, j];
                }
            }
            return result;
        }
    }
}
