using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_project
{
    public class Boat
    {
        public string Nom { get; set; }
        public int Longueur { get; set; }
        public bool EstCoule { get; set; }
        public bool Horientation { get; set; }
        

        // constructeur du bateau 
        public Boat(string nom, int longueur)
        {
            Nom = nom;
            Longueur = longueur;
            EstCoule = false;
        }
    }
}
