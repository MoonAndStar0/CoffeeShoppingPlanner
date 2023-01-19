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
        //Turns date into a number that can be compared to other date values to see which is higher to see the newest date
        public int getDateValue(string date)
        {
            string[] dateArray = date.Split('/');
            int[] dateInt = Array.ConvertAll(dateArray, s => int.Parse(s));

            int returnsum = 0;

            returnsum += dateInt[0] * 30; //Month
            returnsum += dateInt[1] * 1; //Day
            returnsum += dateInt[2] * 365; //year

            return returnsum;
        }

        //A method used to load the second list, where all the names are combined into one entry
        public void LoadCoffeeListSum(List<string> names, List<string> paid, List<string> count, List<string> date)
        {
            CoffeeListSum.Items.Clear();

            Dictionary<string, Coffee> sumDictionary = new Dictionary<string, Coffee>();

            for(int i = 0; i < names.Count; i++)
            {
                if (sumDictionary.ContainsKey(names[i]))
                {
                    sumDictionary[names[i]].paid = (Int32.Parse(sumDictionary[names[i]].paid) + Int32.Parse(paid[i])).ToString();
                    sumDictionary[names[i]].count = (Int32.Parse(sumDictionary[names[i]].count) + Int32.Parse(count[i])).ToString();

                    if (getDateValue(sumDictionary[names[i]].date) < getDateValue(date[i]))
                    {
                        sumDictionary[names[i]].date = date[i];
                    }
                }
                else
                {
                    Coffee newEntry = new Coffee();
                    newEntry.name = names[i];
                    newEntry.paid = paid[i];
                    newEntry.count = count[i];
                    newEntry.date = date[i];

                    sumDictionary[names[i]] = newEntry;
                }
            }

            foreach(KeyValuePair<string, Coffee> coffee in sumDictionary)
            {
                CoffeeListSum.Items.Add(coffee.Value);
            }
        }

        string fileName = @"data.txt";

        List<string> names = new List<string>();
        List<string> paid = new List<string>();
        List<string> count = new List<string>();
        List<string> date = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            /*
            //Loads an image for the program and the icon
            var uri = new Uri("pack://application:,,,/CoffeeShoppingPlanner;component/images/coffeeImage.png");
            var icon = new Uri("pack://application:,,,/CoffeeShoppingPlanner;component/images/coffeeIcon.ico");
            coffeeImage.Source = new BitmapImage(uri);
            Icon = new BitmapImage(icon);
            */

            //If the file does not exist MainWindow() will be skipped and the file be created once the user puts a entry on the list
            //You can use File.ReadAllText(fileName).ToString() == "" for when the file is completly empty, but exists, to not crash the program
            //idk how useful that will be tho.
            if (!File.Exists(fileName))
            {
                return;
            }

            //Splits the contents of data.txt into lists so they can be used as needed
            string[] fileContents = File.ReadAllLines(@fileName);

            names = fileContents[0].Split(';').ToList();
            paid = fileContents[1].Split(';').ToList();
            count = fileContents[2].Split(';').ToList();
            date = fileContents[3].Split(';').ToList();

            //Loads the list
            for (int i = 0; i < names.Count; i++)
            {
                Coffee loadEntry = new Coffee();
                loadEntry.name = names[i];
                loadEntry.paid = paid[i] + "€";
                loadEntry.count = count[i];
                loadEntry.date = date[i];

                CoffeeList.Items.Add(loadEntry);
            }

            LoadCoffeeListSum(names, paid, count, date);
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
            //Error messages based on the situation
            if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(PaidTB.Text) || string.IsNullOrWhiteSpace(CountTB.Text) || string.IsNullOrWhiteSpace(DateTB.Text))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus", "Fehlermeldung");
                return;
            }
            else if (NameTB.Text.Contains(";"))
            {
                MessageBox.Show("\";\" nicht erlaubt", "Fehlermeldung");
                return;
            }
            else if (CountTB.Text.Contains(",") || CountTB.Text.Contains("."))
            {
                MessageBox.Show("\"Anzahl\" darf kein \",\" oder \".\" enthalten", "Fehlermeldung");
                return;
            }
            else if (PaidTB.Text.StartsWith(",") || PaidTB.Text.StartsWith(".") || PaidTB.Text.EndsWith(",") || PaidTB.Text.EndsWith("."))
            {
                MessageBox.Show("\"Bezahlt\" kann nicht mit \",\" oder \".\" anfangen oder enden", "Fehlermeldung");
                return;
            }

            //If nothing is wrong, the entries will be added to the lists and the Datagrid and added to the file
            names.Add(NameTB.Text.Trim());
            paid.Add(PaidTB.Text.Trim());
            count.Add(CountTB.Text.Trim());
            date.Add(DateTB.Text.Trim());

            Coffee newEntry = new Coffee();
            newEntry.name = NameTB.Text;
            newEntry.paid = PaidTB.Text.Replace(",", ".") + "€";
            newEntry.count = CountTB.Text;
            newEntry.date = DateTB.Text;

            var nl = Environment.NewLine;
            File.WriteAllText(fileName, String.Join(";", names) + nl + String.Join(";", paid) + nl + String.Join(";", count) + nl + String.Join(";", date));

            CoffeeList.Items.Add(newEntry);

            LoadCoffeeListSum(names, paid, count, date);
        }

        // This Codes makes it so that you can only input numbers into Paid and Count
        private void CountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        // allow all digits 0 to 9 plus the dot the comma and the minus sign
        private static readonly Regex rgx = new Regex("[0-9.,]+");
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
            //File.Delete(fileName);
        }
    }
}
