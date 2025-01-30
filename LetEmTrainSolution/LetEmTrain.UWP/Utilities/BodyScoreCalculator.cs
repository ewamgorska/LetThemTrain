using LetEmTrain.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.UWP.Utilities
{
    public class BodyScoreCalculator
    {
        public static string CalculateBodyScore(User user, Progress recentProgress)
        {
            if (recentProgress == null || recentProgress.Weight == 0)
            {
                throw new ArgumentNullException("Add your one-rep-max lifts and current weight first to calculate your Body Score");
            }

            float benchRatio, squatRatio, deadliftRatio;
            string benchLvl, squatLvl, deadliftLvl;
            float benchScore, deadliftScore, squatScore;

            benchRatio = recentProgress.MaxBench / recentProgress.Weight;
            squatRatio = recentProgress.MaxSquat / recentProgress.Weight;
            deadliftRatio = recentProgress.MaxDeadlift / recentProgress.Weight;

            if (user.Gender == 'm')
            {
                if (benchRatio < 0.6) benchLvl = "beginner";
                else if (benchRatio < 1.4) benchLvl = "intermediate";
                else if (benchRatio < 2.4) benchLvl = "advanced";
                else benchLvl = "elite";

                if (squatRatio < 0.8) squatLvl = "beginner";
                else if (squatRatio < 1.8) squatLvl = "intermediate";
                else if (squatRatio < 2.9) squatLvl = "advanced";
                else squatLvl = "elite";

                if (deadliftRatio < 1.0) deadliftLvl = "beginner";
                else if (deadliftRatio < 2.1) deadliftLvl = "intermediate";
                else if (deadliftRatio < 3.3) deadliftLvl = "advanced";
                else deadliftLvl = "elite";

                switch (benchLvl)
                {
                    case "beginner":
                        benchScore = 0.2f + benchRatio * 0.3f;
                        break;
                    case "intermediate":
                        benchScore = 0.4f + (benchRatio - 0.6f) * 0.25f;
                        break;
                    case "advanced":
                        benchScore = 0.6f + (benchRatio - 1.4f) * 0.2f;
                        break;
                    default:
                        benchScore = 0.8f + (benchRatio - 2.4f) * 0.18f;
                        break;
                }

                switch (squatLvl)
                {
                    case "beginner":
                        squatScore = 0.2f + squatRatio * 0.25f;
                        break;
                    case "intermediate":
                        squatScore = 0.4f + (squatRatio - 0.8f) * 0.2f;
                        break;
                    case "advanced":
                        squatScore = 0.6f + (squatRatio - 1.8f) * 0.18f;
                        break;
                    default:
                        squatScore = 0.8f + (squatRatio - 2.9f) * 0.15f;
                        break;
                }

                switch (deadliftLvl)
                {
                    case "beginner":
                        deadliftScore = 0.2f + deadliftRatio * 0.2f;
                        break;
                    case "intermediate":
                        deadliftScore = 0.4f + (deadliftRatio - 1.0f) * 0.18f;
                        break;
                    case "advanced":
                        deadliftScore = 0.6f + (deadliftRatio - 2.1f) * 0.17f;
                        break;
                    default:
                        deadliftScore = 0.8f + (deadliftRatio - 3.3f) * 0.17f;
                        break;
                }
            }
            else
            {
                if (benchRatio < 0.4) benchLvl = "beginner";
                else if (benchRatio < 1.0) benchLvl = "intermediate";
                else if (benchRatio < 1.7) benchLvl = "advanced";
                else benchLvl = "elite";

                if (squatRatio < 0.6) squatLvl = "beginner";
                else if (squatRatio < 1.3) squatLvl = "intermediate";
                else if (squatRatio < 2.3) squatLvl = "advanced";
                else squatLvl = "elite";

                if (deadliftRatio < 0.7) deadliftLvl = "beginner";
                else if (deadliftRatio < 1.6) deadliftLvl = "intermediate";
                else if (deadliftRatio < 2.7) deadliftLvl = "advanced";
                else deadliftLvl = "elite";

                switch (benchLvl)
                {
                    case "beginner":
                        benchScore = 0.2f + benchRatio * 0.5f;
                        break;
                    case "intermediate":
                        benchScore = 0.4f + (benchRatio - 0.4f) * 0.3f;
                        break;
                    case "advanced":
                        benchScore = 0.6f + (benchRatio - 1.0f) * 0.28f;
                        break;
                    default:
                        benchScore = 0.8f + (benchRatio - 1.7f) * 0.25f;
                        break;
                }

                switch (squatLvl)
                {
                    case "beginner":
                        squatScore = 0.2f + squatRatio * 0.3f;
                        break;
                    case "intermediate":
                        squatScore = 0.4f + (squatRatio - 0.6f) * 0.28f;
                        break;
                    case "advanced":
                        squatScore = 0.6f + (squatRatio - 1.3f) * 0.2f;
                        break;
                    default:
                        squatScore = 0.8f + (squatRatio - 2.3f) * 0.17f;
                        break;
                }

                switch (deadliftLvl)
                {
                    case "beginner":
                        deadliftScore = 0.2f + deadliftRatio * 0.28f;
                        break;
                    case "intermediate":
                        deadliftScore = 0.4f + (deadliftRatio - 0.7f) * 0.22f;
                        break;
                    case "advanced":
                        deadliftScore = 0.6f + (deadliftRatio - 1.6f) * 0.18f;
                        break;
                    default:
                        deadliftScore = 0.8f + (deadliftRatio - 2.7f) * 0.15f;
                        break;
                }
            }

            float chestScore = 100 * (benchScore * 0.9f + deadliftScore * 0.1f);
            float legsScore = 100 * (benchScore * 0.05f + squatScore * 0.55f + deadliftScore * 0.4f);
            float coreScore = 100 * (benchScore * 0.05f + squatScore * 0.4f + deadliftScore * 0.55f);
            float armsScore = 100 * (benchScore * 0.65f + squatScore * 0.05f + deadliftScore * 0.3f);
            float backScore = 100 * (benchScore * 0.05f + squatScore * 0.2f + deadliftScore * 0.75f);
            float glutesScore = 100 * (benchScore * 0.05f + squatScore * 0.65f + deadliftScore * 0.3f);

            if (chestScore > 100) chestScore = 100;
            if (legsScore > 100) legsScore = 100;
            if (coreScore > 100) coreScore = 100;
            if (armsScore > 100) armsScore = 100;
            if (backScore > 100) backScore = 100;
            if (glutesScore > 100) glutesScore = 100;

            App.ProgressViewModel.ChestScore = chestScore;
            App.ProgressViewModel.LegsScore = legsScore;
            App.ProgressViewModel.CoreScore = coreScore;
            App.ProgressViewModel.ArmsScore = armsScore;
            App.ProgressViewModel.BackScore = backScore;

            float bodyScore = chestScore * 0.1f + legsScore * 0.25f + coreScore * 0.15f + armsScore * 0.2f + backScore * 0.2f + glutesScore * 0.1f;
            if (bodyScore > 100) bodyScore = 100;

            return $"Your overall bodyscore is: {bodyScore:F1} \n" +
                   $"Chest Score: {chestScore:F1}\n" +
                   $"Legs Score: {legsScore:F1}\n" +
                   $"Core Score: {coreScore:F1}\n" +
                   $"Arms Score: {armsScore:F1}\n" +
                   $"Back Score: {backScore:F1}\n\n" +
                   $"You bench: {benchRatio:F2} of your bodyweight, your level: {benchLvl}\n" +
                   $"You squat: {squatRatio:F2} of your bodyweight, your level: {squatLvl}\n" +
                   $"You deadlift: {deadliftRatio:F2} of your bodyweight, your level: {deadliftLvl}";
        }


    }

}
