using System;

namespace YamlDotNet.Serialization.ObjectFactories
{
	public sealed class LambdaObjectFactory : IObjectFactory
	{
		private readonly Func<Type, object> _factory;

		public LambdaObjectFactory(Func<Type, object> factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			_factory = factory;
		}

		public object Create(Type type)
		{
			return _factory(type);
		}
	}
}
