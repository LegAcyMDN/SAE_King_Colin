using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace King_Colin
{
    /// <summary>
    /// Logique d'interaction pour NiveauBonus.xaml
    /// </summary>
    public partial class NiveauBonus : Window
    {
        private int animeDiard = 0;
        private int animeStatique = 0;

        private bool sensJoueur = true;

        private ImageBrush imageDiard = new ImageBrush();
        private ImageBrush imageStreetFighter = new ImageBrush();
        private ImageBrush imageMarioStatique = new ImageBrush();

        public NiveauBonus()
        {
            InitializeComponent();



            ChargeImage();
            AnimationImage();
        }

        private void ChargeImage()
        {
            imageStreetFighter.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/map3.png"));
            FondEcran2.Fill = imageStreetFighter;
        }

        private void AnimationImage()
        {
            animeDiard++;
            if (animeDiard > 1) { animeDiard = 0; }
            imageDiard.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/Diard/diardJedi-" + animeDiard + ".png"));
            DiardJedi.Fill = imageDiard;

            AppliquerMiroirGauche(Joueur2);
            animeStatique++;
            if (animeStatique > 12) { animeStatique = 0; }
            imageMarioStatique.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Statique/statique-" + animeStatique / 2 + ".png"));
            Joueur2.Fill = imageMarioStatique;
        }

        private void AppliquerMiroirGauche(Rectangle rectangle)
        {
            double joueurX = Canvas.GetLeft(rectangle);
            double joueurWidth = rectangle.Width;
            if (sensJoueur == true)
            {
                ScaleTransform transformation = new ScaleTransform(-1, 1);
                rectangle.RenderTransform = transformation;
                Canvas.SetLeft(rectangle, joueurX + joueurWidth);
                sensJoueur = false;
            }

        }
    }
}
