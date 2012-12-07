﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI;
using System.Linq.Dynamic;
using System.Collections.ObjectModel;

namespace DarkEngines {
	public class ClassTable : CompositeControl {
		public const int ITEM_PER_PAGE = 50;
        public const int PAGE_INDEX = 0;
		public ClassInfo classInfo;
		private string filter {
			get {
				return (string)ViewState["filter"];
			}
			set {
				ViewState["filter"] = value;
			}
		}
		private int itemPerPage {
			get {
				if (ViewState["itemPerPage"] == null) return ITEM_PER_PAGE;
				return (int)ViewState["itemPerPage"];
			}
			set {
				ViewState["itemPerPage"] = value;
			}
		}
        private int pageIndex
        {
            get
            {
                if (ViewState["pageIndex"] == null) return PAGE_INDEX;
                return (int)ViewState["pageIndex"];
            }
            set
            {
                ViewState["pageIndex"] = value;
            }
        }
		private IQueryable<object> datasource;
		private bool rebuilding = false;
		public IQueryable<object> DataSource {
			set {
				datasource = value;
			}
		}
		public Table table;
		protected Collection<DropDownList> ddlFilters = new Collection<DropDownList>();
		protected Collection<ClassTableEditor> editors = new Collection<ClassTableEditor>();
		public event EventHandler<FilterChangedEventArgs> FilterChanged;

		protected DropDownList ddlPage;

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (!Page.IsPostBack) {
				int i = 0;
				foreach (var member in classInfo.Members) {
					var filters = member.Filters.OrderBy(f => f.Position);
					ddlFilters[i].DataSource = filters;
					ddlFilters[i].DataBind();
					i++;
				}
			}
		}
		protected override void CreateChildControls() {
			var members = classInfo.Members.OrderBy(m => m.Position);
			if (!rebuilding) {
				base.CreateChildControls();
				table = new Table();
				Controls.Add(table);
				table.EnableViewState = true;
				var headerRow = new TableRow();
				foreach (var member in members) {
					var headerCell = new TableHeaderCell();
					headerCell.Text = member.Label;
					headerRow.Cells.Add(headerCell);
				}
				table.Rows.Add(headerRow);

				var filterRow = new TableRow();
				filterRow.EnableViewState = true;
				foreach (var member in members) {
					var filterCell = new TableCell();
					filterCell.EnableViewState = true;
					var filters = member.Filters.OrderBy(f => f.Position);
					var ddlFilter = new DropDownList();
					ddlFilter.DataTextField = "Label";
					ddlFilter.DataValueField = "Name";
					var editor = member.Editor;
					filterCell.Controls.Add(ddlFilter);
					filterCell.Controls.Add((Control)editor);
					ddlFilters.Add(ddlFilter);
					ddlFilter.SelectedIndexChanged += new EventHandler(ddlFilter_SelectedIndexChanged);
					editors.Add(editor);
					editor.ValueChanged += new EventHandler<ValueChangedEventArgs>(editor_ValueChanged);
					ddlFilter.EnableViewState = true;
					ddlFilter.AutoPostBack = true;
					filterRow.Cells.Add(filterCell);
				}
				table.Rows.Add(filterRow);

				var paginationRow = new TableRow();
				var itemCountCell = new TableCell();
				itemCountCell.ColumnSpan = members.Count()/2;
				var lblItemCount = new Label();
				lblItemCount.Text = "Item per page:";
				var txtItemPerPage = new TextBox();
				txtItemPerPage.EnableViewState = true;
				lblItemCount.AssociatedControlID = txtItemPerPage.ID;
				txtItemPerPage.AutoPostBack = true;
				txtItemPerPage.TextChanged += txtItemPerPage_TextChanged;

				itemCountCell.Controls.Add(lblItemCount);
				itemCountCell.Controls.Add(txtItemPerPage);

				paginationRow.Cells.Add(itemCountCell);
                table.Rows.Add(paginationRow);
			}
			if (!Page.IsPostBack || rebuilding) {

                var pageIndexCell = new TableCell();
                pageIndexCell.HorizontalAlign = HorizontalAlign.Right;
                pageIndexCell.ColumnSpan = members.Count() / 2;
                var lblPageIndexPrefix = new Label();
                var lblPageIndexSufix = new Label();
                var ddlPageIndex = new DropDownList();
                lblPageIndexPrefix.Text = "Page ";


                lblPageIndexPrefix.AssociatedControlID = ddlPageIndex.ID;
                lblPageIndexSufix.AssociatedControlID = ddlPageIndex.ID;

                var tmpDatasource = filter == null ? datasource : datasource.Where(filter);
                var itemCount = tmpDatasource.Count();
                int pageCount = itemCount / itemPerPage;
                if (itemCount % itemPerPage != 0) pageCount++;
                lblPageIndexSufix.Text = string.Format("on {0}", pageCount);
                ddlPageIndex.SelectedIndexChanged += ddlPageIndex_SelectedIndexChanged;
                ddlPageIndex.Items.AddRange(Enumerable.Range(1, pageCount).Select(x => new ListItem(x.ToString(), (x-1).ToString())).ToArray());
                ddlPageIndex.AutoPostBack = true;
                ddlPageIndex.EnableViewState = true;
                ddlPageIndex.SelectedIndex = pageIndex;

                pageIndexCell.Controls.Add(lblPageIndexPrefix);
                pageIndexCell.Controls.Add(ddlPageIndex);
                pageIndexCell.Controls.Add(lblPageIndexSufix);

                table.Rows[2].Cells.Add(pageIndexCell);

				var dataSourceType = datasource.GetType().GetGenericArguments()[0];
				foreach (var item in tmpDatasource.Skip(itemPerPage * pageIndex).Take(itemPerPage)) {
					var dataRow = new TableRow();
					foreach (var member in members) {
						var dataCell = new TableCell();
						dataCell.Text = dataSourceType.GetProperty(member.Name).GetValue(item, new object[] { }).ToString();
						dataRow.Cells.Add(dataCell);
					}
					table.Rows.AddAt(2, dataRow);
				}
			}
		}

        void ddlPageIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            pageIndex = int.Parse(((DropDownList)sender).SelectedValue);
        }

		void txtItemPerPage_TextChanged(object sender, EventArgs e) {
			itemPerPage = int.Parse(((TextBox)sender).Text);
		}

		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if (Page.IsPostBack) {
				rebuilding = true;
                ChildControlsCreated = false;
                EnsureChildControls();
			}
		}

		protected void OnFilterChanged(object sender, EventArgs e) {
			var result = new Collection<Tuple<string, string, object>>();
			int i = 0;
			int count = classInfo.Members.Count();
			int position = 0;
			while (i < count) {
				position = classInfo.Members[i].Position;
				result.Add(new Tuple<string, string, object>(classInfo.Members[i].Name, ddlFilters[i].SelectedValue, editors[i].GetValue()));
				i++;
			}

			var request = string.Empty;
			var filters = result.Select(tuple => classInfo.CreateFilter(tuple.Item1, tuple.Item2, tuple.Item3 == null ? string.Empty : tuple.Item3.ToString())).Where(f => !string.IsNullOrEmpty(f));
			filter = string.Join(" AND ", filters);
		}

		void ddlFilter_SelectedIndexChanged(object sender, EventArgs e) {
			OnFilterChanged(sender, e);
		}

		protected void editor_ValueChanged(object sender, ValueChangedEventArgs e) {
			OnFilterChanged(sender, e);
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			table.RenderControl(writer);
		}
	}
}
