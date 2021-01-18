namespace Database
{
	public class RoomTypeCategories : ResourceSet<RoomTypeCategory>
	{
		public RoomTypeCategory None;

		public RoomTypeCategory Food;

		public RoomTypeCategory Sleep;

		public RoomTypeCategory Recreation;

		public RoomTypeCategory Bathroom;

		public RoomTypeCategory Hospital;

		public RoomTypeCategory Industrial;

		public RoomTypeCategory Agricultural;

		public RoomTypeCategory Park;

		private RoomTypeCategory Add(string id, string name, string colorName)
		{
			RoomTypeCategory roomTypeCategory = new RoomTypeCategory(id, name, colorName);
			Add(roomTypeCategory);
			return roomTypeCategory;
		}

		public RoomTypeCategories(ResourceSet parent)
			: base("RoomTypeCategories", parent)
		{
			Initialize();
			None = Add("None", "", "roomNone");
			Food = Add("Food", "", "roomFood");
			Sleep = Add("Sleep", "", "roomSleep");
			Recreation = Add("Recreation", "", "roomRecreation");
			Bathroom = Add("Bathroom", "", "roomBathroom");
			Hospital = Add("Hospital", "", "roomHospital");
			Industrial = Add("Industrial", "", "roomIndustrial");
			Agricultural = Add("Agricultural", "", "roomAgricultural");
			Park = Add("Park", "", "roomPark");
		}
	}
}
