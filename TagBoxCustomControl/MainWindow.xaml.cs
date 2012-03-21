using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultipleItemSelectorCustomControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //Tags = new ObservableCollection<string> { "Test1", "Test2", "Test3", "Test4", "Test5" };
            Suggestions = new ObservableCollection<string> { "Test", "Test Test", "my", "new group", "some text","Mina","Hello" };
        }

        public ObservableCollection<string> Tags { get; set; }
        public ObservableCollection<string> Suggestions { get; set; }
        public string Suggestion { get; set; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
