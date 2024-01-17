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
        private bool sensJoueur = true;
        private bool gauche, droite, haut, bas = false;
        private bool jeuEnPause = false;
        private bool aMarteau = false;

        private readonly Regex plateforme = new Regex("^rect_plateforme[0-9]$");
        private readonly Regex echelle = new Regex("^rect_echelle[0-9]{2}$");
        private readonly Regex ennemi = new Regex("^rect_ennemi[0-9]$");
        private readonly Regex baril = new Regex("^rect_baril[0-9]$");

        // liste des éléments rectangles
        private List<System.Windows.Shapes.Rectangle> itemsARetirer = new List<System.Windows.Shapes.Rectangle>();
        private System.Windows.Shapes.Rectangle rectangle;

        //animation jeux
        private int animePortail = 0;
        private int animeMarteau = 0;
        private int animeStatique = 0;
        private int animeCourir = 0;

        //img scene
        private ImageBrush imageJeux = new ImageBrush();
        private ImageBrush imageStreetFighter = new ImageBrush();
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
        private int vitesseTirEnnemi = 7;
        private int vitesseTirTonneau = 7;

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
        // private double velociteY = 0;
        //private const double gravite = 0.1;
        //private bool enSaut = false;
        public MainWindow()
        {
            InitializeComponent();

            /*MenuWindow fenetreMenu = new MenuWindow();
            fenetreMenu.ShowDialog();
            if (fenetreMenu.DialogResult == false)
            {
                Application.Current.Shutdown();
            }*/

            temps.Tick += Jeu;
            temps.Interval = TimeSpan.FromMilliseconds(10);
            temps.Start();

            temps.Tick += Gravite;
            ChargeImage();

            musiqueJeux.MediaEnded += MusiqueJeu_Fin;

            //ADD
            this.cv_Jeux.KeyDown += new System.Windows.Input.KeyEventHandler(this.cv_Jeux_KeyDown);
            this.cv_Jeux.KeyUp += new System.Windows.Input.KeyEventHandler(this.cv_Jeux_KeyUp);
        }
        //gravié pour le joueur ADD

        bool enSaut;
        const float gravite = 9.8f;
        const float deltaTime = 0.016f;
        const float jumpForce = 10;
        const float jumpMaxOffset = 1f;

        float timeJump = 0;
        float timeJumpStart = 0;
        float timeJumpEnd = 0;
        float jumpBoost = 0;
        double velociteY = 0;
        double velocityToReachY = 0;

        bool isGrounded;
        bool isJumping;
        private void Gravite(object sender, EventArgs e)
        {

            velociteY = Lerp(velociteY, velocityToReachY, (jumpBoost * 5 + 10f) * deltaTime);


        }

        private double Lerp(double firstFloat, double secondFloat, float by)
        {
            return firstFloat + (secondFloat - firstFloat) * by;
        }

        private void JumpStart()
        {
            timeJumpStart = timeJump;
        }

        private void JumpEnd()
        {
            timeJumpEnd = timeJump;

            Console.WriteLine("start :" + timeJumpStart);
            Console.WriteLine("end :" + timeJumpEnd);
            float time = timeJumpEnd - timeJumpStart;

            jumpBoost = Math.Min(time, 1);
            Jump();
        }

        private void Jump()
        {

            if (isGrounded == true && isJumping == false)
            {
                //TODO JUMP
                isGrounded = false;
                isJumping = true;
                velocityToReachY = jumpForce * -1;
                //Player sizes
                //double playerHeight = joueur1.ActualHeight;
                //double playerWidth = joueur1.ActualWidth;

                ////Rect colliders
                //Rect joueurBornes = new Rect(Canvas.GetLeft(joueur1), Canvas.GetTop(joueur1), playerWidth, playerHeight);
                //Canvas.SetTop(joueur1, joueurBornes.Top -50);
            }

        }

        /*private void Gravite(object sender, EventArgs e)
        {
            /* double maxY = cv_Jeux.Height - joueur1.Height;
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
             }
            double maxY = cv_Jeux.Height - rect_joueur1.Height;
            double actuelY = Canvas.GetTop(rect_joueur1);

            bool touchePlateforme = false;
            if (enSaut)
            {
                velociteY += gravite;
                Canvas.SetTop(rect_joueur1, Canvas.GetTop(rect_joueur1) + velociteY);
                foreach (System.Windows.Shapes.Rectangle plateforme in ListeDesPlateformes())
                {
                    Rect joueur = new Rect(Canvas.GetLeft(rect_joueur1), Canvas.GetTop(rect_joueur1), rect_joueur1.Width, rect_joueur1.Height);
                    Rect plateformeBornes = new Rect(Canvas.GetLeft(plateforme), Canvas.GetTop(plateforme), plateforme.Width, plateforme.Height);

                    if (joueur.IntersectsWith(plateformeBornes))
                    {
                        Console.WriteLine("touché!");
                        touchePlateforme = true;
                        velociteY = 0;
                        enSaut = false;
                        Canvas.SetBottom(rect_joueur1, Canvas.GetTop(plateforme));

                        // Ajuster la position du joueur au sommet de la plateforme
                        Canvas.SetTop(rect_joueur1, plateformeBornes.Top - rect_joueur1.Height);
                    }
                }

                if (!touchePlateforme && actuelY > maxY)
                {
                    Canvas.SetTop(rect_joueur1, maxY);
                    velociteY = 0;
                    enSaut = false;
                }
            }
        }*/

        private List<System.Windows.Shapes.Rectangle> ListeDesPlateformes()
        {
            List<System.Windows.Shapes.Rectangle> plateformes = new List<System.Windows.Shapes.Rectangle>();
            plateformes.Add(rect_plateforme1);
            plateformes.Add(rect_plateforme2);
            plateformes.Add(rect_plateforme3);
            plateformes.Add(rect_plateforme4);
            plateformes.Add(rect_plateforme5);
            return plateformes;
        }
        private List<System.Windows.Shapes.Rectangle> ListeDesPigeons()
        {
            List<System.Windows.Shapes.Rectangle> ennemis = new List<System.Windows.Shapes.Rectangle>();
            ennemis.Add(rect_ennemi1);
            ennemis.Add(rect_ennemi2);
            ennemis.Add(rect_ennemi3);
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
            rect_Salle_Secrete.Fill = imageSecrette;

            imageJeux.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/fondEcran.png"));
            rect_FondEcran.Fill = imageJeux;

            imageStreetFighter.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/map3.png"));
            rect_FondEcran2.Fill = imageStreetFighter;

            imageJoueur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Statique/statique-0.png"));
            rect_joueur1.Fill = imageJoueur;

            imagePrincesse.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/princessejustin.png"));
            rect_princesse.Fill = imagePrincesse;

            imageDonkey.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Boss/colinperenoel.png"));
            rect_donkeykong.Fill = imageDonkey;

            imageMarteau.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Element/marteauNoir.png"));
            rect_Marteau.Fill = imageMarteau;

            imageMarioSaut.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Mario/Courir/courir-5.png"));
        }

        private void AnimationImage()
        {
            animePortail++;
            if (animePortail > 16) { animePortail = 0; }
            imagePortail.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Portail/portail-" + animePortail / 2 + ".png"));
            rect_Portail.Fill = imagePortail;

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
            //  musiqueJeux.Open(uri);
            // musiqueJeux.Play();
        }

        private void MusiqueJeu_Fin(object sender, EventArgs e)
        {
            musiqueJeux.Stop();
            musiqueJeux.Play();
        }

        private void cv_Jeux_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = true;
                droite = false;
                AppliquerMiroirDroite(rect_joueur1);

                if (aMarteau)
                { rect_joueur1.Fill = imageMarioMarteau; }
                else
                    MarioCourtImage();
            }

            if (e.Key == Key.D)
            {
                droite = true;
                gauche = false;
                AppliquerMiroirGauche(rect_joueur1);

                if (aMarteau)
                { rect_joueur1.Fill = imageMarioMarteau; }
                else
                    MarioCourtImage();
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
                enSaut = false;
                if (aMarteau)
                { rect_joueur1.Fill = imageMarioMarteau; }
                else
                    rect_joueur1.Fill = imageMarioStatique;
                JumpStart();
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
                    AppliquerMiroirDroite(rect_joueur1);
                    rect_joueur1.Fill = imageMarioMarteau;
                }

                else
                {
                    AppliquerMiroirGauche(rect_joueur1);
                    rect_joueur1.Fill = imageMarioStatique;
                }
            }

            if (e.Key == Key.D)
            {
                droite = false;
                if (aMarteau)
                {
                    AppliquerMiroirGauche(rect_joueur1);
                    rect_joueur1.Fill = imageMarioMarteau;
                }

                else
                {
                    AppliquerMiroirDroite(rect_joueur1);
                    rect_joueur1.Fill = imageMarioStatique;
                }
            }



            if (e.Key == Key.Z)
            {
                haut = false;

                if (aMarteau)
                { rect_joueur1.Fill = imageMarioMarteau; }
                else
                    rect_joueur1.Fill = imageMarioStatique;
            }

            if (e.Key == Key.S)
            {
                bas = false;

                if (aMarteau)
                { rect_joueur1.Fill = imageMarioMarteau; }
                else
                    rect_joueur1.Fill = imageMarioStatique;
            }

            if (e.Key == Key.Space)
            {
                if (!enSaut)
                {
                    enSaut = true;
                    velociteY = -3.25;
                }
                JumpEnd();
            }
        }

        private void AppliquerMiroirGauche(System.Windows.Shapes.Rectangle rectangle)
        {
            double joueurX = Canvas.GetLeft(rect_joueur1);
            double joueurWidth = rect_joueur1.Width;
            if (sensJoueur == true)
            {
                ScaleTransform transformation = new ScaleTransform(-1, 1);
                rectangle.RenderTransform = transformation;
                Canvas.SetLeft(rect_joueur1, joueurX + joueurWidth);
                sensJoueur = false;
            }

        }

        private void AppliquerMiroirDroite(System.Windows.Shapes.Rectangle rectangle)
        {
            double joueurX = Canvas.GetLeft(rect_joueur1);
            double joueurWidth = rect_joueur1.Width;
            if (sensJoueur == false)
            {
                ScaleTransform transformation = new ScaleTransform(1, 1);
                rectangle.RenderTransform = transformation;
                Canvas.SetLeft(rect_joueur1, joueurX - joueurWidth);
                sensJoueur = true;
            }


        }

        private void MarioCourtImage()
        {
            rect_joueur1.Fill = imageMarioCourir;
            rect_joueur1.Width = 55;
            rect_joueur1.Height = 71;
        }

        private List<System.Windows.Shapes.Rectangle> ListeDesEchelles()
        {
            List<System.Windows.Shapes.Rectangle> echelles = new List<System.Windows.Shapes.Rectangle>();
            echelles.Add(rect_echelle01);
            echelles.Add(rect_echelle02);
            echelles.Add(rect_echelle03);
            echelles.Add(rect_echelle04);
            echelles.Add(rect_echelle05);
            echelles.Add(rect_echelle06);
            echelles.Add(rect_echelle09);
            echelles.Add(rect_echelle10);
            return echelles;
        }

        private void Jeu(object sender, EventArgs e)
        {


            switch (LancementNiveauBonus())
            {
                case false:
                    if (tirEnnemi.Next(100) < 1)
                        LancerTonneau();
                    timeJump += deltaTime;
                    ActionMarteau();
                    AnimationImage();



                    MouvementHorizontaux();
                    MovementJoueurVertical();


                    Rect joueur = new Rect(Canvas.GetLeft(rect_joueur1), Canvas.GetTop(rect_joueur1), rect_joueur1.Width, rect_joueur1.Height);
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
                            double joueur1Top = Canvas.GetTop(rect_joueur1);

                            if (haut && joueur1Top > topEchelle)
                            {
                                Canvas.SetTop(rect_joueur1, Math.Max(topEchelle, joueur1Top - vitesseJoueur));
                            }
                            else if (haut && joueur1Top <= topEchelle)
                            {
                                Canvas.SetTop(rect_joueur1, joueur1Top - vitesseJoueur);
                            }
                            else if (bas && joueur1Top + rect_joueur1.Height < bottomEchelle)
                            {
                                Canvas.SetTop(rect_joueur1, Math.Min(bottomEchelle - rect_joueur1.Height, joueur1Top + vitesseJoueur));
                            }
                        }

                        MouvementDonkey();
                        MouvementEnnemis();


                    }
                    else
                    {

                        //MouvementHorizontaux();
                        //    RetireLesItems();
                    }
                    if (tirEnnemi.Next(1000) < 1)
                        LancerToastFeu();

                    //   Victoire();
                    break;

                case true:
                    //oe c'est greg
                    break;


            }

        }

        private void ActionMarteau()
        {
            Rect joueur = new Rect(Canvas.GetLeft(rect_joueur1), Canvas.GetTop(rect_joueur1), rect_joueur1.Width, rect_joueur1.Height);
            Rect marteau = new Rect(Canvas.GetLeft(rect_Marteau), Canvas.GetTop(rect_Marteau), rect_Marteau.Width, rect_Marteau.Height);
            Rect passage = new Rect(Canvas.GetLeft(rect_passage), Canvas.GetTop(rect_passage), rect_passage.Width, rect_passage.Height);

            if (joueur.IntersectsWith(marteau))
            {
                rect_joueur1.Fill = imageMarioMarteau;
                rect_joueur1.Height = 60;
                rect_joueur1.Width = 60;
                rect_Marteau.Visibility = Visibility.Hidden;
                aMarteau = true;
            }

            /*foreach (var tirEnnemi in cv_Jeux.Children.OfType<System.Windows.Shapes.Rectangle>().Where(r => r.Tag.Equals("tirsEnnemi")))
            {
                Rect tirRect = new Rect(Canvas.GetLeft(tirEnnemi), Canvas.GetTop(tirEnnemi), tirEnnemi.Width, tirEnnemi.Height);

                if (joueur.IntersectsWith(tirRect) && aMarteau)
                { itemsARetirer.Add(tirEnnemi); }
            }

            foreach (var ennemiRect in ListeDesPigeons().Select(e => new Rect(Canvas.GetLeft(e), Canvas.GetTop(e), e.Width, e.Height)))
            {
                if (aMarteau && joueur.IntersectsWith(ennemiRect))
                { itemsARetirer.Add(ListeDesPigeons().First(e => joueur.IntersectsWith(new Rect(Canvas.GetLeft(e), Canvas.GetTop(e), e.Width, e.Height)))); }
            }*/

            if (aMarteau && passage.IntersectsWith(joueur))
            {
                cv_Secrete.Visibility = Visibility.Visible;

                if (Canvas.GetLeft(rect_joueur1) + rect_joueur1.Width >= Canvas.GetLeft(cv_Secrete) + cv_Secrete.Width)
                { cv_Jeux.Visibility = Visibility.Hidden; }
            }
        }
        //ADD
        private void MovementJoueurVertical()
        {

            //Canvas sizes
            double canvasMaxWidth = cv_Jeux.ActualHeight;
            double canvasMaxHeight = cv_Jeux.ActualHeight;

            //Player sizes
            double playerHeight = rect_joueur1.ActualHeight;
            double playerWidth = rect_joueur1.ActualWidth;

            //Rect colliders
            Rect joueurBornes = new Rect(Canvas.GetLeft(rect_joueur1), Canvas.GetTop(rect_joueur1), playerWidth, playerHeight);
            Rect canvasBornes = new Rect(0, canvasMaxHeight, canvasMaxWidth + 1, canvasMaxHeight);


            Canvas.SetTop(rect_joueur1, joueurBornes.Top + velociteY);


            if (joueurBornes.IntersectsWith(canvasBornes) && !isJumping)
            {
                isGrounded = true;
                velocityToReachY = 0;
                velociteY = 0;
                Canvas.SetTop(rect_joueur1, canvasBornes.Height - joueurBornes.Height);
                return;
            }

            if (-velociteY >= (jumpForce - jumpMaxOffset) && isJumping)
            {
                isJumping = false;
                return;
            }


            if (isGrounded == false && isJumping == false)
            {
                velocityToReachY = gravite;
                return;
            }

        }

        private void MouvementHorizontaux()
        {
            double canvasMax = cv_Jeux.ActualWidth;
            double joueurX = Canvas.GetLeft(rect_joueur1);
            double joueurWidth = rect_joueur1.Width;
            Console.WriteLine(joueurX + " " + joueurWidth);
            Console.WriteLine(joueurX + joueurWidth);
            if (gauche && droite)
                return;
            if (gauche && joueurX <= 0 && !aMarteau)
            {
                Canvas.SetLeft(rect_joueur1, 0);
                return;
            }
            if (droite && joueurX >= canvasMax)
            {
                Canvas.SetLeft(rect_joueur1, canvasMax);
                return;
            }
            if (droite)
            {
                Canvas.SetLeft(rect_joueur1, joueurX + vitesseJoueur);
                return;
            }
            if (gauche)
            {
                Canvas.SetLeft(rect_joueur1, joueurX - vitesseJoueur);
                return;
            }
        }

        private void MouvementDonkey()
        {
            double canvasMax = cv_Jeux.ActualWidth;
            double joueurX = Canvas.GetLeft(rect_joueur1);
            double donkey = Canvas.GetLeft(rect_donkeykong);
            double joueurWidth = rect_joueur1.Width;
            if (donkey <= joueurX)
            {
                Canvas.SetLeft(rect_donkeykong, donkey + vitesseDonkey);
                return;
            }
            else if (donkey >= joueurX)
            {
                Canvas.SetLeft(rect_donkeykong, donkey - vitesseDonkey);
                return;
            }
            else if (donkey <= 0)
            {
                Canvas.SetLeft(rect_donkeykong, 0);
                return;
            }
            else if (donkey + rect_donkeykong.Width >= canvasMax)
            {
                Canvas.SetRight(rect_donkeykong, canvasMax - joueurWidth);
                return;
            }


        }
        private void LancerTonneau()
        {
            Console.WriteLine("touché!");
            System.Windows.Shapes.Rectangle tirsBoss = new System.Windows.Shapes.Rectangle
            {
                Tag = "tirsEnnemi",
                Height = 51,
                Width = 66,
                Fill = imageTirBoss
            };

            Canvas.SetTop(tirsBoss, Canvas.GetTop(rect_donkeykong) + rect_donkeykong.Height);
            Canvas.SetLeft(tirsBoss, Canvas.GetLeft(rect_donkeykong) + rect_donkeykong.Width / 2);
            cv_Jeux.Children.Add(tirsBoss);


            tempsTirBaril.Tick += (sender, e) =>
            {
                tempsTirBaril.Start();
                Canvas.SetTop(tirsBoss, Canvas.GetTop(tirsBoss) + vitesseTirTonneau);
                //  CollisionTirs(tirsBoss);
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
                int mouvements = deplacementEnnemi.Next(0, 3);
                double ennemi = Canvas.GetLeft(ennemis);
                if (Canvas.GetRight(ennemis) + rect_ennemi1.Width >= canvasMax)
                {
                    Canvas.SetRight(ennemis, canvasMax + ennemis.Width);
                }
                else if (Canvas.GetLeft(ennemis) <= 0)
                {
                    Canvas.SetLeft(ennemis, 0 + vitesseEnnemi); //prblm avec les limites canvas + pigeon limite gauche reste au meme endroit + droite sort du champs
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
            double joueurX = Canvas.GetLeft(rect_joueur1);
            double ennemi = Canvas.GetLeft(rect_ennemi1);
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
                tempstirEnnemi.Start();
                if (joueurX <= ennemi)
                {
                    tempstirEnnemi.Tick += (sender, e) =>
                    {
                        Canvas.SetLeft(tirsEnn, Canvas.GetLeft(tirsEnn) - vitesseTirEnnemi);
                        // CollisionTirs(tirsEnn);
                    };
                }
                else
                {
                    tempstirEnnemi.Tick += (sender, e) =>
                    {
                        Canvas.SetLeft(tirsEnn, Canvas.GetLeft(tirsEnn) + vitesseTirEnnemi);
                        // CollisionTirs(tirsEnn);
                    };
                }
                tempstirEnnemi.Interval = TimeSpan.FromMilliseconds(10);
                tempstirEnnemi.Start();
                if (Canvas.GetLeft(tirsEnn) < 0)
                {
                    itemsARetirer.Add(tirsEnn);
                    // RetireLesItems();
                    tempstirEnnemi.Stop();
                }// remove toast a revoir 
            }
        }
        /*private void CollisionTirs(System.Windows.Shapes.Rectangle rectangle)
        {
            Rect joueur = new Rect(Canvas.GetLeft(rect_joueur1), Canvas.GetTop(rect_joueur1), rect_joueur1.Width, rect_joueur1.Height);

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
            Rect joueur = new Rect(Canvas.GetLeft(rect_joueur1), Canvas.GetTop(rect_joueur1), rect_joueur1.Width, rect_joueur1.Height);
            Rect peach = new Rect(Canvas.GetLeft(rect_princesse), Canvas.GetTop(rect_princesse), rect_princesse.Width, rect_princesse.Height);
            if (joueur.IntersectsWith(peach))
            {
                temps.Stop();
                MessageBox.Show("Gagné !!", "Fin de partie", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
                AfficherLesCredits();
                return;
            }
        }
        private void Defaite()
        {
            temps.Stop();
            tempsTirBaril.Stop();
            tempstirEnnemi.Stop();
            MessageBox.Show("Perdu", "Fin de partie", MessageBoxButton.OK,
            MessageBoxImage.Stop);
            AfficherLesCredits();
            return;
        }*/

        private bool LancementNiveauBonus()
        {
            bool lancement = false;
            return lancement;
            //jeu bonus : type street fighter (joueur se bat avec un marteau et doit esquiver les bananes que donkey lance ?)
        }
        private void AfficherLesCredits()
        {

        }
        /*private void FinBonus()
        {
            if (/*le joueur obtient le jetpack*//*)
            {
                //deblocage du jeu bonus
            }
        }*/
    }


    /*Niveaux de difficultés : 
     * -facile : pas d'ennemis, juste Donkey qui tire des tonneaux, plateformes droites, beaucoup d'échelles
     * -moyen : peu d'ennemis, tir de tonneaux plus rapides, plateformes droites, un peu moins d'échelles
     * -difficile : plus d'ennemis, tir plus rapides et fréquents, plateformes penchées
     * les ennemis ne se déplacent pas, donkey suit le joueur mais est moins rapide 
     */

}

// Gestion du mouvement horizontal si nécessaire (gauche et droite)

