using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Battleship_project
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(RulesPage));

        }

        private void Ranking_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(RankingPage));

        }
    }
}
