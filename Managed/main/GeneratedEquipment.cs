public class GeneratedEquipment
{
	public static void LoadGeneratedEquipment()
	{
		EquipmentConfigManager.Instance.RegisterEquipment(new EquippableBalloonConfig());
		EquipmentConfigManager.Instance.RegisterEquipment(new AtmoSuitConfig());
		EquipmentConfigManager.Instance.RegisterEquipment(new JetSuitConfig());
		EquipmentConfigManager.Instance.RegisterEquipment(new LeadSuitConfig());
		EquipmentConfigManager.Instance.RegisterEquipment(new OxygenMaskConfig());
		EquipmentConfigManager.Instance.RegisterEquipment(new WarmVestConfig());
		EquipmentConfigManager.Instance.RegisterEquipment(new CoolVestConfig());
		EquipmentConfigManager.Instance.RegisterEquipment(new FunkyVestConfig());
	}
}
