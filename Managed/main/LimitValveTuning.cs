public class LimitValveTuning
{
	public const float MAX_LIMIT = 500f;

	public const float DEFAULT_LIMIT = 100f;

	public static NonLinearSlider.Range[] GetDefaultSlider()
	{
		return new NonLinearSlider.Range[2]
		{
			new NonLinearSlider.Range(70f, 100f),
			new NonLinearSlider.Range(30f, 500f)
		};
	}
}
