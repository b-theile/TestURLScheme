using System.Collections.Generic;
using System.Collections.ObjectModel;
using TestURLScheme.ViewModels;
using Xamarin.Forms;

namespace TestURLScheme.Views
{
    public partial class SingleStartPage : ContentPage
    {
        public SingleStartPage()
        {
            InitializeComponent();
            BindingContext = new SingleStartViewModel();
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
                UpdateSelectedReqMeasurements(e.CurrentSelection);
            else
                UpdateSelectedReqMeasurements(null);
        }

        private void UpdateSelectedReqMeasurements(IEnumerable<object> list)
        {
            if(BindingContext is SingleStartViewModel viewModel)
            {
                viewModel.SelectedReqMeasurements = list == null ? null : new ObservableCollection<object>(list);
            }
        }

        private void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new PcToInstrumentPage())
;        }
    }
}
