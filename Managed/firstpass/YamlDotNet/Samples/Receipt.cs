using System;

namespace YamlDotNet.Samples
{
	public class Receipt
	{
		public string receipt { get; set; }

		public DateTime date { get; set; }

		public Customer customer { get; set; }

		public Item[] items { get; set; }

		public Address bill_to { get; set; }

		public Address ship_to { get; set; }

		public string specialDelivery { get; set; }
	}
}
