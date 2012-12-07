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
			Member(new MemberInfo<Test>(test => test.Id, new ClassTableTextBox<int?>(x => {
				int t;
				if (int.TryParse(x, out t)) {
					return t;
				} else {
					return null;
				}
			}), "Id", 0));
			Member(new MemberInfo<Test>(test => test.Name, new ClassTableTextBox<string>(x=>x), "Name", 1));
			Filters<long?>(test => test.Id, IntegerFilters.Filters);
			Filters<string>(test => test.Name, StringFilters.Filters);
			
		}
	}
}