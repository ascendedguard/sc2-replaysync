namespace ReplaySync
{
    using System.Windows;

    using ReplaySync.Properties;

    /// <summary>
    /// Interaction logic for InputAddressDialog.xaml
    /// </summary>
    public partial class InputAddressDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputAddressDialog"/> class.
        /// </summary>
        public InputAddressDialog()
        {
            InitializeComponent();
            txtIPAddress.Text = Settings.Default.LastAddress;
        }

        /// <summary> Gets the address entered into the text field. </summary>
        public string Address
        {
            get
            {
                return txtIPAddress.Text;
            }
        }

        private void AcceptClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
