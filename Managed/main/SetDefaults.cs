public class SetDefaults
{
	public static void Initialize()
	{
		KSlider.DefaultSounds[0] = GlobalAssets.GetSound("Slider_Start");
		KSlider.DefaultSounds[1] = GlobalAssets.GetSound("Slider_Move");
		KSlider.DefaultSounds[2] = GlobalAssets.GetSound("Slider_End");
		KSlider.DefaultSounds[3] = GlobalAssets.GetSound("Slider_Boundary_Low");
		KSlider.DefaultSounds[4] = GlobalAssets.GetSound("Slider_Boundary_High");
		KScrollRect.DefaultSounds[KScrollRect.SoundType.OnMouseScroll] = GlobalAssets.GetSound("Mousewheel_Move");
		WidgetSoundPlayer.getSoundPath = GetSoundPath;
	}

	private static string GetSoundPath(string sound_name)
	{
		return GlobalAssets.GetSound(sound_name);
	}
}
