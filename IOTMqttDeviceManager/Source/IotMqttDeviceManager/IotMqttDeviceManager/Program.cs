using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.Devices.Client;
using AZD = Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace IotMqttDeviceManager
{
    class Program
    {

        private static int kCounter = 0;
        private static List<DeviceClient> kDeviceConnectionList = null;
        private static AZD.RegistryManager kRegistryManager = null;        
        private static string kIoTHubConnectionString = "HostName=iotmqtthub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=dQopZGKDzsLk7OEgEtMGqUCAplZXykbV4GktopSeTdo=";
        private static string kDeviceConnectionString = "HostName=iotmqtthub.azure-devices.net;DeviceId={0};SharedAccessKey={1}";
        private static int kTotalDevices = 32;
        private static int kRemoveMaxCount = 32;        

        private static async Task AddDevicesAsync()
        {

            kRegistryManager = AZD.RegistryManager.CreateFromConnectionString(kIoTHubConnectionString);            
            kDeviceConnectionList = new List<DeviceClient>();

            for (int idx = 0; idx < kTotalDevices; ++idx)
            {

                var deviceIdString = $"dev{idx}";
                try
                {

                    var device = await kRegistryManager.AddDeviceAsync(new AZD.Device(deviceIdString));
                    var connString = device.Authentication.SymmetricKey.PrimaryKey;
                    connString = string.Format(kDeviceConnectionString, deviceIdString, connString);

                    Console.WriteLine(connString);

                    kDeviceConnectionList.Add(DeviceClient.CreateFromConnectionString(connString));
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);

                }


            }

        }
        private static async Task RemoveDevicesAsync()
        {

            kRegistryManager = AZD.RegistryManager.CreateFromConnectionString(kIoTHubConnectionString);

            do
            {

                var devicesList = (await kRegistryManager.GetDevicesAsync(kRemoveMaxCount)).ToList();
                if (devicesList?.Count == 0)
                    break;

                try
                {

                    await kRegistryManager.RemoveDevices2Async(devicesList);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);

                }

            }while (true);
            
        }
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello DM!");
            await RemoveDevicesAsync();
            await AddDevicesAsync();
            await Console.In.ReadLineAsync();
            
        }
    }
}
