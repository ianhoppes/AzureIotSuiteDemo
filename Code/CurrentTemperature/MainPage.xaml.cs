using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Microsoft.IoT.DeviceCore.Sensors;
using Microsoft.IoT.Devices.Adc;
using Windows.Devices.Gpio;
using Microsoft.IoT.DeviceCore.Adc;
using Microsoft.IoT.Devices.Sensors;
using System.Diagnostics;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CurrentTemperature
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int ReportInterval = 5000;   // milliseconds
        private AdcProviderManager _adcManager;
        private IAnalogSensor _temperatureSensor;
        private AzureIoTHub _hub;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Start GPIO
            //var gpioController = GpioController.GetDefault();
            //if (gpioController != null)
            //{
            //    CreateAdc();
            //    _temperatureSensor = await CreateTmp36Sensor();
            //}
            //else
                _temperatureSensor = CreateMockSensor();

            _hub = new AzureIoTHub();
        }

        private void CreateAdc()
        {
            // Create ADC manager
            _adcManager = new AdcProviderManager();

            // Add ADC chips
            _adcManager.Providers.Add(
                new MCP3008()
                {
                    ChannelMode = 0
                });
        }

        private async Task<IAnalogSensor> CreateTmp36Sensor()
        {
            var adcControllers = await _adcManager.GetControllersAsync();

            var tmp36Sensor = new AnalogSensor()
            {
                AdcChannel = adcControllers[0].OpenChannel(0),
                ReportInterval = ReportInterval
            };
            tmp36Sensor.ReadingChanged += Tmp36Sensor_ReadingChanged;
            return tmp36Sensor;
        }

        private void Tmp36Sensor_ReadingChanged(IAnalogSensor sender, AnalogSensorReadingChangedEventArgs args)
        {
            var reading = ConvertReadingToFahrenheit(args.Reading.Value);
            SendToAzure(reading);
            UpdateUi(reading);
        }

        private double ConvertReadingToFahrenheit(double reading)
        {
            var milliVolts = reading * (3300.0 / 1024.0);
            var tempC = (milliVolts - 500) / 10;
            var tempF = (tempC * 9.0 / 5.0) + 32;
            return tempF;
        }

        private IAnalogSensor CreateMockSensor()
        {
            var mockSensor = new MockTemperatureSensor(ReportInterval);
            mockSensor.ReadingChanged += MockSensor_ReadingChanged;
            return mockSensor;
        }

        private void MockSensor_ReadingChanged(IAnalogSensor sender, AnalogSensorReadingChangedEventArgs args)
        {
            var reading = args.Reading.Value;
            SendToAzure(reading);
            UpdateUi(reading);
        }

        private async void SendToAzure(double temperature)
        {
            await _hub.SendEvent(new SensorDataContract
            {
                Guid = Guid.NewGuid().ToString(),
                Organization = "RDU Global Azure Bootcamp",
                DisplayName = "Temperature",
                Location = "Durham, NC",
                MeasureName  = "Temperature",
                UnitOfMeasure = "F",
                Value = temperature,
                TimeCreated = DateTime.UtcNow
            });
        }

        private async void UpdateUi(double temperature)
        {
            await Dispatcher.RunIdleAsync((s) =>
            {
                Temperature.Text = Math.Round(temperature, 0, MidpointRounding.AwayFromZero).ToString();
                if (temperature >= 85)
                    Temperature.Foreground = new SolidColorBrush(Colors.Red);
                else if (temperature <= 32)
                    Temperature.Foreground = new SolidColorBrush(Colors.Blue);
                else
                    Temperature.Foreground = new SolidColorBrush(Colors.Green);
            });
        }
    }
}
