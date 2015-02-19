using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IphoneSimulator.Models;

namespace IphoneSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        bool _captured = false;
        double _xShape, _xCanvas, _yShape, _yCanvas;
        UIElement _source = null;
        double[] Radius = new double[3];
        readonly double[] _reference = { 1, 5, 10 };
        //private static Uri _uri = new Uri("http://localhost:54923/");
        private static Uri _uri = new Uri("http://ibeaconapp2.test.dropit.se/");
        List<Bus> _busList=new List<Bus>();

        public MainWindow()
        {
            InitializeComponent();
            var image = new ImageBrush() { ImageSource = new BitmapImage((new Uri(@"C:\Users\E601332\Documents\Visual Studio 2013\Projects\IphoneSimulator\IphoneSimulator\Images\bus.jpeg", UriKind.Absolute))) };
            
            Canvas.Background = image;
            
            double xp = (Canvas.GetLeft(Ellipse) - Ellipse.ActualWidth / 2) * .03;
            double yp = 2.55 - (Canvas.GetTop(Ellipse) - Ellipse.ActualHeight / 2) * .03;
            for (int i = 0; i < 3; i++)
            {
                Radius[i] = Calculate(xp, yp, _reference[i]);
            }
            BeaconLabel1.Content = Math.Round(Radius[0], 2).ToString();
            BeaconLabel2.Content = Math.Round(Radius[1], 2).ToString();
            Beaconlabel3.Content = Math.Round(Radius[2], 2).ToString();
            XPositionLabel.Content = "x=" + Math.Round(_xShape*33.3);
            Dropdown.Items.Add(new Item("Bus 1", 1));
            Dropdown.Items.Add(new Item("Bus 2", 2));
            Dropdown.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var id = Guid.NewGuid();
            IdBox.Items.Add(id);
            IdBox.SelectedItem = id;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            double x = (Canvas.GetLeft(Ellipse)-Ellipse.ActualWidth/2)*.03;
            double y = (Canvas.GetTop(Ellipse)-Ellipse.ActualHeight/2)*.03;
            var major = (Item)Dropdown.SelectedItem;
            var id = IdBox.Text;
            RunAsync(id, Radius,"Enter");
        }

        public static double Calculate(double x, double y, double reference)
        {
            return (Math.Sqrt(Math.Pow(x - reference, 2) + y * y));
        }

        private static async Task RunAsync(string id,double[] r,string type)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _uri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                string jsonString = "{\"Longitude\": \"123\",\"Latitude\": \"123\", \"Speed\": \"13.8888\", \"Course\": \"90\",\"DateTime\":\"Jan 21, 2015, 1:57 PM\","
                                    + "\"Type\" :  \""+type+"\",\"Id\" : \"" + id + "\","
                                    +
                                    "\"Beacons\": [{ \"UUID\" : \"73676723-7400-0000-ffff-0000ffff0005\", \"Major\" : 2,\"Minor\" : 682,\"Accuracy\" : \"" + r[0].ToString("G", culture) + "\",\"Proximity\" : \"Far\"},"
                                    +
                                    "{ \"UUID\" : \"73676723-7400-0000-ffff-0000ffff0006\",\"Major\" : 2,\"Minor\" : 682,\"Accuracy\" : \"" + r[1].ToString("G", culture) + "\",\"Proximity\" : \"Far\"},"
                                    +
                                    "{ \"UUID\" : \"73676723-7400-0000-ffff-0000ffff0007\",\"Major\" : 2,\"Minor\" : 682,\"Accuracy\" : \"" + r[2].ToString("G", culture) + "\",\"Proximity\" : \"Far\"}] }";

                await client.PostAsJsonAsync("api/Values", jsonString);
            }
        }

        private static async Task RunAsyncGet()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _uri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP GET
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                string jsonString = client.GetStringAsync("api/Values").Result;
                await client.PostAsJsonAsync("api/Values", jsonString);
            }
        }
        
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _source = (UIElement)sender;
            Mouse.Capture(_source);
            _captured = true;
            _xShape = Canvas.GetLeft(_source);
            _xCanvas = e.GetPosition(Canvas).X;
            _yShape = Canvas.GetTop(_source);
            _yCanvas = e.GetPosition(Canvas).Y;
        }
        
        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (_captured)
            {
                double x = e.GetPosition(Canvas).X;
                double y = e.GetPosition(Canvas).Y;
                _xShape += x - _xCanvas;
                Canvas.SetLeft(_source, _xShape);
                _xCanvas = x;
                _yShape += y - _yCanvas;
                Canvas.SetTop(_source, _yShape);
                _yCanvas = y;
                double xp = (Canvas.GetLeft(Ellipse) - Ellipse.ActualWidth / 2) * .03;
                double yp = 2.55-(Canvas.GetTop(Ellipse) - Ellipse.ActualHeight / 2) * .03;
                
                for (int i = 0; i < 3; i++)
                {
                    Radius[i] = Calculate(xp, yp, _reference[i]);
                }
                BeaconLabel1.Content = Math.Round(Radius[0],2).ToString();
                BeaconLabel2.Content = Math.Round(Radius[1],2).ToString();
                Beaconlabel3.Content = Math.Round(Radius[2],2).ToString();
                XPositionLabel.Content = "x=" + Math.Round(xp,2);
                YPositionLabel.Content = "y=" + Math.Round(yp,2);
            }
        }
        
        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            _captured = false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _uri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP GET
                string jsonString = client.GetStringAsync("api/Values").Result;
                _busList = new JavaScriptSerializer().Deserialize<List<Bus>>(jsonString);
                foreach (var passenger in _busList[0].Passengers)
                {
                    if (!IdBox.Items.Contains(passenger.Id)) IdBox.Items.Add(passenger.Id);
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
        
            double x = (Canvas.GetLeft(Ellipse)-Ellipse.ActualWidth/2)*.03;
            double y = (Canvas.GetTop(Ellipse)-Ellipse.ActualHeight/2)*.03;
            var major = (Item)Dropdown.SelectedItem;
            string id = IdBox.SelectedItem.ToString();
            RunAsync(id, Radius,"Exit");
        }

        private void DropDownClosed(object sender, EventArgs e)
        {
            string id = IdBox.Text;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            IdBox.Items.Clear(); 
            using (var client = new HttpClient())
            {
                client.BaseAddress = _uri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP GET
                string jsonString = client.GetStringAsync("api/Values").Result;
                _busList = new JavaScriptSerializer().Deserialize<List<Bus>>(jsonString);
                foreach (var passenger in _busList[0].Passengers)
                {
                    IdBox.Items.Add(passenger.Id);
                }
            }

        }
    }

    public class Item
    {
        public string Name;
        public int Value;
        public Item(string name, int value)
        {
            Name = name; Value = value;
        }
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Name;
        }
    }
}
