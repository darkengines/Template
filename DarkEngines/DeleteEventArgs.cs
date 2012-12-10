using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkEngines
{
    public class DeleteEventArgs: EventArgs
    {
        public object Item
        {
            get;
            private set;
        }
        public DeleteEventArgs(object item)
        {
            Item = item;
        }
    }
}
