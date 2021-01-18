using System.Collections.Generic;
using UnityEngine;

public class PrioritizableRenderer
{
	private Mesh mesh;

	private int layer;

	private Material material;

	private int prioritizableCount;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private int[] triangles;

	private List<Prioritizable> prioritizables;

	private PrioritizeTool tool;

	public PrioritizeTool currentTool
	{
		get
		{
			return tool;
		}
		set
		{
			tool = value;
		}
	}

	public PrioritizableRenderer()
	{
		layer = LayerMask.NameToLayer("UI");
		Shader shader = Shader.Find("Klei/Prioritizable");
		Texture2D texture = Assets.GetTexture("priority_overlay_atlas");
		material = new Material(shader);
		material.SetTexture(Shader.PropertyToID("_MainTex"), texture);
		prioritizables = new List<Prioritizable>();
		mesh = new Mesh();
		mesh.name = "Prioritizables";
		mesh.MarkDynamic();
	}

	public void Cleanup()
	{
		material = null;
		vertices = null;
		uvs = null;
		prioritizables = null;
		triangles = null;
		Object.DestroyImmediate(mesh);
		mesh = null;
	}

	public void RenderEveryTick()
	{
		using (new KProfiler.Region("PrioritizableRenderer"))
		{
			if (GameScreenManager.Instance == null || SimDebugView.Instance == null || SimDebugView.Instance.GetMode() != OverlayModes.Priorities.ID)
			{
				return;
			}
			prioritizables.Clear();
			Grid.GetVisibleExtents(out var min, out var max);
			int height = max.y - min.y;
			int width = max.x - min.x;
			Extents extents = new Extents(min.x, min.y, width, height);
			List<ScenePartitionerEntry> list = new List<ScenePartitionerEntry>();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.prioritizableObjects, list);
			foreach (ScenePartitionerEntry item in list)
			{
				Prioritizable prioritizable = (Prioritizable)item.obj;
				if (prioritizable != null && prioritizable.showIcon && prioritizable.IsPrioritizable() && tool.IsActiveLayer(tool.GetFilterLayerFromGameObject(prioritizable.gameObject)))
				{
					prioritizables.Add(prioritizable);
				}
			}
			if (prioritizableCount != prioritizables.Count)
			{
				prioritizableCount = prioritizables.Count;
				vertices = new Vector3[4 * prioritizableCount];
				uvs = new Vector2[4 * prioritizableCount];
				triangles = new int[6 * prioritizableCount];
			}
			if (prioritizableCount == 0)
			{
				return;
			}
			for (int i = 0; i < prioritizables.Count; i++)
			{
				Prioritizable prioritizable2 = prioritizables[i];
				Vector3 vector = Vector3.zero;
				KAnimControllerBase component = prioritizable2.GetComponent<KAnimControllerBase>();
				vector = ((!(component != null)) ? prioritizable2.transform.GetPosition() : component.GetWorldPivot());
				vector.x += prioritizable2.iconOffset.x;
				vector.y += prioritizable2.iconOffset.y;
				Vector2 vector2 = new Vector2(0.2f, 0.3f) * prioritizable2.iconScale;
				float z = -5f;
				int num = 4 * i;
				vertices[num] = new Vector3(vector.x - vector2.x, vector.y - vector2.y, z);
				vertices[1 + num] = new Vector3(vector.x - vector2.x, vector.y + vector2.y, z);
				vertices[2 + num] = new Vector3(vector.x + vector2.x, vector.y - vector2.y, z);
				vertices[3 + num] = new Vector3(vector.x + vector2.x, vector.y + vector2.y, z);
				PrioritySetting masterPriority = prioritizable2.GetMasterPriority();
				float num2 = -1f;
				if (masterPriority.priority_class >= PriorityScreen.PriorityClass.high)
				{
					num2 += 9f;
				}
				if (masterPriority.priority_class >= PriorityScreen.PriorityClass.topPriority)
				{
					num2 += 0f;
				}
				num2 += (float)masterPriority.priority_value;
				float num3 = 0.1f * num2;
				float num4 = 0f;
				float num5 = 0.1f;
				float num6 = 1f;
				uvs[num] = new Vector2(num3, num4);
				uvs[1 + num] = new Vector2(num3, num4 + num6);
				uvs[2 + num] = new Vector2(num3 + num5, num4);
				uvs[3 + num] = new Vector2(num3 + num5, num4 + num6);
				int num7 = 6 * i;
				triangles[num7] = num;
				triangles[1 + num7] = num + 1;
				triangles[2 + num7] = num + 2;
				triangles[3 + num7] = num + 2;
				triangles[4 + num7] = num + 1;
				triangles[5 + num7] = num + 3;
			}
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.SetTriangles(triangles, 0);
			mesh.RecalculateBounds();
			Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, layer, GameScreenManager.Instance.worldSpaceCanvas.GetComponent<Canvas>().worldCamera, 0, null, castShadows: false, receiveShadows: false);
		}
	}
}
