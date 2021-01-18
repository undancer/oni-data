using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization
{
	internal sealed class LazyComponentRegistrationList<TArgument, TComponent> : IEnumerable<Func<TArgument, TComponent>>, IEnumerable
	{
		public sealed class LazyComponentRegistration
		{
			public readonly Type ComponentType;

			public readonly Func<TArgument, TComponent> Factory;

			public LazyComponentRegistration(Type componentType, Func<TArgument, TComponent> factory)
			{
				ComponentType = componentType;
				Factory = factory;
			}
		}

		public sealed class TrackingLazyComponentRegistration
		{
			public readonly Type ComponentType;

			public readonly Func<TComponent, TArgument, TComponent> Factory;

			public TrackingLazyComponentRegistration(Type componentType, Func<TComponent, TArgument, TComponent> factory)
			{
				ComponentType = componentType;
				Factory = factory;
			}
		}

		private class RegistrationLocationSelector : IRegistrationLocationSelectionSyntax<TComponent>
		{
			private readonly LazyComponentRegistrationList<TArgument, TComponent> registrations;

			private readonly LazyComponentRegistration newRegistration;

			public RegistrationLocationSelector(LazyComponentRegistrationList<TArgument, TComponent> registrations, LazyComponentRegistration newRegistration)
			{
				this.registrations = registrations;
				this.newRegistration = newRegistration;
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.InsteadOf<TRegistrationType>()
			{
				if (newRegistration.ComponentType != typeof(TRegistrationType))
				{
					registrations.EnsureNoDuplicateRegistrationType(newRegistration.ComponentType);
				}
				int index = registrations.EnsureRegistrationExists<TRegistrationType>();
				registrations.entries[index] = newRegistration;
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.After<TRegistrationType>()
			{
				registrations.EnsureNoDuplicateRegistrationType(newRegistration.ComponentType);
				int num = registrations.EnsureRegistrationExists<TRegistrationType>();
				registrations.entries.Insert(num + 1, newRegistration);
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.Before<TRegistrationType>()
			{
				registrations.EnsureNoDuplicateRegistrationType(newRegistration.ComponentType);
				int index = registrations.EnsureRegistrationExists<TRegistrationType>();
				registrations.entries.Insert(index, newRegistration);
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.OnBottom()
			{
				registrations.EnsureNoDuplicateRegistrationType(newRegistration.ComponentType);
				registrations.entries.Add(newRegistration);
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.OnTop()
			{
				registrations.EnsureNoDuplicateRegistrationType(newRegistration.ComponentType);
				registrations.entries.Insert(0, newRegistration);
			}
		}

		private class TrackingRegistrationLocationSelector : ITrackingRegistrationLocationSelectionSyntax<TComponent>
		{
			private readonly LazyComponentRegistrationList<TArgument, TComponent> registrations;

			private readonly TrackingLazyComponentRegistration newRegistration;

			public TrackingRegistrationLocationSelector(LazyComponentRegistrationList<TArgument, TComponent> registrations, TrackingLazyComponentRegistration newRegistration)
			{
				this.registrations = registrations;
				this.newRegistration = newRegistration;
			}

			void ITrackingRegistrationLocationSelectionSyntax<TComponent>.InsteadOf<TRegistrationType>()
			{
				if (newRegistration.ComponentType != typeof(TRegistrationType))
				{
					registrations.EnsureNoDuplicateRegistrationType(newRegistration.ComponentType);
				}
				int index = registrations.EnsureRegistrationExists<TRegistrationType>();
				Func<TArgument, TComponent> innerComponentFactory = registrations.entries[index].Factory;
				registrations.entries[index] = new LazyComponentRegistration(newRegistration.ComponentType, (TArgument arg) => newRegistration.Factory(innerComponentFactory(arg), arg));
			}
		}

		private readonly List<LazyComponentRegistration> entries = new List<LazyComponentRegistration>();

		public int Count => entries.Count;

		public IEnumerable<Func<TArgument, TComponent>> InReverseOrder
		{
			get
			{
				int i = entries.Count - 1;
				while (i >= 0)
				{
					yield return entries[i].Factory;
					int num = i - 1;
					i = num;
				}
			}
		}

		public LazyComponentRegistrationList<TArgument, TComponent> Clone()
		{
			LazyComponentRegistrationList<TArgument, TComponent> lazyComponentRegistrationList = new LazyComponentRegistrationList<TArgument, TComponent>();
			foreach (LazyComponentRegistration entry in entries)
			{
				lazyComponentRegistrationList.entries.Add(entry);
			}
			return lazyComponentRegistrationList;
		}

		public void Add(Type componentType, Func<TArgument, TComponent> factory)
		{
			entries.Add(new LazyComponentRegistration(componentType, factory));
		}

		public void Remove(Type componentType)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].ComponentType == componentType)
				{
					entries.RemoveAt(i);
					return;
				}
			}
			throw new KeyNotFoundException($"A component registration of type '{componentType.FullName}' was not found.");
		}

		public IRegistrationLocationSelectionSyntax<TComponent> CreateRegistrationLocationSelector(Type componentType, Func<TArgument, TComponent> factory)
		{
			return new RegistrationLocationSelector(this, new LazyComponentRegistration(componentType, factory));
		}

		public ITrackingRegistrationLocationSelectionSyntax<TComponent> CreateTrackingRegistrationLocationSelector(Type componentType, Func<TComponent, TArgument, TComponent> factory)
		{
			return new TrackingRegistrationLocationSelector(this, new TrackingLazyComponentRegistration(componentType, factory));
		}

		public IEnumerator<Func<TArgument, TComponent>> GetEnumerator()
		{
			return entries.Select((LazyComponentRegistration e) => e.Factory).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private int IndexOfRegistration(Type registrationType)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				if (registrationType == entries[i].ComponentType)
				{
					return i;
				}
			}
			return -1;
		}

		private void EnsureNoDuplicateRegistrationType(Type componentType)
		{
			if (IndexOfRegistration(componentType) != -1)
			{
				throw new InvalidOperationException($"A component of type '{componentType.FullName}' has already been registered.");
			}
		}

		private int EnsureRegistrationExists<TRegistrationType>()
		{
			int num = IndexOfRegistration(typeof(TRegistrationType));
			if (num == -1)
			{
				throw new InvalidOperationException($"A component of type '{typeof(TRegistrationType).FullName}' has not been registered.");
			}
			return num;
		}
	}
}
