using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkEngines {
	public class SaveOrUpdateEventArgs: EventArgs {
		public object Item {
			get;
			private set;
		}
		public SaveOrUpdateEventArgs(object item) {
			Item = item;
		}
	}
}
