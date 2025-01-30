using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LetEmTrain.UWP.ViewModels
{
    public class ExerciseViewModel : BindableBase
    {
        public ObservableCollection<Exercise> Exercises { get; set; }
        public ObservableCollection<string> MainBodyParts { get; set; }

        private Exercise _selectedExercise;
        public Exercise SelectedExercise
        {
            get => _selectedExercise;
            set => Set(ref _selectedExercise, value);
        }

        private Exercise _exercise;
        public Exercise Exercise
        {
            get { return _exercise; }
            set
            {
                _exercise = value;
            }
        }

        private string _name;
        public string Name {
            get { return _name; }
            set 
            { 
                _name = value;
            } 
        }
     
        private byte[] _image;
        public byte[] Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }
        private string _mainBodyPart;
        public string MainBodyPart 
        {
            get { return _mainBodyPart; }
            set
            {
                _mainBodyPart = value;
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
            }
        }


        public ExerciseViewModel()
        {
            Exercises = new ObservableCollection<Exercise>();
            Exercise = new Exercise();
            MainBodyParts = new ObservableCollection<string>
            {
                "All",          
                "Chest",
                "Back",
                "Arms",
                "Legs",
                "Core",
                "Glutes"
            };

        }

        public async void LoadAllAsync()
        {
            using (var uow = new UnitOfWork())
            {
                var exercise_list = await uow.ExerciseRepository.FindAllAsync();
                Exercises.Clear();
                foreach (var ex in exercise_list)
                {
                    Exercises.Add(ex);
                }
            }
        }


        public async Task SearchExercisesAsync(string name)
        {
            using (var uow = new UnitOfWork())
            {
                var exercises = await uow.ExerciseRepository.SearchExercisesByNameAsync(name);

                Exercises.Clear();
                foreach (var exercise in exercises)
                {
                    Exercises.Add(exercise);
                }
            }
        }


        public async Task FilterExercisesByMuscleGroupAsync(string bodyPart)
        {
            using (var uow = new UnitOfWork())
            {
                List<Exercise> exercises;

                if (string.IsNullOrWhiteSpace(bodyPart) || bodyPart == "All")
                {
                    exercises = await uow.ExerciseRepository.FindAllAsync();
                }
                else
                {
                    exercises = await uow.ExerciseRepository.SearchExercisesByMuscleGroupAsync(bodyPart);
                }

                Exercises.Clear();
                foreach (var exercise in exercises)
                {
                    Exercises.Add(exercise);
                }
            }
        }
    }
}
