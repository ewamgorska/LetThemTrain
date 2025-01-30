using System;
using System.Collections.Generic;
using System.Text;

using LetEmTrain.Domain.SeedWork;

namespace LetEmTrain.Domain.Models
{
    public class User : Entity
    {

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public double Height { get; set; }
        public char Gender { get; set; }
        public int Age { get; set; }
        public int ActivityLevel { get; set; }
        public DateTime MemberSince { get; set; }
        public int FitnessGoals { get; set; }
        public string DietType { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool IsActive { get; set; } = true;

        public List<WorkoutPlan> WorkoutPlans {  get; set; } = new List<WorkoutPlan>();
        public List<Progress> ProgressList { get; set; } = new List<Progress>();

        //public User() { ProfilePicturePath = "ms-appx:///Assets/default_user.jpg"; }
        public User(string name, string email,string password, double height, char gender,
            int age, int actlvl, int goals, string diet)
        {
            this.Username = name;
            this.Email = email;
            this.Password = password;
            this.Height = height;   
            this.Gender = gender;
            this.Age = age;
            this.ActivityLevel = actlvl;
            this.MemberSince = DateTime.Now;
            this.FitnessGoals = goals;
            this.DietType = diet;
        }

        public User()
        {
        }
    }
}
