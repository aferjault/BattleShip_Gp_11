using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Battleship_project
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class RulesPage : Page
    {
        public RulesPage()
        {
            this.InitializeComponent();
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(MainPage));
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (Facile.IsChecked == true || Intermediaire.IsChecked == true || Difficile.IsChecked == true) frame.Navigate(typeof(GamePage)); ;
        }
    }
}
