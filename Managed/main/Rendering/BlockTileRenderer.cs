using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering
{
	public class BlockTileRenderer : MonoBehaviour
	{
		public enum RenderInfoLayer
		{
			Built,
			UnderConstruction,
			Replacement
		}

		[Flags]
		public enum Bits
		{
			UpLeft = 0x80,
			Up = 0x40,
			UpRight = 0x20,
			Left = 0x10,
			Right = 0x8,
			DownLeft = 0x4,
			Down = 0x2,
			DownRight = 0x1
		}

		protected class RenderInfo
		{
			private struct AtlasInfo
			{
				public Bits requiredConnections;

				public Bits forbiddenConnections;

				public Vector4 uvBox;

				public string name;
			}

			private AtlasInfo[] atlasInfo = null;

			private bool[,] dirtyChunks;

			private int queryLayer;

			private Material material;

			private int renderLayer;

			private Mesh[,] meshChunks;

			private DecorRenderInfo decorRenderInfo;

			private Vector2 trimUVSize;

			private Vector3 rootPosition;

			private Dictionary<int, int> occupiedCells = new Dictionary<int, int>();

			private SimHashes element;

			private float decorZOffset = -1f;

			private const float scale = 0.5f;

			private const float core_size = 256f;

			private const float trim_size = 64f;

			private const float cell_size = 1f;

			private const float world_trim_size = 0.25f;

			public RenderInfo(BlockTileRenderer renderer, int queryLayer, int renderLayer, BuildingDef def, SimHashes element)
			{
				this.queryLayer = queryLayer;
				this.renderLayer = renderLayer;
				rootPosition = new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer));
				this.element = element;
				material = new Material(def.BlockTileMaterial);
				if (def.BlockTileIsTransparent)
				{
					material.renderQueue = RenderQueues.Liquid;
					decorZOffset = Grid.GetLayerZ(Grid.SceneLayer.TileFront) - Grid.GetLayerZ(Grid.SceneLayer.Liquid) - 1f;
				}
				else if (def.SceneLayer == Grid.SceneLayer.TileMain)
				{
					material.renderQueue = RenderQueues.BlockTiles;
				}
				material.DisableKeyword("ENABLE_SHINE");
				if (element != SimHashes.Void)
				{
					material.SetTexture("_MainTex", def.BlockTileAtlas.texture);
					material.name = def.BlockTileAtlas.name + "Mat";
					if (def.BlockTileShineAtlas != null)
					{
						material.SetTexture("_SpecularTex", def.BlockTileShineAtlas.texture);
						material.EnableKeyword("ENABLE_SHINE");
					}
				}
				else
				{
					material.SetTexture("_MainTex", def.BlockTilePlaceAtlas.texture);
					material.name = def.BlockTilePlaceAtlas.name + "Mat";
				}
				int num = Grid.WidthInCells / 16 + 1;
				int num2 = Grid.HeightInCells / 16 + 1;
				meshChunks = new Mesh[num, num2];
				dirtyChunks = new bool[num, num2];
				for (int i = 0; i < num2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						dirtyChunks[j, i] = true;
					}
				}
				BlockTileDecorInfo blockTileDecorInfo = ((element == SimHashes.Void) ? def.DecorPlaceBlockTileInfo : def.DecorBlockTileInfo);
				if ((bool)blockTileDecorInfo)
				{
					decorRenderInfo = new DecorRenderInfo(num, num2, queryLayer, def, blockTileDecorInfo);
				}
				string name = def.BlockTileAtlas.items[0].name;
				int length = name.Length;
				int num3 = (length -= 4);
				int num4 = num3 - 8;
				int num5 = num4 - 1;
				int startIndex = num5 - 8;
				atlasInfo = new AtlasInfo[def.BlockTileAtlas.items.Length];
				for (int k = 0; k < atlasInfo.Length; k++)
				{
					TextureAtlas.Item item = def.BlockTileAtlas.items[k];
					string value = item.name.Substring(startIndex, 8);
					string value2 = item.name.Substring(num4, 8);
					int requiredConnections = Convert.ToInt32(value, 2);
					int forbiddenConnections = Convert.ToInt32(value2, 2);
					atlasInfo[k].requiredConnections = (Bits)requiredConnections;
					atlasInfo[k].forbiddenConnections = (Bits)forbiddenConnections;
					atlasInfo[k].uvBox = item.uvBox;
					atlasInfo[k].name = item.name;
				}
				trimUVSize = new Vector2(0.03125f, 0.03125f);
			}

			public void FreeResources()
			{
				UnityEngine.Object.DestroyImmediate(material);
				material = null;
				atlasInfo = null;
				for (int i = 0; i < meshChunks.GetLength(0); i++)
				{
					for (int j = 0; j < meshChunks.GetLength(1); j++)
					{
						if (meshChunks[i, j] != null)
						{
							UnityEngine.Object.DestroyImmediate(meshChunks[i, j]);
							meshChunks[i, j] = null;
						}
					}
				}
				meshChunks = null;
				decorRenderInfo = null;
				occupiedCells.Clear();
			}

			public void AddCell(int cell)
			{
				int value = 0;
				occupiedCells.TryGetValue(cell, out value);
				occupiedCells[cell] = value + 1;
				MarkDirty(cell);
			}

			public void RemoveCell(int cell)
			{
				int value = 0;
				occupiedCells.TryGetValue(cell, out value);
				if (value > 1)
				{
					occupiedCells[cell] = value - 1;
				}
				else
				{
					occupiedCells.Remove(cell);
				}
				MarkDirty(cell);
			}

			public void MarkDirty(int cell)
			{
				Vector2I chunkIdx = GetChunkIdx(cell);
				dirtyChunks[chunkIdx.x, chunkIdx.y] = true;
			}

			public void MarkDirtyIfOccupied(int cell)
			{
				if (occupiedCells.ContainsKey(cell))
				{
					MarkDirty(cell);
				}
			}

			public void Render(int x, int y)
			{
				if (meshChunks[x, y] != null)
				{
					Graphics.DrawMesh(meshChunks[x, y], rootPosition, Quaternion.identity, material, renderLayer);
				}
				if (decorRenderInfo != null)
				{
					decorRenderInfo.Render(x, y, rootPosition - new Vector3(0f, 0f, 0.5f), renderLayer);
				}
			}

			public void Rebuild(BlockTileRenderer renderer, int chunk_x, int chunk_y, List<Vector3> vertices, List<Vector2> uvs, List<int> indices, List<Color> colours)
			{
				if (!dirtyChunks[chunk_x, chunk_y] && !renderer.ForceRebuild)
				{
					return;
				}
				dirtyChunks[chunk_x, chunk_y] = false;
				vertices.Clear();
				uvs.Clear();
				indices.Clear();
				colours.Clear();
				for (int i = chunk_y * 16; i < chunk_y * 16 + 16; i++)
				{
					for (int j = chunk_x * 16; j < chunk_x * 16 + 16; j++)
					{
						int num = i * Grid.WidthInCells + j;
						if (!occupiedCells.ContainsKey(num))
						{
							continue;
						}
						Bits connectionBits = renderer.GetConnectionBits(j, i, queryLayer);
						for (int k = 0; k < atlasInfo.Length; k++)
						{
							bool flag = (atlasInfo[k].requiredConnections & connectionBits) == atlasInfo[k].requiredConnections;
							bool flag2 = (atlasInfo[k].forbiddenConnections & connectionBits) != 0;
							if (flag && !flag2)
							{
								Color cellColour = renderer.GetCellColour(num, element);
								AddVertexInfo(atlasInfo[k], trimUVSize, j, i, connectionBits, cellColour, vertices, uvs, indices, colours);
								break;
							}
						}
					}
				}
				Mesh mesh = meshChunks[chunk_x, chunk_y];
				if (vertices.Count > 0)
				{
					if (mesh == null)
					{
						mesh = new Mesh();
						mesh.name = "BlockTile";
						meshChunks[chunk_x, chunk_y] = mesh;
					}
					mesh.Clear();
					mesh.SetVertices(vertices);
					mesh.SetUVs(0, uvs);
					mesh.SetColors(colours);
					mesh.SetTriangles(indices, 0);
				}
				else if (mesh != null)
				{
					meshChunks[chunk_x, chunk_y] = null;
					mesh = null;
				}
				if (decorRenderInfo != null)
				{
					decorRenderInfo.Rebuild(renderer, occupiedCells, chunk_x, chunk_y, decorZOffset, 16, vertices, uvs, colours, indices, element);
				}
			}

			private void AddVertexInfo(AtlasInfo atlas_info, Vector2 uv_trim_size, int x, int y, Bits connection_bits, Color color, List<Vector3> vertices, List<Vector2> uvs, List<int> indices, List<Color> colours)
			{
				Vector2 vector = new Vector2(x, y);
				Vector2 v = vector + new Vector2(1f, 1f);
				Vector2 item = new Vector2(atlas_info.uvBox.x, atlas_info.uvBox.w);
				Vector2 item2 = new Vector2(atlas_info.uvBox.z, atlas_info.uvBox.y);
				if ((connection_bits & Bits.Left) == 0)
				{
					vector.x -= 0.25f;
				}
				else
				{
					item.x += uv_trim_size.x;
				}
				if ((connection_bits & Bits.Right) == 0)
				{
					v.x += 0.25f;
				}
				else
				{
					item2.x -= uv_trim_size.x;
				}
				if ((connection_bits & Bits.Up) == 0)
				{
					v.y += 0.25f;
				}
				else
				{
					item2.y -= uv_trim_size.y;
				}
				if ((connection_bits & Bits.Down) == 0)
				{
					vector.y -= 0.25f;
				}
				else
				{
					item.y += uv_trim_size.y;
				}
				int count = vertices.Count;
				vertices.Add(vector);
				vertices.Add(new Vector2(v.x, vector.y));
				vertices.Add(v);
				vertices.Add(new Vector2(vector.x, v.y));
				uvs.Add(item);
				uvs.Add(new Vector2(item2.x, item.y));
				uvs.Add(item2);
				uvs.Add(new Vector2(item.x, item2.y));
				indices.Add(count);
				indices.Add(count + 1);
				indices.Add(count + 2);
				indices.Add(count);
				indices.Add(count + 2);
				indices.Add(count + 3);
				colours.Add(color);
				colours.Add(color);
				colours.Add(color);
				colours.Add(color);
			}
		}

		private class DecorRenderInfo
		{
			private struct TriangleInfo
			{
				public int sortOrder;

				public int i0;

				public int i1;

				public int i2;
			}

			private int queryLayer;

			private BlockTileDecorInfo decorInfo;

			private Mesh[,] meshChunks;

			private Material material;

			private List<TriangleInfo> triangles = new List<TriangleInfo>();

			private static Vector2 simplex_scale = new Vector2(92.41f, 87.16f);

			public DecorRenderInfo(int num_x_chunks, int num_y_chunks, int query_layer, BuildingDef def, BlockTileDecorInfo decorInfo)
			{
				this.decorInfo = decorInfo;
				queryLayer = query_layer;
				material = new Material(def.BlockTileMaterial);
				if (def.BlockTileIsTransparent)
				{
					material.renderQueue = RenderQueues.Liquid;
				}
				else if (def.SceneLayer == Grid.SceneLayer.TileMain)
				{
					material.renderQueue = RenderQueues.BlockTiles;
				}
				material.SetTexture("_MainTex", decorInfo.atlas.texture);
				if (decorInfo.atlasSpec != null)
				{
					material.SetTexture("_SpecularTex", decorInfo.atlasSpec.texture);
					material.EnableKeyword("ENABLE_SHINE");
				}
				else
				{
					material.DisableKeyword("ENABLE_SHINE");
				}
				meshChunks = new Mesh[num_x_chunks, num_y_chunks];
			}

			public void FreeResources()
			{
				decorInfo = null;
				UnityEngine.Object.DestroyImmediate(material);
				material = null;
				for (int i = 0; i < meshChunks.GetLength(0); i++)
				{
					for (int j = 0; j < meshChunks.GetLength(1); j++)
					{
						if (meshChunks[i, j] != null)
						{
							UnityEngine.Object.DestroyImmediate(meshChunks[i, j]);
							meshChunks[i, j] = null;
						}
					}
				}
				meshChunks = null;
				triangles.Clear();
			}

			public void Render(int x, int y, Vector3 position, int renderLayer)
			{
				if (meshChunks[x, y] != null)
				{
					Graphics.DrawMesh(meshChunks[x, y], position, Quaternion.identity, material, renderLayer);
				}
			}

			public void Rebuild(BlockTileRenderer renderer, Dictionary<int, int> occupiedCells, int chunk_x, int chunk_y, float z_offset, int chunkEdgeSize, List<Vector3> vertices, List<Vector2> uvs, List<Color> colours, List<int> indices, SimHashes element)
			{
				vertices.Clear();
				uvs.Clear();
				triangles.Clear();
				colours.Clear();
				indices.Clear();
				for (int i = chunk_y * chunkEdgeSize; i < chunk_y * chunkEdgeSize + chunkEdgeSize; i++)
				{
					for (int j = chunk_x * chunkEdgeSize; j < chunk_x * chunkEdgeSize + chunkEdgeSize; j++)
					{
						int num = i * Grid.WidthInCells + j;
						if (occupiedCells.ContainsKey(num))
						{
							Color cellColour = renderer.GetCellColour(num, element);
							Bits decorConnectionBits = renderer.GetDecorConnectionBits(j, i, queryLayer);
							AddDecor(j, i, z_offset, decorConnectionBits, cellColour, vertices, uvs, triangles, colours);
						}
					}
				}
				if (vertices.Count > 0)
				{
					Mesh mesh = meshChunks[chunk_x, chunk_y];
					if (mesh == null)
					{
						mesh = new Mesh();
						mesh.name = "DecorRender";
						meshChunks[chunk_x, chunk_y] = mesh;
					}
					triangles.Sort((TriangleInfo a, TriangleInfo b) => a.sortOrder.CompareTo(b.sortOrder));
					for (int k = 0; k < triangles.Count; k++)
					{
						indices.Add(triangles[k].i0);
						indices.Add(triangles[k].i1);
						indices.Add(triangles[k].i2);
					}
					mesh.Clear();
					mesh.SetVertices(vertices);
					mesh.SetUVs(0, uvs);
					mesh.SetColors(colours);
					mesh.SetTriangles(indices, 0);
				}
				else
				{
					meshChunks[chunk_x, chunk_y] = null;
				}
			}

			private void AddDecor(int x, int y, float z_offset, Bits connection_bits, Color colour, List<Vector3> vertices, List<Vector2> uvs, List<TriangleInfo> triangles, List<Color> colours)
			{
				for (int i = 0; i < decorInfo.decor.Length; i++)
				{
					BlockTileDecorInfo.Decor decor = decorInfo.decor[i];
					if (decor.variants == null || decor.variants.Length == 0)
					{
						continue;
					}
					bool flag = (connection_bits & decor.requiredConnections) == decor.requiredConnections;
					bool flag2 = (connection_bits & decor.forbiddenConnections) != 0;
					if (!flag || flag2)
					{
						continue;
					}
					float num = PerlinSimplexNoise.noise((float)(i + x + connection_bits) * simplex_scale.x, (float)(i + y + connection_bits) * simplex_scale.y);
					if (!(num < decor.probabilityCutoff))
					{
						int num2 = (int)((float)(decor.variants.Length - 1) * num);
						int count = vertices.Count;
						Vector3 b = new Vector3(x, y, z_offset) + decor.variants[num2].offset;
						Vector3[] vertices2 = decor.variants[num2].atlasItem.vertices;
						foreach (Vector3 a in vertices2)
						{
							vertices.Add(a + b);
							colours.Add(colour);
						}
						uvs.AddRange(decor.variants[num2].atlasItem.uvs);
						int[] indices = decor.variants[num2].atlasItem.indices;
						for (int k = 0; k < indices.Length; k += 3)
						{
							triangles.Add(new TriangleInfo
							{
								sortOrder = decor.sortOrder,
								i0 = indices[k] + count,
								i1 = indices[k + 1] + count,
								i2 = indices[k + 2] + count
							});
						}
					}
				}
			}
		}

		[SerializeField]
		private bool forceRebuild = false;

		[SerializeField]
		private Color highlightColour = new Color(1.25f, 1.25f, 1.25f, 1f);

		[SerializeField]
		private Color selectColour = new Color(1.5f, 1.5f, 1.5f, 1f);

		[SerializeField]
		private Color invalidPlaceColour = Color.red;

		private const float TILE_ATLAS_WIDTH = 2048f;

		private const float TILE_ATLAS_HEIGHT = 2048f;

		private const int chunkEdgeSize = 16;

		protected Dictionary<KeyValuePair<BuildingDef, RenderInfoLayer>, RenderInfo> renderInfo = new Dictionary<KeyValuePair<BuildingDef, RenderInfoLayer>, RenderInfo>();

		private int selectedCell = -1;

		private int highlightCell = -1;

		private int invalidPlaceCell = -1;

		public bool ForceRebuild => forceRebuild;

		public static RenderInfoLayer GetRenderInfoLayer(bool isReplacement, SimHashes element)
		{
			return isReplacement ? RenderInfoLayer.Replacement : ((element == SimHashes.Void) ? RenderInfoLayer.UnderConstruction : RenderInfoLayer.Built);
		}

		public BlockTileRenderer()
		{
			forceRebuild = false;
		}

		public void FreeResources()
		{
			foreach (KeyValuePair<KeyValuePair<BuildingDef, RenderInfoLayer>, RenderInfo> item in renderInfo)
			{
				if (item.Value != null)
				{
					item.Value.FreeResources();
				}
			}
			renderInfo.Clear();
		}

		private static bool MatchesDef(GameObject go, BuildingDef def)
		{
			return go != null && go.GetComponent<Building>().Def == def;
		}

		public virtual Bits GetConnectionBits(int x, int y, int query_layer)
		{
			Bits bits = (Bits)0;
			GameObject gameObject = Grid.Objects[y * Grid.WidthInCells + x, query_layer];
			BuildingDef def = ((gameObject != null) ? gameObject.GetComponent<Building>().Def : null);
			if (y > 0)
			{
				int num = (y - 1) * Grid.WidthInCells + x;
				if (x > 0 && MatchesDef(Grid.Objects[num - 1, query_layer], def))
				{
					bits |= Bits.DownLeft;
				}
				if (MatchesDef(Grid.Objects[num, query_layer], def))
				{
					bits |= Bits.Down;
				}
				if (x < Grid.WidthInCells - 1 && MatchesDef(Grid.Objects[num + 1, query_layer], def))
				{
					bits |= Bits.DownRight;
				}
			}
			int num2 = y * Grid.WidthInCells + x;
			if (x > 0 && MatchesDef(Grid.Objects[num2 - 1, query_layer], def))
			{
				bits |= Bits.Left;
			}
			if (x < Grid.WidthInCells - 1 && MatchesDef(Grid.Objects[num2 + 1, query_layer], def))
			{
				bits |= Bits.Right;
			}
			if (y < Grid.HeightInCells - 1)
			{
				int num3 = (y + 1) * Grid.WidthInCells + x;
				if (x > 0 && MatchesDef(Grid.Objects[num3 - 1, query_layer], def))
				{
					bits |= Bits.UpLeft;
				}
				if (MatchesDef(Grid.Objects[num3, query_layer], def))
				{
					bits |= Bits.Up;
				}
				if (x < Grid.WidthInCells + 1 && MatchesDef(Grid.Objects[num3 + 1, query_layer], def))
				{
					bits |= Bits.UpRight;
				}
			}
			return bits;
		}

		private bool IsDecorConnectable(GameObject src, GameObject target)
		{
			if (src != null && target != null)
			{
				IBlockTileInfo component = src.GetComponent<IBlockTileInfo>();
				IBlockTileInfo component2 = target.GetComponent<IBlockTileInfo>();
				if (component != null && component2 != null)
				{
					return component.GetBlockTileConnectorID() == component2.GetBlockTileConnectorID();
				}
			}
			return false;
		}

		public virtual Bits GetDecorConnectionBits(int x, int y, int query_layer)
		{
			Bits bits = (Bits)0;
			GameObject src = Grid.Objects[y * Grid.WidthInCells + x, query_layer];
			if (y > 0)
			{
				int num = (y - 1) * Grid.WidthInCells + x;
				if (x > 0 && Grid.Objects[num - 1, query_layer] != null)
				{
					bits |= Bits.DownLeft;
				}
				if (Grid.Objects[num, query_layer] != null)
				{
					bits |= Bits.Down;
				}
				if (x < Grid.WidthInCells - 1 && Grid.Objects[num + 1, query_layer] != null)
				{
					bits |= Bits.DownRight;
				}
			}
			int num2 = y * Grid.WidthInCells + x;
			if (x > 0 && IsDecorConnectable(src, Grid.Objects[num2 - 1, query_layer]))
			{
				bits |= Bits.Left;
			}
			if (x < Grid.WidthInCells - 1 && IsDecorConnectable(src, Grid.Objects[num2 + 1, query_layer]))
			{
				bits |= Bits.Right;
			}
			if (y < Grid.HeightInCells - 1)
			{
				int num3 = (y + 1) * Grid.WidthInCells + x;
				if (x > 0 && Grid.Objects[num3 - 1, query_layer] != null)
				{
					bits |= Bits.UpLeft;
				}
				if (Grid.Objects[num3, query_layer] != null)
				{
					bits |= Bits.Up;
				}
				if (x < Grid.WidthInCells + 1 && Grid.Objects[num3 + 1, query_layer] != null)
				{
					bits |= Bits.UpRight;
				}
			}
			return bits;
		}

		public void LateUpdate()
		{
			Render();
		}

		private void Render()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			if (GameUtil.IsCapturingTimeLapse())
			{
				vector2I = new Vector2I(0, 0);
				vector2I2 = new Vector2I(Grid.WidthInCells / 16, Grid.HeightInCells / 16);
			}
			else
			{
				GridArea visibleArea = GridVisibleArea.GetVisibleArea();
				vector2I = new Vector2I(visibleArea.Min.x / 16, visibleArea.Min.y / 16);
				vector2I2 = new Vector2I((visibleArea.Max.x + 16 - 1) / 16, (visibleArea.Max.y + 16 - 1) / 16);
			}
			foreach (KeyValuePair<KeyValuePair<BuildingDef, RenderInfoLayer>, RenderInfo> item in renderInfo)
			{
				RenderInfo value = item.Value;
				for (int i = vector2I.y; i < vector2I2.y; i++)
				{
					for (int j = vector2I.x; j < vector2I2.x; j++)
					{
						value.Rebuild(this, j, i, MeshUtil.vertices, MeshUtil.uvs, MeshUtil.indices, MeshUtil.colours);
						value.Render(j, i);
					}
				}
			}
		}

		public Color GetCellColour(int cell, SimHashes element)
		{
			if (cell == selectedCell)
			{
				return selectColour;
			}
			if (cell == invalidPlaceCell && element == SimHashes.Void)
			{
				return invalidPlaceColour;
			}
			if (cell == highlightCell)
			{
				return highlightColour;
			}
			return Color.white;
		}

		public static Vector2I GetChunkIdx(int cell)
		{
			Vector2I vector2I = Grid.CellToXY(cell);
			return new Vector2I(vector2I.x / 16, vector2I.y / 16);
		}

		public void AddBlock(int renderLayer, BuildingDef def, bool isReplacement, SimHashes element, int cell)
		{
			KeyValuePair<BuildingDef, RenderInfoLayer> key = new KeyValuePair<BuildingDef, RenderInfoLayer>(def, GetRenderInfoLayer(isReplacement, element));
			if (!renderInfo.TryGetValue(key, out var value))
			{
				int queryLayer = (int)(isReplacement ? def.ReplacementLayer : def.TileLayer);
				value = new RenderInfo(this, queryLayer, renderLayer, def, element);
				renderInfo[key] = value;
			}
			value.AddCell(cell);
		}

		public void RemoveBlock(BuildingDef def, bool isReplacement, SimHashes element, int cell)
		{
			KeyValuePair<BuildingDef, RenderInfoLayer> key = new KeyValuePair<BuildingDef, RenderInfoLayer>(def, GetRenderInfoLayer(isReplacement, element));
			if (renderInfo.TryGetValue(key, out var value))
			{
				value.RemoveCell(cell);
			}
		}

		public void Rebuild(ObjectLayer layer, int cell)
		{
			foreach (KeyValuePair<KeyValuePair<BuildingDef, RenderInfoLayer>, RenderInfo> item in renderInfo)
			{
				if (item.Key.Key.TileLayer == layer)
				{
					item.Value.MarkDirty(cell);
				}
			}
		}

		public void SelectCell(int cell, bool enabled)
		{
			UpdateCellStatus(ref selectedCell, cell, enabled);
		}

		public void HighlightCell(int cell, bool enabled)
		{
			UpdateCellStatus(ref highlightCell, cell, enabled);
		}

		public void SetInvalidPlaceCell(int cell, bool enabled)
		{
			UpdateCellStatus(ref invalidPlaceCell, cell, enabled);
		}

		private void UpdateCellStatus(ref int cell_status, int cell, bool enabled)
		{
			if (enabled)
			{
				if (cell == cell_status)
				{
					return;
				}
				if (cell_status != -1)
				{
					foreach (KeyValuePair<KeyValuePair<BuildingDef, RenderInfoLayer>, RenderInfo> item in renderInfo)
					{
						item.Value.MarkDirtyIfOccupied(cell_status);
					}
				}
				cell_status = cell;
				foreach (KeyValuePair<KeyValuePair<BuildingDef, RenderInfoLayer>, RenderInfo> item2 in renderInfo)
				{
					item2.Value.MarkDirtyIfOccupied(cell_status);
				}
			}
			else
			{
				if (cell_status != cell)
				{
					return;
				}
				foreach (KeyValuePair<KeyValuePair<BuildingDef, RenderInfoLayer>, RenderInfo> item3 in renderInfo)
				{
					item3.Value.MarkDirty(cell_status);
				}
				cell_status = -1;
			}
		}
	}
}
