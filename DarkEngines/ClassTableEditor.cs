﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DarkEngines {
	public interface ClassTableEditor {
		event EventHandler<ValueChangedEventArgs> ValueChanged;
		object GetValue();
		void SetValue(object value);
		void SetAutoPostBack(bool value);
	}
}
