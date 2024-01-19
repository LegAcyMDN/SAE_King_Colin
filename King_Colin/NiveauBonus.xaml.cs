using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

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

        private DispatcherTimer temps = new DispatcherTimer();

        public NiveauBonus()
        {
            InitializeComponent();

            temps.Tick += StreetFighter;
            temps.Interval = TimeSpan.FromMilliseconds(10);
            temps.Start();

        }

        private void StreetFighter(object sender, EventArgs e)
        {
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
            if (animeDiard > 32) { animeDiard = 0; }
            imageDiard.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/Diard/diardJedi-" + animeDiard / 32 + ".png"));
            DiardJedi.Fill = imageDiard;

            AppliquerMiroirGauche(Joueur2);
            animeStatique++;
            if (animeStatique > 48) { animeStatique = 0; }
            imageMarioStatique.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Statique/statique-" + animeStatique / 8 + ".png"));
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
