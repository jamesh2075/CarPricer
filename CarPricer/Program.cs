#region Instructions
/*
 * You are tasked with writing an algorithm that determines the value of a used car, 
 * given several factors.
 *
 *    AGE:    Given the number of months of how old the car is, reduce its value one-half 
 *            (0.5) percent.
 *            After 10 years, it's value cannot be reduced further by age. This is not 
 *            cumulative.
 *            
 *    MILES:    For every 1,000 miles on the car, reduce its value by one-fifth of a
 *              percent (0.2). Do not consider remaining miles. After 150,000 miles, it's 
 *              value cannot be reduced further by miles.
 *            
 *    PREVIOUS OWNER:    If the car has had more than 2 previous owners, reduce its value 
 *                       by twenty-five (25) percent. If the car has had no previous  
 *                       owners, add ten (10) percent of the FINAL car value at the end.
 *                    
 *    COLLISION:        For every reported collision the car has been in, remove two (2) 
 *                      percent of it's value up to five (5) collisions.
 *
 *    RELIABILITY:      If the Make is Toyota, add 5%.  If the Make is Ford, subtract $500.
 *
 *
 *    PROFITABILITY:    The final calculated price cannot exceed 90% of the purchase price. 
 *    
 * 
 *    Each factor should be off of the result of the previous value in the order of
 *        1. AGE
 *        2. MILES
 *        3. PREVIOUS OWNER
 *        4. COLLISION
 *        5. RELIABILITY
 *        
 *    E.g., Start with the current value of the car, then adjust for age, take that  
 *    result then adjust for miles, then collision, previous owner, and finally reliability. 
 *    Note that if previous owner, had a positive effect, then it should be applied 
 *    AFTER step 5. If a negative effect, then BEFORE step 5.
 */
#endregion

using NUnit.Framework;
using System;

/// <summary>
/// James Henry's sample code for Kevin Lacourse with Delinea
/// 3/1/2022
/// </summary>
namespace CarPricer
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        Car car1 = new Car { Make = "Ford", PurchaseValue= 35000m, AgeInMonths= 3 * 12, NumberOfCollisions=1, NumberOfMiles=50000, NumberOfPreviousOwners =1};
    //        Car car2 = new Car { Make = "Toyota", PurchaseValue= 35000m, AgeInMonths = 3 * 12, NumberOfCollisions = 1, NumberOfMiles=150000, NumberOfPreviousOwners =1};
    //        Car car3 = new Car { Make = "Tesla", PurchaseValue= 35000m, AgeInMonths = 3 * 12, NumberOfCollisions = 1, NumberOfMiles=250000, NumberOfPreviousOwners =1};
    //        Car car4 = new Car { Make = "toyota", PurchaseValue= 35000m, AgeInMonths = 3 * 12, NumberOfCollisions = 0, NumberOfMiles=250000, NumberOfPreviousOwners =1};
    //        Car car5 = new Car { Make = "Acura", PurchaseValue= 35000m, AgeInMonths = 3 * 12, NumberOfCollisions = 1, NumberOfMiles=250000, NumberOfPreviousOwners =0};
    //        Car car6 = new Car { Make = null, PurchaseValue= 80000m, AgeInMonths=8, NumberOfCollisions = 1, NumberOfMiles = 10000, NumberOfPreviousOwners =0};

    //        PriceDeterminator d = new PriceDeterminator();
    //        Console.WriteLine(d.DetermineCarPrice(car1));
    //        Console.WriteLine(d.DetermineCarPrice(car2));
    //        Console.WriteLine(d.DetermineCarPrice(car3));
    //        Console.WriteLine(d.DetermineCarPrice(car4));
    //        Console.WriteLine(d.DetermineCarPrice(car5));
    //        Console.WriteLine(d.DetermineCarPrice(car6));
    //    }
    //}

    public class Car
    {
        public decimal PurchaseValue { get; set; }
        public int AgeInMonths { get; set; }
        public int NumberOfMiles { get; set; }
        public int NumberOfPreviousOwners { get; set; }
        public int NumberOfCollisions { get; set; }
        public string Make { get; set; }
    }

    public class PriceDeterminator
    {
        public decimal DetermineCarPrice(Car car)
        {
            decimal price = car.PurchaseValue;

            var ageOfCar = car.AgeInMonths > 120 ? 120 : car.AgeInMonths;
            price -= price * ageOfCar * .005m;

            var miles = car.NumberOfMiles > 150000 ? 150000 : car.NumberOfMiles;
            price -= price * (miles/1000 * .002m);

            decimal ownerSupplementalPercent = 0;
            if (car.NumberOfPreviousOwners > 2)
                price -= price * .25m;
            else if (car.NumberOfPreviousOwners == 0)
                ownerSupplementalPercent = .10m;

            var collisions = car.NumberOfCollisions > 5 ? 5 : car.NumberOfCollisions;
            price -= price * collisions * .02m;

            if (car.Make?.ToLower() == "toyota")
                price += price * .05m;
            else if (car.Make?.ToLower() == "ford")
                price -= 500;

            price += price * ownerSupplementalPercent;

            var profitabilityLimit = 0.9m * car.PurchaseValue;
            price = price > profitabilityLimit ? profitabilityLimit : price;

            return price;
        }
    }


    [TestFixture]
    public class UnitTests
    {

        [Test]
        public void CalculateCarValue()
        {
            AssertCarValue(24813.40m, 35000m, 3 * 12, 50000, 1, 1, "Ford");
            AssertCarValue(20672.61m, 35000m, 3 * 12, 150000, 1, 1, "Toyota");
            AssertCarValue(19688.20m, 35000m, 3 * 12, 250000, 1, 1, "Tesla");
            AssertCarValue(21094.5m, 35000m, 3 * 12, 250000, 1, 0, "toyota");
            AssertCarValue(21657.02m, 35000m, 3 * 12, 250000, 0, 1, "Acura");
            AssertCarValue(72000m, 80000m, 8, 10000, 0, 1, null);
        }

        private static void AssertCarValue(decimal expectValue, decimal purchaseValue,
        int ageInMonths, int numberOfMiles, int numberOfPreviousOwners, int
        numberOfCollisions, string make)
        {
            Car car = new Car
            {
                AgeInMonths = ageInMonths,
                NumberOfCollisions = numberOfCollisions,
                NumberOfMiles = numberOfMiles,
                NumberOfPreviousOwners = numberOfPreviousOwners,
                PurchaseValue = purchaseValue,
                Make = make
            };
            PriceDeterminator priceDeterminator = new PriceDeterminator();
            var carPrice = priceDeterminator.DetermineCarPrice(car);
            Assert.AreEqual(expectValue, carPrice);
        }
    }
}
