using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaronThurm.Lib
{
    /// <summary>
    /// This class is used to encapsulate any IComparable instance, along side with an OrderBy value, telling in which order
    /// the IComparable shuold be compared to (when applied to another instance with the same OrderBy value).
    /// e.g. Comparing two stringa in descending order would be achived by:
    /// OrderedComparable.Desc("abc").CompareTo(OrderedComparable.Desc("xyz")
    /// 
    /// Obviously, this comparing is redundent, since we might just use the '-' sign (-"abc".CompareTo("xyz"), but it is 
    /// vital in situations where we cannot use the explicit "CompareTo" method, e.g. when using "Aspect comparing".
    /// e.g. We can now use the following code to compare two Person instances, with the 'Name' property in descending order:
    /// person1.CompareToByAspects(person2, x => OrderedComparable.Desc(x.Name), x => x.Age /* will be compared ascending */)
    /// </summary>
    public class OrderedComparable : IComparable<OrderedComparable>, IComparable
    {
        public enum OrderByEnum { Desc, Asc }

        private OrderByEnum _orderBy;
        private IComparable _innerItem;

        public OrderedComparable(IComparable innerItem, OrderByEnum orderBy)
        {
            _orderBy = orderBy;
            _innerItem = innerItem;
        }


        public int CompareTo(OrderedComparable obj)
        {
            if (this._orderBy != obj._orderBy)
                throw new InvalidOperationException("Order by mismatch");

            // Compare this's inner item to the other's inner item - defualt comparing is always ascending
            int ascCompareResult = this._innerItem.CompareTo(obj._innerItem);

            // If we should compare Asc, then just return the comparing result, otherwise, revert it.
            if (_orderBy == OrderByEnum.Asc)
                return ascCompareResult;
            return -ascCompareResult;
        }

        public int CompareTo(object obj)
        {
            int ret = this.CompareTo((OrderedComparable)obj);
            return ret;
        }


        public static OrderedComparable Desc(IComparable item)
        {
            var ret = new OrderedComparable(item, OrderByEnum.Desc);
            return ret;
        }

        public static OrderedComparable Asc(IComparable item)
        {
            var ret = new OrderedComparable(item, OrderByEnum.Asc);
            return ret;
        }
    }
}
