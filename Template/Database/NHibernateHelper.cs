using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using NHibernate;

namespace Template.Database {
	public sealed class NHibernateHelper {
		private const string currentSessionKey = "nhibernate.current_session";
		private static readonly ISessionFactory sessionFactory;
		private static readonly Configuration configuration;
		static NHibernateHelper() {
			var fluentConfiguration = Fluently.Configure()
				.Mappings(mapper => mapper.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
			configuration = fluentConfiguration.BuildConfiguration();
			sessionFactory = fluentConfiguration.BuildSessionFactory();
		}
		public static ISession GetCurrentSession() {
			var context = HttpContext.Current;
			var currentSession = context.Items[currentSessionKey] as ISession;
			if (currentSession == null) {
				currentSession = sessionFactory.OpenSession();
				context.Items[currentSessionKey] = currentSession;
			}
			return currentSession;
		}
		public static void CloseSession() {
			var context = HttpContext.Current;
			var currentSession = context.Items[currentSessionKey] as ISession;
			if (currentSession != null) {
				currentSession.Close();
				context.Items.Remove(currentSessionKey);
			}
		}
		public static void BuildSchema() {
			var export = new SchemaExport(configuration);
			export.Drop(true, true);
			export.Create(true, true);
		}
	}
}