using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Battleship_project
{
    public class Cellule:Button
    {
        public Brush _couleur;

        public Brush Couleur
        {
            get { return _couleur; }
            set
            {
                _couleur = value;
                Background = _couleur;
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public bool IsBoat { get; set; }
        public bool IsHit { get; set; }
       

        //Constructeur de la cellule qui lui atribue la fonction Cell_Trapped
        public Cellule(int x, int y)
        {
            X = x;
            Y = y;
            Couleur = new SolidColorBrush(Colors.Transparent);
            Width = Height = 40;
            Margin = new Thickness(0.5);
            BorderThickness = new Thickness(0.5);
            BorderBrush = new SolidColorBrush(Colors.Black);
            IsHit= false;
            Click += Cell_Tapped;

        }


        // Permet de tir sur des cases et d'y changer la couleur en fonction de la cellule 
        private void Cell_Tapped(object sender, RoutedEventArgs e)
        {
            //
            Windows.Storage.ApplicationDataContainer stopint = Windows.Storage.ApplicationData.Current.LocalSettings;
            int shoot =(int) stopint.Values["int"];

        
            // Mode De Tire pour le GamePage
            if (shoot==1)
            {
                Cellule cell = sender as Cellule;

                // Vérifier si la cellule est occupée par un bateau
                if (cell.IsHit)
                {
                    // La cellule est déjà touchée, rien à faire
                    return;
                }
                else if (cell.IsBoat)
                {
                    // La cellule est occupée par un bateau
                    cell.Couleur = new SolidColorBrush(Windows.UI.Colors.Red);
                    cell.IsHit = true;
                    Save.Score= Save.Score + 2 +2*Save.Niveau;
                    shoot = 2;
                    stopint.Values["int"] = shoot;
                    
                }
                else
                {
                    // La cellule est vide
                    cell.Couleur = new SolidColorBrush(Windows.UI.Colors.Blue);
                    cell.IsHit = true;
                    shoot = 2;
                    stopint.Values["int"] = shoot;
                }
            }

          
            //Mode pour placer les bateaux pour pose PoseBateau
            
            if (shoot == 0)
            {
                if (IsHit == false & Save.PlacerBateau)
                {
                    // La cellule est vide
                    Couleur = new SolidColorBrush(Windows.UI.Colors.Blue);
                    IsHit = true;
                    Save.PlacerBateau = false;
                }
                else if (IsHit == true)
                {
                    Couleur = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    IsHit = false;
                    Save.PlacerBateau = true;
                }
            }
        }
        

    }
}
