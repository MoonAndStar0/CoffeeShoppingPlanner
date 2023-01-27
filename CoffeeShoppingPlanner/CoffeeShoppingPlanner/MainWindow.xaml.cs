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
using static System.Net.Mime.MediaTypeNames;

namespace CoffeeShoppingPlanner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void WriteEntry(string name, string paid, string count, string date, DataGrid list)
        {
            Coffee newEntry = new Coffee();

            newEntry.name = name;
            newEntry.paid = paid;
            newEntry.count = count;
            newEntry.date = date;

            list.Items.Add(newEntry);
        }

        public void CalculateNextBuyer()
        {
            Dictionary<string, Coffee> sumDictionary = LoadCoffeeSumList(names, paid, count, date);
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
        public Dictionary<string, Coffee> LoadCoffeeSumList(List<string> names, List<string> paid, List<string> count, List<string> date)
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

            NameTB.Items.Clear();
            foreach(KeyValuePair<string, Coffee> coffee in sumDictionary)
            {
                WriteEntry(coffee.Value.name, coffee.Value.paid + "€", coffee.Value.count, coffee.Value.date, CoffeeListSum);
                NameTB.Items.Add(coffee.Value.name);
            }

            return sumDictionary;
        }

        string fileName = @"data.txt";

        List<string> names = new List<string>();
        List<string> paid = new List<string>();
        List<string> count = new List<string>();
        List<string> date = new List<string>();

        string nl = Environment.NewLine;

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

            try
            {
                names = fileContents[0].Split(';').ToList();
                paid = fileContents[1].Split(';').ToList();
                count = fileContents[2].Split(';').ToList();
                date = fileContents[3].Split(';').ToList();

                //Loads the list
                for (int i = 0; i < names.Count; i++)
                {
                    WriteEntry(names[i], paid[i] + "€", count[i], date[i], CoffeeList);
                }
            }
            catch
            {
                string currentTime = DateTime.Now.ToString();
                string crptdFile = @"error_" + currentTime.Replace('.', '_').Replace(' ', '_').Replace(':', '_') + ".txt";
                string crptdFileBackup = @"error_backup_" + currentTime.Replace('.', '_').Replace(' ', '_').Replace(':', '_') + ".txt";
                File.Create(crptdFile).Close();
                File.Replace(fileName, crptdFile, crptdFileBackup);
                names.Clear();
                paid.Clear();
                count.Clear();
                date.Clear();
                CoffeeList.Items.Clear();
                MessageBox.Show("Es gab einen Fehler mit Ihrer Datei" + nl + "Dateiinhalt in " + crptdFile, "Fehlermeldung");
            }
            
            LoadCoffeeSumList(names, paid, count, date);
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
            if (ErrorMessages())
            {
                return;
            }
            
            //If nothing is wrong, the entries will be added to the lists and the Datagrid and added to the file
            names.Add(NameTB.Text.Trim());
            paid.Add(PaidTB.Text.Trim().Replace(".", ","));
            count.Add(CountTB.Text.Trim());
            date.Add(DateTB.SelectedDate.Value.ToShortDateString());

            WriteEntry(NameTB.Text, PaidTB.Text.Replace(".", ",") + "€", CountTB.Text, DateTB.Text, CoffeeList);

            File.WriteAllText(fileName, String.Join(";", names) + nl + String.Join(";", paid) + nl + String.Join(";", count) + nl + String.Join(";", date));

            LoadCoffeeSumList(names, paid, count, date);
            CalculateNextBuyer();
        }
        
        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var selectedItem = CoffeeList.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Kein Eintrag ausgewählt", "Fehlermeldung");
                return;
            }

            int indexOfSelectedItem = CoffeeList.Items.IndexOf(CoffeeList.SelectedItem);

            CoffeeList.Items.Remove(selectedItem);

            names.RemoveAt(indexOfSelectedItem);
            paid.RemoveAt(indexOfSelectedItem);
            count.RemoveAt(indexOfSelectedItem);
            date.RemoveAt(indexOfSelectedItem);

            File.WriteAllText(fileName, String.Join(";", names) + nl + String.Join(";", paid) + nl + String.Join(";", count) + nl + String.Join(";", date));

            LoadCoffeeSumList(names, paid, count, date);
            CalculateNextBuyer();
        }
        
        private void EditButton_Clicked(object sender, EventArgs e)
        {
            var selectedItem = CoffeeList.SelectedItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Kein Eintrag ausgewählt", "Fehlermeldung");
                return;
            }

            if (ErrorMessages())
            {
                return;
            }

            int indexOfSelectedItem = CoffeeList.Items.IndexOf(CoffeeList.SelectedItem);

            names[indexOfSelectedItem] = NameTB.Text.Trim();
            paid[indexOfSelectedItem] = PaidTB.Text.Trim().Replace(".", ",");
            count[indexOfSelectedItem] = CountTB.Text.Trim();
            date[indexOfSelectedItem] = DateTB.Text.Trim();
            CoffeeList.Items.Clear();
            
            for (int i = 0; i < names.Count; i++)
            {
                WriteEntry(names[i], paid[i] + "€", count[i], date[i], CoffeeList);
            }

            File.WriteAllText(fileName, String.Join(";", names) + nl + String.Join(";", paid) + nl + String.Join(";", count) + nl + String.Join(";", date));

            LoadCoffeeSumList(names, paid, count, date);
            CalculateNextBuyer();
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

        private void PaidTB_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string str = PaidTB.Text;
            int len = PaidTB.Text.Length;
        }

        private void NameDropDownSelectionChanged(object sender, RoutedEventArgs e)
        {
            if(NameTB.SelectedItem == null)
            {
                return;
            }

            NameTB.Text = NameTB.SelectedItem.ToString();
        }

        public void CoffeeListDoubleClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = CoffeeList.SelectedItem;

            int indexOfSelectedItem = CoffeeList.Items.IndexOf(selectedItem);

            NameTB.Text = names[indexOfSelectedItem];
            PaidTB.Text = paid[indexOfSelectedItem];
            CountTB.Text = count[indexOfSelectedItem];
            DateTB.Text = date[indexOfSelectedItem];
        }

        private bool ErrorMessages()
        {
            //Error messages based on the situation
            if (string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(PaidTB.Text) || string.IsNullOrWhiteSpace(CountTB.Text) || string.IsNullOrWhiteSpace(DateTB.Text))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus", "Fehlermeldung");
                return true;
            }
            else if (NameTB.Text.Contains(";"))
            {
                MessageBox.Show("\";\" nicht erlaubt", "Fehlermeldung");
                return true;
            }
            else if (CountTB.Text.Contains(",") || CountTB.Text.Contains("."))
            {
                MessageBox.Show("\"Anzahl\" darf kein \",\" oder \".\" enthalten", "Fehlermeldung");
                return true;
            }
            else if (PaidTB.Text.StartsWith(",") || PaidTB.Text.StartsWith(".") || PaidTB.Text.EndsWith(",") || PaidTB.Text.EndsWith("."))
            {
                MessageBox.Show("\"Bezahlt\" kann nicht mit \",\" oder \".\" anfangen oder enden", "Fehlermeldung");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
