using ServerlessCarRent.Common.Models.PickupLocation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Dtos
{
	public class InitializePickupLocationDto
	{
		public string City { get; set; }
		public string Location { get; set; }
		public PickupLocationStatus Status { get; set; }
	}
}
