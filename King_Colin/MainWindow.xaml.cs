using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace King_Colin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool gauche, droite, haut, bas = false, frappe = false;
        private bool jeuEnPause = false;
        private bool touche = false;

        private readonly Regex plateforme = new Regex("^plateforme[0-9]$");
        private readonly Regex echelle = new Regex("^echelle[0-9]{2}$");
        private readonly Regex ennemi = new Regex("^ennemi[0-9]$");
        private readonly Regex baril = new Regex("^baril[0-9]$");

        // liste des éléments rectangles
        private List<System.Windows.Shapes.Rectangle> itemsARetirer = new List<System.Windows.Shapes.Rectangle>();
        private System.Windows.Shapes.Rectangle rectangle;

        //animation jeux
        private int animePortail = 0;

        //tous les skins
        private ImageBrush imageJeux = new ImageBrush();
        private ImageBrush imageStreetFighter = new ImageBrush();
        private ImageBrush imageJoueur = new ImageBrush();
        private ImageBrush imagePrincesse = new ImageBrush();
        private ImageBrush imageDonkey = new ImageBrush();
        private ImageBrush imageEnnemi = new ImageBrush();
        private ImageBrush imageEchelle = new ImageBrush();
        private ImageBrush imagePlateforme = new ImageBrush();
        private ImageBrush imageTonneau = new ImageBrush();
        private ImageBrush imageTirBoss = new ImageBrush();
        private ImageBrush imageTirEnn = new ImageBrush();
        private ImageBrush imagePortail = new ImageBrush();
        private ImageBrush imageSecrette = new ImageBrush();

        //vitesses et timer
        private int vitesseDonkey = 3;
        private int vitesseJoueur = 5;
        private int vitesseEnnemi = 3;
        private int vitesseTirEnnemi = 7;
        private int vitesseTirTonneau = 7;
        private int timerTonneau;
        private int timerTirEnnemi;

        //Différent DispatcherTimer pour gérer différent éléments
        private DispatcherTimer temps = new DispatcherTimer();

        //gravité pour le joueur
        private double velociteY = 0;
        private const double gravite = 0.1;
        private bool enSaut = false;

        //Donkey Kong aka Colin
        private Random tirEnnemi = new Random();
        private Random deplacementEnnemi = new Random();

        //Musique
        private MediaPlayer musiqueJeux = new MediaPlayer();


        public MainWindow()
        {
            InitializeComponent();

            MenuWindow fenetreMenu = new MenuWindow();
            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
            {
                Application.Current.Shutdown();
            }

            temps.Tick += Jeu;
            temps.Interval = TimeSpan.FromMilliseconds(10);
            temps.Start();

            temps.Tick += Gravite;
            ChargeImage();

            musiqueJeux.MediaEnded += MusiqueJeu_Fin;
            //lancement du timer 
        }

        private void Gravite(object sender, EventArgs e)
        {
            /*double maxY = cv_Jeux.Height - joueur1.Height;
            double actuelY = Canvas.GetTop(joueur1);

            bool touchePlateforme = false;

            if (enSaut)
            {
                velociteY += gravite;
                double nouvelY = Canvas.GetTop(joueur1) + velociteY;

                Rect joueurBornes = new Rect(Canvas.GetLeft(joueur1), Canvas.GetTop(joueur1), joueur1.Width, joueur1.Height);

                foreach (System.Windows.Shapes.Rectangle plateforme in ListeDesPlateformes())
                {
                    Rect plateformeBornes = new Rect(Canvas.GetLeft(plateforme), Canvas.GetTop(plateforme), plateforme.Width, plateforme.Height);
                    Console.WriteLine(velociteY);

                    if (joueurBornes.IntersectsWith(plateformeBornes) && velociteY >= 0)
                    {
                        touchePlateforme = true;
                        nouvelY = Canvas.GetTop(plateforme) - joueur1.Height;
                        velociteY = 0;                  
                        enSaut = false;
                    }
                }

                Canvas.SetTop(joueur1, nouvelY);

                if (!touchePlateforme)
                {
                    if (actuelY > maxY)
                    {
                        Canvas.SetTop(joueur1, maxY);
                        velociteY = 0;
                        enSaut = false;
                    }
                }
            }*/

            double maxY = cv_Jeux.Height - joueur1.Height;
            double actuelY = Canvas.GetTop(joueur1);

            bool touchePlateforme = false;

            if (enSaut)
            {
                velociteY += gravite;
                Canvas.SetTop(joueur1, Canvas.GetTop(joueur1) + velociteY);

                foreach (System.Windows.Shapes.Rectangle plateforme in ListeDesPlateformes())
                {
                    Rect joueur = new Rect(Canvas.GetLeft(joueur1), Canvas.GetTop(joueur1), joueur1.Width, joueur1.Height);
                    Rect plateformeBornes = new Rect(Canvas.GetLeft(plateforme), Canvas.GetTop(plateforme), plateforme.Width, plateforme.Height);

                    if (joueur.IntersectsWith(plateformeBornes) && velociteY >= 0)
                    {
                        touchePlateforme = true;
                        velociteY = 0;
                        enSaut = false;

                        // Ajuster la position du joueur au sommet de la plateforme
                        Canvas.SetTop(joueur1, plateformeBornes.Top - joueur1.Height);
                    }
                }

                if (!touchePlateforme && actuelY > maxY)
                {
                    Canvas.SetTop(joueur1, maxY);
                    velociteY = 0;
                    enSaut = false;
                }
            }
        }

        private List<System.Windows.Shapes.Rectangle> ListeDesPlateformes()
        {
            List<System.Windows.Shapes.Rectangle> plateformes = new List<System.Windows.Shapes.Rectangle>();
            plateformes.Add(plateforme1);
            plateformes.Add(plateforme2);
            plateformes.Add(plateforme3);
            plateformes.Add(plateforme4);
            plateformes.Add(plateforme5);
            return plateformes;
        }
        private List<System.Windows.Shapes.Rectangle> ListeDesPigeons()
        {
            List<System.Windows.Shapes.Rectangle> ennemis = new List<System.Windows.Shapes.Rectangle>();
            ennemis.Add(ennemi1);
            ennemis.Add(ennemi2);
            ennemis.Add(ennemi3);
            return ennemis;
        }
        private void ChargeImage()
        {
            imagePlateforme.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/plateforme.png"));
            imageEchelle.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/echelle.png"));
            imageEnnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Ennemi/pigeon.png"));
            imageTonneau.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/barilC1.png"));
            imageTirBoss.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/barilC2.png"));
            imageTirEnn.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Ennemi/toastenfeu.png"));


            foreach (UIElement element in cv_Jeux.Children)
            {
                if (element is System.Windows.Shapes.Rectangle)
                {
                    rectangle = (System.Windows.Shapes.Rectangle)element;

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
            Salle_Secret.Fill = imageSecrette;

            imageJeux.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/fondEcran.png"));
            FondEcran.Fill = imageJeux;

            imageStreetFighter.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/map3.png"));
            FondEcran2.Fill = imageStreetFighter;

            imageJoueur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/marioLou.png"));
            joueur1.Fill = imageJoueur;

            imagePrincesse.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/princessejustin.png"));
            princesse.Fill = imagePrincesse;

            imageDonkey.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/colinperenoel.png"));
            donkeykong.Fill = imageDonkey;
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

        private void cv_Jeux_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            { gauche = true; }

            if (e.Key == Key.D)
            { droite = true; }

            if (e.Key == Key.Z)
            { haut = true; }

            if (e.Key == Key.S)
            { bas = true; }

            if (e.Key == Key.Space)
            { enSaut = false; }
            
            //faire un switch ??? a mediter
            //rajouter une condition pour dire disponible suelement dans level bonus

            if (e.Key == Key.P)
            {
                if (!jeuEnPause)
                {
                    temps.Stop();
                    jeuEnPause = true;
                }
            }

            if (e.Key == Key.R)
            {
                if (jeuEnPause)
                {
                    temps.Start();
                    jeuEnPause = false;
                }
            }

            if (e.Key == Key.Escape)
            { Application.Current.Shutdown(); }

        }

        private void cv_Jeux_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            { gauche = false; }

            if (e.Key == Key.D)
            { droite = false; }

            if (e.Key == Key.Z)
            { haut = false; }

            if (e.Key == Key.S)
            { bas = false; }

            if (e.Key == Key.Space && !enSaut)
            {
                enSaut = true;
                velociteY = -3.25;
            }
        }

        private List<System.Windows.Shapes.Rectangle> ListeDesEchelles()
        {
            List<System.Windows.Shapes.Rectangle> echelles = new List<System.Windows.Shapes.Rectangle>();
            echelles.Add(echelle01);
            echelles.Add(echelle02);
            echelles.Add(echelle03);
            echelles.Add(echelle04);
            echelles.Add(echelle05);
            echelles.Add(echelle06);
            echelles.Add(echelle09);
            echelles.Add(echelle10);
            return echelles;
        }

        private void Jeu(object sender, EventArgs e)
        {
            switch (LancementNiveauBonus())
            {
                case false:
                    if (!jeuEnPause)
                    {
                        AnimationImage();

                        Rect joueur = new Rect(Canvas.GetLeft(joueur1), Canvas.GetTop(joueur1), joueur1.Width, joueur1.Height);
                        bool devantEchelle = false;

                        foreach (System.Windows.Shapes.Rectangle echelle in ListeDesEchelles())
                        {
                            if (joueur.IntersectsWith(new Rect(Canvas.GetLeft(echelle), Canvas.GetTop(echelle), echelle.Width, echelle.Height)))
                            {
                                devantEchelle = true;
                                break;
                            }
                        }

                        if (devantEchelle)
                        {
                            System.Windows.Shapes.Rectangle echelle = ListeDesEchelles().FirstOrDefault(e => joueur.IntersectsWith(new Rect(Canvas.GetLeft(e), Canvas.GetTop(e), e.Width, e.Height)));

                            if (echelle != null)
                            {
                                double topEchelle = Canvas.GetTop(echelle);
                                double bottomEchelle = topEchelle + echelle.Height;

                                if (haut && Canvas.GetTop(joueur1) > topEchelle)
                                {
                                    Canvas.SetTop(joueur1, Math.Max(topEchelle, Canvas.GetTop(joueur1) - vitesseJoueur));
                                }
                                else if (haut && Canvas.GetTop(joueur1) <= topEchelle)
                                {
                                    Canvas.SetTop(joueur1, Canvas.GetTop(joueur1) - vitesseJoueur);
                                }
                                else if (bas && Canvas.GetTop(joueur1) + joueur1.Height < bottomEchelle)
                                {
                                    Canvas.SetTop(joueur1, Math.Min(bottomEchelle - joueur1.Height, Canvas.GetTop(joueur1) + vitesseJoueur));
                                }
                            }
                            MouvementHorizontaux();
                        }
                        else
                        {
                            
                            MouvementHorizontaux();
                            RetireLesItems();
                        }
                            MouvementDonkey();
                            MouvementEnnemis();
                       

                        Victoire();
                    }
                    break;
                case true:
                    break;

            }

        }

        private void AnimationImage()
        {
            animePortail++;
            if (animePortail > 8) { animePortail = 0; }
            imagePortail.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Portail/portail-" + animePortail + ".png"));
            Portail.Fill = imagePortail;
        }

        // Gestion du mouvement horizontal si nécessaire (gauche et droite)
        private void MouvementHorizontaux()
        {
            double canvasMax = cv_Jeux.ActualWidth;
            double joueurX = Canvas.GetLeft(joueur1);
            double joueurWidth = joueur1.Width;

            if (gauche && droite)
                return;
            if (gauche && joueurX <= 0)
            {
                Canvas.SetLeft(joueur1, 0);
                return;
            }
            if (droite && joueurX + joueurWidth >= canvasMax)
            {
                Canvas.SetLeft(joueur1, canvasMax - joueurWidth);
                return;
            }
            if (droite)
            {
                Canvas.SetLeft(joueur1, joueurX + vitesseJoueur);
                return;
            }
            if (gauche)
            {
                Canvas.SetLeft(joueur1, joueurX - vitesseJoueur);
                return;
            }
        }

        /*private void FinBonus()
        {
            if (/*le joueur obtient le jetpack*//*)
            {
                //deblocage du jeu bonus
            }
        }*/

        private void MouvementDonkey()
        {
            double canvasMax = cv_Jeux.ActualWidth;
            double joueurX = Canvas.GetLeft(joueur1);
            double donkey = Canvas.GetLeft(donkeykong);

            if (donkey < joueurX)
            {
                Canvas.SetLeft(donkeykong, donkey + vitesseDonkey);
            }

            else if (donkey > joueurX)
            {
                Canvas.SetLeft(donkeykong, donkey - vitesseDonkey);
            }
            else if (donkey <= 0)
            {
                Canvas.SetLeft(donkeykong, 0);
            }
            else if (donkey + donkeykong.Width >= canvasMax)
            {
                Canvas.SetLeft(donkeykong, canvasMax - donkeykong.Width);
            }

            if (tirEnnemi.Next(100) < 2)
                LancerTonneau();
        }
        private void LancerTonneau()
        {
            System.Windows.Shapes.Rectangle tirsBoss = new System.Windows.Shapes.Rectangle
            {
                Tag = "tirsEnnemi",
                Height = 51,
                Width = 66,
                Fill = imageTirBoss
            };

            Canvas.SetTop(tirsBoss, Canvas.GetTop(donkeykong) + donkeykong.Height);
            Canvas.SetLeft(tirsBoss, Canvas.GetLeft(donkeykong) + donkeykong.Width / 2);
            cv_Jeux.Children.Add(tirsBoss);

            DispatcherTimer tempsTirBaril = new DispatcherTimer();
            tempsTirBaril.Tick += (sender, e) =>
            {
                Canvas.SetTop(tirsBoss, Canvas.GetTop(tirsBoss) + vitesseTirTonneau);
                CollisionTirs(tirsBoss);

                if (Canvas.GetTop(tirsBoss) > cv_Jeux.ActualHeight)
                {
                    cv_Jeux.Children.Remove(tirsBoss);
                    tempsTirBaril.Stop();
                }
            };

            tempsTirBaril.Interval = TimeSpan.FromMilliseconds(10);
            tempsTirBaril.Start();
        }
        private void MouvementEnnemis()
        {
            double canvasMax = cv_Jeux.ActualWidth;

            foreach (System.Windows.Shapes.Rectangle ennemis in ListeDesPigeons())
            {
                double ennemi = Canvas.GetLeft(ennemis);
                int mouvements = deplacementEnnemi.Next(0, 3);
                if (Canvas.GetRight(ennemis) + ennemi1.Width >= canvasMax)
                {
                    Canvas.SetRight(ennemis, canvasMax - ennemi1.Width);
                }
                else if (Canvas.GetLeft(ennemis) <= 0)
                {
                    Canvas.SetLeft(ennemis, 0 + vitesseEnnemi); //prblm avec les limites canvas + pigeon limite gauche reste au meme endroit + droite sort du champs
                }
                else if (mouvements == 1)
                {
                    Canvas.SetLeft(ennemis, ennemi + vitesseEnnemi);
                }
                else if (mouvements == 2)
                {
                    Canvas.SetLeft(ennemis, ennemi - vitesseEnnemi);
                }
               /* if (tir.Next(100) < 1)
                    LancerToastFeu();*/
            }
        }
      /*  private void LancerToastFeu()
        {
            double joueurX = Canvas.GetLeft(joueur1);
            double ennemi = Canvas.GetLeft(ennemi1);
            foreach (System.Windows.Shapes.Rectangle ennemis in ListeDesPigeons())
            {
                System.Windows.Shapes.Rectangle tirsEnn = new System.Windows.Shapes.Rectangle
                {
                    Tag = "tirsEnnemi",
                    Height = 51,
                    Width = 66,
                    Fill = imageTirEnn
                };
                cv_Jeux.Children.Add(tirsEnn);

                Canvas.SetTop(tirsEnn, Canvas.GetTop(ennemis));
                Canvas.SetLeft(tirsEnn, Canvas.GetLeft(ennemis) + ennemis.Height);
                DispatcherTimer tempstirEnnemi = new DispatcherTimer();
                if (joueurX <= ennemi)
                {
                    tempstirEnnemi.Tick += (sender, e) =>
                    {
                        Canvas.SetLeft(tirsEnn, Canvas.GetLeft(tirsEnn) - vitesseTirEnnemi);
                        CollisionTirs(tirsEnn);
                    };
                }
                else
                {
                    tempstirEnnemi.Tick += (sender, e) =>
                    {
                        Canvas.SetLeft(tirsEnn, Canvas.GetLeft(tirsEnn) + vitesseTirEnnemi);
                        CollisionTirs(tirsEnn);
                    };
                }
                tempstirEnnemi.Interval = TimeSpan.FromMilliseconds(10);
                tempstirEnnemi.Start();
                if (Canvas.GetLeft(tirsEnn) < 0)
                {
                    itemsARetirer.Add(tirsEnn);
                    RetireLesItems();
                    tempstirEnnemi.Stop();
                }// remove toast a revoir 
            }
        }*/
        private void RetireLesItems()
        {
            foreach (System.Windows.Shapes.Rectangle y in itemsARetirer)
            {
                // on les enlève du canvas
                cv_Jeux.Children.Remove(y);
            }
        }
        private void Victoire()
        {
            Rect joueur = new Rect(Canvas.GetLeft(joueur1), Canvas.GetTop(joueur1), joueur1.Width, joueur1.Height);
            Rect peach = new Rect(Canvas.GetLeft(princesse), Canvas.GetTop(princesse), princesse.Width, princesse.Height);
            if (joueur.IntersectsWith(peach))
            {
                temps.Stop();
                MessageBox.Show("Gagné !!", "Fin de partie", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
                AfficherLesCredits();
            }
        }
        private void Defaite()
        {
            temps.Stop();
            MessageBox.Show("Perdu", "Fin de partie", MessageBoxButton.OK,
            MessageBoxImage.Stop);
            AfficherLesCredits();
        }
        private void CollisionTirs(System.Windows.Shapes.Rectangle rectangle)
        {
            Rect joueur = new Rect(Canvas.GetLeft(joueur1), Canvas.GetTop(joueur1), joueur1.Width, joueur1.Height);

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
                        // on ajoute l’ennemi de la liste à supprimer eton décrémente le nombre d’ennemis
                        itemsARetirer.Add(y);
                        Defaite();
                    }
                }
            }
        }
        private void MouvementMarteau()
        {
            //gif marteau qui tappe  
        }
        private bool LancementNiveauBonus()
        {
            bool lancement = false;
            return lancement;
            //jeu bonus : type street fighter (joueur se bat avec un marteau et doit esquiver les bananes que donkey lance ?)
        }
        private void AfficherLesCredits()
        {

        }
    }
}

/*Niveaux de difficultés : 
 * -facile : pas d'ennemis, juste Donkey qui tire des tonneaux, plateformes droites, beaucoup d'échelles
 * -moyen : peu d'ennemis, tir de tonneaux plus rapides, plateformes droites, un peu moins d'échelles
 * -difficile : plus d'ennemis, tir plus rapides et fréquents, plateformes penchées
 * les ennemis ne se déplacent pas, donkey suit le joueur mais est moins rapide 
 */
