using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using LetEmTrain.UWP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace LetEmTrain.UWP.ViewModels
{
    public class ProgressViewModel : BindableBase
    {

        public ObservableCollection<Progress> Progresses { get; set; }

        public Progress LastProgress { get; set; }

        private Progress _progress;
        public Progress Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
            }
        }

        private string _bodyScore;
        public string BodyScore
        {
            get { return _bodyScore; }
            set
            {
                _bodyScore = value;
                OnPropertyChanged(nameof(BodyScore));
            }
        }
        public float ArmsScore { get; set; }
        public float ChestScore { get; set; }
        public float BackScore { get; set; }
        public float CoreScore { get; set; }
        public float LegsScore { get; set; }
        public string ArmsImagePath { get; set; }
        public string ChestImagePath { get; set; }
        public string BackImagePath { get; set; }
        public string CoreImagePath { get; set; }
        public string LegsImagePath { get; set; }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    FilterProgressDataAsync();
                }
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    FilterProgressDataAsync();
                }
            }
        }

        public ProgressViewModel()
        {
            Progresses = new ObservableCollection<Progress>();
            ArmsImagePath = "ms-appx:///Assets/arms/arms.png";
            ChestImagePath = "ms-appx:///Assets/chest/chest.png";
            BackImagePath = "ms-appx:///Assets/back/back.png";
            CoreImagePath = "ms-appx:///Assets/core/core.png";
            LegsImagePath = "ms-appx:///Assets/legs/legs.png";
        }

        // Loading every Progress of the logged user
        public async Task LoadProgressesAsync()
        {
            using (var uow = new UnitOfWork())
            {
                var progresses = await uow.ProgressRepository.FindAllUserProgressAsync(App.UserViewModel.LoggedUser.Id);
                Progresses.Clear();
                foreach (var progress in progresses)
                {
                    Progresses.Add(progress);
                }
            }
        }

        public async Task LoadLastProgressAsync()
        {
            using (var uow = new UnitOfWork())
            {
                var progress = await uow.ProgressRepository.FindLastAsync(App.UserViewModel.LoggedUser.Id);
                LastProgress = progress;
            }
        }

        internal async Task<Progress> AddProgress(float maxbp, float maxsq, float maxdl, float weight)
        {

            Progress = new Progress();
            Progress lastProgress = null;
            using (var uow = new UnitOfWork())
            {

                if (maxbp == 0 || maxsq == 0 || maxdl == 0)
                {
                    lastProgress = await uow.ProgressRepository.FindLastAsync(App.UserViewModel.LoggedUser.Id);
                }
                if (lastProgress != null)
                {
                    if (maxbp == 0) maxbp = lastProgress.MaxBench;
                    if (maxsq == 0) maxsq = lastProgress.MaxSquat;
                    if (maxdl == 0) maxdl = lastProgress.MaxDeadlift;
                }
                Progress.Date = DateTime.Now;
                Progress.MaxBench = maxbp;
                Progress.MaxSquat = maxsq;
                Progress.MaxDeadlift = maxdl;
                Progress.UserId = App.UserViewModel.LoggedUser.Id;
                Progress.Weight = weight;
                uow.ProgressRepository.Create(Progress);
                await uow.SaveAsync();

                Progresses.Add(Progress);
            }
            return Progress;
        }

        public async void FilterProgressDataAsync()
        {
            List<Progress> filteredProgresses = new List<Progress>();

            if (StartDate.HasValue && EndDate.HasValue)
            {
                using (var uow = new UnitOfWork())
                {
                    filteredProgresses = await uow.ProgressRepository
                        .FindAllByTimePeriodAsync(App.UserViewModel.LoggedUser.Id, StartDate.Value, EndDate.Value);
                }
            }
            else
            {
                await LoadProgressesAsync();
                return;
            }

            // Czyszczenie i dodawanie przefiltrowanych danych
            Progresses.Clear();
            foreach (var progress in filteredProgresses)
            {
                Progresses.Add(progress);
            }
        }

        public async Task DeleteProgressAsync(Progress progressToDelete)
        {
            using (var uow = new UnitOfWork())
            {
                uow.ProgressRepository.Delete(progressToDelete);
                await uow.SaveAsync();
                Progresses.Remove(progressToDelete);
            }
        }

        public void RecalculateBodyScore()
        {
            try
            {
                var recentProgress = Progresses.OrderByDescending(p => p.Date).FirstOrDefault();
                if (recentProgress != null)
                {
                    BodyScore = BodyScoreCalculator.CalculateBodyScore(App.UserViewModel.LoggedUser, recentProgress);
                    ArmsImagePath = GetImagePath("arms", ArmsScore);
                    ChestImagePath = GetImagePath("chest", ChestScore);
                    BackImagePath = GetImagePath("back", BackScore);
                    CoreImagePath = GetImagePath("core", CoreScore);
                    LegsImagePath = GetImagePath("legs", LegsScore);
    }
                else
                {
                    BodyScore = "No progress data available to calculate Body Score.";
                    ArmsImagePath = "ms-appx:///Assets/arms/arms.png";
                    ChestImagePath = "ms-appx:///Assets/chest/chest.png";
                    BackImagePath = "ms-appx:///Assets/back/back.png";
                    CoreImagePath = "ms-appx:///Assets/core/core.png";
                    LegsImagePath = "ms-appx:///Assets/legs/legs.png";
                }
            }
            catch (Exception ex)
            {
                BodyScore = $"Error calculating Body Score: {ex.Message}";
            }
        }

        private string GetImagePath(string bodyPart, float score)
        {
            if (score < 40)
            {
                return $"ms-appx:///Assets/{bodyPart}/front-{bodyPart}-beginer.png";
            }
            else if (score < 60)
            {
                return $"ms-appx:///Assets/{bodyPart}/front-{bodyPart}-intermediate.png";
            }
            else if (score < 80)
            {
                return $"ms-appx:///Assets/{bodyPart}/front-{bodyPart}-advanced.png";
            }
            else
            {
                return $"ms-appx:///Assets/{bodyPart}/front-{bodyPart}-elite.png";
            }
        }


        public float CalculateOneRepMax(float lift, int reps, float bodyWeight, int type)
        {
            // 0 - bench press 1 - squat 2 - deadlift
            if (type == 0)
                lift += bodyWeight * 0.10f;
            if (type == 1)
                lift += bodyWeight * 0.35f;
            if (type == 2)
                lift += bodyWeight * 0.40f;

            // Different formulas based on reps range
            if (reps >= 1 && reps <= 5)
            {
                lift = lift * (1 + 0.025f * reps);
            }
            else if (reps >= 6 && reps <= 12)
            {
                lift = lift * 40 / (41 - reps);
            }else if(reps > 12 && reps <= 30)
            {
                lift *= (float)1.37;
                lift = lift * (1 + 0.018f * (reps - 12));
            }else if (reps > 30)
            {
                lift *= (float)1.37;
                lift = lift * (1 + (0.018f * 18) + (0.012f * (reps - 30)));
            }
            else
            {
                return 0;
            }
            if (type == 0) lift -= bodyWeight * 0.1f;
            if (type == 1) lift -= bodyWeight * 0.35f;
            if (type == 2) lift -= bodyWeight * 0.40f;
            return lift;
        }
    }
}
