using System.Collections.ObjectModel;
using TimelinePulse.Services;
using Xamarin.Forms;

namespace TimelinePulse.ViewModels
{
    public class TimelinePulseViewModel : BindableObject
    {
        private ObservableCollection<Models.Task> _tasks;

        public TimelinePulseViewModel()
        {
            Tasks = new ObservableCollection<Models.Task>();

            LoadData();
        }

        public ObservableCollection<Models.Task> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged();
            }
        }

        private void LoadData()
        {
            var tasks = TaskService.Instance.GetTasks();

            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }
    }
}