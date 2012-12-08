using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkEngines {
	public class ClassTableInteger64TextboxProvider: IClassTableEditorProvider {
		public ClassTableEditor GetNewEditor() {
			return new ClassTableTextBox<Int64?>(x => {
				int t;
				if (int.TryParse(x, out t)) {
					return t;
				} else {
					return null;
				}
			});
		}
	}
}
