using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DarkEngines {
	public class ClassTableEntityEditor: CompositeControl {
		public Table table;
		public int Columns {
			get;
			set;
		}
		public object Entity {
			get;
			set;
		}
		public ClassInfo ClassInfo {
			get;
			set;
		}
		protected Dictionary<string, ClassTableEditor> fieldEditorMap {
			get;
			set;
		}

		public ClassTableEntityEditor() {
			fieldEditorMap = new Dictionary<string, ClassTableEditor>();
		}

		protected override void CreateChildControls() {
			base.CreateChildControls();
			table = new Table();

			var members = ClassInfo.Members.OrderBy(member => member.Position);
			TableRow editorRow = null;
			
			int i = 0;
			foreach (var member in members) {

				if (i == 0 || (i) % Columns == 0) {
					editorRow = new TableRow();
					editorRow.EnableViewState = true;
					table.Rows.Add(editorRow);
				}

				var editorCell = new TableCell();
				var labelCell = new TableCell();
				editorCell.EnableViewState = true;
				var lblEditor = new Label();
				lblEditor.Text = member.Label;
				var editor = member.Editor;
				editor.SetValue(ExpressionHelper.ObjectFromMemberName(Entity, member.Name));
				fieldEditorMap.Add(member.Name, editor);
				labelCell.Controls.Add(lblEditor);
				editorCell.Controls.Add((Control)editor);
				editor.ValueChanged += new EventHandler<ValueChangedEventArgs>(editor_ValueChanged);
				editorRow.Cells.Add(labelCell);
				editorRow.Cells.Add(editorCell);
				i++;
			}
			var buttonsRow = new TableRow();
			var buttonsCell = new TableCell();
			buttonsCell.ColumnSpan = Columns;
			var btnSave = new Button();
			btnSave.Text = "Save";
			btnSave.EnableViewState = true;
			btnSave.Click += btnSave_Click;
			buttonsCell.Controls.Add(btnSave);
			buttonsRow.Cells.Add(buttonsCell);
			table.Rows.Add(buttonsRow);
			Controls.Add(table);
		}

		void btnSave_Click(object sender, EventArgs e) {
			throw new NotImplementedException();
		}

		private void editor_ValueChanged(object sender, ValueChangedEventArgs e) {
			throw new NotImplementedException();
		}

		protected override void Render(HtmlTextWriter writer) {
			base.Render(writer);
		}
	}
}
