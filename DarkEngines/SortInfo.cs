using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkEngines
{
    [Serializable()] 
    public class SortInfo
    {
        public string Member
        {
            get;
            set;
        }
        public string Order
        {
            get;
            set;
        }
        public SortInfo(string member, string order)
        {
            Member = member;
            Order = order;
        }
        public string ToString()
        {
            return string.Format("{0} {1}", Member, Order);
        }
    }
}
