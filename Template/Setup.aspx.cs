using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Template.Database;

namespace Template {
	public partial class Setup : System.Web.UI.Page {
		protected void Page_Load(object sender, EventArgs e) {
		}

		protected void btnResetSchema_Click(object sender, EventArgs e) {
			NHibernateHelper.BuildSchema();
		}

		protected void btnGenerateUsers_Click(object sender, EventArgs e) {
			string[] words = null;
			using (var reader = new StreamReader(new FileStream(@"E:\Template\Template\Content\dico.txt", FileMode.Open, FileAccess.Read))) {
				string word = string.Empty;
				int n = 0;
				while ((word = reader.ReadLine()) != null) {
					n++;
				}
				words = new string[n];
				reader.BaseStream.Seek(0, 0);
				n = 0;
				while ((word = reader.ReadLine()) != null) {
					words[n] = word;
					n++;
				}
			}
			var generator = new Random();
			int i, rnd;
			i = 0;
			var user = new Test();
			var session = NHibernateHelper.GetCurrentSession();
			while (i < 300000) {
				rnd = generator.Next(words.Count());
				user.Id = 0;
				user.Name = words[rnd];
				rnd = generator.Next(words.Count());
				NHibernateHelper.GetCurrentSession().SaveOrUpdate(user);
				session.Clear();
				i++;
			}
			NHibernateHelper.GetCurrentSession().Flush();
		}
	}
}