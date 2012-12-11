using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace DarkEngines {
	public class ClassTableTextBox<T>: CompositeControl, ClassTableEditor {
		public event EventHandler<ValueChangedEventArgs> ValueChanged;
		private TextBox _textbox;
		public ClassTableTextBox(Func<string, T> converter) {
			Converter = converter;
			_textbox = new TextBox();
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			Controls.Add(_textbox);
			EnableViewState = true;
			_textbox.TextChanged += new EventHandler(_textbox_TextChanged);
		}

		public void SetAutoPostBack(bool value) {
			_textbox.AutoPostBack = value;
		}

		void _textbox_TextChanged(object sender, EventArgs e) {
			if (ValueChanged != null) {
				ValueChanged(this, new ValueChangedEventArgs(Converter(_textbox.Text)));
			}
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
		}

		public Func<string, T> Converter {
			get;
			set;
		}
		public object GetValue() {
			return Converter(_textbox.Text);
		}
		public void SetValue(object value) {
			_textbox.Text = value == null ? null : value.ToString();
		}
	}
}
