using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LetEmTrain.UWP.ViewModels
{
    public class WorkoutPlanViewModel : BindableBase
    {
        public ObservableCollection<ExerciseSet> ExerciseSets { get; set; }
        public ObservableCollection<WorkoutPlan> WorkoutPlans { get; set; }

        public WorkoutPlanViewModel()
        {
            WorkoutPlans = new ObservableCollection<WorkoutPlan>();
            ExerciseSets = new ObservableCollection<ExerciseSet>();
        }

        private string _name;
        public string Name 
        {
            get {  return _name; }
            set { _name = value; }
        }

        private WorkoutPlan _selectedWorkoutPlan;
        public WorkoutPlan SelectedWorkoutPlan
        {
            get => _selectedWorkoutPlan;
            set => Set(ref _selectedWorkoutPlan, value);
        }

        private string _placeholder;
        public string Placeholder
        {
            get => _placeholder;
            set => Set(ref _placeholder, value);
        }


        public async void LoadAllAsync()
        {

            using (var uow = new UnitOfWork())
            {
                var userId = App.UserViewModel.LoggedUser.Id;
                var workoutplan_list = await uow.WorkoutPlanRepository.FindAllUserWorkoutPlans(userId);
                WorkoutPlans.Clear();
                foreach(var workoutplan in workoutplan_list)
                {
                    WorkoutPlans.Add(workoutplan);
                }

                if (WorkoutPlans.Count != 0)
                {
                    Placeholder = "";
                }
                else
                {
                    Placeholder = "U haven't added Workout Plan yet";
                }
            }
        }

        public async Task SearchWorkoutPlansAsync(string name)
        {
            using (var uow = new UnitOfWork())
            {
                var workouts = await uow.WorkoutPlanRepository.FindAllByNameWithTextAsync(name);

                WorkoutPlans.Clear();
                foreach (var plan in workouts)
                {
                    WorkoutPlans.Add(plan);
                }
                
            }
        }
        
        public async Task DeleteWorkoutPlanAsync(WorkoutPlan plan)
        {
            using (var uow = new UnitOfWork())
            {
                uow.WorkoutPlanRepository.Delete(plan);
                await uow.SaveAsync();
                WorkoutPlans.Remove(plan);
            }
            
        }

        public async Task CreateWorkoutPlanAsync(string name, string description)
        {
            WorkoutPlan plan = new WorkoutPlan(App.UserViewModel.LoggedUser.Id, name, description);
            using (var uow = new UnitOfWork())
            {
                uow.WorkoutPlanRepository.Create(plan);
                await uow.SaveAsync();
            }
            WorkoutPlans.Add(plan);
        }


        public async Task SaveEditedWorkoutPlan()
        {
            var name = SelectedWorkoutPlan.Name;
            var description = SelectedWorkoutPlan.Description;
            WorkoutPlan plan = new WorkoutPlan(App.UserViewModel.LoggedUser.Id, name, description);
            plan.Id = SelectedWorkoutPlan.Id;
            using (var uow = new UnitOfWork())
            {
                uow.WorkoutPlanRepository.Update(plan);
                await uow.SaveAsync();
            }
            App.ExerciseSetViewModel.ExerciseSets.Clear();
        }



    }
}
