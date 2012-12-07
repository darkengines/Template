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
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			_textbox = new TextBox();
			Controls.Add(_textbox);
			EnableViewState = true;
			_textbox.AutoPostBack = true;
			_textbox.TextChanged += new EventHandler(_textbox_TextChanged);
		}

		void _textbox_TextChanged(object sender, EventArgs e) {
			ValueChanged(this, new ValueChangedEventArgs(Converter(_textbox.Text)));
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
	}
}
