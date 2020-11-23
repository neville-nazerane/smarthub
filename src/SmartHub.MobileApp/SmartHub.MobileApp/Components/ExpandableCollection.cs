using SmartHub.MobileApp.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace SmartHub.MobileApp.Components
{
    public class ExpandableCollection : ContentView
    {

        //public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource),
        //                                                                                     typeof(IEnumerable<object>),
        //                                                                                     typeof(ExpandableCollection));

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate),
                                                                                               typeof(DataTemplate),
                                                                                               typeof(ExpandableCollection));


        //public IEnumerable<object> ItemsSource
        //{
        //    get => (IEnumerable<object>)GetValue(ItemsSourceProperty);
        //    set => SetValue(ItemsSourceProperty, value);
        //}

        public DataTemplate ItemTemplate 
        { 
            get => (DataTemplate) GetValue(ItemTemplateProperty); 
            set => SetValue(ItemTemplateProperty, value); 
        }

        private CollectionView collectionView;
        private readonly ActivityIndicator _loadingComponent;
        private readonly CollectionLoadControl _loadControl;

        public ExpandableCollection()
        {
            _loadingComponent = new ActivityIndicator
            {
                IsEnabled = true,
                IsVisible = true,
                IsRunning = true
            };
            _loadControl = new CollectionLoadControl();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent is null)
                _loadControl.PropertyChanged -= LoadControl_PropertyChanged;
            else
                _loadControl.PropertyChanged += LoadControl_PropertyChanged;

        }

        private void LoadControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CollectionLoadControl.IsLoading):
                    Content = _loadingComponent;
                    break;
                case nameof(CollectionLoadControl.Items):
                    SetupItems();
                    break;
            }
        }

        private void SetupItems()
        {
            if (_loadControl?.Items is null)
            {
                Content = null;
                return;
            }
            collectionView = new CollectionView();
            var source = new ObservableCollection<object>();
            collectionView.ItemsSource = source;
            Content = collectionView;
            foreach (var item in _loadControl.Items) source.Add(item);
        }

    }
}
