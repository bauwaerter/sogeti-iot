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
        #region constants
        private const string BaggageState = "7f910683-ef6d-46e9-8a1f-06f49b4491a6";
        private const string ScanTimestamp = "b06bd54b-4622-41a3-b888-7ac482ef65c5";
        private const string FirstName = "43438b74-7957-4c9c-998f-ba1c61a7b52c";
        private const string LastName = "f6f14ca9-a460-4682-a23d-bd3d4281fc4c";
        private const string PhoneNumber = "8a247ca9-ca66-4341-8f7a-098bfc193438";
        private const string BusId = "00887147-365e-46a7-8c49-965fe4fb784c";
        private const string RfId = "30180c14-b478-47dd-a98d-2804c4ae16f8";
        #endregion

        private bool NewDevice { get; set; }
        private ExistingDevice ExistingDevice { get; set; }
        private Device Device { get; set; }
        private string Token { get; set; }
        SerialPort mySerialPort;

        public MainWindow()
        {
            var port = SerialPort.GetPortNames()[0];
            string[] ports = SerialPort.GetPortNames();
            //var port = "COM4";

            mySerialPort = new SerialPort(port);

            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            //mySerialPort.Handshake = Handshake.None;
            //mySerialPort.RtsEnable = true;
            //mySerialPort.DtrEnable = true;

            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            mySerialPort.Open();
            
            InitializeComponent();
            
            //disable text-box UI in preperation for first scan
            HideElements(true);
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
            indata = indata.Replace("\r\n", string.Empty);
            indata = Regex.Replace(indata, @"[^0-9a-zA-Z]+", "");

            this.Dispatcher.Invoke(() =>
            {
                this.Cursor = Cursors.Wait;
                GreetingTextBlock.IsEnabled = false;
                RFIDTextBlock.Content = indata;
                RFIDTextBlock.IsEnabled = false;
            });

            mySerialPort.Close();
            ProcessRFID(indata);
        }

        private void ProcessRFID(string indata)
        {
            GetRefreshToken();
            CheckDevice(indata);
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            grid.Opacity = 0.15;
            HydrateDevice();
            CreateAndUpdateDevice();
        }

        private async void CheckDevice(string indata)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.us1.covisint.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json;fetchattributetypes=true;fetcheventtemplates=true;fetchcommandtemplates=true");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "device/v3/devices?name=" + indata);
            HttpResponseMessage response = await client.SendAsync(request);

            //put try catch around response and display error message. Have response of a 500 we can use saved in a txt
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            if (responseBody == "\n[\n]")
            {
                NewDevice = true;
                ExistingDevice = new ExistingDevice();
            }
            else
            {
                NewDevice = false;
                var jsonArr = JArray.Parse(responseBody);
                foreach (var item in jsonArr.Children())
                {
                    var myObj = item.ToString();
                    ExistingDevice = JsonConvert.DeserializeObject<ExistingDevice>(myObj);
                }
            }

            HideElements(false);
            PopulateUI();

            this.Dispatcher.Invoke(() =>
            {
                this.Cursor = Cursors.Arrow;
            });
        }

        private async void CreateAndUpdateDevice()
        {
            GetRefreshToken();

            if (NewDevice)
            {
                //create
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://api.us1.covisint.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "device/v3/tasks/createDeviceFromTemplate?deviceTemplateId=2ccb21ac-dc8c-44cd-a9a7-5b5ddf87fd43");
                HttpResponseMessage response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(responseBody);

                Device = JsonConvert.DeserializeObject<Device>(json.ToString());
            }

            PopulateDevice();

            //update
            var updateClient = new HttpClient();
            updateClient.BaseAddress = new Uri("https://api.us1.covisint.com/");
            updateClient.DefaultRequestHeaders.Accept.Clear();
            updateClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
            updateClient.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json");

            HttpRequestMessage updateRequest = new HttpRequestMessage(HttpMethod.Put, "/device/v3/devices/" + Device.id);
            string updateJson = JsonConvert.SerializeObject(Device);
            updateRequest.Content = new StringContent(updateJson, Encoding.UTF8, "application/vnd.com.covisint.platform.device.v2+json");

            HttpResponseMessage updateResponse = await updateClient.SendAsync(updateRequest);
            updateResponse.EnsureSuccessStatusCode();

            //activate
            var activateClient = new HttpClient();
            activateClient.BaseAddress = new Uri("https://api.us1.covisint.com/");
            activateClient.DefaultRequestHeaders.Accept.Clear();
            activateClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
            activateClient.DefaultRequestHeaders.Add("Accept", "application/vnd.com.covisint.platform.device.v2+json");

            HttpRequestMessage activateRequest = new HttpRequestMessage(HttpMethod.Post, "/device/v3/devices/" + Device.id + "/tasks/activate");
            HttpResponseMessage activateResponse = await activateClient.SendAsync(activateRequest);
            activateResponse.EnsureSuccessStatusCode();

            //register
            var registerClient = new HttpClient();
            registerClient.BaseAddress = new Uri("https://api.us1.covisint.com/");
            registerClient.DefaultRequestHeaders.Accept.Clear();
            registerClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
            registerClient.DefaultRequestHeaders.Add("Accept", "accept/json");

            HttpRequestMessage registerRequest = new HttpRequestMessage(HttpMethod.Post, "/device/v3/devices/" + Device.id + "/tasks/register");
            registerRequest.Content = new StringContent("{\"authNType\":\"NO_AUTH\"}", Encoding.UTF8, "application/vnd.com.covisint.platform.device.register.v1+json");
            HttpResponseMessage registerResponse = await registerClient.SendAsync(registerRequest);
            activateResponse.EnsureSuccessStatusCode();

            mySerialPort.Open();
            HideElements(true);
            this.Cursor = Cursors.Arrow;
            grid.Opacity = 1.00;
        }
        #region helper methods
        private void PopulateDevice()
        {
            Device.attributes.standard.Where(x => x.attributeTypeId == BaggageState).FirstOrDefault().value = 0;
            Device.attributes.standard.Where(x => x.attributeTypeId == ScanTimestamp).FirstOrDefault().value = GetEpochTime();
            Device.attributes.standard.Where(x => x.attributeTypeId == FirstName).FirstOrDefault().value = FNameTextBox.Text;
            Device.attributes.standard.Where(x => x.attributeTypeId == LastName).FirstOrDefault().value = LNameTextBox.Text;
            Device.attributes.standard.Where(x => x.attributeTypeId == PhoneNumber).FirstOrDefault().value = Convert.ToInt64(PhoneTextBox.Text);
            Device.attributes.standard.Where(x => x.attributeTypeId == BusId).FirstOrDefault().value = BusNumTextBox.Text;
            Device.attributes.standard.Where(x => x.attributeTypeId == RfId).FirstOrDefault().value = RFIDTextBlock.Content.ToString();
            Device.name.FirstOrDefault().text = RFIDTextBlock.Content.ToString();
        }

        private int GetEpochTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }

        private void HydrateDevice()
        {
            if (!NewDevice)
            {
                Device = new Device()
                {
                    id = ExistingDevice.id,
                    version = ExistingDevice.version,
                    creator = ExistingDevice.creator,
                    creatorAppId = ExistingDevice.creatorAppId,
                    creation = ExistingDevice.creation,
                    realm = ExistingDevice.realm,
                    name = ExistingDevice.name.Select(x => new Name { lang = x.lang, text = x.text }).ToArray(),
                    parentDeviceTemplateId = ExistingDevice.parentDeviceTemplateId,
                    state = new State { lifecycleState = ExistingDevice.state.lifecycleState, operationalState = ExistingDevice.state.operationalState },
                    attributes = new Attributes() { standard = ExistingDevice.attributes.standard.Select(x => new Standard() { attributeTypeId = x.attributeType.id, value = x.value.ToString() }).ToList() },
                    observableEvents = ExistingDevice.observableEvents.Select(x => x.id).ToArray(),
                    isActive = ExistingDevice.isActive
                };
            }
            else
            {
                Device = new Device() { attributes = new Attributes() { standard = new List<Standard>() } };
            }
        }

        private void GetRefreshToken()
        {
            var client = new RestClient("https://api.us1.covisint.com/oauth/v3/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Basic cGFOSFhMZ0lKeHpRSXRiakhPbTNWV1pFcUozc29NQWQ6U2JGTTg2S2o2OWpGajZlUg==");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var access_token = JObject.Parse(response.Content.ToString())["access_token"].ToString();

            Token = access_token;
        }

        private void PopulateUI()
        {
            if (!NewDevice)
            {
                this.Dispatcher.Invoke(() =>
                {
                    FNameTextBox.Text = ExistingDevice.attributes.standard.Where(x => x.attributeType.id == FirstName).FirstOrDefault().value.ToString();
                    LNameTextBox.Text = ExistingDevice.attributes.standard.Where(x => x.attributeType.id == LastName).FirstOrDefault().value.ToString();
                    PhoneTextBox.Text = ExistingDevice.attributes.standard.Where(x => x.attributeType.id == PhoneNumber).FirstOrDefault().value.ToString();
                    BusNumTextBox.Text = ExistingDevice.attributes.standard.Where(x => x.attributeType.id == BusId).FirstOrDefault().value.ToString();
                    RFIDTextBlock.Content = ExistingDevice.attributes.standard.Where(x => x.attributeType.id == RfId).FirstOrDefault().value.ToString();
                });
            }
            else
            {
                ClearText();
            }
        }

        private void ClearText()
        {
            this.Dispatcher.Invoke(() =>
            {
                FNameTextBox.Text = String.Empty;
                LNameTextBox.Text = String.Empty;
                PhoneTextBox.Text = String.Empty;
                BusNumTextBox.Text = String.Empty;
            });
        }

        private void HideElements(bool hideElements)
        {
            var hideGreeting = !hideElements ? Visibility.Hidden : Visibility.Visible;
            var hideOrShowElement = hideElements ? Visibility.Hidden : Visibility.Visible;

            this.Dispatcher.Invoke(() =>
            {
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
            });
        }
        #endregion
    }
}
