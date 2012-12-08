using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkEngines {
	public class ClassTableIntegerTextboxProvider: IClassTableEditorProvider {
		public ClassTableEditor GetNewEditor() {
			return new ClassTableTextBox<int?>(x => {
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
