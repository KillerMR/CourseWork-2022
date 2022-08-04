// <copyright file="EncryptionOfPictureWindow.xaml.cs" company="PlaceholderCompany">
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
    /// Логика взаимодействия для EncryptionOfPicture.xaml.
    /// </summary>
    public partial class EncryptionOfPictureWindow : Window
    {
        private static Bitmap usersPicture;
        private static string remOfBin;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionOfPictureWindow"/> class.
        /// </summary>
        public EncryptionOfPictureWindow()
        {
            InitializeComponent();
            dTextBlock.Visibility = Visibility.Hidden;
            nTextBlock.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Відкриття діалогового вікна для вибору картинки.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void OpenPictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDial = new OpenFileDialog();
            if (openFileDial.ShowDialog() == true)
            {
                try
                {
                    PictureImage.Source = new BitmapImage(new Uri(openFileDial.FileName));
                    usersPicture = new Bitmap(openFileDial.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ви вибрали не зображення", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Виклик функції шифрування картинки.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void EncryptPictureButton_Click(object sender, RoutedEventArgs e)
        {
            PictureEncryption();
        }

        /// <summary>
        /// Функція шифрування картинки.
        /// </summary>
        private void PictureEncryption()
        {
            dTextBlock.Text = "d = ";
            nTextBlock.Text = "n = ";
            DateTime currentDateAndTime = DateTime.Now;
            FileStream fileWithEncryptedData = null;
            SaveFileDialog saveFileDial = new SaveFileDialog();
            saveFileDial.DefaultExt = ".txt";
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

            int n = p * q;
            int m = (p - 1) * (q - 1);
            int d = CalculateD(m);
            int e = CalculateE(d, m);

            if (usersPicture != null)
            {
                string encryptedPicture = RSA_Algorithm_Encrypt(usersPicture, e, n);
                encryptedPicture += $",{remOfBin},{usersPicture.Width} {usersPicture.Height}";

                // Користувач може обрати куди хоче зберегти зашифровану картинку
                if (MessageBox.Show("Ви хочете зберегти файл у теку за замовчуванням?", "Увага", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    Directory.CreateDirectory(@"Data\\Images");
                    fileWithEncryptedData = File.Create(@"Data\\Images\\EncryptedPicture " + currentDateAndTime.ToString("G").Replace(':', '_') + ".txt");
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

                File.AppendAllText(fileWithEncryptedData.Name, encryptedPicture);

                dTextBlock.Visibility = Visibility.Visible;
                nTextBlock.Visibility = Visibility.Visible;
                dTextBlock.Text += d.ToString();
                nTextBlock.Text += n.ToString();
            }
            else
            {
                MessageBox.Show("Ви не вибрали зображення", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                p = rndNumber.Next(rndNumber.Next(24,28), 38);
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
                q = rndNumber.Next(rndNumber.Next(23, 26), 33);
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
        /// Вираховування числа d, яке є взаємно простим з m
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
        /// Шифрування картинки за RSA-алгоритмом.
        /// </summary>
        /// <param name="pictureToEncrypt">Картинка, яку потрібно зашифрувати.</param>
        /// <param name="e">Число, яке вираховується в функції CalculateE.</param>
        /// <param name="n">Максимальний розмір блоків чисел.</param>
        /// <returns>Зашифрована картинка</returns>
        private string RSA_Algorithm_Encrypt(Bitmap pictureToEncrypt, int e, int n)
        {
            string rgbFromPicture = PictureToRGB(pictureToEncrypt);
            string[] pixelArray = rgbFromPicture.Split(',');

            BigInteger encryptedNumber;
            string convertedRGBValues = string.Empty;
            string encryptedRGBValues = string.Empty;
            // Перебираємо RGB значення кожного пікселя, та переводимо їх в двійкову систему
            foreach (string pixel in pixelArray)
            {
                string[] rgbValueArray = pixel.Split(' ');
                for (int i = 0; i < rgbValueArray.Length; i++)
                {
                    if (rgbValueArray[i] == string.Empty)
                    {
                        break;
                    }

                    convertedRGBValues += ConvertRGBValue(rgbValueArray[i]);
                }
            }

            string[] convertedRGBValueArray = BinToInt(convertedRGBValues).Split(' ');

            // Шифруємо числа, які отримали із двійкового коду, який був взятий із RGB значень кожного пікселя
            for (int i = 0; i < convertedRGBValueArray.Length; i++)
            {
                if (convertedRGBValueArray[i] == string.Empty)
                {
                    break;
                }

                int convertedRGBValue = Convert.ToInt32(convertedRGBValueArray[i]);

                encryptedNumber = new BigInteger(convertedRGBValue);

                // Піднесення числа в степінь.
                encryptedNumber = BigInteger.Pow(encryptedNumber, e);

                BigInteger nSecond = new BigInteger(n);

                // Ділення числа на інше по модулю
                encryptedNumber = encryptedNumber % nSecond;

                encryptedRGBValues += encryptedNumber.ToString() + " ";
            }

            return encryptedRGBValues;
        }

        /// <summary>
        /// Перетворення R, G, або B - значення в двійкову систему.
        /// </summary>
        /// <param name="rgbValueToConvert">R, G або B - значення, яке потрібно перетворити.</param>
        /// <returns>Двійковий код із R, G або B - значення.</returns>
        private string ConvertRGBValue(string rgbValueToConvert)
        {
            // Тут я перевожу RGB значення в двійкову систему, і якщо двійковий код менше 8 символів, то перед ним додаються нулі
            int rgb = Convert.ToInt32(rgbValueToConvert);
            string rgbConverted = Convert.ToString(rgb, 2);
            int i = 0;
            int byteLength = rgbConverted.Length;
            rgbConverted = string.Empty;
            while (i < 8 - byteLength)
            {
                rgbConverted += "0";
                i++;
            }

            rgbConverted += Convert.ToString(rgb, 2);
            return rgbConverted;
        }

        /// <summary>
        /// Перетворення двійкового коду в десяткову систему по 9 бітів.
        /// </summary>
        /// <param name="binToInteger">Двійковий код, який потрібно перевести.</param>
        /// <returns>Рядок із числом, взятим із двійкового коду по 9 бітів.</returns>
        private string BinToInt(string binToInteger)
        {
            string convertedBinToInt = string.Empty;
            
            // Вибираємо по 9 блоків бітів та перетворюємо в десяткові числа
            for (int i = 0; i < binToInteger.Length / 9; i++)
            {
                int convertedInteger = Convert.ToInt32(binToInteger.Substring(i * 9, 9), 2);
                convertedBinToInt += convertedInteger.ToString() + " ";
            }

            // Вибираємо залишок із двійкового коду
            int integerlength = binToInteger.Length / 9;
            int startPositionToSubstring = integerlength * 9;
            int countOfBin = binToInteger.Length - startPositionToSubstring;
            remOfBin = binToInteger.Substring(startPositionToSubstring, countOfBin);

            return convertedBinToInt;
        }

        /// <summary>
        /// Взяття пікселів із зображення.
        /// </summary>
        /// <param name="pictureToConvert">Картинка, пікселі якої будемо вибирати.</param>
        /// <returns>R, G, та B - значення із картинки.</returns>
        private string PictureToRGB(Bitmap pictureToConvert)
        {
            System.Drawing.Color[] rgbFromPixelsArray = new System.Drawing.Color[pictureToConvert.Width * pictureToConvert.Height];

            // Вибираємо пікселі із картинки та передаємо у масив
            for (int i = 0; i < pictureToConvert.Height; i++)
            {
                for (int j = 0; j < pictureToConvert.Width; j++)
                {
                    rgbFromPixelsArray[(i * pictureToConvert.Width) + j] = pictureToConvert.GetPixel(j, i);
                }
            }

            string rgbFromPicture = string.Empty;
            
            // Отримуємо RGB значення пікселів із масива Color
            foreach (System.Drawing.Color rgbFromPixel in rgbFromPixelsArray)
            {
                rgbFromPicture += rgbFromPixel.R + " " + rgbFromPixel.G + " " + rgbFromPixel.B + ",";
            }

            return rgbFromPicture;
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
        /// Відкриття вікна розшифрування картинки.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void ToPictureDecryptionButton_Click(object sender, RoutedEventArgs e)
        {
            DecryptionOfPictureWindow decryptionOfPicture = new DecryptionOfPictureWindow();
            decryptionOfPicture.Show();
            Close();
        }
    }
}
