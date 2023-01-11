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
        string fileName = @"data.txt";

        List<string> names;
        List<string> paid;
        List<string> count;
        List<string> date;

        public MainWindow()
        {
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, " \n \n \n ");
            }

            InitializeComponent();

            string[] fileContents = File.ReadAllLines(@fileName);

            names = fileContents[0].Split(';').ToList();
            paid = fileContents[1].Split(';').ToList();
            count = fileContents[2].Split(';').ToList();
            date = fileContents[3].Split(';').ToList();

            //Loads the list
            for(int i = 0; i < names.Count; i++)
            {
                if (names[0] != " " || paid[0] != " " || count[0] != " " || date[0] != " ")
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
            names.Add(NameTB.Text);
            paid.Add(PaidTB.Text);
            count.Add(CountTB.Text);
            date.Add(DateTB.Text);

            Coffee newEntry = new Coffee();
            newEntry.name = NameTB.Text;
            newEntry.paid = PaidTB.Text;
            newEntry.count = CountTB.Text;
            newEntry.date = DateTB.Text;

            File.WriteAllText(fileName, String.Join(";", names).Replace(" ;", "") + "\n" + String.Join(";", paid).Replace(" ;", "") + "\n" + String.Join(";", count).Replace(" ;", "") + "\n" + String.Join(";", date).Replace(" ;", ""));

            CoffeeList.Items.Add(newEntry);
        }
    }
}
