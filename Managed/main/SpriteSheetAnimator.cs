using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimator
{
	public struct AnimInfo
	{
		public int frame;

		public float elapsedTime;

		public Vector3 pos;

		public Quaternion rotation;

		public Vector2 size;

		public Color32 colour;
	}

	private SpriteSheet sheet;

	private Mesh mesh;

	private MaterialPropertyBlock materialProperties;

	private List<AnimInfo> anims = new List<AnimInfo>();

	private List<AnimInfo> rotatedAnims = new List<AnimInfo>();

	public SpriteSheetAnimator(SpriteSheet sheet)
	{
		this.sheet = sheet;
		mesh = new Mesh();
		mesh.name = "SpriteSheetAnimator";
		mesh.MarkDynamic();
		materialProperties = new MaterialPropertyBlock();
		materialProperties.SetTexture("_MainTex", sheet.texture);
	}

	public void Play(Vector3 pos, Quaternion rotation, Vector2 size, Color colour)
	{
		AnimInfo item;
		if (rotation == Quaternion.identity)
		{
			List<AnimInfo> list = anims;
			item = new AnimInfo
			{
				elapsedTime = 0f,
				pos = pos,
				rotation = rotation,
				size = size,
				colour = colour
			};
			list.Add(item);
		}
		else
		{
			List<AnimInfo> list2 = rotatedAnims;
			item = new AnimInfo
			{
				elapsedTime = 0f,
				pos = pos,
				rotation = rotation,
				size = size,
				colour = colour
			};
			list2.Add(item);
		}
	}

	private void GetUVs(int frame, out Vector2 uv_bl, out Vector2 uv_br, out Vector2 uv_tl, out Vector2 uv_tr)
	{
		int num = frame / sheet.numXFrames;
		int num2 = frame % sheet.numXFrames;
		float x = (float)num2 * sheet.uvFrameSize.x;
		float x2 = (float)(num2 + 1) * sheet.uvFrameSize.x;
		float y = 1f - (float)(num + 1) * sheet.uvFrameSize.y;
		float y2 = 1f - (float)num * sheet.uvFrameSize.y;
		uv_bl = new Vector2(x, y);
		uv_br = new Vector2(x2, y);
		uv_tl = new Vector2(x, y2);
		uv_tr = new Vector2(x2, y2);
	}

	public int GetFrameFromElapsedTime(float elapsed_time)
	{
		return Mathf.Min(sheet.numFrames, (int)(elapsed_time / (71f / (678f * (float)Math.PI))));
	}

	public int GetFrameFromElapsedTimeLooping(float elapsed_time)
	{
		int num = (int)(elapsed_time / (71f / (678f * (float)Math.PI)));
		if (num > sheet.numFrames)
		{
			num %= sheet.numFrames;
		}
		return num;
	}

	public void UpdateAnims(float dt)
	{
		UpdateAnims(dt, anims);
		UpdateAnims(dt, rotatedAnims);
	}

	private void UpdateAnims(float dt, IList<AnimInfo> anims)
	{
		int num = anims.Count;
		int num2 = 0;
		while (num2 < num)
		{
			AnimInfo value = anims[num2];
			value.elapsedTime += dt;
			value.frame = Mathf.Min(sheet.numFrames, (int)(value.elapsedTime / (71f / (678f * (float)Math.PI))));
			if (value.frame >= sheet.numFrames)
			{
				num--;
				anims[num2] = anims[num];
				anims.RemoveAt(num);
			}
			else
			{
				anims[num2] = value;
				num2++;
			}
		}
	}

	public void Render(List<AnimInfo> anim_infos, bool apply_rotation)
	{
		ListPool<Vector3, SpriteSheetAnimManager>.PooledList pooledList = ListPool<Vector3, SpriteSheetAnimManager>.Allocate();
		ListPool<Vector2, SpriteSheetAnimManager>.PooledList pooledList2 = ListPool<Vector2, SpriteSheetAnimManager>.Allocate();
		ListPool<Color32, SpriteSheetAnimManager>.PooledList pooledList3 = ListPool<Color32, SpriteSheetAnimManager>.Allocate();
		ListPool<int, SpriteSheetAnimManager>.PooledList pooledList4 = ListPool<int, SpriteSheetAnimManager>.Allocate();
		mesh.Clear();
		if (apply_rotation)
		{
			int count = anim_infos.Count;
			for (int i = 0; i < count; i++)
			{
				AnimInfo animInfo = anim_infos[i];
				Vector2 vector = animInfo.size * 0.5f;
				Vector3 b = animInfo.rotation * -vector;
				Vector3 b2 = animInfo.rotation * new Vector2(vector.x, 0f - vector.y);
				Vector3 b3 = animInfo.rotation * new Vector2(0f - vector.x, vector.y);
				Vector3 b4 = animInfo.rotation * vector;
				pooledList.Add(animInfo.pos + b);
				pooledList.Add(animInfo.pos + b2);
				pooledList.Add(animInfo.pos + b4);
				pooledList.Add(animInfo.pos + b3);
				GetUVs(animInfo.frame, out var uv_bl, out var uv_br, out var uv_tl, out var uv_tr);
				pooledList2.Add(uv_bl);
				pooledList2.Add(uv_br);
				pooledList2.Add(uv_tr);
				pooledList2.Add(uv_tl);
				pooledList3.Add(animInfo.colour);
				pooledList3.Add(animInfo.colour);
				pooledList3.Add(animInfo.colour);
				pooledList3.Add(animInfo.colour);
				int num = i * 4;
				pooledList4.Add(num);
				pooledList4.Add(num + 1);
				pooledList4.Add(num + 2);
				pooledList4.Add(num);
				pooledList4.Add(num + 2);
				pooledList4.Add(num + 3);
			}
		}
		else
		{
			int count2 = anim_infos.Count;
			for (int j = 0; j < count2; j++)
			{
				AnimInfo animInfo2 = anim_infos[j];
				Vector2 vector2 = animInfo2.size * 0.5f;
				Vector3 b5 = -vector2;
				Vector3 b6 = new Vector2(vector2.x, 0f - vector2.y);
				Vector3 b7 = new Vector2(0f - vector2.x, vector2.y);
				Vector3 b8 = vector2;
				pooledList.Add(animInfo2.pos + b5);
				pooledList.Add(animInfo2.pos + b6);
				pooledList.Add(animInfo2.pos + b8);
				pooledList.Add(animInfo2.pos + b7);
				GetUVs(animInfo2.frame, out var uv_bl2, out var uv_br2, out var uv_tl2, out var uv_tr2);
				pooledList2.Add(uv_bl2);
				pooledList2.Add(uv_br2);
				pooledList2.Add(uv_tr2);
				pooledList2.Add(uv_tl2);
				pooledList3.Add(animInfo2.colour);
				pooledList3.Add(animInfo2.colour);
				pooledList3.Add(animInfo2.colour);
				pooledList3.Add(animInfo2.colour);
				int num2 = j * 4;
				pooledList4.Add(num2);
				pooledList4.Add(num2 + 1);
				pooledList4.Add(num2 + 2);
				pooledList4.Add(num2);
				pooledList4.Add(num2 + 2);
				pooledList4.Add(num2 + 3);
			}
		}
		mesh.SetVertices(pooledList);
		mesh.SetUVs(0, pooledList2);
		mesh.SetColors(pooledList3);
		mesh.SetTriangles(pooledList4, 0);
		Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, sheet.material, sheet.renderLayer, null, 0, materialProperties);
		pooledList4.Recycle();
		pooledList3.Recycle();
		pooledList2.Recycle();
		pooledList.Recycle();
	}

	public void Render()
	{
		Render(anims, apply_rotation: false);
		Render(rotatedAnims, apply_rotation: true);
	}
}
