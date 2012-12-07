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
		}

		protected void Page_Load(object sender, EventArgs e) {
			classTable.DataBind();
			if (!IsPostBack) {
				classTable.DataBind();
			}
		}
	}
}