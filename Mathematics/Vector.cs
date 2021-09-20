using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathematics
{

    public struct Vector<T>
    {

        public Vector(int lengde)
        {
            Items = new T[lengde];
        }
        public T[] Items { get; set; }

        public int Length()
        {
            return Items.Length;
        }

        public ref T this[int row] => ref Items[row];

        public static T[] ShiftArrayRight(T[] v)
        {
            var OverflowItem = v[v.Length - 1];
            Array.Copy(v, 0, v, 1, v.Length - 1);
            v[0] = OverflowItem;
            return v;
        }

        public static T[] ShiftArray(T[] v, int index)
        {
            if (Math.Abs(index) > v.Length)
                throw new IndexOutOfRangeException("Index of shifted array components cannot be larger than array itself. Modulus calcualtions not supported");


            // Use modular arithmetic, assumes that abs(index) <= v.Length
            if (index < 0)
                index += v.Length;

            T[] Items = new T[v.Length];
            //public static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length);
            Array.Copy(v, 0, Items, index, v.Length - index);
            Array.Copy(v, v.Length - index, Items, 0, index);
            return Items;
        }

        public static Vector<T> operator +(Vector<T> a, Vector<T> b)
        {
            if (a.Length() == b.Length())
            {
                Vector<T> result = new Vector<T>(a.Length());
                for (int i = 0; i < a.Length(); i++)
                {
                    result[i] = (dynamic)a[i] + (dynamic)b[i];
                }
                return result;
            }
            throw new Exception("Dimension mismatch");
        }

        public static Vector<T> operator +(Vector<T> a, double[] b)
        {
            if (a.Length() == b.Length)
            {
                Vector<T> result = new Vector<T>(a.Length());
                for (int i = 0; i < a.Length(); i++)
                {
                    result[i] = (dynamic)a[i] + (dynamic)b[i];
                }
                return result;
            }
            throw new Exception("Dimension mismatch");
        }

        public static Vector<T> operator -(Vector<T> a, double[] b)
        {
            if (a.Length() == b.Length)
            {
                Vector<T> result = new Vector<T>(a.Length());
                for (int i = 0; i < a.Length(); i++)
                {
                    result[i] = (dynamic)a[i] - (dynamic)b[i];
                }
                return result;
            }
            throw new Exception("Dimension mismatch");
        }

        public static Vector<T> operator -(Vector<T> a, Vector<T> b)
        {
            if (a.Length() == b.Length())
            {
                Vector<T> result = new Vector<T>(a.Length());
                for (int i = 0; i < a.Length(); i++)
                {
                    result[i] = (dynamic)a[i] - (dynamic)b[i];
                }
                return result;
            }
            throw new Exception("Dimension mismatch");
        }
        public static Vector<T> operator +(double[] b, Vector<T> a)
        {
            if (a.Length() == b.Length)
            {
                Vector<T> result = new Vector<T>(a.Length());
                for (int i = 0; i < a.Length(); i++)
                {
                    result[i] = (dynamic)a[i] + (dynamic)b[i];
                }
                return result;
            }
            throw new Exception("Dimension mismatch");
        }

        public static Vector<T> operator +(Vector<T> a, double b)
        {
                Vector<T> result = new Vector<T>(a.Length());
                for (int i = 0; i < a.Length(); i++)
                {
                    result[i] = (dynamic)a[i] + (dynamic)b;
                }
            return result;
        }

        public static Vector<T> operator -(Vector<T> a, double b)
        {
            Vector<T> result = new Vector<T>(a.Length());
            for (int i = 0; i < a.Length(); i++)
            {
                result[i] = (dynamic)a[i] - (dynamic)b;
            }
            return result;
        }

        public static Vector<T> operator -(double b, Vector<T> a)
        {
            Vector<T> result = new Vector<T>(a.Length());
            for (int i = 0; i < a.Length(); i++)
            {
                result[i] = -(dynamic)a[i] + (dynamic)b;
            }
            return result;
        }
        public static Vector<T> operator +(double b, Vector<T> a)
        {
            Vector<T> result = new Vector<T>(a.Length());
            for (int i = 0; i < a.Length(); i++)
            {
                result[i] = (dynamic)a[i] + (dynamic)b;
            }
            return result;
        }

        public static Vector<T> operator -(Vector<T> a)
        {
            Vector<T> result = new Vector<T>(a.Length());
            for (int i = 0; i < a.Length(); i++)
                {
                    result[i] = -(dynamic)a[i];
                }

            return a;
        }

        public static Vector<T> operator ^(double a, Vector<T> b)
        {
            Vector<T> result = new Vector<T>(b.Length());
            for (int i = 0; i < b.Length(); i++)
            {
                result.Items[i] = Math.Pow(a, (dynamic)b[i]);

            }
            return result;
        }

        public T Sum()
        {
            dynamic res = 0;
            foreach (T item in Items)
            {
                res += (dynamic)item;
            }
            return (T)res;
        
        }

        public static Vector<T> operator *(Vector<T> a, double b)
        {
        Vector<T> result = new Vector<T>(a.Length());
        for (int i = 0; i < a.Length(); i++)
            {
                result[i] = (dynamic)a[i] * (dynamic)b;
            }
        return result;
        }

        public static Vector<T> operator *(double b, Vector<T> a)
        {
            Vector<T> result = new Vector<T>(a.Length());
            for (int i = 0; i < a.Length(); i++)
            {
                result[i] = (dynamic)a[i] * (dynamic)b;
            }
            return result;
        }

        public static Vector<T> operator /(Vector<T> a, double b)
        {
            Vector<T> result = new Vector<T>(a.Length());
            for (int i = 0; i < a.Length(); i++)
            {
                result[i] = (dynamic)a[i] / (dynamic)b;
            }
            return result;
        }

        public static implicit operator Vector<T>(double[] d)
        {
            Vector<T> a = new Vector<T>();
            T[] r= new T[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                r[i] = (dynamic)d[i];
            }
            a.Items = r;
            return a;
        }
    }
}
