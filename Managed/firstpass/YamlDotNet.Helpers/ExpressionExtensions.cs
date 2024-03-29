using System;
using System.Linq.Expressions;
using System.Reflection;

namespace YamlDotNet.Helpers
{
	public static class ExpressionExtensions
	{
		public static PropertyInfo AsProperty(this LambdaExpression propertyAccessor)
		{
			PropertyInfo propertyInfo = TryGetMemberExpression<PropertyInfo>(propertyAccessor);
			if (propertyInfo == null)
			{
				throw new ArgumentException("Expected a lambda expression in the form: x => x.SomeProperty", "propertyAccessor");
			}
			return propertyInfo;
		}

		private static TMemberInfo TryGetMemberExpression<TMemberInfo>(LambdaExpression lambdaExpression) where TMemberInfo : MemberInfo
		{
			if (lambdaExpression.Parameters.Count != 1)
			{
				return null;
			}
			Expression expression = lambdaExpression.Body;
			if (expression is UnaryExpression unaryExpression)
			{
				if (unaryExpression.NodeType != ExpressionType.Convert)
				{
					return null;
				}
				expression = unaryExpression.Operand;
			}
			if (!(expression is MemberExpression memberExpression))
			{
				return null;
			}
			if (memberExpression.Expression != lambdaExpression.Parameters[0])
			{
				return null;
			}
			return memberExpression.Member as TMemberInfo;
		}
	}
}
