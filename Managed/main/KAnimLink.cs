using System;
using UnityEngine;

public class KAnimLink
{
	public bool syncTint = true;

	private KAnimControllerBase master;

	private KAnimControllerBase slave;

	public KAnimLink(KAnimControllerBase master, KAnimControllerBase slave)
	{
		this.slave = slave;
		this.master = master;
		Register();
	}

	private void Register()
	{
		master.OnOverlayColourChanged += OnOverlayColourChanged;
		KAnimControllerBase kAnimControllerBase = master;
		kAnimControllerBase.OnTintChanged = (Action<Color>)Delegate.Combine(kAnimControllerBase.OnTintChanged, new Action<Color>(OnTintColourChanged));
		KAnimControllerBase kAnimControllerBase2 = master;
		kAnimControllerBase2.OnHighlightChanged = (Action<Color>)Delegate.Combine(kAnimControllerBase2.OnHighlightChanged, new Action<Color>(OnHighlightColourChanged));
		master.onLayerChanged += slave.SetLayer;
	}

	public void Unregister()
	{
		if (master != null)
		{
			master.OnOverlayColourChanged -= OnOverlayColourChanged;
			KAnimControllerBase kAnimControllerBase = master;
			kAnimControllerBase.OnTintChanged = (Action<Color>)Delegate.Remove(kAnimControllerBase.OnTintChanged, new Action<Color>(OnTintColourChanged));
			KAnimControllerBase kAnimControllerBase2 = master;
			kAnimControllerBase2.OnHighlightChanged = (Action<Color>)Delegate.Remove(kAnimControllerBase2.OnHighlightChanged, new Action<Color>(OnHighlightColourChanged));
			if (slave != null)
			{
				master.onLayerChanged -= slave.SetLayer;
			}
		}
	}

	private void OnOverlayColourChanged(Color32 c)
	{
		if (slave != null)
		{
			slave.OverlayColour = c;
		}
	}

	private void OnTintColourChanged(Color c)
	{
		if (syncTint && slave != null)
		{
			slave.TintColour = c;
		}
	}

	private void OnHighlightColourChanged(Color c)
	{
		if (slave != null)
		{
			slave.HighlightColour = c;
		}
	}
}
