using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkEngines {
	public interface IClassTableEditorProvider {
		ClassTableEditor GetNewEditor();
	}
}
