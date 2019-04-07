using TimelinePulse.ViewModels;
using Xamarin.Forms;

namespace TimelinePulse.Views
{
    public partial class TimelinePulseView : ContentPage
	{
		public TimelinePulseView ()
		{
			InitializeComponent ();

            BindingContext = new TimelinePulseViewModel();
		}
	}
}