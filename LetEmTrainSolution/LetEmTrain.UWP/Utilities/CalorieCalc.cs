using LetEmTrain.UWP.ViewModels;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;

namespace LetEmTrain.UWP.Utilities
{
    public class CalorieCalc
    {
        public static string CalculateCalories(int age, float weight, int height, char gender, int activitylvl, int goals, string diet)
        {

            double bmr = gender == 'm'
            ? 10 * weight + 6.25 * height - 5 * age + 5  // Male formula
            : 10 * weight + 6.25 * height - 5 * age - 161; // Female formula
            
            double activityMultiplier;

            switch (activitylvl)
            {
                case 1:
                    activityMultiplier = 1.1;
                    break;
                case 2:
                    activityMultiplier = 1.275;
                    break;
                case 3:
                    activityMultiplier = 1.45;
                    break;
                    case 4:
                    activityMultiplier = 1.625;
                    break;
                case 5:
                    activityMultiplier = 1.8;
                    break;
                default:
                    throw new ArgumentException("Invalid activity level\n");

                            
            }

            double goalKcal;
            switch (goals)
            {
                case 1:
                    goalKcal = -500;
                    break;
                case 2:
                    goalKcal = -300;
                    break;
                case 3:
                    goalKcal = 0;
                    break;
                case 4:
                    goalKcal = 300;
                    break;
                case 5:
                    goalKcal = 500;
                    break;
                default:
                    throw new ArgumentException("Invalid fitness goals");

            }


            if (gender == 'm') goalKcal *= 1.2;

            double CaloriesNeeded = bmr * activityMultiplier + goalKcal;
            int protein = 0;
            int carbs = 0;
            int fats = 0;

            switch (diet)
            {
                case "keto":
                    protein = (int)((CaloriesNeeded * 0.20) / 4);
                    carbs = (int)((CaloriesNeeded * 0.05) / 4);
                    fats = (int)((CaloriesNeeded * 0.75) / 9);
                    break;
                case "standard":
                    protein = (int)((CaloriesNeeded * 0.20) / 4);
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

            string result = $"You should consume: {CaloriesNeeded} kcal | BMR: {bmr} kcal\n" +
                            $" Carbs: {carbs}g | Protein: {protein}g | Fats: {fats}g";
       
            return result;
        }
    }
}
