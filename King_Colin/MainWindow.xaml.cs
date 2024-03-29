﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NiveauBonus niveauBonusFenetre = new NiveauBonus();

        private bool sensJoueur = true;
        private bool gauche, droite, haut, bas = false;
        private bool jeuEnPause = false;
        private bool aMarteau = false;

        private readonly Regex plateforme = new Regex("^Plateforme[0-9]$");
        private readonly Regex echelle = new Regex("^Echelle[0-9]{2}$");
        private readonly Regex ennemi = new Regex("^Ennemi[0-9]$");
        private readonly Regex baril = new Regex("^Baril[0-9]$");

        // liste des éléments rectangles
        private List<Rectangle> itemsARetirer = new List<System.Windows.Shapes.Rectangle>();
        private Rectangle rectangle;

        private List<Rectangle> plateformes = new List<Rectangle>();
        private List<Rectangle> pigeons = new List<Rectangle>();
        private List<Rectangle> echelles = new List<Rectangle>();

        //animation jeux
        private int animePortail = 0;
        private int animeMarteau = 0;
        private int animeStatique = 0;
        private int animeCourir = 0;

        //img scene
        private ImageBrush imageJeux = new ImageBrush();
        private ImageBrush imageSecrette = new ImageBrush();

        //img personnage et ennemi
        private ImageBrush imageJoueur = new ImageBrush();
        private ImageBrush imagePrincesse = new ImageBrush();
        private ImageBrush imageDonkey = new ImageBrush();
        private ImageBrush imageEnnemi = new ImageBrush();

        //img mario
        private ImageBrush imageMarioMarteau = new ImageBrush();
        private ImageBrush imageMarioStatique = new ImageBrush();
        private ImageBrush imageMarioCourir = new ImageBrush();
        private ImageBrush imageMarioSaut = new ImageBrush();

        //img element
        private ImageBrush imageEchelle = new ImageBrush();
        private ImageBrush imagePlateforme = new ImageBrush();
        private ImageBrush imageTonneau = new ImageBrush();
        private ImageBrush imagePortail = new ImageBrush();
        private ImageBrush imageMarteau = new ImageBrush();

        //img tir
        private ImageBrush imageTirBoss = new ImageBrush();
        private ImageBrush imageTirEnn = new ImageBrush();


        //vitesses et timer
        private int vitesseDonkey = 3;
        private int vitesseJoueur = 5;
        private int vitesseEnnemi = 3;
        private int vitesseTirEnnemi = 3;
        private int vitesseTirTonneau = 3;

        public int VitesseTirEnnemi
        {
            get { return vitesseTirEnnemi; }
            set { vitesseTirEnnemi = value; }
        }

        public int VitesseTirTonneau
        {
            get { return vitesseTirTonneau; }
            set { vitesseTirTonneau = value; }
        }

        //Différent DispatcherTimer pour gérer différent éléments
        private DispatcherTimer temps = new DispatcherTimer();
        DispatcherTimer tempsTirBaril = new DispatcherTimer();
        DispatcherTimer tempstirEnnemi = new DispatcherTimer();


        //Donkey Kong aka Colin
        private Random tirEnnemi = new Random();
        private Random deplacementEnnemi = new Random();

        //Musique
        private MediaPlayer musiqueJeux = new MediaPlayer();

        //gravié pour le joueur 

        const float deltaTemps = 0.016f;
        const float forceSaut = 20;
        const float jumpMax = 1f;

        double velociteY = 0;
        double velociteAAtteindre = 0;

        int plateformeActuelle = 0;
        bool auSol = true;

        public MainWindow()
        {
            InitializeComponent();

            MenuWindow fenetreMenu = new MenuWindow();
            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
            {
                Application.Current.Shutdown();
            }

            if (fenetreMenu.DialogResult == true)
            {
                int selectionDifficulteIndex = fenetreMenu.SelectedDifficultyIndex;

                switch (selectionDifficulteIndex)
                {
                    case (0):
                        VitesseTirEnnemi = 3;
                        VitesseTirTonneau = 3;
                        break;

                    case (1):
                        VitesseTirEnnemi = 5;
                        VitesseTirTonneau = 5;
                        break;

                    case (2):
                        VitesseTirEnnemi = 10;
                        VitesseTirTonneau = 10;
                        break;

                    default:
                        break;
                }
            }

            temps.Tick += Jeu;
            temps.Interval = TimeSpan.FromMilliseconds(10);
            temps.Start();

            temps.Tick += VelociteJoueur;
            ChargeImage();
            ListeDesPlateformes();
            ListeDesPigeons();
            ListeDesEchelles();

            musiqueJeux.MediaEnded += MusiqueJeu_Fin;
        }

        //Listes 
        private void ListeDesPlateformes()
        {
            plateformes.Add(Plateforme1);
            plateformes.Add(Plateforme2);
            plateformes.Add(Plateforme3);
            plateformes.Add(Plateforme4);
            plateformes.Add(Plateforme5);
        }
        private void ListeDesPigeons()
        {
            pigeons.Add(Ennemi1);
            pigeons.Add(Ennemi2);
            pigeons.Add(Ennemi3);
        }
        private void ListeDesEchelles()
        {
            echelles.Add(Echelle01);
            echelles.Add(Echelle02);
            echelles.Add(Echelle03);
            echelles.Add(Echelle04);
            echelles.Add(Echelle05);
            echelles.Add(Echelle06);
            echelles.Add(Echelle09);
            echelles.Add(Echelle10);
        }

        //images, animations et musiques 
        private void ChargeImage()
        {
            imagePlateforme.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/plateforme.png"));
            imageEchelle.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/echelle.png"));
            imageEnnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Ennemi/pigeon.png"));
            imageTonneau.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/Colin/barilC1.png"));
            imageTirBoss.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/Colin/barilC2.png"));
            imageTirEnn.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Ennemi/toastenfeu.png"));

            foreach (UIElement element in cv_Jeux.Children)
            {
                if (element is Rectangle)
                {
                    rectangle = (Rectangle)element;

                    if (plateforme.IsMatch(rectangle.Name))
                    { rectangle.Fill = imagePlateforme; }

                    if (echelle.IsMatch(rectangle.Name))
                    { rectangle.Fill = imageEchelle; }

                    if (ennemi.IsMatch(rectangle.Name))
                    { rectangle.Fill = imageEnnemi; }

                    if (baril.IsMatch(rectangle.Name))
                    { rectangle.Fill = imageTonneau; }
                }
            }

            imageSecrette.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/salleSecrette.jpg"));
            SalleSecrete.Fill = imageSecrette;

            imageJeux.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/fondEcran.png"));
            FondEcran.Fill = imageJeux;

            imageJoueur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Statique/statique-0.png"));
            Joueur1.Fill = imageJoueur;

            imagePrincesse.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/princessejustin.png"));
            Princesse.Fill = imagePrincesse;

            imageDonkey.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/Colin/colinperenoel.png"));
            DonkeyKong.Fill = imageDonkey;

            imageMarteau.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/marteauNoir.png"));
            Marteau.Fill = imageMarteau;

            imageMarioSaut.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Courir/courir-5.png"));
        }
        private void AnimationImage()
        {
            animePortail++;
            if (animePortail > 16) { animePortail = 0; }
            imagePortail.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Portail/portail-" + animePortail / 2 + ".png"));
            Portail.Fill = imagePortail;

            animeMarteau++;
            if (animeMarteau > 10) { animeMarteau = 0; }
            imageMarioMarteau.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Marteau/marteau-" + animeMarteau / 2 + ".png"));

            animeStatique++;
            if (animeStatique > 12) { animeStatique = 0; }
            imageMarioStatique.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Statique/statique-" + animeStatique / 2 + ".png"));

            animeCourir++;
            if (animeCourir > 18) { animeCourir = 0; }
            imageMarioCourir.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Courir/courir-" + animeCourir / 2 + ".png"));
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Musique/RipeSeeds.mp3", UriKind.Relative);
            musiqueJeux.Open(uri);
            musiqueJeux.Play();
        }
        private void MusiqueJeu_Fin(object sender, EventArgs e)
        {
            musiqueJeux.Stop();
            musiqueJeux.Play();
        }
        private void AppliquerMiroirGauche(Rectangle rectangle)
        {
            double joueurX = Canvas.GetLeft(rectangle);
            double joueurLargeur = rectangle.Width;
            if (sensJoueur == true)
            {
                ScaleTransform transformation = new ScaleTransform(-1, 1);
                rectangle.RenderTransform = transformation;
                Canvas.SetLeft(rectangle, joueurX + joueurLargeur);
                sensJoueur = false;
            }
        }
        private void AppliquerMiroirDroite(Rectangle rectangle)
        {
            double joueurX = Canvas.GetLeft(rectangle);
            double joueurLargeur = rectangle.Width;
            if (sensJoueur == false)
            {
                ScaleTransform transformation = new ScaleTransform(1, 1);
                rectangle.RenderTransform = transformation;
                Canvas.SetLeft(rectangle, joueurX - joueurLargeur);
                sensJoueur = true;
            }
        }
        private void ActionMarteau()
        {
            Rect joueur = new Rect(Canvas.GetLeft(Joueur1), Canvas.GetTop(Joueur1), Joueur1.Width, Joueur1.Height);
            Rect marteau = new Rect(Canvas.GetLeft(Marteau), Canvas.GetTop(Marteau), Marteau.Width, Marteau.Height);
            Rect passage = new Rect(Canvas.GetLeft(Passage), Canvas.GetTop(Passage), Passage.Width, Passage.Height);

            if (joueur.IntersectsWith(marteau))
            {
                Joueur1.Fill = imageMarioMarteau;
                Joueur1.Height = 60;
                Joueur1.Width = 60;
                Marteau.Visibility = Visibility.Hidden;
                aMarteau = true;
            }

            if (aMarteau && passage.IntersectsWith(joueur))
            {
                cv_Secrete.Visibility = Visibility.Visible;
                aMarteau = false;
            }
        }

        //controles des touches 
        private void cv_Jeux_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = true;
                droite = false;
                AppliquerMiroirDroite(Joueur1);

                if (aMarteau)
                { Joueur1.Fill = imageMarioMarteau; }
                else
                    Joueur1.Fill = imageMarioCourir;
            }

            if (e.Key == Key.D)
            {
                droite = true;
                gauche = false;
                AppliquerMiroirGauche(Joueur1);

                if (aMarteau)
                { Joueur1.Fill = imageMarioMarteau; }
                else
                    Joueur1.Fill = imageMarioCourir;
            }
            if (e.Key == Key.D && e.Key == Key.Q)
            {
                return;
            }

            if (e.Key == Key.Z)
            { haut = true; }

            if (e.Key == Key.S)
            { bas = true; }

            if (e.Key == Key.Space)
            {
                if (aMarteau)
                { Joueur1.Fill = imageMarioMarteau; }
                else
                    Joueur1.Fill = imageMarioStatique;
                Saut();
            }

            if (e.Key == Key.P)
            {
                if (!jeuEnPause)
                {
                    temps.Stop();
                    tempsTirBaril.Stop();
                    tempstirEnnemi.Stop();
                    jeuEnPause = true;
                }
            }

            if (e.Key == Key.R)
            {
                if (jeuEnPause)
                {
                    temps.Start();
                    tempsTirBaril.Start();
                    tempstirEnnemi.Start();
                    jeuEnPause = false;
                }
            }

            if (e.Key == Key.Escape)
            { Application.Current.Shutdown(); }
        }
        private void cv_Jeux_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = false;
                if (aMarteau)
                {
                    AppliquerMiroirDroite(Joueur1);
                    Joueur1.Fill = imageMarioMarteau;
                }

                else
                {
                    AppliquerMiroirGauche(Joueur1);
                    Joueur1.Fill = imageMarioStatique;
                }
            }

            if (e.Key == Key.D)
            {
                droite = false;
                if (aMarteau)
                {
                    AppliquerMiroirGauche(Joueur1);
                    Joueur1.Fill = imageMarioMarteau;
                }

                else
                {
                    AppliquerMiroirDroite(Joueur1);
                    Joueur1.Fill = imageMarioStatique;
                }
            }

            if (e.Key == Key.Z)
            {
                haut = false;

                if (aMarteau)
                { Joueur1.Fill = imageMarioMarteau; }
                else
                    Joueur1.Fill = imageMarioStatique;
            }

            if (e.Key == Key.S)
            {
                bas = false;

                if (aMarteau)
                { Joueur1.Fill = imageMarioMarteau; }
                else
                    Joueur1.Fill = imageMarioStatique;
            }
        }


        //main 
        private void Jeu(object sender, EventArgs e)
        {
            switch (LancementNiveauBonus())
            {
                case false:
                    if (tirEnnemi.Next(1000) < 1)
                        LancerTonneau();

                    ActionMarteau();
                    AnimationImage();
                    MouvementDonkey();
                    MouvementEnnemis();
                    ToucheEchelleEtPlateformes();
                    MouvementHorizontaux();
                    MouvementJoueurVerticaux();
                    RetireLesItems();

                    if (tirEnnemi.Next(1000) < 1)
                        LancerToastFeu();

                    Victoire();
                    break;

                case true:
                    NiveauStreetFighter();
                    break;
            }

        }

        //Gestion des mouvements du joueur et gravité
        private void ToucheEchelleEtPlateformes()
        {
            Rect joueur = new Rect(Canvas.GetLeft(Joueur1), Canvas.GetTop(Joueur1), Joueur1.Width, Joueur1.Height);
            bool devantEchelle = false;

            foreach (Rectangle echelle in echelles)
            {
                if (joueur.IntersectsWith(new Rect(Canvas.GetLeft(echelle), Canvas.GetTop(echelle), echelle.Width, echelle.Height)))
                {
                    devantEchelle = true;
                    break;
                }
            }

            if (devantEchelle)
            {
                Rectangle echelle = echelles.FirstOrDefault(e => joueur.IntersectsWith(new Rect(Canvas.GetLeft(e), Canvas.GetTop(e), e.Width, e.Height)));

                if (echelle != null)
                {
                    double hautEchelle = Canvas.GetTop(echelle);
                    double basEchelle = hautEchelle + echelle.Height;
                    double joueur1Haut = Canvas.GetTop(Joueur1);

                    if (haut && joueur1Haut > hautEchelle)
                    {
                        Canvas.SetTop(Joueur1, Math.Max(hautEchelle, joueur1Haut - vitesseJoueur));
                    }
                    else if (haut && joueur1Haut <= hautEchelle)
                    {
                        Canvas.SetTop(Joueur1, joueur1Haut - vitesseJoueur);
                    }
                    else if (bas && joueur1Haut + Joueur1.Height < basEchelle)
                    {
                        Canvas.SetTop(Joueur1, Math.Min(basEchelle - Joueur1.Height, joueur1Haut + vitesseJoueur));
                    }

                    for (int i = 0; i < plateformes.Count; i++)
                    {
                        Rect plateformeRect = new Rect(Canvas.GetLeft(plateformes[i]), Canvas.GetTop(plateformes[i]), plateformes[i].Width, plateformes[i].Height);

                        if (joueur1Haut + Joueur1.ActualHeight <= plateformeRect.Top + 10)
                        {
                            plateformeActuelle = i;
                        }

                    }
                }
            }
        }
        private void VelociteJoueur(object sender, EventArgs e)
        {
            velociteY = Adoucissement(velociteY, velociteAAtteindre, 15 * deltaTemps);
        }
        private double Adoucissement(double premierFloat, double deuxiemeFloat, float by)
        {
            return premierFloat + (deuxiemeFloat - premierFloat) * by;
        }
        private void Saut()
        {

            if (auSol == true)
            {

                auSol = false;
                velociteAAtteindre = forceSaut * -1;
            }

        }
        private void MouvementJoueurVerticaux()
        {

            //tailles joueur
            double joueurHauteur = Joueur1.ActualHeight;
            double joueurLargeur = Joueur1.ActualWidth;

            //Rect colliders
            Rect joueurBornes = new Rect(Canvas.GetLeft(Joueur1), Canvas.GetTop(Joueur1), joueurLargeur, joueurHauteur);

            //Effet de la gravité
            Canvas.SetTop(Joueur1, joueurBornes.Top + velociteY);

            for (int i = 0; i <= plateformeActuelle; i++)
            {


                Rectangle plateformeEnCours = plateformes[i];
                Rect plateformeRect = new Rect(Canvas.GetLeft(plateformeEnCours), Canvas.GetTop(plateformeEnCours), plateformeEnCours.Width, plateformeEnCours.Height);

                if (joueurBornes.IntersectsWith(plateformeRect) && velociteAAtteindre > 0)
                {
                    auSol = true;
                    velociteAAtteindre = 0;
                    velociteY = 0;
                    Canvas.SetTop(Joueur1, plateformeRect.Top - Joueur1.Height);
                    return;
                }
            }



            if (-velociteY >= (forceSaut - jumpMax))
            {
                return;
            }

            if (auSol == false)
            {
                velociteAAtteindre = velociteAAtteindre + forceSaut / 10;
                return;
            }
        }
        private void MouvementHorizontaux()
        {
            double canvasMax = cv_Jeux.ActualWidth;
            double canvasMin = 0;

            double joueurX = Canvas.GetLeft(Joueur1);
            switch (cv_Secrete.Visibility)
            {
                case Visibility.Visible:
                    canvasMax = cv_Secrete.ActualWidth;
                    Joueur1.Visibility = Visibility.Hidden;
                    Joueur2.Visibility = Visibility.Visible;
                    Joueur1 = Joueur2;
                    break;
                case Visibility.Hidden:
                    break;
            }

            if (gauche && joueurX <= canvasMin && !aMarteau)
            {
                Canvas.SetLeft(Joueur1, canvasMin);
                return;
            }

            if (droite && joueurX >= canvasMax)
            {
                Canvas.SetLeft(Joueur1, canvasMax);
                return;
            }

            if (droite)
            {
                Canvas.SetLeft(Joueur1, joueurX + vitesseJoueur);
                return;
            }

            if (gauche)
            {
                Canvas.SetLeft(Joueur1, joueurX - vitesseJoueur);
                return;
            }
        }

        //Mouvements des ennemis et tirs
        private void MouvementDonkey()
        {
            double canvasMax = cv_Jeux.ActualWidth;
            double donkeyLargeur = DonkeyKong.ActualWidth;
            double donkey = Canvas.GetLeft(DonkeyKong);

            if (droite && donkey + donkeyLargeur <= canvasMax)
            {
                Canvas.SetLeft(DonkeyKong, donkey + vitesseDonkey);
                return;
            }
            else if (gauche && donkey >= 0)
            {
                Canvas.SetLeft(DonkeyKong, donkey - vitesseDonkey);
                return;
            }
            else if (gauche && donkey <= 0)
            {
                Canvas.SetLeft(DonkeyKong, 0);
                return;
            }
            else if (droite && donkey + donkeyLargeur >= canvasMax)
            {
                Canvas.SetRight(DonkeyKong, canvasMax - donkeyLargeur);
                return;
            }
        }
        private void LancerTonneau()
        {
            Rectangle tirsBoss = new System.Windows.Shapes.Rectangle
            {
                Tag = "tirsEnnemi",
                Height = 51,
                Width = 66,
                Fill = imageTirBoss
            };

            Canvas.SetTop(tirsBoss, Canvas.GetTop(DonkeyKong) + DonkeyKong.Height);
            Canvas.SetLeft(tirsBoss, Canvas.GetLeft(DonkeyKong) + DonkeyKong.Width / 2);
            cv_Jeux.Children.Add(tirsBoss);

            tempsTirBaril.Tick += (sender, e) =>
            {
                tempsTirBaril.Start();
                Canvas.SetTop(tirsBoss, Canvas.GetTop(tirsBoss) + vitesseTirTonneau);
                CollisionTirs(tirsBoss);
                if (Canvas.GetTop(tirsBoss) > cv_Jeux.ActualHeight)
                {
                    itemsARetirer.Add(tirsBoss);
                    tempsTirBaril.Stop();
                }
            };

            tempsTirBaril.Interval = TimeSpan.FromMilliseconds(10);
            tempsTirBaril.Start();
        }
        private void MouvementEnnemis()
        {
            double canvasMax = cv_Jeux.ActualWidth;

            foreach (Rectangle ennemis in pigeons)
            {
                int mouvements = deplacementEnnemi.Next(0, 3);
                double ennemi = Canvas.GetLeft(ennemis);
                if (Canvas.GetRight(ennemis) + Ennemi1.Width >= canvasMax)
                {
                    Canvas.SetRight(ennemis, canvasMax + ennemis.Width);
                }

                else if (Canvas.GetLeft(ennemis) <= 0)
                {
                    Canvas.SetLeft(ennemis, 0 + vitesseEnnemi);
                }

                else if (mouvements == 2)
                {
                    Canvas.SetLeft(ennemis, ennemi + vitesseEnnemi);
                }

                else if (mouvements == 1)
                {
                    Canvas.SetLeft(ennemis, ennemi - vitesseEnnemi);
                }

                if (tirEnnemi.Next(1000) < 1)
                    LancerToastFeu();
            }
        }
        private void LancerToastFeu()
        {
            Rect joueur = new Rect(Canvas.GetLeft(Joueur1), Canvas.GetTop(Joueur1), Joueur1.Width, Joueur1.Height);

            double joueurX = Canvas.GetLeft(Joueur1);
            double ennemi = Canvas.GetLeft(Ennemi1);
            foreach (Rectangle ennemis in pigeons)
            {
                Rectangle tirsEnn = new Rectangle
                {
                    Tag = "tirsEnnemi",
                    Height = 51,
                    Width = 66,
                    Fill = imageTirEnn
                };
                Rect tirsEnnemi = new Rect(Canvas.GetLeft(tirsEnn), Canvas.GetTop(tirsEnn), tirsEnn.Width, tirsEnn.Height);
                cv_Jeux.Children.Add(tirsEnn);

                Canvas.SetTop(tirsEnn, Canvas.GetTop(ennemis));
                Canvas.SetLeft(tirsEnn, Canvas.GetLeft(ennemis) + ennemis.Height);
                tempstirEnnemi.Start();

                if (joueurX <= ennemi)
                {

                    tempstirEnnemi.Tick += (sender, e) =>
                    {
                        Canvas.SetLeft(tirsEnn, Canvas.GetLeft(tirsEnn) - vitesseTirEnnemi);
                        CollisionTirs(tirsEnn);
                        if (Canvas.GetLeft(tirsEnn) <= 0)
                        {
                            itemsARetirer.Add(tirsEnn);
                            RetireLesItems();
                        }
                    };
                }

                else
                {

                    tempstirEnnemi.Tick += (sender, e) =>
                    {
                        Canvas.SetLeft(tirsEnn, Canvas.GetLeft(tirsEnn) + vitesseTirEnnemi);
                        CollisionTirs(tirsEnn);
                        if (Canvas.GetLeft(tirsEnn) >= cv_Jeux.ActualWidth - tirsEnn.Width)
                        {
                            itemsARetirer.Add(tirsEnn);
                            RetireLesItems();
                        }

                    };
                }

                tempstirEnnemi.Interval = TimeSpan.FromMilliseconds(10);
                tempstirEnnemi.Start();
            }
        }

        //Collisions, victoire et défaite simple
        private void CollisionTirs(System.Windows.Shapes.Rectangle rectangle)
        {
            Rect joueur = new Rect(Canvas.GetLeft(Joueur1), Canvas.GetTop(Joueur1), Joueur1.Width, Joueur1.Height);

            foreach (var y in cv_Jeux.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                // si le rectangle est un ennemi
                if (y is System.Windows.Shapes.Rectangle && (string)y.Tag == "tirsEnnemi")
                {
                    // création d’un rectangle correspondant à l’ennemi
                    Rect ennemi = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                    // on vérifie la collision
                    // appel à la méthode IntersectsWith pour détecter la collision
                    if (joueur.IntersectsWith(ennemi))
                    {
                        itemsARetirer.Add(y);
                        Defaite();
                    }
                }
            }
        }
        private void Victoire()
        {
            Rect joueur = new Rect(Canvas.GetLeft(Joueur1), Canvas.GetTop(Joueur1), Joueur1.Width, Joueur1.Height);
            Rect peach = new Rect(Canvas.GetLeft(Princesse), Canvas.GetTop(Princesse), Princesse.Width, Princesse.Height);
            if (joueur.IntersectsWith(peach))
            {
                temps.Stop();
                tempsTirBaril.Stop();
                tempstirEnnemi.Stop();
                txt_Fin.Visibility = Visibility.Visible;
                but_Fin.Visibility = Visibility.Visible;
                txt_Fin.Text = "VICTOIRE";
                txt_Fin.FontSize = 50;
                return;
            }
        }
        private void Defaite()
        {
            temps.Stop();
            tempsTirBaril.Stop();
            tempstirEnnemi.Stop();
            txt_Fin.Visibility = Visibility.Visible;
            but_Fin.Visibility = Visibility.Visible;
            txt_Fin.Text = "DEFAITE";
            txt_Fin.FontSize = 50;
            return;
        }

        private void but_Fin_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Niveau bonus
        private bool LancementNiveauBonus()
        {
            bool lancement = false;

            Rect joueur = new Rect(Canvas.GetLeft(Joueur1), Canvas.GetTop(Joueur1), Joueur1.Width, Joueur1.Height);
            Rect portail = new Rect(Canvas.GetLeft(Portail), Canvas.GetTop(Portail), Portail.Width, Portail.Height);

            if (joueur.IntersectsWith(portail))
            { lancement = true; }

            return lancement;
        }
        private void NiveauStreetFighter()
        {

            cv_Jeux.Visibility = Visibility.Hidden;
            cv_Secrete.Visibility = Visibility.Hidden;

            niveauBonusFenetre.Owner = this;
            niveauBonusFenetre.ShowDialog();
            temps.Stop();
            tempsTirBaril.Stop();
            tempstirEnnemi.Stop();
        }

        //retrait des tirs qui sortent du canvas 
        private void RetireLesItems()
        {
            foreach (Rectangle y in itemsARetirer)
            {
                // on les enlève du canvas
                cv_Jeux.Children.Remove(y);
            }
        }
    }
}

