using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoffeeShoppingPlanner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string fileName = "data.txt";
            string[] fileContents = File.ReadAllLines(@fileName);

            List<string> names = fileContents[0].Split(';').ToList();
            List<string> paid = fileContents[1].Split(';').ToList();
            List<string> count = fileContents[2].Split(';').ToList();
            List<string> date = fileContents[3].Split(';').ToList();
            
            InitializeComponent();
        }
    
        public class Coffee
        {
            public string name { get; set; }
            public string paid { get; set; }
            public string count { get; set; }
            public string date { get; set; }
        }
    
        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            Coffee newEntry = new Coffee();
            newEntry.name = NameTB.Text;
            newEntry.paid = PaidTB.Text;
            newEntry.count = CountTB.Text;
            newEntry.date = DateTB.Text;

            CoffeeList.Items.Add(newEntry);
        }
    }
}
