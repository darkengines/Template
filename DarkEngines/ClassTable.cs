﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI;
using System.Linq.Dynamic;
using System.Collections.ObjectModel;

namespace DarkEngines
{
    public class ClassTable : CompositeControl
    {
        public const int ITEM_PER_PAGE = 25;
        public const int PAGE_INDEX = 0;
        public ClassInfo classInfo;
		public event EventHandler<SaveOrUpdateEventArgs> SaveOrUpdate;
        public event EventHandler<DeleteEventArgs> Delete;
        private IQueryable<object> filteredDatasource
        {
            get
            {
                if (filter == null && !sortList.Any())
                {
                    return datasource.Skip(itemPerPage * pageIndex).Take(itemPerPage);
                } else {
                    IQueryable<object> result = datasource;
                    if (filter != null)
                    {
                        result = datasource.Where(filter);
                    }
                    if (sortList.Any())
                    {
                        result = result.OrderBy(string.Join(", ", sortList.Select(item => item.ToString())));
                    }
                    return result.Skip(itemPerPage * pageIndex).Take(itemPerPage);
                }
            }
        }
        private string filter
        {
            get
            {
                return (string)ViewState["filter"];
            }
            set
            {
                ViewState["filter"] = value;
            }
        }
        private int editItemId
        {
            get
            {
                if (ViewState["editItemId"] == null) return -1;
                return (int)ViewState["editItemId"];
            }
            set
            {
                ViewState["editItemId"] = value;
            }
        }
        private bool createNew
        {
            get
            {
                if (ViewState["createNew"] == null) return false;
                return (bool)ViewState["createNew"];
            }
            set
            {
                ViewState["createNew"] = value;
            }
        }
        private int itemPerPage
        {
            get
            {
                if (ViewState["itemPerPage"] == null) return ITEM_PER_PAGE;
                return (int)ViewState["itemPerPage"];
            }
            set
            {
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
        public IQueryable<object> DataSource
        {
            set
            {
                datasource = value;
            }
        }
        public Table table;
        protected Collection<DropDownList> ddlFilters = new Collection<DropDownList>();
        protected Collection<ClassTableEditor> editors = new Collection<ClassTableEditor>();
        protected DropDownList ddlPageIndex;
        protected Label lblPageIndexSufix;
        public event EventHandler<FilterChangedEventArgs> FilterChanged;
        protected bool refreshIndices = false;
        protected DropDownList ddlPage;
        protected List<SortInfo> sortList
        {
            get
            {
                if (ViewState["sortList"] == null)
                {
                    var list = new List<SortInfo>();
                    ViewState["sortList"] = list;
                    return list;
                }
                else
                {
                    return (List<SortInfo>)ViewState["sortList"];
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                int i = 0;
                foreach (var member in classInfo.Members)
                {
                    var filters = member.Filters.OrderBy(f => f.Position);
                    ddlFilters[i].DataSource = filters;
                    ddlFilters[i].DataBind();
                    i++;
                }
                var itemCount = filteredDatasource.Count();
                int pageCount = itemCount / itemPerPage;
                if (itemCount % itemPerPage != 0) pageCount++;
                lblPageIndexSufix.Text = string.Format("on {0}", pageCount);
                ddlPageIndex.DataSource = Enumerable.Range(1, pageCount).Select(x => new { Label = x.ToString(), Value = (x - 1).ToString() }).ToArray();
                ddlPageIndex.SelectedIndex = pageIndex;
                ddlPageIndex.DataBind();
            }
        }
        protected override void CreateChildControls()
        {
			var dataSourceType = datasource.GetType().GetGenericArguments()[0];
            var members = classInfo.Members.OrderBy(m => m.Position);
            if (!rebuilding)
            {
                base.CreateChildControls();
                table = new Table();
                Controls.Add(table);
                table.EnableViewState = true;
                var headerRow = new TableRow();
                headerRow.EnableViewState = true;
                foreach (var member in members)
                {
                    var headerCell = new TableHeaderCell();
                    headerCell.EnableViewState = true;
                    var lnkButtonSort = new LinkButton();
                    lnkButtonSort.Attributes["data"] = member.Name;
                    lnkButtonSort.EnableViewState = true;
                    lnkButtonSort.ID = string.Format("sortButton_{0}", member.Name);
                    lnkButtonSort.Text = member.Label;
                    lnkButtonSort.Click += lnkButtonSort_Click;
                    headerCell.Controls.Add(lnkButtonSort); 
                    headerRow.Cells.Add(headerCell);
                }
                var actionsHeaderCell = new TableHeaderCell();
                actionsHeaderCell.Text = "Actions";
                headerRow.Cells.Add(actionsHeaderCell);

                table.Rows.Add(headerRow);

                var filterRow = new TableRow();
                filterRow.EnableViewState = true;
                foreach (var member in members)
                {
                    var filterCell = new TableCell();
                    filterCell.EnableViewState = true;
                    var filters = member.Filters.OrderBy(f => f.Position);
                    var ddlFilter = new DropDownList();
                    ddlFilter.DataTextField = "Label";
                    ddlFilter.DataValueField = "Name";
                    var editor = member.Editor;
					editor.SetAutoPostBack(true);
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

                var buttonNewEntry = new LinkButton();
                buttonNewEntry.EnableViewState = true;
                buttonNewEntry.Text = "New entry";
                buttonNewEntry.Click += buttonNewEntry_Click;

                var cellNewEntry = new TableCell();
                cellNewEntry.Controls.Add(buttonNewEntry);

                filterRow.Cells.Add(cellNewEntry);
                table.Rows.Add(filterRow);
            }
            
                if (rebuilding)
                {
                    int y = 2;
                    int count = table.Rows.Count-1;
                    while (y < count)
                    {
                        table.Rows.RemoveAt(2);
                        y++;
                    }
					if (createNew) {
						var createNewRow = new TableRow();
						createNewRow.EnableViewState = true;
						var createNewCell = new TableCell();
						createNewCell.EnableViewState = true;
						createNewCell.ColumnSpan = members.Count() + 1;
						var newEntityEditor = new ClassTableEntityEditor();
						newEntityEditor.EnableViewState = true;
						newEntityEditor.ClassInfo = classInfo;
						newEntityEditor.ID = "newEntityEditor";
						newEntityEditor.Entity = Activator.CreateInstance(dataSourceType);
						newEntityEditor.Columns = members.Count() + 1;
						newEntityEditor.SaveOrUpdate += entityEditor_SaveOrUpdate;

						createNewCell.Controls.Add(newEntityEditor);
						createNewRow.Cells.Add(createNewCell);
						table.Rows.Add(createNewRow);
					}
                }
                
                int i = 0;
                foreach (var item in filteredDatasource)
                {
                    var dataRow = new TableRow();
                    dataRow.EnableViewState = true;
                    if (i == editItemId)
                    {
                        var editorCell = new TableCell();
                        editorCell.EnableViewState = true;
                        var entityEditor = new ClassTableEntityEditor();
                        entityEditor.Entity = item;
                        entityEditor.EnableViewState = true;
                        entityEditor.ID = "entityEditor";
                        entityEditor.ClassInfo = classInfo;
						entityEditor.SaveOrUpdate += entityEditor_SaveOrUpdate;
                        entityEditor.Columns = classInfo.Members.Count() + 1;
                        editorCell.ColumnSpan = classInfo.Members.Count() + 1;
                        editorCell.Controls.Add(entityEditor);
                        dataRow.Cells.Add(editorCell);
                    }
                    else
                    {
                        foreach (var member in members)
                        {
                            var dataCell = new TableCell();
                            dataCell.Text = dataSourceType.GetProperty(member.Name).GetValue(item, new object[] { }).ToString();
                            dataRow.Cells.Add(dataCell);
                        }
                        var actionCell = new TableCell();
                        actionCell.EnableViewState = true;

                        var btnLnkEdit = new LinkButton();
                        btnLnkEdit.ID = string.Format("editButton_{0}", i);
                        btnLnkEdit.Text = "Edit";
                        btnLnkEdit.Click += btnLnkEdit_Click;
                        btnLnkEdit.EnableViewState = true;
                        actionCell.Controls.Add(btnLnkEdit);

                        var btnLnkDelete = new LinkButton();
                        btnLnkDelete.ID = string.Format("deleteButton_{0}", i);
                        btnLnkDelete.Text = "Delete";
                        btnLnkDelete.Attributes["data"] = ExpressionHelper.ObjectFromMemberName(item, "Id").ToString();
                        btnLnkDelete.Click += btnLnkDelete_Click;
                        btnLnkDelete.EnableViewState = true;
                        actionCell.Controls.Add(btnLnkDelete);

                        dataRow.Cells.Add(actionCell);

                    }

                    if (rebuilding)
                    {
                        table.Rows.AddAt(i + 3, dataRow);
                    }
                    else
                    {
                        table.Rows.Add(dataRow);
                    }
                    i++;
                }
                if (!rebuilding)
                {
                    var paginationRow = new TableRow();
                    paginationRow.EnableViewState = true;
                    var itemCountCell = new TableCell();
                    itemCountCell.EnableViewState = true;
                    itemCountCell.ColumnSpan = members.Count() / 2;
                    var lblItemCount = new Label();
                    lblItemCount.Text = "Item per page:";
                    var txtItemPerPage = new TextBox();
                    itemCountCell.Controls.Add(lblItemCount);
                    itemCountCell.Controls.Add(txtItemPerPage);
                    txtItemPerPage.EnableViewState = true;
                    txtItemPerPage.AutoPostBack = true;
                    lblItemCount.AssociatedControlID = txtItemPerPage.ID;

                    txtItemPerPage.TextChanged += txtItemPerPage_TextChanged;



                    paginationRow.Cells.Add(itemCountCell);

                    var pageIndexCell = new TableCell();
                    pageIndexCell.EnableViewState = true;
                    pageIndexCell.HorizontalAlign = HorizontalAlign.Right;
                    pageIndexCell.ColumnSpan = members.Count() / 2;

                    var lblPageIndexPrefix = new Label();
                    lblPageIndexPrefix.Text = "Page ";

                    lblPageIndexSufix = new Label();
                    var ddlPageIndex = new DropDownList();
                    this.ddlPageIndex = ddlPageIndex;
                    pageIndexCell.Controls.Add(ddlPageIndex);
                    ddlPageIndex.ID = "ddlPageIndex";
                    ddlPageIndex.AutoPostBack = true;
                    ddlPageIndex.EnableViewState = true;
                    ddlPageIndex.DataTextField = "Label";
                    ddlPageIndex.DataValueField = "Value";
                    ddlPageIndex.SelectedIndexChanged += new EventHandler(ddlPageIndex_SelectedIndexChanged);

                    lblPageIndexPrefix.AssociatedControlID = ddlPageIndex.ID;
                    lblPageIndexSufix.AssociatedControlID = ddlPageIndex.ID;

                    pageIndexCell.Controls.Add(lblPageIndexPrefix);
                    paginationRow.Cells.Add(pageIndexCell);
                    pageIndexCell.Controls.Add(lblPageIndexSufix);

                    table.Rows.Add(paginationRow);

					if (createNew) {
						var createNewRow = new TableRow();
						createNewRow.EnableViewState = true;
						var createNewCell = new TableCell();
						createNewCell.EnableViewState = true;
						createNewCell.ColumnSpan = members.Count() + 1;
						var newEntityEditor = new ClassTableEntityEditor();
						newEntityEditor.EnableViewState = true;
						newEntityEditor.Entity = Activator.CreateInstance(dataSourceType);
						newEntityEditor.ClassInfo = classInfo;
						newEntityEditor.ID = "newEntityEditor";
						newEntityEditor.Columns = members.Count() + 1;
						newEntityEditor.SaveOrUpdate += entityEditor_SaveOrUpdate;

						createNewCell.Controls.Add(newEntityEditor);
						createNewRow.Cells.Add(createNewCell);
						table.Rows.Add(createNewRow);
					}
                }
        }

        void buttonNewEntry_Click(object sender, EventArgs e)
        {
            createNew = true;
        }

        void btnLnkDelete_Click(object sender, EventArgs e)
        {
            var f = string.Format("Id = {0}", ((LinkButton)sender).Attributes["data"]);
            Delete(this, new DeleteEventArgs(datasource.Where(f).ToArray().First()));
        }

        void lnkButtonSort_Click(object sender, EventArgs e)
        {
            var column = ((LinkButton)sender).Attributes["data"];
            var order = "DESC";
            var sortInfo = sortList.FirstOrDefault(item => item.Member == column);
            if (sortInfo == null)
            {
                sortList.Insert(0, new SortInfo(column, order));
            } else {
                sortList.Remove(sortInfo);
                if (sortInfo.Order == "ASC") {
                    sortInfo.Order = "DESC";
                } else {
                    sortInfo.Order = "ASC";
                }
                sortList.Insert(0, sortInfo);
            }
        }

		void entityEditor_SaveOrUpdate(object sender, SaveOrUpdateEventArgs e) {
			SaveOrUpdate(this, e);
			editItemId = -1;
		}

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
        }

        void btnLnkEdit_Click(object sender, EventArgs e)
        {
            editItemId = int.Parse(((LinkButton)sender).ID.Substring("editButton_".Length));
        }

        void ddlPageIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            pageIndex = int.Parse(((DropDownList)sender).SelectedValue);
			editItemId = -1;
        }

        void txtItemPerPage_TextChanged(object sender, EventArgs e)
        {
            itemPerPage = int.Parse(((TextBox)sender).Text);
            refreshIndices = true;
			editItemId = -1;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (Page.IsPostBack)
            {
                rebuilding = true;
                CreateChildControls();
            }
            if (refreshIndices)
            {
                var itemCount = filteredDatasource.Count();
                int pageCount = itemCount / itemPerPage;
                if (itemCount % itemPerPage != 0) pageCount++;
                lblPageIndexSufix.Text = string.Format("on {0}", pageCount == 0 ? 1 : pageCount);
                ddlPageIndex.DataSource = Enumerable.Range(1, pageCount == 0 ? 1 : pageCount).Select(x => new { Label = x.ToString(), Value = (x - 1).ToString() }).ToArray();
                ddlPageIndex.SelectedIndex = pageIndex;
                ddlPageIndex.DataBind();
            }

        }

        protected void OnFilterChanged(object sender, EventArgs e)
        {
            var result = new Collection<Tuple<string, string, object>>();
            int i = 0;
            int count = classInfo.Members.Count();
            int position = 0;
            while (i < count)
            {
                position = classInfo.Members[i].Position;
                result.Add(new Tuple<string, string, object>(classInfo.Members[i].Name, ddlFilters[i].SelectedValue, editors[i].GetValue()));
                i++;
            }

            var request = string.Empty;
            var filters = result.Select(tuple => classInfo.CreateFilter(tuple.Item1, tuple.Item2, tuple.Item3 == null ? string.Empty : tuple.Item3.ToString())).Where(f => !string.IsNullOrEmpty(f));
            filter = string.Join(" AND ", filters);
            refreshIndices = true;
			editItemId = -1;
        }

        void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnFilterChanged(sender, e);
			editItemId = -1;
        }

        protected void editor_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            OnFilterChanged(sender, e);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            table.RenderControl(writer);
        }
    }
}
