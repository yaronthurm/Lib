using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using YaronThurm.Lib;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// A class that we can compare it's properties later
        /// </summary>
        public class ClassForComparing
        {
            public int ID;
            public int IntValue;
            public string StringValue;

            public override string ToString()
            {
                return ID.ToString();
            }
        }

        [TestMethod]
        public void TestCompare()
        {
            // Sort ascending
            ClassForComparing a = new ClassForComparing { ID = 1, StringValue = "abc", IntValue = 1 };
            ClassForComparing b = new ClassForComparing { ID = 2, StringValue = "abc", IntValue = 2 };
            ClassForComparing c = new ClassForComparing { ID = 3, StringValue = "xyz", IntValue = 1 };

            // Comparing 'a' to 'b' by string ascending, Should be a == b
            int ret = a.CompareToByAspects(b, x => x.StringValue);
            Assert.AreEqual(ret, 0);

            // Comparing 'a' to 'c' by string ascending, Should be a < c
            ret = a.CompareToByAspects(c, x => x.StringValue);
            Assert.AreEqual(ret, -1);

            // Comparing 'a' to 'c' by string descending, Should be a > c
            ret = a.CompareToByAspects(c, x => OrderedComparable.Desc(x.StringValue));
            Assert.AreEqual(ret, 1);

            // Comparing 'a' to 'b' by string ascending and then by int ascending, Should be a < b
            ret = a.CompareToByAspects(b, x => x.StringValue, x => x.IntValue);
            Assert.AreEqual(ret, -1);

            // Comparing 'a' to 'b' by string ascending and then by int ascending, Should be a < b (still)
            ret = a.CompareToByAspects(b, x => x.StringValue, x => OrderedComparable.Asc(x.IntValue));
            Assert.AreEqual(ret, -1);

            // Comparing 'a' to 'b' by string ascending and then by int descending, Should be a > b
            ret = a.CompareToByAspects(b, x => x.StringValue, x => OrderedComparable.Desc(x.IntValue));
            Assert.AreEqual(ret, 1);
        }

                
        [TestMethod]
        public void TestSorting()
        {
            // Sort ascending
            List<ClassForComparing> list = new List<ClassForComparing>{
                new ClassForComparing{ID=1, StringValue = "abc", IntValue = 1},
                new ClassForComparing{ID=2, StringValue = "abc", IntValue = 2},
                new ClassForComparing{ID=3, StringValue = "xyz", IntValue = 1},
                new ClassForComparing{ID=4, StringValue = "xyz", IntValue = 2}};

            // all ascending
            list.Sort(new GenericComparer<ClassForComparing>(x => x.StringValue, x => x.IntValue));
            this.CheckListOrder(list, 1, 2, 3, 4);

            // one aspect descending, the other ascending
            list.Sort(new GenericComparer<ClassForComparing>(x => OrderedComparable.Desc(x.StringValue), x => x.IntValue));
            this.CheckListOrder(list, 3, 4, 1, 2);

            // All descending
            list.Sort(new GenericComparer<ClassForComparing>(x => OrderedComparable.Desc(x.StringValue), x => OrderedComparable.Desc(x.IntValue)));
            this.CheckListOrder(list, 4, 3, 2, 1);
        }

        /// <summary>
        /// Given a list of instances and an expected order, will Assert.Fail if order is wrong
        /// </summary>
        /// <param name="list"></param>
        /// <param name="IDs"></param>
        private void CheckListOrder(List<ClassForComparing> list, params int[] IDs)
        {
            if (IDs.Length != list.Count)
                throw new Exception("Invalid lenght for checking list order");

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ID != IDs[i])
                    Assert.Fail(string.Format("ID {0} was found in index {1}, but expected ID {2}", list[i], i, IDs[i]));
            }
        }
    }
}
