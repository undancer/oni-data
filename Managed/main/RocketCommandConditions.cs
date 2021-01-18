public class RocketCommandConditions : KMonoBehaviour
{
	public ConditionDestinationReachable reachable;

	public ConditionHasAstronaut hasAstronaut;

	public ConditionPassengersOnBoard passengersOnBoard;

	public ConditionNoExtraPassengers noExtraPassengers;

	public ConditionHasAtmoSuit hasSuit;

	public CargoBayIsEmpty cargoEmpty;

	public ConditionHasMinimumMass destHasResources;

	public ConditionAllModulesComplete allModulesComplete;

	public ConditionHasEngine hasEngine;

	public ConditionHasNosecone hasNosecone;

	public ConditionFlightPathIsClear flightPathIsClear;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RocketModule component = GetComponent<RocketModule>();
		reachable = (ConditionDestinationReachable)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionDestinationReachable(GetComponent<RocketModule>()));
		allModulesComplete = (ConditionAllModulesComplete)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionAllModulesComplete(GetComponent<LaunchableRocket>()));
		if (GetComponent<LaunchableRocket>().registerType == LaunchableRocket.RegisterType.Spacecraft)
		{
			destHasResources = (ConditionHasMinimumMass)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionHasMinimumMass(GetComponent<CommandModule>()));
			hasAstronaut = (ConditionHasAstronaut)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionHasAstronaut(GetComponent<CommandModule>()));
			hasSuit = (ConditionHasAtmoSuit)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionHasAtmoSuit(GetComponent<CommandModule>()));
			cargoEmpty = (CargoBayIsEmpty)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new CargoBayIsEmpty(GetComponent<CommandModule>()));
		}
		else if (GetComponent<LaunchableRocket>().registerType == LaunchableRocket.RegisterType.Clustercraft)
		{
			hasEngine = (ConditionHasEngine)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionHasEngine(GetComponent<LaunchableRocket>()));
			hasNosecone = (ConditionHasNosecone)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionHasNosecone(GetComponent<LaunchableRocket>()));
			passengersOnBoard = (ConditionPassengersOnBoard)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketBoard, new ConditionPassengersOnBoard(GetComponent<PassengerRocketModule>()));
			noExtraPassengers = (ConditionNoExtraPassengers)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketBoard, new ConditionNoExtraPassengers(GetComponent<PassengerRocketModule>()));
		}
		flightPathIsClear = (ConditionFlightPathIsClear)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketFlight, new ConditionFlightPathIsClear(base.gameObject, 1));
	}
}
