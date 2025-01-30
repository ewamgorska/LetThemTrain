using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using LetEmTrain.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain
{


    internal class BodyScoreCounter
    {

    public static async Task CountAsync(User user)
    {
            using (var uow = new UnitOfWork())
            {
                
                Progress recentProgress = await uow.ProgressRepository.FindLastAsync(user.Id);
                if (recentProgress == null || recentProgress.Weight == 0)
                {
                    Console.WriteLine("Add your one-rep-max lifts and current weight first to calculate your Body Score\n");
                    throw new ArgumentNullException();
                }

                Console.WriteLine($"user id: {user.Id}, recentProgress: {recentProgress}");
                    float benchRatio, squatRatio, deadliftRatio;
                    string benchLvl, squatLvl, deadliftLvl;
                    float benchScore, deadliftScore, squatScore;

                    benchRatio = recentProgress.MaxBench / recentProgress.Weight;
                    squatRatio = recentProgress.MaxSquat / recentProgress.Weight;
                    deadliftRatio = recentProgress.MaxDeadlift / recentProgress.Weight;


                    if (user.Gender == 'm')
                    {
                        if (benchRatio < 0.6) benchLvl = "begginer";
                        else if (benchRatio < 1.4) benchLvl = "intermediate";
                        else if (benchRatio < 2.4) benchLvl = "advanced";
                        else benchLvl = "elite"; // max = 3.5

                        if (squatRatio < 0.8) squatLvl = "begginer";
                        else if (squatRatio < 1.8) squatLvl = "intermediate";
                        else if (squatRatio < 2.9) squatLvl = "advanced";
                        else squatLvl = "elite"; //max = 4.2

                        if (deadliftRatio < 1.0) deadliftLvl = "begginer";
                        else if (deadliftRatio < 2.1) deadliftLvl = "intermediate";
                        else if (deadliftRatio < 3.3) deadliftLvl = "advanced";
                        else deadliftLvl = "elite"; // max = 4.5

                        switch (benchLvl)
                        {
                            case "begginer":
                                benchScore = (float)(0.2 + benchRatio * 0.3);
                                break;
                            case "intermediate":
                                benchScore = (float)(0.4 + (benchRatio - 0.6) * 0.25);
                                break;
                            case "advanced":
                                benchScore = (float)(0.6 + (benchRatio - 1.4) * 0.2);
                                break;
                            default:
                                benchScore = (float)(0.8 + (benchRatio - 2.4) * 0.18);
                                break;
                        }

                        switch (squatLvl)
                        {
                            case "begginer":
                                squatScore = (float)(0.2 + squatRatio * 0.25);
                                break;
                            case "intermediate":
                                squatScore = (float)(0.4 + (squatRatio - 0.8) * 0.2);
                                break;
                            case "advanced":
                                squatScore = (float)(0.6 + (squatRatio - 1.8) * 0.18);
                                break;
                            default:
                                squatScore = (float)(0.8 + (squatRatio - 2.9) * 0.15);
                                break;
                        }

                        switch (deadliftLvl)
                        {
                            case "begginer":
                                deadliftScore = (float)(0.2 + deadliftRatio * 0.2);
                                break;
                            case "intermediate":
                                deadliftScore = (float)(0.4 + (deadliftRatio - 1.0) * 0.18);
                                break;
                            case "advanced":
                                deadliftScore = (float)(0.6 + (deadliftRatio - 2.1) * 0.17);
                                break;
                            default:
                                deadliftScore = (float)(0.8 + (deadliftRatio - 3.3) * 0.17);
                                break;
                        }
                    }

                    // GENDER = FEMALE
                    else
                    {
                        if (benchRatio < 0.4) benchLvl = "begginer";
                        else if (benchRatio < 1.0) benchLvl = "intermediate";
                        else if (benchRatio < 1.7) benchLvl = "advanced";
                        else benchLvl = "elite"; // max = 2.5

                        if (squatRatio < 0.6) squatLvl = "begginer";
                        else if (squatRatio < 1.3) squatLvl = "intermediate";
                        else if (squatRatio < 2.3) squatLvl = "advanced";
                        else squatLvl = "elite"; //max = 3.5

                        if (deadliftRatio < 0.7) deadliftLvl = "begginer";
                        else if (deadliftRatio < 1.6) deadliftLvl = "intermediate";
                        else if (deadliftRatio < 2.7) deadliftLvl = "advanced";
                        else deadliftLvl = "elite"; // max = 4.0

                        switch (benchLvl)
                        {
                            case "begginer":
                                benchScore = (float)(0.2 + benchRatio * 0.5);
                                break;
                            case "intermediate":
                                benchScore = (float)(0.4 + (benchRatio - 0.4) * 0.3);
                                break;
                            case "advanced":
                                benchScore = (float)(0.6 + (benchRatio - 1.0) * 0.28);
                                break;
                            default:
                                benchScore = (float)(0.8 + (benchRatio - 1.7) * 0.25);
                                break;
                        }

                        switch (squatLvl)
                        {
                            case "begginer":
                                squatScore = (float)(0.2 + squatRatio * 0.3);
                                break;
                            case "intermediate":
                                squatScore = (float)(0.4 + (squatRatio - 0.6) * 0.28);
                                break;
                            case "advanced":
                                squatScore = (float)(0.6 + (squatRatio - 1.3) * 0.2);
                                break;
                            default:
                                squatScore = (float)(0.8 + (squatRatio - 2.3) * 0.17);
                                break;
                        }

                        switch (deadliftLvl)
                        {
                            case "begginer":
                                deadliftScore = (float)(0.2 + deadliftRatio * 0.28);
                                break;
                            case "intermediate":
                                deadliftScore = (float)(0.4 + (deadliftRatio - 0.7) * 0.22);
                                break;
                            case "advanced":
                                deadliftScore = (float)(0.6 + (deadliftRatio - 1.6) * 0.18);
                                break;
                            default:
                                deadliftScore = (float)(0.8 + (deadliftRatio - 2.7) * 0.15);
                                break;
                        }

                    }

                    float chestScore = (float)(100 * (benchScore * 0.9 + deadliftScore * 0.1));
                    float legsScore = (float)(100 * (benchScore * 0.05 + squatScore * 0.55 + deadliftScore * 0.4));
                    float coreScore = (float)(100 * (benchScore * 0.05 + squatScore * 0.4 + deadliftScore * 0.55));
                    float armsScore = (float)(100 * (benchScore * 0.65 + squatScore * 0.05 + deadliftScore * 0.3));
                    float backScore = (float)(100 * (benchScore * 0.05 + squatScore * 0.2 + deadliftScore * 0.75));
                    float glutesScore = (float)(100 * (benchScore * 0.05 + squatScore * 0.65 + deadliftScore * 0.3));
                    float bodyScore = (float)(chestScore * 0.1 + legsScore * 0.25 + coreScore * 0.15 + armsScore * 0.2 + backScore * 0.2 + glutesScore * 0.10);

                    Console.WriteLine($"Your overall bodyscore is: {bodyScore:F1} \n - Chest Score: {chestScore:F1}\n - Legs Score: {legsScore:F1}\n" +
                        $" - Core Score: {coreScore:F1}\n - Arms Score: {armsScore:F1}\n - Back Score: {backScore:F1}\n - Glutes Score: {glutesScore:F1}\n" +
                        $"\nYou bench: {benchRatio:F2} of your bodyweight, your level: {benchLvl}\n" +
                        $"You squat: {squatRatio:F2} of your bodyweight, your level: {squatLvl}\n" +
                        $"You deadlift: {deadliftRatio:F2} of your bodyweight, your level: {deadliftLvl}");
             
  
            }

        }
    }
}
