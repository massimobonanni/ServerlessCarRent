using ServerlessCarRent.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Utilities
{
    internal static class MailUtilities
    {
        public static string GenerateHtmlContentForAdmin(RentalStatusChangeOrchestratorDto context)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("<html><body>");
            strBuilder.AppendLine($"<h1>Car: [{context.CarPlate}] - {context.CarModel}</h1>");
            strBuilder.AppendLine($"<p>Rental status: {context.NewRentalStatus}</p>");
            strBuilder.AppendLine($"<p>Car rented by: {context.CurrentRenter?.LastName} {context.CurrentRenter?.FirstName} ({context.CurrentRenter?.Email})</p>");
            strBuilder.AppendLine("</body></html>");

            return strBuilder.ToString();
        }
        
        public static string GenerateTextContentForAdmin(RentalStatusChangeOrchestratorDto context)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"Car: [{context.CarPlate}] - {context.CarModel}");
            strBuilder.AppendLine($"Rental status: {context.NewRentalStatus}");
            strBuilder.AppendLine($"Car rented by: {context.CurrentRenter?.LastName} {context.CurrentRenter?.FirstName} ({context.CurrentRenter?.Email})");

            return strBuilder.ToString();

        }
    }
}
