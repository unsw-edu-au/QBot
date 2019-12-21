using EncryptionHelper;
using System.Windows;

namespace StringEncryption
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            var inputString = RawInputTextBox.Text;
            CryptoTransform cryptoTransform = new CryptoTransform(Helper.PASSPHRASE, Helper.INITVECTOR);

            var outputString = cryptoTransform.Encrypt(inputString);
            EncryptOutputTextBox.Text = outputString;
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            var inputString = EncryptInputTextBox.Text;
            CryptoTransform cryptoTransform = new CryptoTransform(Helper.PASSPHRASE, Helper.INITVECTOR);

            var outputString = cryptoTransform.Decrypt(inputString);
            DecryptOutputTextBox.Text = outputString;
        }

    }
}
