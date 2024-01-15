using System;
using System.Drawing;
using System.Net.NetworkInformation;
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
        private bool gauche, droite, haut, bas = false, frappe = false;

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
        private DispatcherTimer timer = new DispatcherTimer();

        private MediaPlayer musiqueJeux = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += Jeu;
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Start();

            MenuWindow fenetreMenu = new MenuWindow();
            fenetreMenu.ShowDialog();
            if(fenetreMenu.DialogResult == false)
            { Application.Current.Shutdown(); }
            //images sur les carrés            
            imageJeux.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Jeux.jpg"));
            FondEcran.Fill = imageJeux;
            imagePlateforme.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/plateforme.png"));
            plateforme1.Fill = imagePlateforme;
            //imageJoueur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/joueur.png"));
            //imagePrincesse.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/princesse.png"));
            //imageDonkey.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/donkey.png"));
            //imageEnnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/ennemi.png"));
            imageEchelle.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/echelle.png"));
            echelle1.Fill = imageEchelle;
            //imageTonneau.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/tonneau.png"));
            //imageTirEnnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/tirEnnemi.png"));
            musiqueJeux.MediaEnded += MusiqueJeu_Fin;
            //lancement du timer 
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

        private void AppuieDesTouches(object sender, KeyEventArgs e)
        {
            // on gère les booléens gauche et droite et haut et bas en fonction de l’appui de la touche (déplacement)
            if (e.Key == Key.Left)
            {
                gauche = true;
            }
            if (e.Key == Key.Right)
            {
                droite = true;
            }
            if (e.Key == Key.Up)
            {
                haut = true;
            }
            if (e.Key == Key.Down)
            {
                bas = true;
            }
            // test de l’appui sur la barre espace (frappe)
            if (e.Key == Key.Space)
            {
                frappe = true;
            }
        }
        private void MouvementJoueur()
        { //a revoir 
            if (gauche && Canvas.GetLeft(joueur1) > 0)
            {
                Canvas.SetLeft(joueur1, Canvas.GetLeft(joueur1) - vitesseJoueur);
            }
            else if (droite && Canvas.GetLeft(joueur1) + joueur1.Width <
            Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(joueur1, Canvas.GetLeft(joueur1) + vitesseJoueur);
            }
            else if (haut && Canvas.GetTop(joueur1) + joueur1.Width <
            Application.Current.MainWindow.Width)
            {
                Canvas.SetTop(joueur1, Canvas.GetTop(joueur1) + vitesseJoueur);
            }
        }

        private void Ennemis()
        {
            //gestion des ennemies, apparition et mouvement
        }
        private void TirsEnnemies(double x, double y)
        {
            //gestion des tis ennemies
            System.Windows.Shapes.Rectangle nouveauTirEnnemi = new System.Windows.Shapes.Rectangle
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
            cv_Jeux.Children.Add(nouveauTirEnnemi);
        }
        private void TestTouchéTonneau(System.Windows.Shapes.Rectangle x, Rect joueur)
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


        private void Jeu(object sender, EventArgs e)
        {
            // création d’un rectangle joueur pour la détection de collision

            Rect joueur = new Rect(Canvas.GetLeft(joueur1), Canvas.GetTop(joueur1),
            joueur1.Width, joueur1.Height);
        }
    }
}

/*Niveaux de difficultés : 
 * -facile : pas d'ennemis, juste Donkey qui tire des tonneaux, plateformes droites, beaucoup d'échelles
 * -moyen : peu d'ennemis, tir de tonneaux plus rapides, plateformes droites, un peu moins d'échelles
 * -difficile : plus d'ennemis, tir plus rapides et fréquents, plateformes penchées
 * les ennemis ne se déplacent pas, donkey suit le joueur mais est moins rapide 
 */
