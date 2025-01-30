using LetEmTrain.Domain.Repositories;
using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetEmTrain.Domain.Models
{
    public class Progress : Entity
    {
        public DateTime Date { get; set; }
        public float Weight { get; set; }
        public float MaxBench { get; set; }
        public float MaxSquat { get; set; }
        public float MaxDeadlift { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public Progress() { }
        public Progress(int userId, float bench, float squat, float deadlift, float weight)
        {
            this.UserId = userId;
            this.Weight = weight;
            this.MaxBench = bench;
            this.MaxSquat = squat;
            this.MaxDeadlift = deadlift;
            this.Date = DateTime.Now;

        }

        public override string ToString()
        {

            return $"Date: {Date} \nMax bench: {MaxBench} kg \nMax Squat: {MaxSquat} kg \nMax Deadlift: {MaxDeadlift} kg \nYour current weight: {Weight} kg";
        }
    }
}
