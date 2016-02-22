using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaronThurm.Lib
{
    public class GenericComparer<T> : IComparer<T>
    {
        private Func<T, IComparable>[] _comparingMethods;
        public GenericComparer(params Func<T, IComparable>[] comparingMethods)
        {
            this._comparingMethods = comparingMethods;
        }

        public int Compare(T x, T y)
        {
            return x.CompareToByAspects(y, _comparingMethods);
        }
    }
}
