// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Ascend">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   ViewModel for the MainWindow, handling all application logic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReplaySync
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using ReplaySync.MVVM;

    /// <summary> ViewModel for the MainWindow, handling all application logic. </summary>
    public class MainWindowViewModel : ObservableObject
    {
        /// <summary> Current dispatcher used for updating databound fields. </summary>
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary> Timer used for updating the listening thread. </summary>
        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render, Dispatcher.CurrentDispatcher);

        /// <summary> Command backing for the ListenCommand property. </summary>
        private ICommand listenCommand;

        /// <summary> Command backing for the SyncCommand property. </summary>
        private ICommand syncCommand;

        /// <summary> Field backing for the IPAddressText property. </summary>
        private string ipAddressText;

        /// <summary> Field backing for the CaptureImage property. </summary>
        private BitmapSource captureImage;

        /// <summary> Indicates all running threads should stop repeating. </summary>
        private bool stopListen;

        /// <summary> Initializes a new instance of the <see cref="MainWindowViewModel"/> class. </summary>
        public MainWindowViewModel()
        {
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            this.timer.Tick += this.TimerTick;
        }

        /// <summary> Gets the command initializing the "Listen" process. </summary>
        public ICommand ListenCommand
        {
            get
            {
                return this.listenCommand ?? (this.listenCommand = new RelayCommand(this.Listen));
            }
        }

        /// <summary> Gets a command for connecting to an IP address and beginning the sync process. </summary>
        public ICommand SyncCommand
        {
            get
            {
                return this.syncCommand ?? (this.syncCommand = new RelayCommand(this.Sync));
            }
        }

        /// <summary> Gets or sets the IP address to connect to, in string format. </summary>
        public string IPAddressText
        {
            get
            {
                return this.ipAddressText;
            }

            set
            {
                this.ipAddressText = value;
                RaisePropertyChanged("IPAddressText");
            }
        }

        /// <summary> Gets or sets the current captured image. </summary>
        public BitmapSource CaptureImage
        {
            get
            {
                return this.captureImage;
            }

            set
            {
                this.captureImage = value;
                RaisePropertyChanged("CaptureImage");
            }
        }

        /// <summary> Stops all running threads and stops the listening timer. Deallocates all memory. </summary>
        public void Shutdown()
        {
            this.stopListen = true;
            this.timer.Stop();
            CaptureScreenshot.Destroy();
        }

        /// <summary> Opens a listening UDP port to broadcast timer updates. </summary>
        private void Listen()
        {
            // Start the capture timer.
            this.timer.Start();

            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint endPoint = null;
            
            foreach (var entry in hostEntry.AddressList)
            {
                if (entry.AddressFamily == AddressFamily.InterNetwork)
                {
                    endPoint = new IPEndPoint(entry, 11000);
                }
            }             

            if (endPoint == null)
            {
                MessageBox.Show("This application does not support IPv6 yet.");
                return;
            }

            var socket = new Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
                { ReceiveTimeout = 1000 };
            socket.Bind(endPoint);

            var thr = new Thread(
                new ThreadStart(
                    delegate
                        {
                            while (this.stopListen == false)
                            {
                                var inBuffer = new byte[100];
                                EndPoint senderRemote = new IPEndPoint(IPAddress.Any, 0);

                                try
                                {
                                    socket.ReceiveFrom(inBuffer, ref senderRemote);
                                }
                                catch (SocketException)
                                {
                                    continue;
                                }

                                if (inBuffer[0] == 0x01)
                                {
                                    while (this.CaptureImage == null)
                                    {
                                        Thread.Sleep(250);
                                    }

                                    Action act = delegate
                                        {
                                            var bmp = this.CaptureImage;

                                            if (bmp == null)
                                            {
                                                return;
                                            }

                                            var encoder = new JpegBitmapEncoder();
                                            encoder.Frames.Add(BitmapFrame.Create(bmp));
                                            
                                            var stream = new MemoryStream();
                                            encoder.Save(stream);

                                            socket.SendTo(stream.ToArray(), senderRemote);
                                        };

                                    this.dispatcher.BeginInvoke(act);
                                }                                
                            }
                        }));

            thr.Start();
        }

        /// <summary> Connects to an IP address and syncs the time between computers. </summary>
        private void Sync()
        {
            string str = this.IPAddressText;
            var nums = str.Split('.');

            if (nums.Length != 4)
            {
                return;
            }

            var ip = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                ip[i] = byte.Parse(nums[i]);
            }

            var endPoint = new IPEndPoint(new IPAddress(ip), 11000);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) { ReceiveTimeout = 1000 };

            socket.Connect(endPoint);

            var thr = new Thread(
                new ThreadStart(
                    delegate
                    {
                        var buffer = new byte[5000];
  
                        while (this.stopListen == false)
                        {
                            socket.Send(new byte[] { 0x01 });

                            try
                            {
                                socket.Receive(buffer);
                            }
                            catch (SocketException)
                            {
                                continue;
                            }

                            using (var stream = new MemoryStream(buffer))
                            {
                                var decoder = new JpegBitmapDecoder(
                                    stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                                var frame = decoder.Frames[0];
                                this.CaptureImage = frame;
                            }

                            Thread.Sleep(250);
                        }
                    }));

            thr.Start();
        }

        /// <summary> Grabs a screen capture of the game timer on every tick. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void TimerTick(object sender, EventArgs e)
        {
            var rect = new Rect(15, 775, 80, 21);
            var img = CaptureScreenshot.Capture(rect);
            this.CaptureImage = img;
        }
    }
}
