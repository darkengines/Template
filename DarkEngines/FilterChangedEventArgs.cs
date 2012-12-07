using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DarkEngines {
	public class FilterChangedEventArgs: EventArgs {
		public Collection<Tuple<string, string, object>> FilterInfoSet {
			get;
			private set;
		}
		public FilterChangedEventArgs(Collection<Tuple<string, string, object>> filterInfoSet) {
			FilterInfoSet = filterInfoSet;
		}
	}
}
