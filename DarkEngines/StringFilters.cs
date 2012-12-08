using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DarkEngines {
	public static class StringFilters {
		private static Collection<Filter> filters;
		static StringFilters() {
			filters = new Collection<Filter>();
			
			filters.Add(new Filter((m, v) => string.Format("{0}.Contains(\"{1}\")", m, v), "Contain", "⊃", 0));
			filters.Add(new Filter((m, v) => string.Format("{0}.Equals(\"{1}\")", m, v), "Equal", "=", 1));
			//filters.Add(new Filter((m, v) => string.Format("{0}.CompareTo(\"{1}\") < 1", m, v), "LessThan", "<", 2));
			//filters.Add(new Filter((m, v) => string.Format("{0}.CompareTo(\"{1}\") > 1", m, v), "GreaterThan", ">", 3));
		}
		public static IEnumerable<Filter> Filters {
			get {
				return filters;
			}
		}
	}
}
