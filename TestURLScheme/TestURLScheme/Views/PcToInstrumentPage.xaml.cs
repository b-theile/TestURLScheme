using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestURLScheme.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestURLScheme.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PcToInstrumentPage : ContentPage
    {
        public PcToInstrumentPage()
        {
            InitializeComponent();
            this.BindingContext = new PcToInstrumentViewModel();
        }
    }
}