using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace DarkEngines {
	public abstract class ClassInfo {
		public Collection<MemberInfo> Members = new Collection<MemberInfo>();
		public string CreateFilter(string memberName, string filterName, string value) {
			var member = Members.First(m => m.Name == memberName);
			var filter = member.Filters.First(f => f.Name == filterName);
			return filter.FilterGenerator(memberName, value);
		}
	}
	public abstract class ClassInfo<T>: ClassInfo {
		public void Member(MemberInfo<T> memberInfo) {
			Members.Add(memberInfo);
		}
		public void Member(Expression<Func<T, object>> member, ClassTableEditor editor, string label, int position) {
			Members.Add(new MemberInfo<T>(member, editor, label, position));
		}
		public void Filter<MT>(Expression<Func<T, MT>> member, Filter filter) {
			var memberName = ExpressionHelper.MemberToString<T, MT>(member);
			var memberInfo = Members.First(m => m.Name == memberName);
			memberInfo.Filters.Add(filter);
		}
		public void Filters<MT>(Expression<Func<T, MT>> member, IEnumerable<Filter> filters) {
			var memberName = ExpressionHelper.MemberToString<T, MT>(member);
			var memberInfo = Members.First(m => m.Name == memberName);
			foreach (var filter in filters) {
				memberInfo.Filters.Add(filter);
			}
		}
	}
}