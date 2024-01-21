using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private int animeAttaque = 0;

        private int vitesseTirJoueur = 5;

        private bool sensJoueur = true;

        private List<Rectangle> itemsARetirer = new List<Rectangle>();

        private ImageBrush imageDiard = new ImageBrush();

        private ImageBrush imageStreetFighter = new ImageBrush();
        private ImageBrush imageMarioStatique = new ImageBrush();

        private ImageBrush imageVieBoss = new ImageBrush();
        private ImageBrush imageMortBoss = new ImageBrush();

        private ImageBrush imageVieJoueur = new ImageBrush();
        private ImageBrush imageTirJoueur = new ImageBrush();

        private DispatcherTimer temps = new DispatcherTimer();
        private DispatcherTimer tempsTirJoueur = new DispatcherTimer();

        private MediaPlayer musiqueCombat = new MediaPlayer();

        public NiveauBonus()
        {
            InitializeComponent();

            temps.Tick += StreetFighter;
            temps.Interval = TimeSpan.FromMilliseconds(10);
            temps.Start();

            ChargeImage();

            musiqueCombat.MediaEnded += MusiqueCombat_Fin;
        }

        private void StreetFighter(object sender, EventArgs e)
        {
            AnimationImage();
            RetireLesItems();
        }

        private void NiveauBonus_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Musique/LadyOfThe80.mp3", UriKind.Relative);
         //   musiqueCombat.Open(uri);
          //  musiqueCombat.Play();
        }

        private void MusiqueCombat_Fin(object sender, EventArgs e)
        {
          //  musiqueCombat.Stop();
           // musiqueCombat.Play();
        }

        private void ChargeImage()
        {
            imageStreetFighter.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/map3.png"));
            FondEcran2.Fill = imageStreetFighter;

            imageVieBoss.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/VieBoss/vieBoss-0.png"));
            BarreBoss.Fill = imageVieBoss;

            imageVieJoueur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/VieJoueur/vieJoueur-0.png"));

            imageMortBoss.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/VieBoss/vieBoss-1.png"));
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

            animeAttaque++;
            if (animeAttaque > 9) { animeAttaque = 0; }
            imageTirJoueur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/Attaque/approved-" + animeAttaque + ".png"));
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
        private void cv_Combat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            { Application.Current.Shutdown(); }

            if (e.Key == Key.Space)
            { AttaquesJoueur(); }
        }

        private void AttaquesJoueur()
        {
            Rectangle tirJoueur = new Rectangle
            {
                Tag = "tirJoueur",
                Width = 200,
                Height = 113,
                Fill = imageTirJoueur
            };

            Canvas.SetTop(tirJoueur, Canvas.GetTop(Joueur2) + Joueur2.ActualHeight - tirJoueur.Height);
            Canvas.SetLeft(tirJoueur, Canvas.GetLeft(Joueur2) - tirJoueur.Width);
            cv_Combat.Children.Add(tirJoueur);

            tempsTirJoueur.Tick += (sender, e) =>
            {
                Canvas.SetLeft(tirJoueur, Canvas.GetLeft(tirJoueur) - vitesseTirJoueur);
                CollisionTirsBoss(tirJoueur);
                if (Canvas.GetLeft(tirJoueur) <= 0)
                {
                    itemsARetirer.Add(tirJoueur);
                    RetireLesItems();
                }// remove toast a revoir 
            };

            tempsTirJoueur.Interval = TimeSpan.FromMilliseconds(10);
            tempsTirJoueur.Start();
        }

        private void CollisionTirsBoss(Rectangle tirJoueur)
        {
            if (Canvas.GetLeft(tirJoueur) <= Canvas.GetLeft(DiardJedi) + DiardJedi.ActualWidth && Canvas.GetTop(tirJoueur) >= Canvas.GetTop(DiardJedi) && Canvas.GetTop(tirJoueur) <= Canvas.GetTop(DiardJedi) + DiardJedi.ActualHeight)
            {
                // Le tir du joueur a touché le boss
                itemsARetirer.Add(tirJoueur);
                RetireLesItems();
                BarreBoss.Fill = imageMortBoss;
                temps.Stop();

                txt_Victoire.Visibility = Visibility.Visible;
                but_Quitter.Visibility = Visibility.Visible;
            }
        }
        private void but_Quitter_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RetireLesItems()
        {
            foreach (Rectangle y in itemsARetirer)
            { cv_Combat.Children.Remove(y); }
        }
    }
}