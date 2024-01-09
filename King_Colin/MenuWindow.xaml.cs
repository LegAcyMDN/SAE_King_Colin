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
    /// Logique d'interaction pour MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private ImageBrush imageMenu = new ImageBrush();

        private MediaPlayer musiqueMenu = new MediaPlayer();

        public MenuWindow()
        {
            InitializeComponent();
            imageMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Img/Menu.jpg"));
            FondMenu.Fill = imageMenu;
            musiqueMenu.MediaEnded += MusiqueMenu_Fin;
        }

        private void MenuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Musique/TheDying.mp3", UriKind.Relative);
            musiqueMenu.Open(uri);
            musiqueMenu.Play();
        }

        private void MusiqueMenu_Fin(object sender, EventArgs e)
        {
            musiqueMenu.Stop();
            musiqueMenu.Play();
        }
    }
}
