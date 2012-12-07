using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkEngines {
	public class ValueChangedEventArgs: EventArgs {
		public object Value {
			get;
			private set;
		}
		public ValueChangedEventArgs(object value) {
			Value = value;
		}
	}
}
