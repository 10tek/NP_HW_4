using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace AudioMessage
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PORT = 3231;
        private const string IP_ADDRESS = "127.0.0.1";
        private UdpClient updSender;
        private UdpClient udpListeniner;
        private WaveIn input;
        private WaveOut output;
        private BufferedWaveProvider BufferStream;

        public MainWindow()
        {
            InitializeComponent();
            input = new WaveIn();
            input.WaveFormat = new WaveFormat(8000, 16, 1);
            input.DataAvailable += AudioInit;
            output = new WaveOut();
            BufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            output.Init(BufferStream);

            Task.Run(() => Listening());
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            input.StartRecording();
        }

        private async void AudioInit(object sender, WaveInEventArgs e)
        {
            updSender = new UdpClient();
            try
            {
                await updSender.SendAsync(e.Buffer, e.Buffer.Length, IP_ADDRESS, PORT);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task Listening()
        {
            udpListeniner = new UdpClient(PORT);
            output.Play();
            try
            {
                while (true)
                {
                    var data = await udpListeniner.ReceiveAsync();
                    BufferStream.AddSamples(data.Buffer, 0, data.Buffer.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            udpListeniner.Close();
        }
    }
}
