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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace IoT_Dallas_of_Things_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {        
        private Device Device { get; set; }

        public MainWindow()
        {
            SerialPort mySerialPort = new SerialPort("COM5");

            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.DtrEnable = true;

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

        private async void get_Click(object sender, RoutedEventArgs e)
        {
            var token = GetRefreshToken();
            string indata = "82003C81F1CE";

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.us1.covisint.com/");

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json;fetchattributetypes=true;fetcheventtemplates=true;fetchcommandtemplates=true");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "device/v3/devices?name=" + indata);

            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync(); 

            if (responseBody == "\n[\n]") {

            }
            else
            {
                var jsonArr = JArray.Parse(responseBody);
                foreach (var item in jsonArr.Children())
                {
                    var myObj = item.ToString();
                    Device = JsonConvert.DeserializeObject<Device>(myObj);
                }

                HideElements(false);

                PopulateUI();
            } 
        }

        private async void create_Click(object sender, RoutedEventArgs e)
        {
            //Client ID: paNHXLgIJxzQItbjHOm3VWZEqJ3soMAd
            //Client Secret: SbFM86Kj69jFj6eR
            var token = GetRefreshToken();

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.us1.covisint.com/");

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "device/v3/tasks/createDeviceFromTemplate?deviceTemplateId=2ccb21ac-dc8c-44cd-a9a7-5b5ddf87fd43");
            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(responseBody);

            Device = JsonConvert.DeserializeObject<Device>(json.ToString());

            Device.name[0].text = "testing";
        }

        private string GetRefreshToken()
        {
            var client = new RestClient("https://api.us1.covisint.com/oauth/v3/token");
            var request = new RestRequest(Method.POST);            
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Basic cGFOSFhMZ0lKeHpRSXRiakhPbTNWV1pFcUozc29NQWQ6U2JGTTg2S2o2OWpGajZlUg==");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var access_token = JObject.Parse(response.Content.ToString())["access_token"].ToString();
           
            return access_token;
        }

        private async void update_Click(object sender, RoutedEventArgs e)
        {
            //deviceID = "4c295e3f-2b97-47eb-9798-10f92058418e";

            var token = GetRefreshToken();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.us1.covisint.com/");

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "/device/v3/devices/" + Device.id);
            string json = JsonConvert.SerializeObject(Device);
            request.Content = new StringContent(json, Encoding.UTF8, "application/vnd.com.covisint.platform.device.v2+json");
            
            HttpResponseMessage response = await client.SendAsync(request);
        }

        private void scan_Click(object sender, RoutedEventArgs e)
        {            
            RFIDTextBlock.IsEnabled = false;
            HideElements(false);
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {            
            HideElements(true);
        }

        private void PopulateUI()
        {
            FNameTextBox.Text = Device.attributes.standard.Where(x => x.attributeType.name == "First Name").FirstOrDefault().value;
            LNameTextBox.Text = Device.attributes.standard.Where(x => x.attributeType.name == "Last Name").FirstOrDefault().value;
            PhoneTextBox.Text = Device.attributes.standard.Where(x => x.attributeType.name == "Phone Number").FirstOrDefault().value;
            BusNumTextBox.Text = Device.attributes.standard.Where(x => x.attributeType.name == "Bus ID").FirstOrDefault().value;
        }

        private void HideElements(bool hideElements)
        {
            var hideGreeting = !hideElements ? Visibility.Hidden : Visibility.Visible;
            var hideOrShowElement = hideElements ? Visibility.Hidden : Visibility.Visible;

            GreetingTextBlock.Visibility = hideGreeting;

            RFIDTextBlock.Visibility = hideOrShowElement;
            FNameTextBox.Visibility = hideOrShowElement;
            LNameTextBox.Visibility = hideOrShowElement;
            PhoneTextBox.Visibility = hideOrShowElement;            
            BusNumTextBox.Visibility = hideOrShowElement;

            TagLabel.Visibility = hideOrShowElement;
            BusLabel.Visibility = hideOrShowElement;
            FirstNameLabel.Visibility = hideOrShowElement;
            LastNameLabel.Visibility = hideOrShowElement;
            PhoneLabel.Visibility = hideOrShowElement;            

            CheckInButton.Visibility = hideOrShowElement;
        }
    }
}
