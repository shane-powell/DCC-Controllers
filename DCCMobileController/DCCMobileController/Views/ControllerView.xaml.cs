using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCCMobileController.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ControllerView : ViewCell
    {
        public ControllerView()
        {

                this.InitializeComponent();
        }

        private void BtnEdit_OnClicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushAsync(
                new DecoderEditorPage() { BindingContext = this.BindingContext });
        }
    }
}