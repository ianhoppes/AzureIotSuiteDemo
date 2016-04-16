using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrentTemperature
{
    public class AzureIoTHub
    {
        private const string DeviceConnectionString = "HostName=gabcdemohub.azure-devices.net;DeviceId=gabcdemodevice;SharedAccessKey=91L+zunmdziMi55xm4+eLk0u55PqWCv0JF38EEeCwnc=";
        private DeviceClient _deviceClient;

        public AzureIoTHub()
        {
            // Create the IoT Hub Device Client instance
            _deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
        }

        // Create a message and send it to IoT Hub.
        public async Task SendEvent(SensorDataContract sensorData)
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Http1);

            var text = JsonConvert.SerializeObject(sensorData);
            var msg = new Message(Encoding.UTF8.GetBytes(text));

            await deviceClient.SendEventAsync(msg);

            string dataBuffer;
            dataBuffer = Guid.NewGuid().ToString();
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
            await _deviceClient.SendEventAsync(eventMessage);
        }

        // Receive messages from IoT Hub
        public async Task ReceiveCommands()
        {
            Message receivedMessage;
            string messageData;
            while (true)
            {
                receivedMessage = await _deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1));

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await _deviceClient.CompleteAsync(receivedMessage);
                }
            }
        }
    }
}
