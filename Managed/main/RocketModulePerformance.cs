using System;

[Serializable]
public class RocketModulePerformance
{
	public float burden;

	public float fuelKilogramPerDistance;

	public float enginePower;

	public float Burden => burden;

	public float FuelKilogramPerDistance => fuelKilogramPerDistance;

	public float EnginePower => enginePower;

	public RocketModulePerformance(float burden, float fuelKilogramPerDistance, float enginePower)
	{
		this.burden = burden;
		this.fuelKilogramPerDistance = fuelKilogramPerDistance;
		this.enginePower = enginePower;
	}
}
