using SmartHub.MobileApp.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Xamarin.Forms;

namespace SmartHub.MobileApp.Components
{
    public class ExpandableCollection : ContentView
    {

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate),
                                                                                               typeof(DataTemplate),
                                                                                               typeof(ExpandableCollection),
                                                                                               propertyChanged: ItemTemplateSet);

        public static readonly BindableProperty LoadControlProperty = BindableProperty.Create(nameof(LoadControl),
                                                                                              typeof(CollectionLoadControl),
                                                                                              typeof(ExpandableCollection),
                                                                                              propertyChanged: LoadControlSet);

        public DataTemplate ItemTemplate 
        { 
            get => (DataTemplate) GetValue(ItemTemplateProperty); 
            set => SetValue(ItemTemplateProperty, value); 
        }
        
        public CollectionLoadControl LoadControl 
        { 
            get => (CollectionLoadControl) GetValue(LoadControlProperty); 
            set => SetValue(LoadControlProperty, value); }

        private readonly CollectionView _collectionView;
        private readonly ActivityIndicator _loadingComponent;

        public ExpandableCollection()
        {
            _loadingComponent = new ActivityIndicator
            {
                IsEnabled = true,
                IsVisible = true,
                IsRunning = true
            };
            LoadControl = new CollectionLoadControl();
            _collectionView = new CollectionView();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            SetUpLoadControl(LoadControl);
        }

        void SetUpLoadControl(LoadControl newValue, LoadControl oldValue = null)
        {
            if (oldValue is not null)
                oldValue.PropertyChanged -= LoadControl_PropertyChanged;

            if (newValue is null) return;
            if (Parent is null)
                newValue.PropertyChanged -= LoadControl_PropertyChanged;
            else
                newValue.PropertyChanged += LoadControl_PropertyChanged;
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
            if (LoadControl?.Items is null)
            {
                _collectionView.ItemsSource = null;
                Content = null;
                return;
            }
            var source = new ObservableCollection<object>();
            _collectionView.ItemsSource = source;
            Content = _collectionView;
            foreach (var item in LoadControl.Items) source.Add(item);
        }

        private static void LoadControlSet(BindableObject bindable, object oldValue, object newValue)
        {
            ((ExpandableCollection)bindable).SetUpLoadControl(newValue as LoadControl, oldValue as LoadControl);
        }

        private static void ItemTemplateSet(BindableObject bindable, object oldValue, object newValue)
        {
            ((ExpandableCollection)bindable)._collectionView.ItemTemplate = (DataTemplate) newValue;
        }

    }
}
