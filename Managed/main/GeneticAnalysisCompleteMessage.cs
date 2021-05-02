using KSerialization;
using STRINGS;

public class GeneticAnalysisCompleteMessage : Message
{
	[Serialize]
	public Tag subSpeciesID;

	public GeneticAnalysisCompleteMessage()
	{
	}

	public GeneticAnalysisCompleteMessage(Tag subSpeciesID)
	{
		this.subSpeciesID = subSpeciesID;
	}

	public override string GetSound()
	{
		return "";
	}

	public override string GetMessageBody()
	{
		PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo = PlantSubSpeciesCatalog.instance.FindSubSpecies(subSpeciesID);
		return MISC.NOTIFICATIONS.GENETICANALYSISCOMPLETE.MESSAGEBODY.Replace("{Plant}", subSpeciesInfo.speciesID.ProperName()).Replace("{Subspecies}", subSpeciesInfo.GetNameWithMutations(subSpeciesInfo.speciesID.ProperName(), identified: true, cleanOriginal: false)).Replace("{Info}", subSpeciesInfo.GetMutationsTooltip());
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.GENETICANALYSISCOMPLETE.NAME;
	}

	public override string GetTooltip()
	{
		PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo = PlantSubSpeciesCatalog.instance.FindSubSpecies(subSpeciesID);
		return MISC.NOTIFICATIONS.GENETICANALYSISCOMPLETE.TOOLTIP.Replace("{Plant}", subSpeciesInfo.speciesID.ProperName());
	}

	public override bool IsValid()
	{
		return subSpeciesID.IsValid;
	}
}
