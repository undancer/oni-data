using System;
using UnityEngine;

[Serializable]
public class AudioSheet
{
	public class SoundInfo : Resource
	{
		public string File;

		public string Anim;

		public string Type;

		public string RequiredDlcId;

		public float MinInterval;

		public string Name0;

		public int Frame0;

		public string Name1;

		public int Frame1;

		public string Name2;

		public int Frame2;

		public string Name3;

		public int Frame3;

		public string Name4;

		public int Frame4;

		public string Name5;

		public int Frame5;

		public string Name6;

		public int Frame6;

		public string Name7;

		public int Frame7;

		public string Name8;

		public int Frame8;

		public string Name9;

		public int Frame9;

		public string Name10;

		public int Frame10;

		public string Name11;

		public int Frame11;
	}

	public TextAsset asset;

	public string defaultType;

	public SoundInfo[] soundInfos;
}
