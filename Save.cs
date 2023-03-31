using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Battleship_project
{
    public static class Save
    {
        //permet de sauvegarder des variables entre les pages 
        public static List<Cellule> PoseCell = new List<Cellule>();

        public static bool PlacerBateau = true;

        public static int Niveau = 0;

        public static int Score = 0;

        public static string NomJoueur = "michel";   
    }
}
