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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.IO.Ports;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace IoT_Dallas_of_Things_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            SerialPort mySerialPort = new SerialPort("COM5");

            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;

            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            //mySerialPort.Open();

            InitializeComponent();

            //disable text-box UI in preperation for first scan
            HideElements(true);
            
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();


            this.Dispatcher.Invoke(() =>
            {
                GreetingTextBlock.Text = "";
                RFIDTextBlock.Content = indata;
                //FNameTextBlock.Text = "First Name:";
            });
        }

        private async void create_Click(object sender, RoutedEventArgs e)
        {
            //Client ID: paNHXLgIJxzQItbjHOm3VWZEqJ3soMAd
            //Client Secret: SbFM86Kj69jFj6eR
            var token = "Bearer 2|sRATc5iweO8FUfjd4bQQjw4ACMaL";
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.us1.covisint.com/");


            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json");
            client.DefaultRequestHeaders.Add("Authorization", token);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "device/v3/tasks/createDeviceFromTemplate?deviceTemplateId=2ccb21ac-dc8c-44cd-a9a7-5b5ddf87fd43");
            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            responseBody = responseBody.Replace("\"", "").Replace(@"\", "");

            string tagID = getBetween(responseBody, "id:", ",");
        }


        private async void update_Click(object sender, RoutedEventArgs e)
        {
            string tagID = "4c295e3f-2b97-47eb-9798-10f92058418e";

            var token = "Bearer 2|sRATc5iweO8FUfjd4bQQjw4ACMaL";
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.us1.covisint.com/");

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json;fetchattributetypes=true;fetcheventtemplates=true;fetchcommandtemplates=true");
            client.DefaultRequestHeaders.Add("Authorization", token);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/device/v3/devices/" + tagID);
            HttpResponseMessage response = await client.SendAsync(request);

            //response = await client.PostAsync("device/" + tagID, requestContent);

            //var requestContent = new FormUrlEncodedContent(new[] {
            //new KeyValuePair<string, string>("00887147-365e-46a7-8c49-965fe4fb784c", "1"),
            //    });
            //HttpResponseMessage response = await client.PostAsync("/devices/" + tagID, requestContent);



        }


        private void scan_Click(object sender, RoutedEventArgs e)
        {
            GreetingTextBlock.Visibility = Visibility.Hidden;            
            RFIDTextBlock.IsEnabled = false;
            HideElements(false);
        }
        
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            GreetingTextBlock.Visibility = Visibility.Visible;
            HideElements(true);            
        }

        private void HideElements(bool hideElements)
        {
            var hideOrShowElement = hideElements ? Visibility.Hidden : Visibility.Visible;

            RFIDTextBlock.Visibility = hideOrShowElement;
            FNameTextBox.Visibility = hideOrShowElement;
            LNameTextBox.Visibility = hideOrShowElement;
            PhoneTextBox.Visibility = hideOrShowElement;
            AddressTextBox.Visibility = hideOrShowElement;
            BusNumTextBox.Visibility = hideOrShowElement;

            TagLabel.Visibility = hideOrShowElement;
            BusLabel.Visibility = hideOrShowElement;
            FirstNameLabel.Visibility = hideOrShowElement;
            LastNameLabel.Visibility = hideOrShowElement;
            PhoneLabel.Visibility = hideOrShowElement;
            AddressLabel.Visibility = hideOrShowElement;

            CheckInButton.Visibility = hideOrShowElement;
        }
    }
}
