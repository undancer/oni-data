using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SolidConduitFlowVisualizer
{
	[Serializable]
	public class Tuning
	{
		public bool renderMesh;

		public float size;

		public float spriteCount;

		public float framesPerSecond;

		public Texture2D backgroundTexture;

		public Texture2D foregroundTexture;
	}

	private class ConduitFlowMesh
	{
		private Mesh mesh;

		private Material material;

		private List<Vector3> positions = new List<Vector3>();

		private List<Vector4> uvs = new List<Vector4>();

		private List<int> triangles = new List<int>();

		private List<Color32> colors = new List<Color32>();

		private int quadIndex;

		public ConduitFlowMesh()
		{
			mesh = new Mesh();
			mesh.name = "ConduitMesh";
			material = new Material(Shader.Find("Klei/ConduitBall"));
		}

		public void AddQuad(Vector2 pos, Color32 color, float size, float is_foreground, float highlight, Vector2I uvbl, Vector2I uvtl, Vector2I uvbr, Vector2I uvtr)
		{
			float num = size * 0.5f;
			positions.Add(new Vector3(pos.x - num, pos.y - num, 0f));
			positions.Add(new Vector3(pos.x - num, pos.y + num, 0f));
			positions.Add(new Vector3(pos.x + num, pos.y - num, 0f));
			positions.Add(new Vector3(pos.x + num, pos.y + num, 0f));
			uvs.Add(new Vector4(uvbl.x, uvbl.y, is_foreground, highlight));
			uvs.Add(new Vector4(uvtl.x, uvtl.y, is_foreground, highlight));
			uvs.Add(new Vector4(uvbr.x, uvbr.y, is_foreground, highlight));
			uvs.Add(new Vector4(uvtr.x, uvtr.y, is_foreground, highlight));
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
			triangles.Add(quadIndex * 4);
			triangles.Add(quadIndex * 4 + 1);
			triangles.Add(quadIndex * 4 + 2);
			triangles.Add(quadIndex * 4 + 2);
			triangles.Add(quadIndex * 4 + 1);
			triangles.Add(quadIndex * 4 + 3);
			quadIndex++;
		}

		public void SetTexture(string id, Texture2D texture)
		{
			material.SetTexture(id, texture);
		}

		public void SetVector(string id, Vector4 data)
		{
			material.SetVector(id, data);
		}

		public void Begin()
		{
			positions.Clear();
			uvs.Clear();
			triangles.Clear();
			colors.Clear();
			quadIndex = 0;
		}

		public void End(float z, int layer)
		{
			mesh.Clear();
			mesh.SetVertices(positions);
			mesh.SetUVs(0, uvs);
			mesh.SetColors(colors);
			mesh.SetTriangles(triangles, 0, calculateBounds: false);
			Graphics.DrawMesh(mesh, new Vector3(GRID_OFFSET.x, GRID_OFFSET.y, z - 0.1f), Quaternion.identity, material, layer);
		}

		public void Cleanup()
		{
			UnityEngine.Object.Destroy(mesh);
			mesh = null;
			UnityEngine.Object.Destroy(material);
			material = null;
		}
	}

	private struct AudioInfo
	{
		public int networkID;

		public int blobCount;

		public float distance;

		public Vector3 position;
	}

	private SolidConduitFlow flowManager;

	private string overlaySound;

	private bool showContents;

	private double animTime;

	private int layer;

	private static Vector2 GRID_OFFSET = new Vector2(0.5f, 0.5f);

	private static int BLOB_SOUND_COUNT = 7;

	private List<AudioInfo> audioInfo;

	private HashSet<int> insulatedCells = new HashSet<int>();

	private Game.ConduitVisInfo visInfo;

	private ConduitFlowMesh movingBallMesh;

	private ConduitFlowMesh staticBallMesh;

	private int highlightedCell = -1;

	private Color32 highlightColour = new Color(0.2f, 0.2f, 0.2f, 0.2f);

	private Tuning tuning;

	public SolidConduitFlowVisualizer(SolidConduitFlow flow_manager, Game.ConduitVisInfo vis_info, string overlay_sound, Tuning tuning)
	{
		flowManager = flow_manager;
		visInfo = vis_info;
		overlaySound = overlay_sound;
		this.tuning = tuning;
		movingBallMesh = new ConduitFlowMesh();
		staticBallMesh = new ConduitFlowMesh();
	}

	public void FreeResources()
	{
		movingBallMesh.Cleanup();
		staticBallMesh.Cleanup();
	}

	private float CalculateMassScale(float mass)
	{
		float t = (mass - visInfo.overlayMassScaleRange.x) / (visInfo.overlayMassScaleRange.y - visInfo.overlayMassScaleRange.x);
		return Mathf.Lerp(visInfo.overlayMassScaleValues.x, visInfo.overlayMassScaleValues.y, t);
	}

	private Color32 GetContentsColor(Element element, Color32 default_color)
	{
		if (element != null)
		{
			Color color = element.substance.conduitColour;
			color.a = 128f;
			return color;
		}
		return default_color;
	}

	private Color32 GetBackgroundColor(float insulation_lerp)
	{
		if (showContents)
		{
			return Color32.Lerp(GlobalAssets.Instance.colorSet.GetColorByName(visInfo.overlayTintName), GlobalAssets.Instance.colorSet.GetColorByName(visInfo.overlayInsulatedTintName), insulation_lerp);
		}
		return Color32.Lerp(visInfo.tint, visInfo.insulatedTint, insulation_lerp);
	}

	public void Render(float z, int render_layer, float lerp_percent, bool trigger_audio = false)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2I vector2I = new Vector2I(Mathf.Max(0, visibleArea.Min.x - 1), Mathf.Max(0, visibleArea.Min.y - 1));
		Vector2I vector2I2 = new Vector2I(Mathf.Min(Grid.WidthInCells - 1, visibleArea.Max.x + 1), Mathf.Min(Grid.HeightInCells - 1, visibleArea.Max.y + 1));
		animTime += Time.deltaTime;
		if (trigger_audio)
		{
			if (audioInfo == null)
			{
				audioInfo = new List<AudioInfo>();
			}
			for (int i = 0; i < audioInfo.Count; i++)
			{
				AudioInfo value = audioInfo[i];
				value.distance = float.PositiveInfinity;
				value.position = Vector3.zero;
				value.blobCount = (value.blobCount + 1) % BLOB_SOUND_COUNT;
				audioInfo[i] = value;
			}
		}
		Vector3 position = CameraController.Instance.transform.GetPosition();
		Element element = null;
		if (tuning.renderMesh)
		{
			float z2 = 0f;
			if (showContents)
			{
				z2 = 1f;
			}
			float w = (float)((int)(animTime / (1.0 / (double)tuning.framesPerSecond)) % (int)tuning.spriteCount) * (1f / tuning.spriteCount);
			movingBallMesh.Begin();
			movingBallMesh.SetTexture("_BackgroundTex", tuning.backgroundTexture);
			movingBallMesh.SetTexture("_ForegroundTex", tuning.foregroundTexture);
			movingBallMesh.SetVector("_SpriteSettings", new Vector4(1f / tuning.spriteCount, 1f, z2, w));
			movingBallMesh.SetVector("_Highlight", new Vector4((float)(int)highlightColour.r / 255f, (float)(int)highlightColour.g / 255f, (float)(int)highlightColour.b / 255f, 0f));
			staticBallMesh.Begin();
			staticBallMesh.SetTexture("_BackgroundTex", tuning.backgroundTexture);
			staticBallMesh.SetTexture("_ForegroundTex", tuning.foregroundTexture);
			staticBallMesh.SetVector("_SpriteSettings", new Vector4(1f / tuning.spriteCount, 1f, z2, 0f));
			staticBallMesh.SetVector("_Highlight", new Vector4((float)(int)highlightColour.r / 255f, (float)(int)highlightColour.g / 255f, (float)(int)highlightColour.b / 255f, 0f));
			for (int j = 0; j < flowManager.GetSOAInfo().NumEntries; j++)
			{
				Vector2I vector2I3 = Grid.CellToXY(flowManager.GetSOAInfo().GetCell(j));
				if (vector2I3 < vector2I || vector2I3 > vector2I2)
				{
					continue;
				}
				SolidConduitFlow.Conduit conduit = flowManager.GetSOAInfo().GetConduit(j);
				SolidConduitFlow.ConduitFlowInfo lastFlowInfo = conduit.GetLastFlowInfo(flowManager);
				SolidConduitFlow.ConduitContents initialContents = conduit.GetInitialContents(flowManager);
				bool flag = lastFlowInfo.direction != SolidConduitFlow.FlowDirection.None;
				if (flag)
				{
					int cell = conduit.GetCell(flowManager);
					int cellFromDirection = SolidConduitFlow.GetCellFromDirection(cell, lastFlowInfo.direction);
					Vector2I vector2I4 = Grid.CellToXY(cell);
					Vector2I vector2I5 = Grid.CellToXY(cellFromDirection);
					Vector2 pos = vector2I4;
					if (cell != -1)
					{
						pos = Vector2.Lerp(new Vector2(vector2I4.x, vector2I4.y), new Vector2(vector2I5.x, vector2I5.y), lerp_percent);
					}
					float a = (insulatedCells.Contains(cell) ? 1f : 0f);
					float b = (insulatedCells.Contains(cellFromDirection) ? 1f : 0f);
					float insulation_lerp = Mathf.Lerp(a, b, lerp_percent);
					Color color = GetBackgroundColor(insulation_lerp);
					Vector2I uvbl = new Vector2I(0, 0);
					Vector2I uvtl = new Vector2I(0, 1);
					Vector2I uvbr = new Vector2I(1, 0);
					Vector2I uvtr = new Vector2I(1, 1);
					float highlight = 0f;
					if (showContents)
					{
						if (flag != initialContents.pickupableHandle.IsValid())
						{
							movingBallMesh.AddQuad(pos, color, tuning.size, 0f, 0f, uvbl, uvtl, uvbr, uvtr);
						}
					}
					else
					{
						element = null;
						if (Grid.PosToCell(new Vector3(pos.x + GRID_OFFSET.x, pos.y + GRID_OFFSET.y, 0f)) == highlightedCell)
						{
							highlight = 1f;
						}
					}
					Color32 contentsColor = GetContentsColor(element, color);
					float num = 1f;
					movingBallMesh.AddQuad(pos, contentsColor, tuning.size * num, 1f, highlight, uvbl, uvtl, uvbr, uvtr);
					if (trigger_audio)
					{
						AddAudioSource(conduit, position);
					}
				}
				if (!initialContents.pickupableHandle.IsValid() || flag)
				{
					continue;
				}
				int cell2 = conduit.GetCell(flowManager);
				Vector2 pos2 = Grid.CellToXY(cell2);
				float insulation_lerp2 = (insulatedCells.Contains(cell2) ? 1f : 0f);
				Vector2I uvbl2 = new Vector2I(0, 0);
				Vector2I uvtl2 = new Vector2I(0, 1);
				Vector2I uvbr2 = new Vector2I(1, 0);
				Vector2I uvtr2 = new Vector2I(1, 1);
				float highlight2 = 0f;
				Color color2 = GetBackgroundColor(insulation_lerp2);
				float num2 = 1f;
				if (showContents)
				{
					staticBallMesh.AddQuad(pos2, color2, tuning.size * num2, 0f, 0f, uvbl2, uvtl2, uvbr2, uvtr2);
				}
				else
				{
					element = null;
					if (cell2 == highlightedCell)
					{
						highlight2 = 1f;
					}
				}
				Color32 contentsColor2 = GetContentsColor(element, color2);
				staticBallMesh.AddQuad(pos2, contentsColor2, tuning.size * num2, 1f, highlight2, uvbl2, uvtl2, uvbr2, uvtr2);
			}
			movingBallMesh.End(z, layer);
			staticBallMesh.End(z, layer);
		}
		if (trigger_audio)
		{
			TriggerAudio();
		}
	}

	public void ColourizePipeContents(bool show_contents, bool move_to_overlay_layer)
	{
		showContents = show_contents;
		layer = ((show_contents && move_to_overlay_layer) ? LayerMask.NameToLayer("MaskedOverlay") : 0);
	}

	private void AddAudioSource(SolidConduitFlow.Conduit conduit, Vector3 camera_pos)
	{
		using (new KProfiler.Region("AddAudioSource"))
		{
			UtilityNetwork network = flowManager.GetNetwork(conduit);
			if (network == null)
			{
				return;
			}
			Vector3 vector = Grid.CellToPosCCC(conduit.GetCell(flowManager), Grid.SceneLayer.Building);
			float num = Vector3.SqrMagnitude(vector - camera_pos);
			bool flag = false;
			for (int i = 0; i < audioInfo.Count; i++)
			{
				AudioInfo value = audioInfo[i];
				if (value.networkID == network.id)
				{
					if (num < value.distance)
					{
						value.distance = num;
						value.position = vector;
						audioInfo[i] = value;
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				AudioInfo item = default(AudioInfo);
				item.networkID = network.id;
				item.position = vector;
				item.distance = num;
				item.blobCount = 0;
				audioInfo.Add(item);
			}
		}
	}

	private void TriggerAudio()
	{
		if (SpeedControlScreen.Instance.IsPaused)
		{
			return;
		}
		CameraController instance = CameraController.Instance;
		int num = 0;
		List<AudioInfo> list = new List<AudioInfo>();
		for (int i = 0; i < this.audioInfo.Count; i++)
		{
			if (instance.IsVisiblePos(this.audioInfo[i].position))
			{
				list.Add(this.audioInfo[i]);
				num++;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			AudioInfo audioInfo = list[j];
			if (audioInfo.distance != float.PositiveInfinity)
			{
				Vector3 position = audioInfo.position;
				position.z = 0f;
				EventInstance instance2 = SoundEvent.BeginOneShot(overlaySound, position);
				instance2.setParameterByName("blobCount", audioInfo.blobCount);
				instance2.setParameterByName("networkCount", num);
				SoundEvent.EndOneShot(instance2);
			}
		}
	}

	public void SetInsulated(int cell, bool insulated)
	{
		if (insulated)
		{
			insulatedCells.Add(cell);
		}
		else
		{
			insulatedCells.Remove(cell);
		}
	}

	public void SetHighlightedCell(int cell)
	{
		highlightedCell = cell;
	}
}
