using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices
{
	public static class Helper
	{
		private class Allocation
		{
			public object Data { get; private set; }

			public Allocation(object data)
			{
				Data = data;
			}
		}

		private class ArrayAllocation : Allocation
		{
			public bool IsElementAllocated { get; private set; }

			public ArrayAllocation(object data, bool isElementAllocated)
				: base(data)
			{
				IsElementAllocated = isElementAllocated;
			}
		}

		private class DelegateHolder
		{
			public Delegate Public { get; private set; }

			public Delegate Private { get; private set; }

			public Delegate[] Additional { get; private set; }

			public ulong? NotificationId { get; set; }

			public DelegateHolder(Delegate publicDelegate, Delegate privateDelegate, params Delegate[] additionalDelegates)
			{
				Public = publicDelegate;
				Private = privateDelegate;
				Additional = additionalDelegates;
			}
		}

		private static Dictionary<IntPtr, Allocation> s_Allocations = new Dictionary<IntPtr, Allocation>();

		private static Dictionary<IntPtr, DelegateHolder> s_Callbacks = new Dictionary<IntPtr, DelegateHolder>();

		public static int GetAllocationCount()
		{
			return s_Allocations.Count;
		}

		public static bool IsOperationComplete(Result result)
		{
			int source = EOS_EResult_IsOperationComplete(result);
			bool target = false;
			TryMarshalGet(source, out target);
			return target;
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern int EOS_EResult_IsOperationComplete(Result result);

		public static string ToHexString(byte[] byteArray)
		{
			return string.Join("", byteArray.Select((byte b) => $"{b:X2}").ToArray());
		}

		internal static bool TryMarshalGet<T>(T source, out T target)
		{
			target = source;
			return true;
		}

		internal static bool TryMarshalGet(IntPtr source, out IntPtr target)
		{
			target = source;
			return true;
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T target) where T : Handle
		{
			return TryConvert<T>(source, out target);
		}

		internal static bool TryMarshalGet(int source, out bool target)
		{
			return TryConvert(source, out target);
		}

		internal static bool TryMarshalGet(long source, out DateTimeOffset? target)
		{
			return TryConvert(source, out target);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T[] target, int arrayLength, bool isElementAllocated)
		{
			return TryFetch<T>(source, out target, arrayLength, isElementAllocated);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T[] target, uint arrayLength, bool isElementAllocated)
		{
			return TryFetch<T>(source, out target, (int)arrayLength, isElementAllocated);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T[] target, int arrayLength)
		{
			return TryMarshalGet<T>(source, out target, arrayLength, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T[] target, uint arrayLength)
		{
			return TryMarshalGet<T>(source, out target, arrayLength, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalGet<T>(IntPtr source, out T? target) where T : struct
		{
			return TryFetch(source, out target);
		}

		internal static bool TryMarshalGet(byte[] source, out string target)
		{
			return TryConvert(source, out target);
		}

		internal static bool TryMarshalGet(IntPtr source, out object target)
		{
			target = null;
			if (TryFetch(source, out BoxedData target2))
			{
				target = target2.Data;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet(IntPtr source, out string target)
		{
			return TryFetch(source, out target);
		}

		internal static bool TryMarshalGet<T, TEnum>(T source, out T target, TEnum currentEnum, TEnum comparisonEnum)
		{
			target = GetDefault<T>();
			if ((int)(object)currentEnum == (int)(object)comparisonEnum)
			{
				target = source;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet<T, TEnum>(T source, out T? target, TEnum currentEnum, TEnum comparisonEnum) where T : struct
		{
			target = GetDefault<T?>();
			if ((int)(object)currentEnum == (int)(object)comparisonEnum)
			{
				target = source;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet<T, TEnum>(IntPtr source, out T target, TEnum currentEnum, TEnum comparisonEnum) where T : Handle
		{
			target = GetDefault<T>();
			if ((int)(object)currentEnum == (int)(object)comparisonEnum)
			{
				return TryMarshalGet(source, out target);
			}
			return false;
		}

		internal static bool TryMarshalGet<TEnum>(IntPtr source, out string target, TEnum currentEnum, TEnum comparisonEnum)
		{
			target = GetDefault<string>();
			if ((int)(object)currentEnum == (int)(object)comparisonEnum)
			{
				return TryMarshalGet(source, out target);
			}
			return false;
		}

		internal static bool TryMarshalGet<TEnum>(int source, out bool? target, TEnum currentEnum, TEnum comparisonEnum)
		{
			target = GetDefault<bool?>();
			if ((int)(object)currentEnum == (int)(object)comparisonEnum && TryConvert(source, out var target2))
			{
				target = target2;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet<TInternal, TPublic>(IntPtr source, out TPublic target) where TInternal : struct where TPublic : class, new()
		{
			target = null;
			if (TryFetch(source, out TInternal target2))
			{
				target = CopyProperties<TPublic>(target2);
				return true;
			}
			return false;
		}

		internal static bool TryMarshalGet<TCallbackInfoInternal, TCallbackInfo>(IntPtr callbackInfoAddress, out TCallbackInfo callbackInfo, out IntPtr clientDataAddress) where TCallbackInfoInternal : struct, ICallbackInfo where TCallbackInfo : class, new()
		{
			callbackInfo = null;
			clientDataAddress = IntPtr.Zero;
			if (TryFetch(callbackInfoAddress, out TCallbackInfoInternal target))
			{
				callbackInfo = CopyProperties<TCallbackInfo>(target);
				clientDataAddress = target.ClientDataAddress;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalSet<T>(ref T target, T source)
		{
			target = source;
			return true;
		}

		internal static bool TryMarshalSet(ref IntPtr target, Handle source)
		{
			return TryConvert(source, out target);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T? source) where T : struct
		{
			return TryAllocate(ref target, source);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, bool isElementAllocated)
		{
			return TryAllocate(ref target, source, isElementAllocated);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source)
		{
			return TryMarshalSet(ref target, source, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, out int arrayLength, bool isElementAllocated)
		{
			arrayLength = 0;
			if (TryMarshalSet(ref target, source, isElementAllocated))
			{
				arrayLength = source.Length;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, out uint arrayLength, bool isElementAllocated)
		{
			arrayLength = 0u;
			int arrayLength2 = 0;
			if (TryMarshalSet(ref target, source, out arrayLength2, isElementAllocated))
			{
				arrayLength = (uint)arrayLength2;
				return true;
			}
			return false;
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, out int arrayLength)
		{
			return TryMarshalSet(ref target, source, out arrayLength, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalSet<T>(ref IntPtr target, T[] source, out uint arrayLength)
		{
			return TryMarshalSet(ref target, source, out arrayLength, !typeof(T).IsValueType);
		}

		internal static bool TryMarshalSet(ref long target, DateTimeOffset? source)
		{
			return TryConvert(source, out target);
		}

		internal static bool TryMarshalSet(ref int target, bool source)
		{
			return TryConvert(source, out target);
		}

		internal static bool TryMarshalSet(ref byte[] target, string source)
		{
			return TryConvert(source, out target);
		}

		internal static bool TryMarshalSet(ref byte[] target, string source, int length)
		{
			return TryConvert(source, out target, length);
		}

		internal static bool TryMarshalSet(ref IntPtr target, string source)
		{
			return TryAllocate(ref target, source);
		}

		internal static bool TryMarshalSet<T, TEnum>(ref T target, T source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable)
		{
			if (source != null)
			{
				TryMarshalDispose(ref disposable);
				if (TryMarshalSet(ref target, source))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return false;
		}

		internal static bool TryMarshalSet<T, TEnum>(ref T target, T? source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable) where T : struct
		{
			if (source.HasValue)
			{
				TryMarshalDispose(ref disposable);
				if (TryMarshalSet(ref target, source.Value))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return true;
		}

		internal static bool TryMarshalSet<T, TEnum>(ref IntPtr target, T source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable) where T : Handle
		{
			if ((Handle)source != (Handle)null)
			{
				TryMarshalDispose(ref disposable);
				if (TryMarshalSet(ref target, source))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return true;
		}

		internal static bool TryMarshalSet<TEnum>(ref IntPtr target, string source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable)
		{
			if (source != null)
			{
				TryMarshalDispose(ref disposable);
				if (TryMarshalSet(ref target, source))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return true;
		}

		internal static bool TryMarshalSet<TEnum>(ref int target, bool? source, ref TEnum currentEnum, TEnum comparisonEnum, object disposable)
		{
			if (source.HasValue)
			{
				TryMarshalDispose(ref disposable);
				if (TryMarshalSet(ref target, source.Value))
				{
					currentEnum = comparisonEnum;
					return true;
				}
			}
			return true;
		}

		internal static bool TryMarshalDispose(ref object value)
		{
			if (value is IDisposable disposable)
			{
				disposable.Dispose();
				return true;
			}
			return false;
		}

		internal static bool TryMarshalDispose<T>(ref T value) where T : IDisposable
		{
			value.Dispose();
			return true;
		}

		internal static bool TryMarshalDispose(ref IntPtr value)
		{
			return TryRelease(ref value);
		}

		internal static bool TryMarshalDispose<TEnum>(ref IntPtr member, TEnum currentEnum, TEnum comparisonEnum)
		{
			if ((int)(object)currentEnum == (int)(object)comparisonEnum)
			{
				return TryRelease(ref member);
			}
			return false;
		}

		internal static T GetDefault<T>()
		{
			return default(T);
		}

		internal static T CopyProperties<T>(object value) where T : new()
		{
			object obj = new T();
			if (obj is IInitializable initializable)
			{
				initializable.Initialize();
			}
			CopyProperties(value, obj);
			return (T)obj;
		}

		internal static void AddCallback(ref IntPtr clientDataAddress, object clientData, Delegate publicDelegate, Delegate privateDelegate, params Delegate[] additionalDelegates)
		{
			TryAllocate(ref clientDataAddress, new BoxedData(clientData));
			s_Callbacks.Add(clientDataAddress, new DelegateHolder(publicDelegate, privateDelegate, additionalDelegates));
		}

		internal static bool TryAssignNotificationIdToCallback(IntPtr clientDataAddress, ulong notificationId)
		{
			if (notificationId != 0L)
			{
				DelegateHolder value = null;
				if (s_Callbacks.TryGetValue(clientDataAddress, out value))
				{
					value.NotificationId = notificationId;
					return true;
				}
			}
			else
			{
				s_Callbacks.Remove(clientDataAddress);
				TryRelease(ref clientDataAddress);
			}
			return false;
		}

		internal static bool TryRemoveCallbackByNotificationId(ulong notificationId)
		{
			IEnumerable<KeyValuePair<IntPtr, DelegateHolder>> source = s_Callbacks.Where((KeyValuePair<IntPtr, DelegateHolder> pair) => pair.Value.NotificationId.HasValue && pair.Value.NotificationId == notificationId);
			if (source.Any())
			{
				IntPtr target = source.First().Key;
				s_Callbacks.Remove(target);
				TryRelease(ref target);
				return true;
			}
			return false;
		}

		internal static bool TryGetAndRemoveCallback<TCallback, TCallbackInfoInternal, TCallbackInfo>(IntPtr callbackInfoAddress, out TCallback callback, out TCallbackInfo callbackInfo) where TCallback : class where TCallbackInfoInternal : struct, ICallbackInfo where TCallbackInfo : class, new()
		{
			callback = null;
			callbackInfo = null;
			IntPtr clientDataAddress = IntPtr.Zero;
			if (TryMarshalGet<TCallbackInfoInternal, TCallbackInfo>(callbackInfoAddress, out callbackInfo, out clientDataAddress) && TryGetAndRemoveCallback<TCallback>(clientDataAddress, callbackInfo, out callback))
			{
				return true;
			}
			return false;
		}

		internal static bool TryGetAdditionalCallback<TDelegate, TCallbackInfoInternal, TCallbackInfo>(IntPtr callbackInfoAddress, out TDelegate callback, out TCallbackInfo callbackInfo) where TDelegate : class where TCallbackInfoInternal : struct, ICallbackInfo where TCallbackInfo : class, new()
		{
			callback = null;
			callbackInfo = null;
			IntPtr clientDataAddress = IntPtr.Zero;
			if (TryMarshalGet<TCallbackInfoInternal, TCallbackInfo>(callbackInfoAddress, out callbackInfo, out clientDataAddress) && TryGetAdditionalCallback<TDelegate>(clientDataAddress, out callback))
			{
				return true;
			}
			return false;
		}

		private static bool TryAllocate<T>(ref IntPtr target, T source)
		{
			TryRelease(ref target);
			if (target != IntPtr.Zero)
			{
				throw new ExternalAllocationException(target, source.GetType());
			}
			if (source == null)
			{
				return false;
			}
			target = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
			Marshal.StructureToPtr(source, target, fDeleteOld: false);
			s_Allocations.Add(target, new Allocation(source));
			return true;
		}

		private static bool TryAllocate<T>(ref IntPtr target, T? source) where T : struct
		{
			TryRelease(ref target);
			if (target != IntPtr.Zero)
			{
				throw new ExternalAllocationException(target, source.GetType());
			}
			if (!source.HasValue)
			{
				return false;
			}
			return TryAllocate(ref target, source.Value);
		}

		private static bool TryAllocate(ref IntPtr target, string source)
		{
			TryRelease(ref target);
			if (target != IntPtr.Zero)
			{
				throw new ExternalAllocationException(target, source.GetType());
			}
			if (source == null)
			{
				return false;
			}
			if (TryConvert(source, out var target2))
			{
				return TryAllocate(ref target, target2, isElementAllocated: false);
			}
			return false;
		}

		private static bool TryAllocate<T>(ref IntPtr target, T[] source, bool isElementAllocated)
		{
			TryRelease(ref target);
			if (target != IntPtr.Zero)
			{
				throw new ExternalAllocationException(target, source.GetType());
			}
			if (source == null)
			{
				return false;
			}
			int num = 0;
			num = ((!isElementAllocated) ? Marshal.SizeOf(typeof(T)) : Marshal.SizeOf(typeof(IntPtr)));
			target = Marshal.AllocHGlobal(source.Length * num);
			s_Allocations.Add(target, new ArrayAllocation(source, isElementAllocated));
			for (int i = 0; i < source.Length; i++)
			{
				T val = (T)source.GetValue(i);
				if (isElementAllocated)
				{
					IntPtr target2 = IntPtr.Zero;
					if (typeof(T) == typeof(string))
					{
						TryAllocate(ref target2, (string)(object)val);
					}
					else if (typeof(T).BaseType == typeof(Handle))
					{
						TryConvert((Handle)(object)val, out target2);
					}
					else
					{
						TryAllocate(ref target2, val);
					}
					Marshal.StructureToPtr(ptr: new IntPtr(target.ToInt64() + i * num), structure: target2, fDeleteOld: false);
				}
				else
				{
					IntPtr ptr2 = new IntPtr(target.ToInt64() + i * num);
					Marshal.StructureToPtr(val, ptr2, fDeleteOld: false);
				}
			}
			return true;
		}

		private static bool TryRelease(ref IntPtr target)
		{
			if (target == IntPtr.Zero)
			{
				return false;
			}
			Allocation value = null;
			if (!s_Allocations.TryGetValue(target, out value))
			{
				return false;
			}
			if (value is ArrayAllocation)
			{
				ArrayAllocation arrayAllocation = value as ArrayAllocation;
				int num = 0;
				num = ((!arrayAllocation.IsElementAllocated) ? Marshal.SizeOf(arrayAllocation.Data.GetType().GetElementType()) : Marshal.SizeOf(typeof(IntPtr)));
				Array array = arrayAllocation.Data as Array;
				for (int i = 0; i < array.Length; i++)
				{
					if (arrayAllocation.IsElementAllocated)
					{
						IntPtr ptr = new IntPtr(target.ToInt64() + i * num);
						ptr = Marshal.ReadIntPtr(ptr);
						TryRelease(ref ptr);
						continue;
					}
					object value2 = array.GetValue(i);
					if (value2 is IDisposable && value2 is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
			}
			if (value.Data is IDisposable && value.Data is IDisposable disposable2)
			{
				disposable2.Dispose();
			}
			Marshal.FreeHGlobal(target);
			s_Allocations.Remove(target);
			target = IntPtr.Zero;
			return true;
		}

		private static bool TryFetch<T>(IntPtr source, out T target)
		{
			target = GetDefault<T>();
			if (source == IntPtr.Zero)
			{
				return false;
			}
			if (s_Allocations.ContainsKey(source))
			{
				Allocation allocation = s_Allocations[source];
				if (allocation.Data.GetType() == typeof(T))
				{
					target = (T)allocation.Data;
					return true;
				}
				throw new TypeAllocationException(source, allocation.Data.GetType(), typeof(T));
			}
			target = (T)Marshal.PtrToStructure(source, typeof(T));
			return true;
		}

		private static bool TryFetch<T>(IntPtr source, out T? target) where T : struct
		{
			target = GetDefault<T?>();
			if (source == IntPtr.Zero)
			{
				return false;
			}
			if (s_Allocations.ContainsKey(source))
			{
				Allocation allocation = s_Allocations[source];
				if (allocation.Data.GetType() == typeof(T))
				{
					target = (T?)allocation.Data;
					return true;
				}
				throw new TypeAllocationException(source, allocation.Data.GetType(), typeof(T));
			}
			target = (T?)Marshal.PtrToStructure(source, typeof(T));
			return true;
		}

		private static bool TryFetch<T>(IntPtr source, out T[] target, int arrayLength, bool isElementAllocated)
		{
			target = null;
			if (source == IntPtr.Zero)
			{
				return false;
			}
			if (s_Allocations.ContainsKey(source))
			{
				Allocation allocation = s_Allocations[source];
				if (allocation.Data.GetType() == typeof(T[]))
				{
					Array array = (Array)allocation.Data;
					if (array.Length == arrayLength)
					{
						target = array as T[];
						return true;
					}
					throw new ArrayAllocationException(source, array.Length, arrayLength);
				}
				throw new TypeAllocationException(source, allocation.Data.GetType(), typeof(T[]));
			}
			int num = 0;
			num = ((!isElementAllocated) ? Marshal.SizeOf(typeof(T)) : Marshal.SizeOf(typeof(IntPtr)));
			List<T> list = new List<T>();
			for (int i = 0; i < arrayLength; i++)
			{
				IntPtr intPtr = new IntPtr(source.ToInt64() + i * num);
				if (isElementAllocated)
				{
					intPtr = Marshal.ReadIntPtr(intPtr);
				}
				TryFetch(intPtr, out T target2);
				list.Add(target2);
			}
			target = list.ToArray();
			return true;
		}

		private static bool TryFetch(IntPtr source, out string target)
		{
			target = null;
			if (source == IntPtr.Zero)
			{
				return false;
			}
			int i;
			for (i = 0; Marshal.ReadByte(source, i) != 0; i++)
			{
			}
			byte[] array = new byte[i];
			Marshal.Copy(source, array, 0, i);
			target = Encoding.UTF8.GetString(array);
			return true;
		}

		private static bool TryConvert<THandle>(IntPtr source, out THandle target) where THandle : Handle
		{
			target = null;
			if (source != IntPtr.Zero)
			{
				target = Activator.CreateInstance(typeof(THandle), source) as THandle;
			}
			return true;
		}

		private static bool TryConvert(Handle source, out IntPtr target)
		{
			target = IntPtr.Zero;
			if (source != null)
			{
				target = source.InnerHandle;
			}
			return true;
		}

		private static bool TryConvert(byte[] source, out string target)
		{
			target = null;
			if (source == null)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < source.Length && source[i] != 0; i++)
			{
				num++;
			}
			target = Encoding.UTF8.GetString(source.Take(num).ToArray());
			return true;
		}

		private static bool TryConvert(string source, out byte[] target, int length)
		{
			if (source == null)
			{
				source = "";
			}
			target = Encoding.UTF8.GetBytes(new string(source.Take(length).ToArray()).PadRight(length, '\0'));
			return true;
		}

		private static bool TryConvert(string source, out byte[] target)
		{
			return TryConvert(source, out target, source.Length + 1);
		}

		private static bool TryConvert(int source, out bool target)
		{
			target = source != 0;
			return true;
		}

		private static bool TryConvert(bool source, out int target)
		{
			target = (source ? 1 : 0);
			return true;
		}

		private static bool TryConvert(DateTimeOffset? source, out long target)
		{
			target = -1L;
			if (source.HasValue)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				long num = (target = (source.Value.UtcDateTime - dateTime).Ticks / 10000000);
			}
			return true;
		}

		private static bool TryConvert(long source, out DateTimeOffset? target)
		{
			target = null;
			if (source >= 0)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				long num = source * 10000000;
				target = new DateTimeOffset(dateTime.Ticks + num, TimeSpan.Zero);
			}
			return true;
		}

		private static void CopyProperties(object source, object target)
		{
			if (source == null || target == null)
			{
				return;
			}
			PropertyInfo[] properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			PropertyInfo[] properties2 = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo sourceProperty in array)
			{
				PropertyInfo propertyInfo = properties2.SingleOrDefault((PropertyInfo property) => property.Name == sourceProperty.Name);
				if (propertyInfo == null || propertyInfo.GetSetMethod(nonPublic: false) == null)
				{
					continue;
				}
				if (sourceProperty.PropertyType == propertyInfo.PropertyType)
				{
					propertyInfo.SetValue(target, sourceProperty.GetValue(source, null), null);
					continue;
				}
				if (propertyInfo.PropertyType.IsArray)
				{
					if (sourceProperty.GetValue(source, null) is Array array2)
					{
						Array array3 = Array.CreateInstance(propertyInfo.PropertyType.GetElementType(), array2.Length);
						for (int j = 0; j < array2.Length; j++)
						{
							object value = array2.GetValue(j);
							object obj = Activator.CreateInstance(propertyInfo.PropertyType.GetElementType());
							CopyProperties(value, obj);
							array3.SetValue(obj, j);
						}
						propertyInfo.SetValue(target, array3, null);
					}
					else
					{
						propertyInfo.SetValue(target, null, null);
					}
					continue;
				}
				object obj2 = null;
				Type type = propertyInfo.PropertyType;
				Type underlyingType = Nullable.GetUnderlyingType(type);
				if (underlyingType != null)
				{
					type = underlyingType;
				}
				else
				{
					obj2 = Activator.CreateInstance(type);
				}
				object value2 = sourceProperty.GetValue(source, null);
				if (value2 != null)
				{
					obj2 = Activator.CreateInstance(type);
					CopyProperties(value2, obj2);
				}
				propertyInfo.SetValue(target, obj2, null);
			}
		}

		private static bool CanRemoveCallback(IntPtr clientDataAddress, object callbackInfo)
		{
			DelegateHolder value = null;
			if (s_Callbacks.TryGetValue(clientDataAddress, out value) && value.NotificationId.HasValue)
			{
				return false;
			}
			PropertyInfo propertyInfo = (from property in callbackInfo.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
				where property.PropertyType == typeof(Result)
				select property).FirstOrDefault();
			if (propertyInfo != null)
			{
				return IsOperationComplete((Result)propertyInfo.GetValue(callbackInfo, null));
			}
			return true;
		}

		private static bool TryGetAndRemoveCallback<TCallback>(IntPtr clientDataAddress, object callbackInfo, out TCallback callback) where TCallback : class
		{
			callback = null;
			if (clientDataAddress != IntPtr.Zero && s_Callbacks.ContainsKey(clientDataAddress))
			{
				callback = s_Callbacks[clientDataAddress].Public as TCallback;
				if (CanRemoveCallback(clientDataAddress, callbackInfo))
				{
					s_Callbacks.Remove(clientDataAddress);
					TryRelease(ref clientDataAddress);
				}
				return true;
			}
			return false;
		}

		private static bool TryGetAdditionalCallback<TCallback>(IntPtr clientDataAddress, out TCallback additionalCallback) where TCallback : class
		{
			additionalCallback = null;
			if (clientDataAddress != IntPtr.Zero && s_Callbacks.ContainsKey(clientDataAddress))
			{
				additionalCallback = s_Callbacks[clientDataAddress].Additional.FirstOrDefault((Delegate delegat) => delegat.GetType() == typeof(TCallback)) as TCallback;
				if (additionalCallback != null)
				{
					return true;
				}
			}
			return false;
		}
	}
}
