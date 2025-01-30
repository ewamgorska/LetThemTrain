using LetEmTrain.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.ConsoleApp
{
    public class KcalCalculator
    {
        public static void CalculateMakros(User u, Progress p)
        {
            if (u == null) throw new ArgumentNullException();
            if(p == null)
            {
                Console.WriteLine("There is no weight records to calculate the needed calories\n");
                throw new ArgumentNullException();
            }

            // Calculate BMR using the Mifflin-St Jeor Equation
            double bmr = u.Gender == 'm'
            ? 10 * p.Weight + 6.25 * u.Height - 5 * u.Age + 5  // Male formula
            : 10 * p.Weight + 6.25 * u.Height - 5 * u.Age - 161; // Female formula

            double activityMultiplier = u.ActivityLevel switch
            {
                1 => 1.1,   // Sedentary
                2 => 1.275, // Lightly
                3 => 1.45,  // Moderately active
                4 => 1.625, // Very active
                5 => 1.8,   // Extra active
                _ => throw new ArgumentException("Invalid activity level\n")
            };

            double goalKcal = u.FitnessGoals switch
            {
                1 => -500,   // Sedentary
                2 => -300, // Lightly
                3 => 0,  // Moderately active
                4 => 300, // Very active
                5 => 500,   // Extra active
                _ => throw new ArgumentException("Invalid activity level")
            };

            if(u.Gender == 'm') goalKcal *= 1.2;

            double CaloriesNeeded = bmr * activityMultiplier + goalKcal;
            int protein = 0;
            int carbs = 0;
            int fats = 0;

            switch(u.DietType)
            {
                case "keto":
                    protein = (int)((CaloriesNeeded * 0.20) / 4);
                    carbs = (int)((CaloriesNeeded * 0.05) / 4);
                    fats = (int)((CaloriesNeeded * 0.75) / 9);
                    break;
                case "standard":
                    protein= (int)((CaloriesNeeded * 0.20) / 4);
                    carbs = (int)((CaloriesNeeded * 0.55) / 4);
                    fats = (int)((CaloriesNeeded * 0.25) / 9);
                    break;
                case "high-protein":
                    protein = (int)((CaloriesNeeded * 0.40) / 4);
                    carbs = (int)((CaloriesNeeded * 0.35) / 4);
                    fats = (int)((CaloriesNeeded * 0.25) / 9);
                    break;
            }

            
            if (protein > 150)
            {
                int over = protein - 150;
                protein -= (int)(0.3 * over);
                carbs += (int)(0.3 * over);
            }

            Console.WriteLine("You should eat (" + (int)(CaloriesNeeded * 0.95) + " - " + (int)(CaloriesNeeded * 1.05) + ") Kcal daily, including:");
            Console.WriteLine(" -  (" + (int)(protein * 0.95) + " - " + (int)(protein * 1.05) + ") g of protein");
            Console.WriteLine(" -  (" + (int)(carbs * 0.95) + " - "+ (int)(carbs * 1.05) + ") g of carbs");
            Console.WriteLine(" -  (" + (int)(fats * 0.95) + " - " + (int)(fats * 1.05) + ") g of fat");
        }

    }
}
