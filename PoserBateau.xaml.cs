using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static System.Net.WebRequestMethods;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Battleship_project
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    
    public sealed partial class PoserBateau : Page
    {
        private bool isHorizontal = false;
        private bool[] buttonsWasClicked = { false,false,false,false,false };
        private bool button3WasClicked = false;


        public PoserBateau()
        {
            this.InitializeComponent();
            
            Windows.Storage.ApplicationDataContainer stopint = Windows.Storage.ApplicationData.Current.LocalSettings;
            stopint.Values["int"] = 0;
            Windows.Storage.ApplicationDataContainer posebateauint = Windows.Storage.ApplicationData.Current.LocalSettings;
            posebateauint.Values["int"] = 0;
            Save.PlacerBateau = true;

            //Crée la grille
            for (int i = 0; i < 10; i++)
            {
                grillePose.RowDefinitions.Add(new RowDefinition());
                grillePose.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < 10; j++)
                {
                    Cellule cellule = new Cellule(i, j);
                    Grid.SetRow(cellule, i);
                    Grid.SetColumn(cellule, j);
                    grillePose.Children.Add(cellule);
                }
            }

        }

        //Enregistre les placements des cellules bateau
        public bool StockData(int bateauLongueur, bool isHorizontal)
        {
            List<Cellule> temp = new List<Cellule>(); //Garde les cellules d'un meme bateau 
            foreach (Cellule cell in grillePose.Children)
            {
                if (cell.IsHit && !Save.PoseCell.Contains(cell))
                {
                    Cellule cellule = (Cellule)grillePose.Children
                      .Cast<FrameworkElement>()
                     .FirstOrDefault(e => Grid.GetRow(e) == cell.Y && Grid.GetColumn(e) == cell.X);

                    for (int i = 0; i < bateauLongueur; i++)
                    {
                        int row = cellule.Y + (isHorizontal ? 0 : i);
                        int col = cellule.X + (isHorizontal ? i : 0);

                        // Vérifier si la cellule est déjà occupée
                        Cellule celltest = grillePose.Children
                            .Cast<Cellule>()
                            .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col && !Save.PoseCell.Contains(e));

                        if (celltest == null)
                        {
                            // Une cellule est déjà occupée, on doit recommencer la génération
                            ShowMessageDialog("", "Le placement du bateau sort de la grille ou deborde sur un bateau . Veuillez recommencer.");
                            return false;
                        }
                        
                       
                        temp.Add(celltest);
                    }
                    if (temp.Count == bateauLongueur)
                    {
                        // Toutes les cellules sont libres, on occupe les cellules et on sort de la boucle
                        foreach (Cellule cellStock in temp)
                        {
                            cellStock.Couleur = new SolidColorBrush(Windows.UI.Colors.Gray);
                            cellStock.IsBoat = true;
                            Save.PoseCell.Add(cellStock);

                        }
                        ShowMessageDialog("", "Le placement du bateau est bon");
                        return true;

                    }
                }
            }
            return false;
        }

        //Permet d'afficher des messages 
        private async void ShowMessageDialog(string title, string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message, title);
            await dialog.ShowAsync();
        }

        
        private void Jeu(object sender, RoutedEventArgs e)
        {
            
            int myInt = Convert.ToInt32(((Button)sender).Tag);
            if (buttonsWasClicked[2] && !button3WasClicked)
            {
                bool passe = StockData(myInt, isHorizontal);
                if (passe)
                {
                    button3WasClicked = true;
                    Save.PlacerBateau = true;
                }
                
            }
            if (!buttonsWasClicked[myInt-1])
            {
                bool passe=StockData(myInt,isHorizontal);
                if (passe)
                {
                    Save.PlacerBateau = true;
                    buttonsWasClicked[myInt - 1] = true;
                }
                
            }
        }

        private void Suivant(object sender, RoutedEventArgs e)
        {
            if (Save.PoseCell.Count > 0)
            {
                frame.Navigate(typeof(GamePage));
            }
        }
        
        //Gérer si le bateau doit être posé verticalement ou horizontalement
        private void Switch(object sender, RoutedEventArgs e)
        {
           
            Button clickedButton = sender as Button;
            string etat = (string)clickedButton.Content;
            if (etat == "Vertical")
            {
                isHorizontal = true;
                etat = "Horizontal";
                clickedButton.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                isHorizontal= false;
                etat = "Vertical";
                clickedButton.Background = new SolidColorBrush(Windows.UI.Colors.Green);
            }
            clickedButton.Content = etat;
        }
    }
}
