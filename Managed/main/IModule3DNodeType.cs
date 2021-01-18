using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using UnityEngine;

public class IModule3DNodeType : IConnectionTypeDeclaration
{
	public string Identifier => "IModule3D";

	public Type Type => typeof(IModule3D);

	public Color Color => Color.magenta;

	public string InKnobTex => "Textures/In_Knob.png";

	public string OutKnobTex => "Textures/Out_Knob.png";
}
