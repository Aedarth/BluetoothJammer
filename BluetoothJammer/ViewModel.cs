using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bluetooth.Services;
using Bluetooth.Model;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Diagnostics;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using System.IO;

namespace BluetoothJammer
{
    public class ViewModel : ViewModelBase
    {
        public SenderBluetoothService Service { get; set; } = new SenderBluetoothService();
        public ObservableCollection<Device> Devices { get; set; } = new ObservableCollection<Device>();

        public ViewModel()
        {
            GetDevices();
        }
        private async void GetDevices()
        {
            var oldDevices = new List<Device>();
            while(true)
            {
                var devices = await Service.GetDevices();
                if (devices.SequenceEqual(oldDevices))
                    continue;
                oldDevices = new List<Device>(devices);
                await App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    Devices.Clear();
                    foreach (var item in oldDevices)
                    {
                        Devices.Add(item);
                        //var success = await Service.Send(item, "Hi, sorry for inconvenience. We are trying to hack bluetooth");
                    }


                    foreach (var dev in Devices.Where(x => x.DeviceInfo.ClassOfDevice.Device == DeviceClass.AudioVideoLoudSpeaker))
                    {
                        try
                        {
                            Guid serviceClass;
                            serviceClass = BluetoothService.SerialPort;
                            var ep = new BluetoothEndPoint(dev.DeviceInfo.DeviceAddress, serviceClass);
                            var cli = new BluetoothClient();
                            cli.Connect(ep);
                            Stream peerStream = cli.GetStream();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                });
            }
        }
    }
}
