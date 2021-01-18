using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;

namespace ProcGen.Noise
{
	public class Selector : NoiseBase
	{
		public enum SelectType
		{
			_UNSET_,
			Blend,
			Select
		}

		public SelectType selectType
		{
			get;
			set;
		}

		public float lower
		{
			get;
			set;
		}

		public float upper
		{
			get;
			set;
		}

		public float edge
		{
			get;
			set;
		}

		public override Type GetObjectType()
		{
			return typeof(Selector);
		}

		public Selector()
		{
			selectType = SelectType.Blend;
			lower = 0f;
			upper = 1f;
			edge = 0.02f;
		}

		public IModule3D CreateModule()
		{
			if (selectType == SelectType.Blend)
			{
				return new Blend();
			}
			Select select = new Select();
			select.SetBounds(lower, upper);
			select.EdgeFalloff = edge;
			return select;
		}

		public IModule3D CreateModule(IModule3D selectModule, IModule3D leftModule, IModule3D rightModule)
		{
			if (selectType == SelectType.Blend)
			{
				return new Blend(selectModule, rightModule, leftModule);
			}
			return new Select(selectModule, rightModule, leftModule, lower, upper, edge);
		}

		public void SetSouces(IModule3D target, IModule3D controlModule, IModule3D rightModule, IModule3D leftModule)
		{
			if (selectType == SelectType.Blend)
			{
				Blend obj = target as Blend;
				obj.ControlModule = controlModule;
				obj.RightModule = rightModule;
				obj.LeftModule = leftModule;
			}
			Select obj2 = target as Select;
			obj2.ControlModule = controlModule;
			obj2.RightModule = rightModule;
			obj2.LeftModule = leftModule;
		}
	}
}
