using Microsoft.IoT.DeviceCore.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace CurrentTemperature
{
    public class MockTemperatureSensor : IAnalogSensor
    {
        private Random _random;
        private DispatcherTimer _timer;
        private AnalogSensorReading _currentReading;

        public event TypedEventHandler<IAnalogSensor, AnalogSensorReadingChangedEventArgs> ReadingChanged;
        public int ReportInterval { get; private set; }

        public MockTemperatureSensor(int reportInterval)
        {
            ReportInterval = reportInterval;
            _random = new Random();
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(reportInterval)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var minimumTemperature = 0;
            var maximumTemperature = 100;
            var mockTemperature = _random.Next(minimumTemperature, maximumTemperature);
            _currentReading = new AnalogSensorReading(mockTemperature, (mockTemperature / maximumTemperature));
            if (ReadingChanged != null)
                ReadingChanged(this, new AnalogSensorReadingChangedEventArgs(_currentReading));
        }

        public void Dispose()
        {
            _timer.Stop();
        }

        public AnalogSensorReading GetCurrentReading()
        {
            return _currentReading;
        }
    }
}
