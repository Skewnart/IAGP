using Seron.Datas;
using Seron.Windows;
using SFML.Window;
using System.Windows;

namespace Seron
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Hide();

            UserData.Initialize();
            ProgramData.Initialize();

            //WorldWindow worldwindow = new WorldWindow(new VideoMode(1024, 768), "Fenêtre de jeu", "test", "landscape");
            WorldWindow worldwindow = new WorldWindow(new VideoMode(1600, 900), "Fenêtre de jeu", null, "landscape");
            worldwindow.Start();

            this.Close();
        }
    }
}
