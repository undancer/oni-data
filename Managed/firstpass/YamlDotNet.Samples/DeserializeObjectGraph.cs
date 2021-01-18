using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Samples
{
	public class DeserializeObjectGraph
	{
		public class Order
		{
			public string Receipt
			{
				get;
				set;
			}

			public DateTime Date
			{
				get;
				set;
			}

			public Customer Customer
			{
				get;
				set;
			}

			public List<OrderItem> Items
			{
				get;
				set;
			}

			[YamlMember(Alias = "bill-to", ApplyNamingConventions = false)]
			public Address BillTo
			{
				get;
				set;
			}

			[YamlMember(Alias = "ship-to", ApplyNamingConventions = false)]
			public Address ShipTo
			{
				get;
				set;
			}

			public string SpecialDelivery
			{
				get;
				set;
			}
		}

		public class Customer
		{
			public string Given
			{
				get;
				set;
			}

			public string Family
			{
				get;
				set;
			}
		}

		public class OrderItem
		{
			[YamlMember(Alias = "part_no", ApplyNamingConventions = false)]
			public string PartNo
			{
				get;
				set;
			}

			public string Descrip
			{
				get;
				set;
			}

			public decimal Price
			{
				get;
				set;
			}

			public int Quantity
			{
				get;
				set;
			}
		}

		public class Address
		{
			public string Street
			{
				get;
				set;
			}

			public string City
			{
				get;
				set;
			}

			public string State
			{
				get;
				set;
			}
		}

		private readonly ITestOutputHelper output;

		private const string Document = "---\n            receipt:    Oz-Ware Purchase Invoice\n            date:        2007-08-06\n            customer:\n                given:   Dorothy\n                family:  Gale\n\n            items:\n                - part_no:   A4786\n                  descrip:   Water Bucket (Filled)\n                  price:     1.47\n                  quantity:  4\n\n                - part_no:   E1628\n                  descrip:   High Heeled \"Ruby\" Slippers\n                  price:     100.27\n                  quantity:  1\n\n            bill-to:  &id001\n                street: |-\n                        123 Tornado Alley\n                        Suite 16\n                city:   East Westville\n                state:  KS\n\n            ship-to:  *id001\n\n            specialDelivery: >\n                Follow the Yellow Brick\n                Road to the Emerald City.\n                Pay no attention to the\n                man behind the curtain.\n...";

		public DeserializeObjectGraph(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Sample(Title = "Deserializing an object graph", Description = "Shows how to convert a YAML document to an object graph.")]
		public void Main()
		{
			StringReader input = new StringReader("---\n            receipt:    Oz-Ware Purchase Invoice\n            date:        2007-08-06\n            customer:\n                given:   Dorothy\n                family:  Gale\n\n            items:\n                - part_no:   A4786\n                  descrip:   Water Bucket (Filled)\n                  price:     1.47\n                  quantity:  4\n\n                - part_no:   E1628\n                  descrip:   High Heeled \"Ruby\" Slippers\n                  price:     100.27\n                  quantity:  1\n\n            bill-to:  &id001\n                street: |-\n                        123 Tornado Alley\n                        Suite 16\n                city:   East Westville\n                state:  KS\n\n            ship-to:  *id001\n\n            specialDelivery: >\n                Follow the Yellow Brick\n                Road to the Emerald City.\n                Pay no attention to the\n                man behind the curtain.\n...");
			Order order = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build().Deserialize<Order>(input);
			output.WriteLine("Order");
			output.WriteLine("-----");
			output.WriteLine();
			foreach (OrderItem item in order.Items)
			{
				output.WriteLine("{0}\t{1}\t{2}\t{3}", item.PartNo, item.Quantity, item.Price, item.Descrip);
			}
			output.WriteLine();
			output.WriteLine("Shipping");
			output.WriteLine("--------");
			output.WriteLine();
			output.WriteLine(order.ShipTo.Street);
			output.WriteLine(order.ShipTo.City);
			output.WriteLine(order.ShipTo.State);
			output.WriteLine();
			output.WriteLine("Billing");
			output.WriteLine("-------");
			output.WriteLine();
			if (order.BillTo == order.ShipTo)
			{
				output.WriteLine("*same as shipping address*");
			}
			else
			{
				output.WriteLine(order.ShipTo.Street);
				output.WriteLine(order.ShipTo.City);
				output.WriteLine(order.ShipTo.State);
			}
			output.WriteLine();
			output.WriteLine("Delivery instructions");
			output.WriteLine("---------------------");
			output.WriteLine();
			output.WriteLine(order.SpecialDelivery);
		}
	}
}
