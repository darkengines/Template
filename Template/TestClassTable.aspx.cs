using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Template.Database;

namespace Template {
	public partial class TestClassTable : System.Web.UI.Page {
		TestClassInfo classInfo = new TestClassInfo();
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			classTable.classInfo = classInfo;
			classTable.DataSource = NHibernateHelper.GetCurrentSession().Query<Test>();
			classTable.SaveOrUpdate +=classTable_SaveOrUpdate;
            classTable.Delete += classTable_Delete;
		}

        void classTable_Delete(object sender, DarkEngines.DeleteEventArgs e)
        {
            NHibernateHelper.GetCurrentSession().Delete(e.Item);
            NHibernateHelper.GetCurrentSession().Flush();
        }

		protected void classTable_SaveOrUpdate(object sender, DarkEngines.SaveOrUpdateEventArgs e) {
			NHibernateHelper.GetCurrentSession().SaveOrUpdate(e.Item);
			NHibernateHelper.GetCurrentSession().Flush();
		}

		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				classTable.DataBind();
			}
		}
	}
}