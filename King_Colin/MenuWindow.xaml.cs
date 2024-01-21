using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace King_Colin
{
    /// <summary>
    /// Logique d'interaction pour MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private ImageBrush imageMenu = new ImageBrush();

        private MediaPlayer musiqueMenu = new MediaPlayer();

        public int SelectedDifficultyIndex { get; private set; }

        public MenuWindow()
        {
            InitializeComponent();
            imageMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Menu.jpg"));
            FondMenu.Fill = imageMenu;
            musiqueMenu.MediaEnded += MusiqueMenu_Fin;
        }

        private void MenuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Musique/TheDying2.mp3", UriKind.Relative);
            musiqueMenu.Open(uri);
            musiqueMenu.Play();
        }

        private void MusiqueMenu_Fin(object sender, EventArgs e)
        {
            musiqueMenu.Stop();
            musiqueMenu.Play();
        }
        private void cb_Difficulte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Difficulte.SelectedIndex >= 0)
            {
                SelectedDifficultyIndex = cb_Difficulte.SelectedIndex;
            }
        }

        private void bt_Jouer_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            musiqueMenu.Stop();
        }
    }
}
