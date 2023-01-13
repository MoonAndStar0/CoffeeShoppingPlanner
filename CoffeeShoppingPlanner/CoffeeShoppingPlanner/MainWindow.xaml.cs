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
using System.Text.RegularExpressions;

namespace CoffeeShoppingPlanner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fileName = @"data.txt";

        List<string> names = new List<string>();
        List<string> paid = new List<string>();
        List<string> count = new List<string>();
        List<string> date = new List<string>();

        public MainWindow()
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            InitializeComponent();

            string[] fileContents = File.ReadAllLines(@fileName);

            names = fileContents[0].Split(';').ToList();
            paid = fileContents[1].Split(';').ToList();
            count = fileContents[2].Split(';').ToList();
            date = fileContents[3].Split(';').ToList();

            //Loads the list
            {
                for (int i = 0; i < names.Count; i++)
                {
                    Coffee loadEntry = new Coffee();
                    loadEntry.name = names[i];
                    loadEntry.paid = paid[i];
                    loadEntry.count = count[i];
                    loadEntry.date = date[i];

                    CoffeeList.Items.Add(loadEntry);
                }
            }
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
            if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(PaidTB.Text) || string.IsNullOrWhiteSpace(CountTB.Text) || string.IsNullOrWhiteSpace(DateTB.Text))
            {
                return;
            }

            names?.Add(NameTB.Text.Trim());
            paid?.Add(PaidTB.Text.Trim());
            count?.Add(CountTB.Text.Trim());
            date?.Add(DateTB.Text.Trim());
            
            Coffee newEntry = new Coffee();
            newEntry.name = NameTB.Text;
            newEntry.paid = PaidTB.Text;
            newEntry.count = CountTB.Text;
            newEntry.date = DateTB.Text;

            var nl = Environment.NewLine;
            File.WriteAllText(fileName, String.Join(";", names) + nl + String.Join(";", paid) + nl + String.Join(";", count) + nl + String.Join(";", date));
            
            CoffeeList.Items.Add(newEntry);
        }

        private void CountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        // allow all digits 0 to 9 plus the dot and the minus sign
        private static readonly Regex rgx = new Regex("[0-9.]+");
        private static bool IsTextAllowed(string text)
        {
            return rgx.IsMatch(text);
        }
        private void CountTB_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string str = CountTB.Text;
            int len = CountTB.Text.Length;
        }

        // Deletes the datagrid after closing the app(for testing)
        ~MainWindow() 
        { 
            File.Delete(fileName);
        }
    }
}
