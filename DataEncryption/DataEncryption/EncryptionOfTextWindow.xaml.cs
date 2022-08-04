// <copyright file="EncryptionOfTextWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DataEncryption
{
    using System;
    using System.IO;
    using System.Numerics;
    using System.Text;
    using System.Windows;
    using Microsoft.Win32;

    /// <summary>
    /// Логика взаимодействия для EncryptionOfText.xaml.
    /// </summary>
    public partial class EncryptionOfTextWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionOfTextWindow"/> class.
        /// </summary>
        public EncryptionOfTextWindow()
        {
            InitializeComponent();
            dTextBlock.Visibility = Visibility.Hidden;
            nTextBlock.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Виклик функції шифрування тексту.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            TextEncryption();
        }

        /// <summary>
        /// Функція шифрування тексту.
        /// </summary>
        private void TextEncryption()
        {
            dTextBlock.Text = "d = ";
            nTextBlock.Text = "n = ";
            DateTime currentDateAndTime = DateTime.Now;
            FileStream fileWithEncryptedData = null;
            SaveFileDialog saveFileDial = new SaveFileDialog();
            saveFileDial.DefaultExt = ".txt";
            if (TextToEncryptionTextBox.Text.Length > 0)
            {
                int p = CalculateP();
                int q = CalculateQ();

                // Перевірка на нерівність p та q(Обов'язкова умова для шифрування)
                while (true)
                {
                    if (p != q)
                    {
                        break;
                    }

                    p = CalculateP();
                    q = CalculateQ();
                }

                string textToEncryption = TextToEncryptionTextBox.Text;
                int n = p * q;
                int m = (p - 1) * (q - 1);
                int d = CalculateD(m);
                int e = CalculateE(d, m);

                string encryptedText = RSA_Algorithm_Encrypt(textToEncryption, e, n);

                // Вибір, куди буде виводитися зашифрований текст
                if (TextBoxOutputCheckRadioButton.IsChecked == true)
                {
                    EncryptedTextTextBox.Text = string.Empty;
                    EncryptedTextTextBox.Text = encryptedText;
                }
                else if (FileOutputCheckRadioButton.IsChecked == true)
                {
                    // Вибір, куди зберігати зашифрований текст
                    if (MessageBox.Show("Ви хочете зберегти файл у теку за замовчуванням?", "Увага", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        Directory.CreateDirectory(@"Data\\Text");
                        fileWithEncryptedData = File.Create(@"Data\\Text\\EncryptedText " + currentDateAndTime.ToString("G").Replace(':', '_') + ".txt");
                        fileWithEncryptedData.Close();
                    }
                    else
                    {
                        if (saveFileDial.ShowDialog() == true)
                        {
                            fileWithEncryptedData = File.Create(saveFileDial.FileName);
                            fileWithEncryptedData.Close();
                        }
                    }

                    try
                    {
                        File.AppendAllText(fileWithEncryptedData.Name, encryptedText);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Файл не збережено", "Увага!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                dTextBlock.Visibility = Visibility.Visible;
                nTextBlock.Visibility = Visibility.Visible;
                dTextBlock.Text += d.ToString();
                nTextBlock.Text += n.ToString();
            }
            else
            {
                MessageBox.Show("Не введений текст для шифрування", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Вираховує просте число - p.
        /// </summary>
        /// <returns>Просте число.</returns>
        private int CalculateP()
        {
            Random rndNumber = new Random();
            int p = 0;
            while (true)
            {
                p = rndNumber.Next(19, 250);
                if (IsNumberSimple(p))
                {
                    break;
                }
            }

            return p;
        }

        /// <summary>
        /// Вираховує просте число - q.
        /// </summary>
        /// <returns>Просте число.</returns>
        private int CalculateQ()
        {
            Random rndNumber = new Random();
            int q = 0;
            while (true)
            {
                q = rndNumber.Next(17, 249);
                if (IsNumberSimple(q))
                {
                    break;
                }
            }

            return q;
        }

        /// <summary>
        /// Перевірка на простоту числа.
        /// </summary>
        /// <param name="number">Число, яке перевіряється на простоту.</param>
        /// <returns>Так чи ні, в залежності від того просте число, чи ні</returns>
        private bool IsNumberSimple(int number)
        {
            if (number < 2)
            {
                return false;
            }

            if (number == 2)
            {
                return true;
            }

            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Вираховування числа d, яке є взаємно простим з m.
        /// </summary>
        /// <param name="m">Число, яке вираховується за певною формулою.</param>
        /// <returns>Число d.</returns>
        private int CalculateD(int m)
        {
            int d = m - 1;

            for (int i = 2; i <= m; i++)
            {
                if ((m % i == 0) && (d % i == 0))
                {
                    d--;
                    i = 1;
                }
            }

            return d;
        }

        /// <summary>
        /// Вираховування числа e.
        /// </summary>
        /// <param name="d">Число, вирахуване за функцієб CalculateD.</param>
        /// <param name="m">Число, вирахуване за певною формулою.</param>
        /// <returns>Число e.</returns>
        private int CalculateE(int d, int m)
        {
            int e = 10;

            while (true)
            {
                if ((e * d) % m == 1)
                {
                    break;
                }
                else
                {
                    e++;
                }
            }

            return e;
        }

        /// <summary>
        /// Функція шифрування.
        /// </summary>
        /// <param name="textToEncryption">Текст, який потрібно зашифрувати</param>
        /// <param name="e">Відкритий ключ.</param>
        /// <param name="n">Максимальне значення блоку.</param>
        /// <returns>Зашифрований текст.</returns>
        private string RSA_Algorithm_Encrypt(string textToEncryption, int e, int n)
        {
            string binFromText = TxtToBin(textToEncryption);
            string[] intFromBinArray = BinToInt(binFromText).Split(' ');
            string encryptedText = string.Empty;
            BigInteger encryptedNumber;

            for (int i = 0; i < intFromBinArray.Length - 1; i++)
            {
                int integerFromBin = Convert.ToInt32(intFromBinArray[i]);

                encryptedNumber = new BigInteger(integerFromBin);
                encryptedNumber = BigInteger.Pow(encryptedNumber, e);

                BigInteger nSecond = new BigInteger(n);

                encryptedNumber = encryptedNumber % nSecond;

                encryptedText += encryptedNumber.ToString() + " ";
            }

            return encryptedText;
        }

        /// <summary>
        /// Перетворення тексту на двійковий код
        /// </summary>
        /// <param name="textToBin">Текст, який перетворюємо на двійковий код.</param>
        /// <returns>Двійковий код тексту</returns>
        private string TxtToBin(string textToBin)
        {
            string binFromText = string.Empty;
            foreach (byte b in Encoding.UTF8.GetBytes(textToBin))
            {
                binFromText += Convert.ToString(b, 2).PadLeft(8, '0');
            }

            return binFromText;
        }

        /// <summary>
        /// Перетворення двійкового коду в десяткову систему.
        /// </summary>
        /// <param name="binToInteger">Двійковий код, який потрібно перетворити на число.</param>
        /// <returns>Число, перетворене із двійкового коду.</returns>
        private string BinToInt(string binToInteger)
        {
            string integersFromBin = string.Empty;
            for (int i = 0; i < binToInteger.Length / 8; i++)
            {
                int intFromBin = Convert.ToInt32(binToInteger.Substring(i * 8, 8), 2);
                integersFromBin += intFromBin.ToString() + " ";
            }

            return integersFromBin.ToString();
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

        /// <summary>
        /// Відкриття вікна розшифрування тексту.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void ToDecryptionWindowButton_Click(object sender, RoutedEventArgs e)
        {
            DecryptionOfTextWindow decryptionOfText = new DecryptionOfTextWindow();
            decryptionOfText.Show();
            Close();
        }
    }
}
