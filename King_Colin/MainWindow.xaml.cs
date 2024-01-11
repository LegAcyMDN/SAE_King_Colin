using System;
using System.ComponentModel;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;

namespace King_Colin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool gauche, droite, haut, bas = false, frappe = false;
        private bool jeuEnPause = false;

        private readonly Regex plateforme = new Regex("^plateforme[0-9]$");
        private readonly Regex echelle = new Regex("^echelle[0-9]{2}$");
        private readonly Regex ennemi = new Regex("^ennemi[0-9]$");

        private System.Windows.Shapes.Rectangle rectangle;

        //tous les skins
        private ImageBrush imageJeux = new ImageBrush();
        private ImageBrush imageJoueur = new ImageBrush();
        private ImageBrush imagePrincesse = new ImageBrush();
        private ImageBrush imageDonkey = new ImageBrush();
        private ImageBrush imageEnnemi = new ImageBrush();
        private ImageBrush imageEchelle = new ImageBrush();
        private ImageBrush imagePlateforme = new ImageBrush();
        private ImageBrush imageTonneau = new ImageBrush();
        private ImageBrush imageTirEnnemi = new ImageBrush();

        //vitesses et timer
        private int vitesseDonkey = 6;
        private int vitesseJoueur = 10;
        private int vitesseTirEnnemi = 10;
        private int vitesseTirTonneau = 10;
        private int timerTonneau;
        private int timerTirEnnemi;
        private DispatcherTimer temps = new DispatcherTimer();

        //gravié pour le joueur
        private double velociteY = 0;
        private const double gravite = 0.1;
        private bool enSaut = false;

        private MediaPlayer musiqueJeux = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            temps.Tick += Jeu;
            temps.Interval = TimeSpan.FromMilliseconds(16);
            temps.Start();

            MenuWindow fenetreMenu = new MenuWindow();
            fenetreMenu.ShowDialog();
            if(fenetreMenu.DialogResult == false)
            { 
                Application.Current.Shutdown();
            }

            temps.Tick += Gravite;
            ChargeImage();         
            //imageTonneau.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/tonneau.png"));
            //imageTirEnnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/tirEnnemi.png"));
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

                if (!touchePlateforme)
                {
                    if (actuelY > maxY)
                    {
                        Canvas.SetTop(joueur1, maxY);
                        velociteY = 0;
                        enSaut = false;
                    }
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

        private void ChargeImage()
        {
            imagePlateforme.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/plateforme.png"));
            imageEchelle.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/echelle.png"));
            imageEnnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/pigeon.png"));

            foreach (UIElement element in cv_Jeux.Children)
            {
                if (element is System.Windows.Shapes.Rectangle)
                {
                    rectangle = (System.Windows.Shapes.Rectangle)element;

                    if (plateforme.IsMatch(rectangle.Name))
                    { rectangle.Fill = imagePlateforme; }

                    if (echelle.IsMatch(rectangle.Name))
                    { rectangle.Fill = imageEchelle; }

                    if(ennemi.IsMatch(rectangle.Name)) 
                    { rectangle.Fill = imageEnnemi; }
                }
            }

            imageJeux.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/fondEcran.png"));
            FondEcran.Fill = imageJeux;

            imageJoueur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/marioLou.png"));
            joueur1.Fill = imageJoueur;

            imagePrincesse.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/princessejustin.png"));
            princesse.Fill = imagePrincesse;

            imageDonkey.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/colinperenoel.png"));
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
            if(e.Key == Key.Q)
            { gauche = true; }

            if(e.Key == Key.D)
            { droite = true; }
            
            if(e.Key == Key.Z)
            { haut = true; }

            if(e.Key == Key.S)
            { bas = true; }

            if (e.Key == Key.Space)
            { 
                enSaut = false;
            }
            
            //rajouter une condition pour dire disponible suelement dans level bonus
            /*if ( == Mouse.LeftButton)
            { frappe = true; }*/
            
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
                if(jeuEnPause)
                {
                    temps.Start();
                    jeuEnPause = false;
                }
            }

            if(e.Key == Key.Escape)
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

            if (e.Key == Key.Space)
            {
                if (!enSaut)
                {
                    enSaut = true;
                    velociteY = -3.25;
                }
            }
            /*
            rajouter une condition pour dire disponible suelement dans level bonus
            if (e.Key == Key.Space)
            { frappe = true; }
            */
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
            if (!jeuEnPause)
            {
                Rect joueur = new Rect(Canvas.GetLeft(joueur1), Canvas.GetTop(joueur1), joueur1.Width, joueur1.Height);
                bool devantEchelle = false;

                foreach (System.Windows.Shapes.Rectangle echelle in ListeDesEchelles())
                {
                    if(joueur.IntersectsWith(new Rect(Canvas.GetLeft(echelle), Canvas.GetTop(echelle), echelle.Width, echelle.Height)))
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

                    if (gauche && Canvas.GetLeft(joueur1) > 0)
                    {
                        Canvas.SetLeft(joueur1, Canvas.GetLeft(joueur1) - vitesseJoueur);
                    }
                    else if (droite && Canvas.GetLeft(joueur1) + joueur1.Width < cv_Jeux.ActualWidth)
                    {
                        Canvas.SetLeft(joueur1, Canvas.GetLeft(joueur1) + vitesseJoueur);
                    }
                }
                else
                {
                    if (gauche && Canvas.GetLeft(joueur1) > 0)
                    {
                        Canvas.SetLeft(joueur1, Canvas.GetLeft(joueur1) - vitesseJoueur);
                    }
                    else if (droite && Canvas.GetLeft(joueur1) + joueur1.Width < cv_Jeux.ActualWidth)
                    {
                        Canvas.SetLeft(joueur1, Canvas.GetLeft(joueur1) + vitesseJoueur);
                    }
                }
            }
        }

        private void Ennemis()
        {
            //gestion des ennemies, apparition et mouvement
        }

        private void TirsEnnemies(double x, double y)
        {
            //gestion des tis ennemies
            /*System.Windows.Shapes.Rectangle nouveauTirEnnemi = new System.Windows.Shapes.Rectangle
            {
                Tag = "tirEnnemi",
                Height = 40,
                Width = 15,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 5
            };
            Canvas.SetTop(nouveauTirEnnemi, y);
            Canvas.SetLeft(nouveauTirEnnemi, x);
            cv_Jeux.Children.Add(nouveauTirEnnemi);*/
        }

        /*private void TestTouchéTonneau(System.Windows.Shapes.Rectangle x, Rect joueur)
        {
            if (x is System.Windows.Shapes.Rectangle && (string)x.Tag == "ennemies")
            {
                // vérification de la collision avec le joueur
                Rect ennemi = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                if (joueur.IntersectsWith(ennemi))
                {
                    // collision avec le joueur et fin de la partie !! ajouter systeme de vie!!!
                    timer.Stop();
                    MessageBox.Show("Perdu", "Fin de partie", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void TestTouchéEnnemi(System.Windows.Shapes.Rectangle x, Rect joueur)
        {
            if (x is System.Windows.Shapes.Rectangle && (string)x.Tag == "ennemies")
            {
                // vérification de la collision avec le joueur
                Rect ennemi = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                if (joueur.IntersectsWith(ennemi))
                {
                    // collision avec le joueur et fin de la partie !! ajouter systeme de vie!!!
                    timer.Stop();
                    MessageBox.Show("Perdu", "Fin de partie", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        /*private void TestVictoire()
        {
            if (/*si le joueur touche donkey avec son arme*//*)
            {
                timer.Stop();
                //systeme de pause a configurer
                //finOuPause.Visibility = Visibility.Visible;
                //fin.Visibility = Visibility.Visible;
                MessageBox.Show("Gagné !!", "Fin de partie", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            }
        }*/

        /*private void FinBonus()
        {
            if (/*le joueur obtient le jetpack*//*)
            {
                //deblocage du jeu bonus
            }
        }*/
        private void MouvementTonneaux()
        {
            //vitesse, deplacement de haut en bas 
        }
        private void MouvementDonkey()
        {
            //mouvement de droite à gauche sur la plateforme la plus haute, en suivant le joueur 
        }

        private void MouvementMarteau()
        {
            //gif marteau qui tappe  
        }
        private void NiveauBonus()
        {
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
