using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace King_Colin.App_code
{
    internal class Joueur
    {

        public string nom { get; private set; } 
        public Rectangle corps { get; private set; }

        public Joueur(Rectangle _corps, string _nom = "joueur-sans-nom") {

            this.corps = _corps;
            this.nom = _nom;



        }

        public void MouvementGauche()
        {
            
            //Rect joueur = new Rect(Canvas.GetLeft(Joueur1), Canvas.GetTop(Joueur1), Joueur1.Width, Joueur1.Height);
            ////Rect passage = new Rect(Canvas.GetLeft(Passage), Canvas.GetTop(Passage), Passage.Width, Passage.Height);
            //double canvasMax = cv_Jeux.ActualWidth;
            //double canvasMin = 0;
            //double joueurX = Canvas.GetLeft(Joueur1);
            //double joueurWidth = Joueur1.Width;
        }



    }
}
