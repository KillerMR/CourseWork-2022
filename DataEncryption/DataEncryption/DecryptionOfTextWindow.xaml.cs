namespace DataEncryption
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Windows;
    using Microsoft.Win32;

    /// <summary>
    /// Логика взаимодействия для DecryptionOfText.xaml.
    /// </summary>
    public partial class DecryptionOfTextWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecryptionOfTextWindow"/> class.
        /// </summary>
        public DecryptionOfTextWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Виклик функції шифрування тексту.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            TextDecryption();
        }

        /// <summary>
        /// Функція шифрування тексту.
        /// </summary>
        private void TextDecryption()
        {
            string pathToEncryptedText = string.Empty;
            FileStream fileWithDecryptedData = null;
            DateTime currentDateAndTime = DateTime.Now;
            OpenFileDialog openFileDial = new OpenFileDialog();
            SaveFileDialog saveFileDial = new SaveFileDialog();
            saveFileDial.DefaultExt = ".txt";

            if ((NumberDTextBox.Text.Length > 0) && (NumberNTextBox.Text.Length > 0))
            {
                int d = Convert.ToInt32(NumberDTextBox.Text);
                int n = Convert.ToInt32(NumberNTextBox.Text);
                string encryptedText = string.Empty;

                // В залежності від вибраного радіобатона, то і буде джерелом тексту
                if (TextBoxInputCheckRadioButton.IsChecked == true)
                {
                    encryptedText = EncryptedTextTextBox.Text;
                }
                else if (FileInputCheckRadioButton.IsChecked == true)
                {
                    if (openFileDial.ShowDialog() == true)
                    {
                        pathToEncryptedText = openFileDial.FileName;
                    }

                    try
                    {
                        encryptedText = File.ReadAllText(pathToEncryptedText);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не вибрано файл", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                string decryptedText = RSA_Algorithm_Decrypt(encryptedText, d, n);

                if (TextBoxOutputCheckRadioButton.IsChecked == true)
                {
                    if (decryptedText == "##")
                    {
                        decryptedText = string.Empty;
                    }

                    DecryptedTextTextBox.Text = decryptedText;
                }
                else if (FileOutputCheckRadioButton.IsChecked == true)
                {
                    // Користувач може вибрати куди зберігати файл
                    if (MessageBox.Show("Ви хочете зберегти файл у теку за замовчуванням?", "Увага", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        Directory.CreateDirectory(@"Data\\Text");
                        fileWithDecryptedData = File.Create(@"Data\\Text\\DecryptedText " + currentDateAndTime.ToString("G").Replace(':', '_') + ".txt");
                        fileWithDecryptedData.Close();
                    }
                    else
                    {
                        if (saveFileDial.ShowDialog() == true)
                        {
                            fileWithDecryptedData = File.Create(saveFileDial.FileName);
                            fileWithDecryptedData.Close();
                        }
                    }

                    if (fileWithDecryptedData != null)
                    {
                        File.AppendAllText(fileWithDecryptedData.Name, decryptedText);
                        fileWithDecryptedData.Close();
                    }
                    else
                    {
                        MessageBox.Show("Файл не збережено", "Увага!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Не введено ключі", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Розшифрування тексту за RSA-алгоритмом.
        /// </summary>
        /// <param name="encryptedText">Зашифрований текст.</param>
        /// <param name="d">Секретний ключ.</param>
        /// <param name="n">Максимальне значення блоку.</param>
        /// <returns>Розшифрований текст.</returns>
        private string RSA_Algorithm_Decrypt(string encryptedText, int d, int n)
        {
            string[] encryptedNumbersArray = encryptedText.Split(' ');
            string binFromNumber = string.Empty;

            BigInteger decryptedNumber;

            foreach (string encryptedNumber in encryptedNumbersArray)
            {
                if (encryptedNumber == string.Empty)
                {
                    break;
                }

                try
                {
                    decryptedNumber = new BigInteger(Convert.ToDouble(encryptedNumber));
                }
                catch (Exception ex)
                {
                    TextBoxInputCheckRadioButton.IsChecked = true;
                    MessageBox.Show("Виберіть, будь ласка, коректний файл", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }

                // Піднесення зашифрованого числа в степінь
                decryptedNumber = BigInteger.Pow(decryptedNumber, d);

                BigInteger nSecond = new BigInteger(n);

                try
                {
                    // Ділення по модулю зашифрованого числа на n
                    decryptedNumber = decryptedNumber % nSecond;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("N не може бути нулем", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                int decryptedInteger = Convert.ToInt32(decryptedNumber.ToString());

                binFromNumber += IntToBin(decryptedInteger.ToString());
            }

            return BinToTxt(binFromNumber);
        }

        /// <summary>
        /// Перетворення цілого числа в двійкове представлення.
        /// </summary>
        /// <param name="integerToBin">Число яке потрібно перевести в двійкову систему.</param>
        /// <returns>Двійковий код числа.</returns>
        private string IntToBin(string integerToBin)
        {
            // Рядок конвертується в двійкову систему із десяткової
            string binFromNumber = Convert.ToString(Convert.ToInt32(integerToBin), 2);
            return binFromNumber.PadLeft(8, '0');
        }

        /// <summary>
        /// Перетворює масив байтів на рядок.
        /// </summary>
        /// <param name="binToText">Двійковий код, який потрібно перетворити на текст.</param>
        /// <returns>Рядок, взятий із масива байтів.</returns>
        private string BinToTxt(string binToText)
        {
            // Беруться кожні 8 бітів і перетворюються на байти, із яких складається масив
            byte[] bytesFromBin = Enumerable.Range(0, binToText.Length / 8).Select(i => Convert.ToByte(binToText.Substring(i * 8, 8), 2)).ToArray();
            string txtFromBin = Encoding.UTF8.GetString(bytesFromBin);
            return txtFromBin;
        }

        /// <summary>
        /// Відкриття вікна шифрування тексту.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void ToEncryptionWindowButton_Click(object sender, RoutedEventArgs e)
        {
            EncryptionOfTextWindow encryptionOfText = new EncryptionOfTextWindow();
            encryptionOfText.Show();
            Close();
        }

        /// <summary>
        /// Відкриття головного вікна.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void ToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            Close();
        }
    }
}
