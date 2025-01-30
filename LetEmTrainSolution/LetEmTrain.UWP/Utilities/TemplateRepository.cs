using LetEmTrain.UWP.ViewModels;
using System.Collections.ObjectModel;

namespace LetEmTrain.UWP.Utilities
{
    public static class TemplateRepository
    {
        public static ObservableCollection<CustomTemplate> AllTemplates { get; } = new ObservableCollection<CustomTemplate>();

        // Lista de templates adicionados pelo usuário
        public static ObservableCollection<CustomTemplate> UserAddedTemplates { get; } = new ObservableCollection<CustomTemplate>();
    }
}
