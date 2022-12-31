using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Models.PickupLocation
{
	public class PickupLocationData
	{
		public string City { get; set; }
		public string Location { get; set; }
		public PickupLocationState Status { get; set; }
		public List<PickupLocationCarData> Cars { get; set; }
	}
}
