using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class KBatchedAnimCanvasRenderer : MonoBehaviour, IMaskable
{
	private struct TextureToCopyEntry
	{
		public int textureId;

		public int sizeId;
	}

	private RectTransform rootRectTransform;

	public KAnimBatch batch;

	public Material uiMat;

	private KAnimConverter.IAnimConverter converter;

	private CompareFunction _cmp = CompareFunction.Never;

	private StencilOp _op = StencilOp.Zero;

	private static TextureToCopyEntry[] texturesToCopy;

	private Vector4 _ClipRect = new Vector4(0f, 0f, 0f, 1f);

	public CanvasRenderer canvass
	{
		get;
		private set;
	}

	public CompareFunction compare
	{
		get
		{
			return _cmp;
		}
		set
		{
			if (_cmp != value)
			{
				if (uiMat != null)
				{
					uiMat.SetInt("_StencilComp", (int)value);
				}
				_cmp = value;
			}
		}
	}

	public StencilOp stencilOp
	{
		get
		{
			return _op;
		}
		set
		{
			if (_op != value)
			{
				if (uiMat != null)
				{
					uiMat.SetInt("_StencilOp", (int)_op);
				}
				_op = value;
			}
		}
	}

	void IMaskable.RecalculateMasking()
	{
		Mask componentInParent = GetComponentInParent<Mask>();
		if (componentInParent != null && componentInParent.enabled)
		{
			compare = CompareFunction.Equal;
			stencilOp = StencilOp.Keep;
		}
		else
		{
			compare = CompareFunction.Disabled;
			stencilOp = StencilOp.Keep;
		}
		if (uiMat != null)
		{
			uiMat.SetInt("_StencilComp", (int)compare);
			uiMat.SetInt("_StencilOp", (int)stencilOp);
		}
	}

	public void SetBatch(KAnimConverter.IAnimConverter conv)
	{
		converter = conv;
		if (conv != null)
		{
			batch = conv.GetBatch();
		}
		else
		{
			batch = null;
			if (uiMat != null)
			{
				Object.Destroy(uiMat);
				uiMat = null;
			}
		}
		if (batch == null)
		{
			return;
		}
		canvass = GetComponent<CanvasRenderer>();
		if (canvass == null)
		{
			canvass = base.gameObject.AddComponent<CanvasRenderer>();
		}
		rootRectTransform = GetComponent<RectTransform>();
		if (rootRectTransform == null)
		{
			rootRectTransform = base.gameObject.AddComponent<RectTransform>();
		}
		if (batch.group.InitOK)
		{
			if (uiMat != null)
			{
				Object.Destroy(uiMat);
				uiMat = null;
			}
			Material material = batch.group.GetMaterial(batch.materialType);
			uiMat = new Material(material);
			((IMaskable)this).RecalculateMasking();
		}
	}

	private void UpdateCanvas()
	{
		canvass.Clear();
		canvass.SetMesh(batch.group.mesh);
		canvass.materialCount = 1;
		canvass.SetMaterial(uiMat, 0);
	}

	private void CopyPropertyBlockToMaterial()
	{
		if (texturesToCopy == null)
		{
			TextureToCopyEntry[] array = new TextureToCopyEntry[4];
			TextureToCopyEntry textureToCopyEntry = new TextureToCopyEntry
			{
				textureId = Shader.PropertyToID("instanceTex"),
				sizeId = Shader.PropertyToID("INSTANCE_TEXTURE_SIZE")
			};
			array[0] = textureToCopyEntry;
			textureToCopyEntry = new TextureToCopyEntry
			{
				textureId = Shader.PropertyToID("buildAndAnimTex"),
				sizeId = Shader.PropertyToID("BUILD_AND_ANIM_TEXTURE_SIZE")
			};
			array[1] = textureToCopyEntry;
			textureToCopyEntry = new TextureToCopyEntry
			{
				textureId = Shader.PropertyToID("symbolInstanceTex"),
				sizeId = Shader.PropertyToID("SYMBOL_INSTANCE_TEXTURE_SIZE")
			};
			array[2] = textureToCopyEntry;
			textureToCopyEntry = new TextureToCopyEntry
			{
				textureId = Shader.PropertyToID("symbolOverrideInfoTex"),
				sizeId = Shader.PropertyToID("SYMBOL_OVERRIDE_INFO_TEXTURE_SIZE")
			};
			array[3] = textureToCopyEntry;
			texturesToCopy = array;
		}
		TextureToCopyEntry[] array2 = texturesToCopy;
		for (int i = 0; i < array2.Length; i++)
		{
			TextureToCopyEntry textureToCopyEntry2 = array2[i];
			uiMat.SetTexture(textureToCopyEntry2.textureId, batch.matProperties.GetTexture(textureToCopyEntry2.textureId));
			uiMat.SetVector(textureToCopyEntry2.sizeId, batch.matProperties.GetVector(textureToCopyEntry2.sizeId));
		}
		for (int j = 0; j < KAnimBatchManager.instance.atlasNames.Length; j++)
		{
			Texture texture = batch.matProperties.GetTexture(KAnimBatchManager.instance.atlasNames[j]);
			if (texture != null)
			{
				uiMat.SetTexture(KAnimBatchManager.instance.atlasNames[j], texture);
			}
		}
		array2 = texturesToCopy;
		for (int i = 0; i < array2.Length; i++)
		{
			TextureToCopyEntry textureToCopyEntry3 = array2[i];
			uiMat.SetTexture(textureToCopyEntry3.textureId, batch.matProperties.GetTexture(textureToCopyEntry3.textureId));
			uiMat.SetVector(textureToCopyEntry3.sizeId, batch.matProperties.GetVector(textureToCopyEntry3.sizeId));
		}
		for (int k = 0; k < KAnimBatchManager.instance.atlasNames.Length; k++)
		{
			Texture texture2 = batch.matProperties.GetTexture(KAnimBatchManager.instance.atlasNames[k]);
			if (texture2 != null)
			{
				uiMat.SetTexture(KAnimBatchManager.instance.atlasNames[k], texture2);
			}
		}
		uiMat.SetFloat(KAnimBatch.ShaderProperty_SUPPORTS_SYMBOL_OVERRIDING, batch.matProperties.GetFloat(KAnimBatch.ShaderProperty_SUPPORTS_SYMBOL_OVERRIDING));
		uiMat.SetFloat(KAnimBatch.ShaderProperty_ANIM_TEXTURE_START_OFFSET, batch.matProperties.GetFloat(KAnimBatch.ShaderProperty_ANIM_TEXTURE_START_OFFSET));
	}

	private void LateUpdate()
	{
		if (batch != null)
		{
			if (base.transform.hasChanged)
			{
				batch.SetDirty(converter);
				base.transform.hasChanged = false;
			}
			_ClipRect.x = rootRectTransform.rect.xMin;
			_ClipRect.y = rootRectTransform.rect.yMin;
			_ClipRect.z = rootRectTransform.rect.xMax;
			_ClipRect.w = rootRectTransform.rect.yMax;
			UpdateCanvas();
			CopyPropertyBlockToMaterial();
		}
	}
}
