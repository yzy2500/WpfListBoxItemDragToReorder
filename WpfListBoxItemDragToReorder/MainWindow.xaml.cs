using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfListBoxItemDragToReorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ObservableCollection<int> list = new ObservableCollection<int>();
        private void ListBox_Initialized(object sender, EventArgs e)
        {
            for (int i = 1; i < 100; i++) list.Add(i);
            if (sender is ListBox box) box.ItemsSource = list;
        }
    }
}
