using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.UWP.ViewModels
{
    public class ExerciseSetViewModel : BindableBase
    {
        public ObservableCollection<ExerciseSet> ExerciseSets { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; }

        private string _reps {  get; set; }
        public string Reps
        {
            get { return _reps; }
            set { _reps = value; }
        }

        private string _sets {  set; get; }
        public string Sets
        {
            get { return _sets; }
            set { _sets = value; }
        }

        public ExerciseSetViewModel()
        {
            ExerciseSets = new ObservableCollection<ExerciseSet>()
            {

            };
        }

        public async Task LoadAllFromWorkoutPlanAsync()
        {
            WorkoutPlan = App.WorkoutPlanViewModel.SelectedWorkoutPlan;
            if (WorkoutPlan.Id != 0)
            {
                using (var uow = new UnitOfWork())
                {
                    var exercise_sets = await uow.ExerciseSetRepository.FindWorkoutPlanAllAsync(WorkoutPlan.Id);

                    ExerciseSets.Clear();
                    foreach (var exercise_set in exercise_sets)
                    {
                        var exercise = await uow.ExerciseRepository.FindByIdAsync(exercise_set.ExerciseId);
                        exercise_set.Exercise = exercise;
                        ExerciseSets.Add(exercise_set);
                        
                    }
                }
            }
        }

        public async Task DeleteExerciseSets(int id)
        {
            using (var uow = new UnitOfWork())
            {
                var existing_sets = await uow.ExerciseSetRepository.FindWorkoutPlanAllAsync(id);

                foreach (var set in existing_sets)
                {
                    uow.ExerciseSetRepository.Delete(set);
                }
                await uow.SaveAsync();
            }
        }



        // Deleting all previous exercise sets of the workout plan and replace them with new ones 
        public async Task SaveExerciseSets()
        {
            WorkoutPlan = App.WorkoutPlanViewModel.SelectedWorkoutPlan;
            await DeleteExerciseSets(WorkoutPlan.Id);

            using (var uow = new UnitOfWork())
            { 
                foreach (var set in ExerciseSets)
                {
                    ExerciseSet exerciseSet = new ExerciseSet();
                    exerciseSet.Reps = set.Reps;
                    exerciseSet.Sets = set.Sets;
                    exerciseSet.ExerciseId = set.ExerciseId;
                    exerciseSet.WorkoutPlanId = set.WorkoutPlanId;
                    uow.ExerciseSetRepository.Create(exerciseSet);

                }
                await uow.SaveAsync();
                ExerciseSets.Clear();
            }
        }


    }
}
