using System.Collections.ObjectModel;
using System.Windows;
using MultipleItemSelectorCustomControl.Model;
using MultipleItemSelectorCustomControl.ViewModel;

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
            LoadItems();
            //Tags = new ObservableCollection<string> { "Test1", "Test2", "Test3", "Test4", "Test5" };
        }

        public ObservableCollection<ItemViewModel> Tags { get; set; }
        public ObservableCollection<ItemViewModel> Suggestions { get; set; }
        public string Suggestion { get; set; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void  LoadItems()
        {
            // In a real app this method would access a database.
            //var items= new Item
            //{
            //    Name = "CEO",
            //    Id = 1,
            //    Children =
            //    {
            //        new Item
            //        {
            //            Name="Software Support",
            //            Id = 2,
            //        },
            //        new Item
            //        {
            //            Name="Director HR/Finance",
            //            Id = 3,
            //            Children=
            //            {
            //                new Item
            //                {
            //                    Name="Reconcillar",
            //                    Id = 4,
            //                },
            //                new Item
            //                {
            //                    Name="HR Manager",
            //                    Id = 5,
            //                }
            //            }
            //        },
            //        new Item
            //        {
            //            Name="Director Of Sales",
            //            Id = 6,
            //            Children=
            //            {
            //                new Item
            //                {
            //                    Name="Regional Manager NE",
            //                    Id = 7,
            //                },
            //                new Item
            //                {
            //                    Name="Regional Manager MidWest",
            //                    Id = 8,
            //                    Children=
            //                    {
            //                        new Item
            //                        {
            //                            Name="Store Manager",
            //                            Id = 9,
            //                            Children =
            //                                {
            //                                    new Item
            //                                    {
            //                                        Name="Sales Representative"
            //                                    },
            //                                    new Item
            //                                    {
            //                                        Name="Store Clerk"
            //                                    }
            //                                }
            //                        },
            //                    }
            //                },
            //                new Item
            //                {
            //                    Name="Regional Manager SE",
            //                    Id = 10,
            //                },
            //            }
            //        },
            //        new Item
            //          {
            //              Name = "Director Of Operations",
            //              Id = 11,
            //              Children = 
            //              {
            //                  new Item
            //                         {
            //                             Name = "Inventory Manager",
            //                             Id = 12,
            //                             Children =
            //                                 {
            //                                     new Item
            //                                     {
            //                                         Name = "Service Manager",
            //                                         Id = 13,
            //                                         Children=
            //                                             {
            //                                                 new Item
            //                                                 {
            //                                                     Name = "Service Tech",
            //                                                     Id = 14,
            //                                                 }
            //                                             }
            //                                     }
            //                                 }
            //                         }
            //              }
            //          }

            //    }
            //};

            var item1 = new Item
                        {
                            Name = "CEO",
                            Id = 1,
                        };
            var item2 = new Item
            {
                Name = "Service Manager",
                Id = 2,
            };
            var rootItem1 = new ItemViewModel(item1);
            var rootItem2 = new ItemViewModel(item2);
            Suggestions = new ObservableCollection<ItemViewModel>();
            Suggestions.Add(rootItem1);
            Suggestions.Add(rootItem2);
        }
    }
}
