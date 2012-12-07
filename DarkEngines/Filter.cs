using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace DarkEngines {
	public class Filter {
		public Func<string, string, string> FilterGenerator;
		public string Name {
			get;
			set;
		}
		public string Label {
			get;
			set;
		}
		public int Position {
			get;
			set;
		}
		public Filter(Func<string, string, string> filterGenerator, string name, string label, int position) {
			FilterGenerator = filterGenerator;
			Name = name;
			Label = label;
			Position = position;
		}
	}
}
