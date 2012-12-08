using DarkEngines;
using FluentNHibernate.Data;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Template {
	public class Test: Entity {
		public virtual string Name {
			get;
			set;
		}
	}
	public class TestClassMap : ClassMap<Test> {
		public TestClassMap() {
			Id(test => test.Id);
			Map(test => test.Name);
		}
	}
	public class TestClassInfo : ClassInfo<Test> {
		public TestClassInfo() {
			Member(new MemberInfo<Test>(test => test.Id, new ClassTableInteger64TextboxProvider(), "Id", 0));
			Member(new MemberInfo<Test>(test => test.Name, new ClassTableStringTextboxProvider() , "Name", 1));
			Filters<long?>(test => test.Id, IntegerFilters.Filters);
			Filters<string>(test => test.Name, StringFilters.Filters);
			
		}
	}
}