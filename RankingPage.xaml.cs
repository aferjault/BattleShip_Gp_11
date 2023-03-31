using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class RankingPage : Page
    {
        public RankingPage()
        {
            this.InitializeComponent();

            

            // Créer un objet DataContractSerializer pour sérialiser la liste en JSON
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<KeyValuePair<string, int>>));

            

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
          

            // Récupérer la chaîne JSON des scores depuis les paramètres d'application
            string retrievedJson = (string)localSettings.Values["scoresList"];

            // Désérialiser la chaîne JSON en une liste de scores
            List<KeyValuePair<string, int>> retrievedList = new List<KeyValuePair<string, int>>();
            if (!string.IsNullOrEmpty(retrievedJson))
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(retrievedJson)))
                {
                    retrievedList = (List<KeyValuePair<string, int>>)serializer.ReadObject(memoryStream);
                }
            }

            // Ajouter chaque nom et score à la grille
            for (int i = 0; i < retrievedList.Count; i++)
            {

                scoresGrid.RowDefinitions.Add(new RowDefinition());

                string nomJoueur = retrievedList[i].Key;
                int score = retrievedList[i].Value;

                TextBlock nomTextBlock = new TextBlock();
                nomTextBlock.Text = nomJoueur;
                nomTextBlock.Margin = new Thickness(0, 10, 0, 10); // 10 pixels de marge en haut et en bas
                Grid.SetColumn(nomTextBlock, 0);
                Grid.SetRow(nomTextBlock, i + 1);
                scoresGrid.Children.Add(nomTextBlock);

                TextBlock scoreTextBlock = new TextBlock();
                scoreTextBlock.Text = score.ToString();
                scoreTextBlock.Margin = new Thickness(0, 10, 0, 10); // 10 pixels de marge en haut et en bas
                Grid.SetColumn(scoreTextBlock, 1);
                Grid.SetRow(scoreTextBlock, i + 1);
                scoresGrid.Children.Add(scoreTextBlock);

            }
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(MainPage));
        }



    }
}
