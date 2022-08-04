// <copyright file="DecryptionOfPictureWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DataEncryption
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Numerics;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Microsoft.Win32;

    /// <summary>
    /// Логика взаимодействия для DecryptionOfPicture.xaml.
    /// </summary>
    public partial class DecryptionOfPictureWindow : Window
    {
        private static string pathToEncryptedPicture = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecryptionOfPictureWindow"/> class.
        /// </summary>
        public DecryptionOfPictureWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Відкриття діалогового вікна для вибору текстового файлу.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void OpenTextFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDial = new OpenFileDialog();
            if (openFileDial.ShowDialog() == true)
            {
                pathToEncryptedPicture = openFileDial.FileName;
            }
        }

        /// <summary>
        /// Виклик функції розшифрування картинки.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void DecryptPictureButton_Click(object sender, RoutedEventArgs e)
        {
            PictureDecryption();
        }

        /// <summary>
        /// Функція розшифрування картинки.
        /// </summary>
        private void PictureDecryption()
        {
            SaveFileDialog saveFileDial = new SaveFileDialog();
            saveFileDial.DefaultExt = ".jpg";
            FileStream fileWithDecryptedData;

            if ((NumberDTextBox.Text.Length > 0) && (NumberNTextBox.Text.Length > 0) && pathToEncryptedPicture.Length > 0)
            {
                int d = Convert.ToInt32(NumberDTextBox.Text);
                int n = Convert.ToInt32(NumberNTextBox.Text);
                string encryptedPicture = File.ReadAllText(pathToEncryptedPicture);

                string decryptedPicture = RSA_Algorithm_Decrypt(encryptedPicture, d, n);

                Bitmap convertedPicture = PixelsToPicture(decryptedPicture);
                if (convertedPicture != null)
                {
                    try
                    {
                        if (saveFileDial.ShowDialog() == true)
                        {

                            fileWithDecryptedData = File.Create(saveFileDial.FileName);
                            fileWithDecryptedData.Close();


                        }

                        // Збереження фото на диск та вивід його на вікно
                        convertedPicture.Save(saveFileDial.FileName);
                        PictureImage.Source = new BitmapImage(new Uri(saveFileDial.FileName));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Файл не збережено", "Увага!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                }
                }
            else
            {
                MessageBox.Show("Ви не ввели ключі, або не вибрали файл", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Збирання RGB значень в пікселі, та створення із цих пікселів картинки.
        /// </summary>
        /// <param name="decryptedPicture">Розшифровані RGB значеня пікселів.</param>
        /// <returns>Розшифрована картинка.</returns>
        private Bitmap PixelsToPicture(string decryptedPicture)
        {
            Bitmap convertedPicture = null;
            try
            {
                string sizeOfPicture = File.ReadAllText(pathToEncryptedPicture).Split(',')[2];
                string[] pixelArray = decryptedPicture.Split(',');
                convertedPicture = new Bitmap(Convert.ToInt32(sizeOfPicture.Split(' ')[0]), Convert.ToInt32(sizeOfPicture.Split(' ')[1]));
                int pixelNumber = 0;

                // Перебір всіх пікселів і задання їм RGB - значень
                for (int i = 0; i < Convert.ToInt32(sizeOfPicture.Split(' ')[1]); i++)
                {
                    for (int j = 0; j < Convert.ToInt32(sizeOfPicture.Split(' ')[0]); j++)
                    {
                        convertedPicture.SetPixel(j, i, Color.FromArgb(Convert.ToInt32(pixelArray[pixelNumber].Split(' ')[0]), Convert.ToInt32(pixelArray[pixelNumber].Split(' ')[1]), Convert.ToInt32(pixelArray[pixelNumber].Split(' ')[2])));
                        pixelNumber++;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return convertedPicture;
        }

        /// <summary>
        /// Розшифрування RGB значень картинки.
        /// </summary>
        /// <param name="pictureToDecrypt">Зашифровані RGB значення картинки.</param>
        /// <param name="d">Секретний ключ.</param>
        /// <param name="n">Максимальне значення блоку.</param>
        /// <returns>Розшифровані RGB значення картинки.</returns>
        private string RSA_Algorithm_Decrypt(string pictureToDecrypt, int d, int n)
        {
            string[] rgbRemaindAndSize = pictureToDecrypt.Split(',');
            string[] encryptedRGBValuesArray = rgbRemaindAndSize[0].Split(' ');
            string binFromRGB = string.Empty;
            string rgbFromBin = string.Empty;

            BigInteger decryptedNumber;

            foreach (string encryptedRGBValue in encryptedRGBValuesArray)
            {
                if (encryptedRGBValue == string.Empty)
                {
                    break;
                }

                try
                {
                    decryptedNumber = new BigInteger(Convert.ToDouble(encryptedRGBValue));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Виберіть, будь ласка, коректний файл", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }

                // Піднесення зашифрованого числа в степінь d
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

                int decryptedRGBValue = Convert.ToInt32(decryptedNumber.ToString());

                binFromRGB += IntToBin(decryptedRGBValue.ToString());
            }
            try
            {
                binFromRGB += File.ReadAllText(pathToEncryptedPicture).Split(',')[1];
            }
            catch(Exception ex)
            {

            }
            rgbFromBin += BinToRGB(binFromRGB);

            return rgbFromBin;
        }

        /// <summary>
        /// Перетворення числа в двійковий код.
        /// </summary>
        /// <param name="decryptedRGBValue">Розшифровані числа, взяті із двійкового коду по 9 бітів.</param>
        /// <returns>Двійковий код RGB значень картинки.</returns>
        private string IntToBin(string decryptedRGBValue)
        {
            int rgb = Convert.ToInt32(decryptedRGBValue);
            string rgbConverted = Convert.ToString(rgb, 2);
            int i = 0;
            int byteLength = rgbConverted.Length;
            rgbConverted = string.Empty;
            // Якщо двійкове представлення числа не містить потрібну кількість нулів і одиничок
            while (i < 9 - byteLength)
            {
                rgbConverted += "0";
                i++;
            }

            rgbConverted += Convert.ToString(rgb, 2);

            return rgbConverted;
        }

        /// <summary>
        /// Перетворення двійкового коду на RGB значення.
        /// </summary>
        /// <param name="binToRGB">Двійковий код, який потрібно перевести.</param>
        /// <returns>RGB значення пікселів.</returns>
        private string BinToRGB(string binToRGB)
        {
            string convertedRGB = string.Empty;
            // Перебір блоками по 8
            for (int i = 0; i < binToRGB.Length / 8; i++)
            {
                try
                {
                    int rgbValueFromBin = Convert.ToInt32(binToRGB.Substring(i * 8, 8), 2);
                    convertedRGB += rgbValueFromBin.ToString();
                    // Для правильного розбиття RGB по пікселях і між собою
                    if ((i + 1) % 3 == 0)
                    {
                        convertedRGB += ",";
                    }
                    else
                    {
                        convertedRGB += " ";
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return convertedRGB;
        }

        /// <summary>
        /// Відкриття вікна шифрування картинки.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void OpenPictureEncryptWindowButton_Click(object sender, RoutedEventArgs e)
        {
            EncryptionOfPictureWindow encryptionOfPicture = new EncryptionOfPictureWindow();
            encryptionOfPicture.Show();
            Close();
        }

        /// <summary>
        /// Відкриття головного вікна.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void OpenMainMenuWindowButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            Close();
        }
    }
}
