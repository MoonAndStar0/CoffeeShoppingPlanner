using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;

namespace CoffeeShoppingPlanner
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("de-DE");
        }
    }
}
