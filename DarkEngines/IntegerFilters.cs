using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DarkEngines {
	public static class IntegerFilters {
		private static Collection<Filter> filters;
		static IntegerFilters() {
			filters = new Collection<Filter>();
			filters.Add(new Filter((m, v) => string.IsNullOrEmpty(m) || string.IsNullOrEmpty(v) ? null : string.Format("{0}={1}", m, v), "Equal", "=", 0));
			filters.Add(new Filter((m, v) => string.IsNullOrEmpty(m) || string.IsNullOrEmpty(v) ? null : string.Format("{0}<{1}", m, v), "LessThan", "<", 1));
			filters.Add(new Filter((m, v) => string.IsNullOrEmpty(m) || string.IsNullOrEmpty(v) ? null : string.Format("{0}>{1}", m, v), "GreaterThan", ">", 2));
		}
		public static IEnumerable<Filter> Filters {
			get {
				return filters;
			}
		}
	}
}
