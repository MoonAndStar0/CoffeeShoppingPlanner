using System;
using System.IO;
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
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace CoffeeShoppingPlanner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fileName = "data.txt";

        List<string> names;
        List<string> paid;
        List<string> count;
        List<string> date;

        public MainWindow()
        {
            InitializeComponent();

            string[] fileContents = File.ReadAllLines(@fileName);

            names = fileContents[0].Split(';').ToList();
            paid = fileContents[1].Split(';').ToList();
            count = fileContents[2].Split(';').ToList();
            date = fileContents[3].Split(';').ToList();
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
            names.Add(NameTB.Text);
            paid.Add(PaidTB.Text);
            count.Add(CountTB.Text);
            date.Add(DateTB.Text);

            Coffee newEntry = new Coffee();
            newEntry.name = NameTB.Text;
            newEntry.paid = PaidTB.Text;
            newEntry.count = CountTB.Text;
            newEntry.date = DateTB.Text;

            File.WriteAllText(@fileName, String.Join(";", names) + "\n" + String.Join(";", paid) + "\n" + String.Join(";", count) + "\n" + String.Join(";", date));

            CoffeeList.Items.Add(newEntry);
        }
    }
}
