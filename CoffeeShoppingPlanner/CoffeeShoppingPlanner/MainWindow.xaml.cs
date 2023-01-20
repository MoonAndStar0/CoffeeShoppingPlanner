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
        public void CalculateNextBuyer()
        {
            Dictionary<string, Coffee> sumDictionary = LoadCoffeeListSum(names, paid, count, date);
            float lowestPaid = float.MaxValue;
            string lowestPaidName = string.Empty;
            DateTime lowestPaidDate = DateTime.MinValue;

            foreach(KeyValuePair<string, Coffee> coffee in sumDictionary)
            {
                if(lowestPaid > float.Parse(coffee.Value.paid))
                {
                    lowestPaid = float.Parse(coffee.Value.paid);
                    lowestPaidName = coffee.Key;
                    lowestPaidDate = DateTime.ParseExact(coffee.Value.date, "dd.MM.yyyy", null);
                }
                else if(lowestPaid == float.Parse(coffee.Value.paid))
                {
                    if (lowestPaidDate > DateTime.ParseExact(coffee.Value.date, "dd.MM.yyyy", null))
                    {
                        lowestPaid = float.Parse(coffee.Value.paid);
                        lowestPaidName = coffee.Key;
                        lowestPaidDate = DateTime.ParseExact(coffee.Value.date, "dd.MM.yyyy", null);
                    }

                }
            }

            nextBuyerName.Text = lowestPaidName;
        }

        //A method used to load the second list, where all the names are combined into one entry
        //Also returns the Dictionary so you can work with it
        public Dictionary<string, Coffee> LoadCoffeeListSum(List<string> names, List<string> paid, List<string> count, List<string> date)
        {
            CoffeeListSum.Items.Clear();

            Dictionary<string, Coffee> sumDictionary = new Dictionary<string, Coffee>();

            for(int i = 0; i < names.Count; i++)
            {
                if (sumDictionary.ContainsKey(names[i]))
                {
                    sumDictionary[names[i]].paid = (float.Parse(sumDictionary[names[i]].paid) + float.Parse(paid[i])).ToString();
                    sumDictionary[names[i]].count = (int.Parse(sumDictionary[names[i]].count) + int.Parse(count[i])).ToString();

                    if (DateTime.ParseExact(sumDictionary[names[i]].date, "dd.MM.yyyy", null) < DateTime.ParseExact(date[i], "dd.MM.yyyy", null))
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

            return sumDictionary;
        }

        string fileName = @"data.txt";

        List<string> names = new List<string>();
        List<string> paid = new List<string>();
        List<string> count = new List<string>();
        List<string> date = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

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
            CalculateNextBuyer();
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
            date.Add(DateTB.SelectedDate.Value.ToShortDateString());

            Coffee newEntry = new Coffee();
            newEntry.name = NameTB.Text;
            newEntry.paid = PaidTB.Text.Replace(",", ".") + "€";
            newEntry.count = CountTB.Text;
            newEntry.date = DateTB.Text;

            var nl = Environment.NewLine;
            File.WriteAllText(fileName, String.Join(";", names) + nl + String.Join(";", paid) + nl + String.Join(";", count) + nl + String.Join(";", date));

            CoffeeList.Items.Add(newEntry);

            LoadCoffeeListSum(names, paid, count, date);
            CalculateNextBuyer();
        }
        /*
        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var selectedItem = CoffeeList.SelectedItem;
              
            if (selectedItem != null)
            {
                CoffeeList.Items.Remove(selectedItem);
            }
            LoadCoffeeListSum(names, paid, count, date);
        }
        */
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
