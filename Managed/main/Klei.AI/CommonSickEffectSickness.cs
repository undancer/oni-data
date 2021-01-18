using UnityEngine;

namespace Klei.AI
{
	public class CommonSickEffectSickness : Sickness.SicknessComponent
	{
		public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("contaminated_crew_fx_kanim", go.transform.GetPosition() + new Vector3(0f, 0f, -0.1f), go.transform, update_looping_sounds_position: true);
			kBatchedAnimController.Play("fx_loop", KAnim.PlayMode.Loop);
			return kBatchedAnimController;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
			KAnimControllerBase kAnimControllerBase = (KAnimControllerBase)instance_data;
			kAnimControllerBase.gameObject.DeleteObject();
		}
	}
}
