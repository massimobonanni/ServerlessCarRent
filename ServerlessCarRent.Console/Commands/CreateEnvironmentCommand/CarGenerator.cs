using System;
using System.Collections.Generic;

public class CarGenerator
{
    private static IDictionary<string, IEnumerable<string>> CarRepositoryBrands =
        new Dictionary<string, IEnumerable<string>>()
        {
            {"Toyota",["Corolla", "Camry", "Prius", "RAV4", "Highlander", "Tacoma", "Tundra", "Sienna", "4Runner", "Supra", "Avalon", "C-HR", "Venza"] },
            {"Ford",["F-150", "Explorer", "Mustang", "Escape", "Bronco", "Ranger", "Edge", "Expedition", "Fusion", "EcoSport", "Transit", "Maverick", "Bronco Sport"] },
            {"BMW",["3 Series", "5 Series", "X5", "X3", "1 Series", "7 Series", "4 Series", "X1", "X6", "2 Series", "i3", "Z4", "M4", "M5", "i8"] },
            {"Mercedes",["C-Class", "E-Class", "S-Class", "GLC", "GLE", "GLA", "A-Class", "CLS", "GLS", "SLC", "G-Class", "AMG GT", "B-Class", "SL", "EQC"] },
            {"Audi",["A3", "A4", "A6", "Q5", "Q7", "A5", "Q3", "A7", "Q8", "TT", "R8", "A8", "S3", "S4", "S5"] },
            {"Honda",["Civic", "Accord", "CR-V", "Fit", "HR-V", "Pilot", "Odyssey", "Ridgeline", "Passport", "Insight", "Clarity", "Civic Type R", "CR-V Hybrid"] },
            {"Hyundai",["Elantra", "Sonata", "Tucson", "Santa Fe", "Accent", "Palisade", "Kona", "Ioniq", "Veloster", "Nexo", "Venue", "Elantra GT", "Ioniq 5"] },
            {"Fiat",["500", "Panda", "Tipo", "500X", "500L", "Doblo", "Punto", "Qubo", "Fiorino", "124 Spider"] },
        };

    private static Random random = new Random();

    public Tuple<string,string> GetCar()
    {
        int carBrandIndex = random.Next(CarRepositoryBrands.Keys.Count);
        var carBrand= CarRepositoryBrands.Keys.ElementAt(carBrandIndex);
        int carModelIndex = random.Next(CarRepositoryBrands[carBrand].Count());
        var carModel = CarRepositoryBrands[carBrand].ElementAt(carModelIndex);
        return new Tuple<string, string>(carBrand, carModel);
    }

}
