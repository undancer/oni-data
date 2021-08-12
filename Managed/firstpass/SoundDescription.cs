using FMOD.Studio;

public struct SoundDescription
{
	public struct Parameter
	{
		public HashedString name;

		public PARAMETER_ID id;

		public static readonly PARAMETER_ID INVALID_ID;
	}

	public string path;

	public float falloffDistanceSq;

	public Parameter[] parameters;

	public OneShotSoundParameterUpdater[] oneShotParameterUpdaters;

	public PARAMETER_ID GetParameterId(HashedString name)
	{
		Parameter[] array = parameters;
		for (int i = 0; i < array.Length; i++)
		{
			Parameter parameter = array[i];
			if (parameter.name == name)
			{
				return parameter.id;
			}
		}
		return Parameter.INVALID_ID;
	}
}
