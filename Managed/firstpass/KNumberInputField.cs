using UnityEngine;

public class KNumberInputField : KInputField
{
	public int decimalPlaces = -1;

	public float currentValue;

	public float minValue;

	public float maxValue;

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void SetAmount(float newValue)
	{
		newValue = Mathf.Clamp(newValue, minValue, maxValue);
		if (decimalPlaces != -1)
		{
			float num = Mathf.Pow(10f, decimalPlaces);
			newValue = Mathf.Round(newValue * num) / num;
		}
		currentValue = newValue;
		SetDisplayValue(currentValue.ToString());
	}

	protected override void ProcessInput(string input)
	{
		input = ((input == "") ? minValue.ToString() : input);
		float num = minValue;
		try
		{
			num = float.Parse(input);
			SetAmount(num);
		}
		catch
		{
		}
	}
}
