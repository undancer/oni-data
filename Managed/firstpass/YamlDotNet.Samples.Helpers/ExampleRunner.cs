using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace YamlDotNet.Samples.Helpers
{
	public class ExampleRunner : MonoBehaviour
	{
		private class StringTestOutputHelper : ITestOutputHelper
		{
			private StringBuilder output = new StringBuilder();

			public void WriteLine()
			{
				output.AppendLine();
			}

			public void WriteLine(string value)
			{
				output.AppendLine(value);
			}

			public void WriteLine(string format, params object[] args)
			{
				output.AppendFormat(format, args);
				output.AppendLine();
			}

			public override string ToString()
			{
				return output.ToString();
			}

			public void Clear()
			{
				output = new StringBuilder();
			}
		}

		private StringTestOutputHelper helper = new StringTestOutputHelper();

		public string[] disabledTests = new string[0];

		public static string[] GetAllTestNames()
		{
			List<string> list = new List<string>();
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (!(type.Namespace == "YamlDotNet.Samples") || !type.IsClass)
				{
					continue;
				}
				bool flag = false;
				MethodInfo[] methods = type.GetMethods();
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.Name == "Main")
					{
						SampleAttribute sampleAttribute = (SampleAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(SampleAttribute));
						if (sampleAttribute != null)
						{
							list.Add(type.Name);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return list.ToArray();
		}

		public static string[] GetAllTestTitles()
		{
			List<string> list = new List<string>();
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (!(type.Namespace == "YamlDotNet.Samples") || !type.IsClass)
				{
					continue;
				}
				bool flag = false;
				MethodInfo[] methods = type.GetMethods();
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.Name == "Main")
					{
						SampleAttribute sampleAttribute = (SampleAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(SampleAttribute));
						if (sampleAttribute != null)
						{
							list.Add(sampleAttribute.Title);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return list.ToArray();
		}

		private void Start()
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (!(type.Namespace == "YamlDotNet.Samples") || !type.IsClass || Array.IndexOf(disabledTests, type.Name) != -1)
				{
					continue;
				}
				bool flag = false;
				MethodInfo[] methods = type.GetMethods();
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.Name == "Main")
					{
						SampleAttribute sampleAttribute = (SampleAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(SampleAttribute));
						if (sampleAttribute != null)
						{
							helper.WriteLine("{0} - {1}", sampleAttribute.Title, sampleAttribute.Description);
							object obj = type.GetConstructor(new Type[1]
							{
								typeof(StringTestOutputHelper)
							}).Invoke(new object[1]
							{
								helper
							});
							methodInfo.Invoke(obj, new object[0]);
							Debug.Log(helper.ToString());
							helper.Clear();
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
	}
}
