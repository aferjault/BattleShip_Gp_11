using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Battleship_project
{
    public sealed partial class GamePage : Page
    {
        public string NomJoueur = Save.NomJoueur;
        public Cellule lastHitCell;
        public int StopShoot { get; set; }

        //Création variable Nombre de Tirs
        private int shootCounter = 0;

        //Création de variables nécessaire au chronomètre
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private bool isRunning;
        public GamePage()
        {

            this.InitializeComponent();

            //génére un bool pour éviter de tirer plein de foi d'affilé
            Windows.Storage.ApplicationDataContainer stopint = Windows.Storage.ApplicationData.Current.LocalSettings;
            stopint.Values["int"]=1;

            //Crée les grilles
            for (int i = 0; i < 10; i++)
            {
                grilleJ1.RowDefinitions.Add(new RowDefinition());
                grilleJ1.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < 10; j++)
                {
                    Cellule cellule = new Cellule(i, j);
                    Grid.SetRow(cellule, i);
                    Grid.SetColumn(cellule, j);
                    grilleJ1.Children.Add(cellule);
                }
            }
            for (int i = 0; i < 10; i++)
            {
                grilleJ2.RowDefinitions.Add(new RowDefinition());
                grilleJ2.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < 10; j++)
                {
                    Cellule cellule = new Cellule(i, j);
                    Grid.SetRow(cellule, i);
                    Grid.SetColumn(cellule, j);
                    grilleJ2.Children.Add(cellule);
                }
            }

            //Les bateaux utilisés
            Boat torpillieur = new Boat("torpillieur",2);
            Boat contretorpillieur1 = new Boat("contretorpillieur", 3);
            Boat contretorpillieur2 = new Boat("contretorpillieur", 3);
            Boat cuirasse = new Boat("cuirasse", 4);
            Boat porteAvion = new Boat("porteAvion", 5);

           
            PlaceBoat(grilleJ1);


            List<Cellule> occupiedCells = new List<Cellule>();

            

            List<Boat> ships = new List<Boat>() {
                new Boat( "Torpillieur", 2 ),
                new Boat("Contretorpillieur1", 3 ),
                new Boat("Contretorpillieur2", 3 ),
                new Boat("Cuirasse", 4 ),
                new Boat( "PorteAvion", 5 )
            };
            int nbrbtx;

            foreach (Boat ship in ships)//place un nombre de bateau en fonction de la difficulté 
            {
                if (Save.Niveau == 2)
                {
                    nbrbtx = 1;
                }
                else
                {
                    nbrbtx = Save.Niveau;
                }
                for (int i = 0; i <= (nbrbtx); i++)
                {
                    PlaceRandomly(ship, grilleJ2, occupiedCells);
                }
            }

            // Création du chronomètre

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;

            // Démarrer le chronomètre
            isRunning = true;
            timer.Start();
        }





        private void PlaceBoat(Grid grid)
        {
            for(int i = 0; i < Save.PoseCell.Count;i++)
            {
                Cellule cell = Save.PoseCell[i];
                int x = cell.X; int y = cell.Y;
                Cellule cellule = (Cellule)grid.Children
                    .Cast<FrameworkElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == x && Grid.GetColumn(e) == y);
                cellule.Couleur = new SolidColorBrush(Windows.UI.Colors.Gray);
                cellule.IsBoat = true;
            }
        }


        // Place le bateau de manière aléatoire 
        private void PlaceRandomly(Boat boat, Grid grid, List<Cellule> occupiedCells)
        {
            Random random = new Random();
            bool isHorizontal;
            
            do
            {
                // Générer une position et une orientation aléatoires
                int x = random.Next(grid.ColumnDefinitions.Count);
                int y = random.Next(grid.RowDefinitions.Count);
                isHorizontal = random.Next(2) == 0;

                // Calculer les cellules que le bateau occupera
                List<Cellule> cells = new List<Cellule>();
                for (int i = 0; i < boat.Longueur; i++)
                {
                    int row = y + (isHorizontal ? 0 : i);
                    int col = x + (isHorizontal ? i : 0);

                    // Vérifier si la cellule est déjà occupée
                    Cellule cell = grid.Children
                        .Cast<Cellule>()
                        .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col && !occupiedCells.Contains(e));

                    if (cell == null)
                    {
                        // Une cellule est déjà occupée, on doit recommencer la génération
                        break;
                    }
                    else
                    {
                        cells.Add(cell);
                    }
                }

                if (cells.Count == boat.Longueur)
                {
                    // Toutes les cellules sont libres, on occupe les cellules et on sort de la boucle
                    foreach (Cellule cell in cells)
                    {
                        cell.Couleur = new SolidColorBrush(Windows.UI.Colors.Transparent);
                        cell.IsBoat=true;
                        occupiedCells.Add(cell);
                        
                    }
                    break;
                }
            } while (true);
        }
        

        //verifie que toute les cellule boat d'un des grilles sont détruite si oui fin de jeu part affichage d'un message
        private bool ToutCellDetruite()
        {
           

            int cont1 = 0;
            int cont2 = 0;
            // Vérifie si chaque cellule est occupée dans la grille J2
            foreach (Cellule cell in grilleJ2.Children)
            {
                if (!cell.IsHit&cell.IsBoat)
                {
                    cont2 += 1;
                }
            }
            //si tout les cellule qui sont bateau sont touché alors le conteur reste à 0 et donc J2 à perdue
            if (cont2 == 0)
            {
                
                Save.Score = Save.Score + 50 + 20 * Save.Niveau;
                ShowMessageDialog("fin", Save.NomJoueur + " a gagné " + Save.Score + " points.");
                // Récupérer la chaîne JSON des scores depuis les paramètres d'application
                // Récupérer les anciennes données stockées dans les paramètres locaux
                // Déclarer et initialiser une liste de noms et scores
                List<KeyValuePair<string, int>> scoresList = new List<KeyValuePair<string, int>>();
                scoresList.Add(new KeyValuePair<string, int>(Save.NomJoueur, Save.Score));
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                string oldScoresJson = localSettings.Values["scoresList"] as string;

                // Créer un objet DataContractSerializer pour sérialiser la liste en JSON
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<KeyValuePair<string, int>>));

                // Si les anciennes données existent, les fusionner avec la nouvelle liste de noms et scores
                if (!string.IsNullOrEmpty(oldScoresJson))
                {
                    // Désérialiser les anciennes données en une liste de paires nom-score en utilisant un objet DataContractSerializer
                    using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(oldScoresJson)))
                    {
                        List<KeyValuePair<string, int>> oldScoresList = serializer.ReadObject(memoryStream) as List<KeyValuePair<string, int>>;

                        // Fusionner les anciennes et nouvelles listes en une seule liste
                        scoresList.AddRange(oldScoresList);
                    }
                }

                // Convertir la liste fusionnée en une chaîne JSON
                string scoresJson = "";
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    serializer.WriteObject(memoryStream, scoresList);
                    scoresJson = Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                // Stocker la nouvelle chaîne JSON fusionnée avec les anciennes données dans les paramètres locaux
                localSettings.Values["scoresList"] = scoresJson;
                //Stop le timer
                isRunning = false;
                return true;
            }
            foreach (Cellule cell in grilleJ1.Children)
            {
                if (!cell.IsHit & cell.IsBoat)
                {
                    cont1 += 1;
                }
            }
            //si tout les cellule qui sont bateau sont touché alors le conteur reste à 0 et donc J1 à perdue
            if (cont1 == 0)
            {
                ShowMessageDialog("fin", "ordi a gagné vous avez gagné "+Save.Score+" points");
                // Récupérer les données du nouveau joueur et les anciennes donné pour les fusionner 
                List<KeyValuePair<string, int>> scoresList = new List<KeyValuePair<string, int>>();
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                scoresList.Add(new KeyValuePair<string, int>(Save.NomJoueur, Save.Score));

                string oldScoresJson = localSettings.Values["scoresList"] as string;

                // Créer un objet DataContractSerializer pour sérialiser la liste en JSON
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<KeyValuePair<string, int>>));

                // Si les anciennes données existent, les fusionner avec la nouvelle liste de noms et scores
                if (!string.IsNullOrEmpty(oldScoresJson))
                {
                    // Désérialiser les anciennes données en une liste de paires nom-score en utilisant un objet DataContractSerializer
                    using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(oldScoresJson)))
                    {
                        List<KeyValuePair<string, int>> oldScoresList = serializer.ReadObject(memoryStream) as List<KeyValuePair<string, int>>;

                        // Fusionner les anciennes et nouvelles listes en une seule liste
                        scoresList.AddRange(oldScoresList);
                    }
                }

                // Convertir la liste fusionnée en une chaîne JSON
                string scoresJson = "";
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    serializer.WriteObject(memoryStream, scoresList);
                    scoresJson = Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                // Stocker la nouvelle chaîne JSON fusionnée avec les anciennes données dans les paramètres locaux
                localSettings.Values["scoresList"] = scoresJson;
                //Stop le timer
                isRunning = false;
                return true;
            }
            else
            {
                
                return false;
            }
        }

        //Tire sur un casse du joueur J1 aléatoirement 
        private void TireEnemie0()
        {
            Random random = new Random();
            for (int i = 0;i <= Save.Niveau; i++)//nombre de tire en fonction de difficulté
            {
                var cellsToHit = grilleJ1.Children.Cast<Cellule>().Where(c => !c.IsHit);
                if (cellsToHit.Any())
                {
                    var cell = cellsToHit.ElementAt(random.Next(cellsToHit.Count()));
                    cell.IsHit = true;
                    cell.Couleur = cell.IsBoat ? new SolidColorBrush(Windows.UI.Colors.Red) : new SolidColorBrush(Windows.UI.Colors.Blue);
                }
            }
        }

        //Tire sur un casse du joueur J1 aléatoirement 
        private List<Cellule> coupsPrecedents = new List<Cellule>();

        //tire qui prend en compte sont dernier coup 
        private void TireEnemie1()
        {
            Random random = new Random();

            // regarde le dernié coup
            var lastCoup = coupsPrecedents.LastOrDefault();
            if (lastCoup != null && lastCoup.IsBoat)
            {
                var neighboringCells = GetNeighboringCells(lastCoup);
                var cellsToHit = neighboringCells.Where(c => !c.IsHit & c.IsBoat);
                if (cellsToHit.Any())
                {
                    var cell = cellsToHit.ElementAt(random.Next(cellsToHit.Count()));
                    cell.IsHit = true;
                    cell.Couleur = new SolidColorBrush(Windows.UI.Colors.Red);
                    coupsPrecedents.Add(cell);
                    return;
                }
                else
                {
                    var cellsToCheck = neighboringCells.Where(c => !c.IsHit);
                    if (cellsToCheck.Any())
                    {
                        var cell = cellsToCheck.ElementAt(random.Next(cellsToCheck.Count()));
                        cell.IsHit = true;
                        cell.Couleur = new SolidColorBrush(Windows.UI.Colors.Blue);
                        coupsPrecedents.Add(cell);
                        return;
                    }
                }
            }

            // Si pas toucher, tire aléatoirement 
            var cellsToHitRandom = grilleJ1.Children.Cast<Cellule>().Where(c => !c.IsHit);
            if (cellsToHitRandom.Any())
            {
                var cell = cellsToHitRandom.ElementAt(random.Next(cellsToHitRandom.Count()));
                cell.IsHit = true;
                cell.Couleur = cell.IsBoat ? new SolidColorBrush(Windows.UI.Colors.Red) : new SolidColorBrush(Windows.UI.Colors.Blue);

                if (cell.IsBoat)
                {
                    coupsPrecedents.Add(cell);
                }
            }

            // Supprimez tous les mouvements plus anciens que le plus récent.
            coupsPrecedents.RemoveAll(coup => coup != lastCoup && coup != coupsPrecedents.LastOrDefault());
        }

        private IEnumerable<Cellule> GetNeighboringCells(Cellule cell)
        {
            var row = cell.X;
            var col = cell.Y;
            var cells = grilleJ1.Children.Cast<Cellule>();
            return cells.Where(c => (c.X == row && Math.Abs(c.Y - col) == 1)
                || (c.Y == col && Math.Abs(c.X - row) == 1));
        }

        //Incremente le compteur de tirs
        private void IncrementShootCounter()
        {
            shootCounter++;
            ShootCounterNbTextBlock.Text = shootCounter.ToString();
        }

        //Met à jour le label du Chronomètre
        private void TimerTick(object sender, object e)
        {
            if (isRunning)
            {
                elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
                TimerNbTextBlock.Text = elapsedTime.ToString(@"hh\:mm\:ss");
            }
        }

        //Permet d'afficher des messages 
        private async void ShowMessageDialog(string title, string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message, title);
            await dialog.ShowAsync();
        }



        //fonction du boutton fin de tour 
        private void EndTurnButton(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer stopint = Windows.Storage.ApplicationData.Current.LocalSettings;
            int shoot = (int)stopint.Values["int"];
            if (shoot == 2)
            {
                IncrementShootCounter();
                ToutCellDetruite();
                if (Save.Niveau == 2)
                {
                    TireEnemie1();
                }
                else
                {
                    TireEnemie0();
                }
            
                
                stopint.Values["int"] = 1;
            }
        }


        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

    }
}
