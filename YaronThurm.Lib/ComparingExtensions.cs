using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ComparingExtensions
    {
        /// <summary>
        /// This method gets 2 objects and an array of methods. 
        /// Each method will extract a single aspect from the objects in order to compare between them, each aspcet at a time.
        /// The method will return -1 if 'a' is "smaller" than 'b', 1 if 'a' is "bigger" than 'b' and 0 if the 'a' and 'b' are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="comparingMethods"></param>
        /// <returns></returns>
        public static int CompareToByAspects<T>(this T a, T b, params Func<T, IComparable>[] comparingMethods)
        {
            if (comparingMethods == null)
                throw new ArgumentNullException();

            if (a == null && b == null) return 0;
            else if (b == null) return -1;
            else if (a == null) return 1;

            for (int i = 0; i < comparingMethods.Length; i++)
            {
                IComparable keyA = comparingMethods[i](a);
                IComparable keyB = comparingMethods[i](b);

                if (keyA == null && keyB == null) continue;
                else if (keyB == null) return -1;
                else if (keyA == null) return 1;

                int c = keyA.CompareTo(keyB);
                if (c != 0) return c;
            }
            return 0;
        }
    }
}
