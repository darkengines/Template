using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DarkEngines {
	public static class ExpressionHelper {
		public static string MemberToString<T>(Expression<Func<T, object>> member) {
			return MemberToString<T, object>(member);
		}
		public static string MemberToString<T, MT>(Expression<Func<T, MT>> member) {
			var memberExpression = member.Body as MemberExpression;
			if (memberExpression == null) {
				var unaryExpression = member.Body as UnaryExpression;
				if (unaryExpression != null) {
					memberExpression = unaryExpression.Operand as MemberExpression;
				}
			}
			if (memberExpression == null) {
				throw new Exception("Unknown expression type");
			}
			var name = memberExpression.Member.Name;
			return name;
		}
	}
}
