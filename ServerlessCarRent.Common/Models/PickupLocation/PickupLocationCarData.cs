using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;

namespace ServerlessCarRent.Common.Models.PickupLocation
{
	public class PickupLocationCarData
	{
		public string Plate { get; set; }
		public string Model { get; set; }
		public CarState Status { get; set; }
		public CarRentalState RentalStatus { get; set; }
	}
}