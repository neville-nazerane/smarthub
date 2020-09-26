using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class DevicesViewModel : BindableObject
    {
        private string _batman = "Batman";

        public string Batman
        {
            get => _batman; set
            {
                _batman = value;
                OnPropertyChanged();
            }
        }
        public ICommand Changed => new Command(() => Batman = "Superman");

    }
}
