// <copyright file="MainMenuWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DataEncryption
{
    using System.Windows;
    using DataEncryption.Properties;

    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml.
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuWindow"/> class.
        /// </summary>
        public MainMenuWindow()
        {
            InitializeComponent();
            UserLoginTextBlock.Text = Settings.Default.ActiveUser;
        }

        /// <summary>
        /// Відкриття вікна довідки.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            InformationWindow informationWindow = new InformationWindow();
            SavePreviousWindowFromInformationInSettings();
            informationWindow.Show();
            Close();
        }

        /// <summary>
        /// Відкриття вікна шифрування тексту.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void EncryptionOfTextButton_Click(object sender, RoutedEventArgs e)
        {
            EncryptionOfTextWindow encryptionOfText = new EncryptionOfTextWindow();
            encryptionOfText.Show();
            Close();
        }

        /// <summary>
        /// Відкриття вікна шифрування картинки.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void EncryptionOfPictureButton_Click(object sender, RoutedEventArgs e)
        {
            EncryptionOfPictureWindow encryptionOfPicture = new EncryptionOfPictureWindow();
            encryptionOfPicture.Show();
            Close();
        }

        /// <summary>
        /// Збереження інформації про попереднє вікно.
        /// </summary>
        private void SavePreviousWindowFromInformationInSettings()
        {
            MainMenuWindow mainMenuWindow = new MainMenuWindow();
            Settings.Default.PreviousWindow = mainMenuWindow.Name;
            Settings.Default.Save();
        }

        /// <summary>
        /// Функція виходу з акаунту
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ActiveUser = string.Empty;
            Settings.Default.Save();
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();
            authorizationWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Викликається, якщо натиснуто кнопку Enter.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void WindowAndTextBoxesAndButton_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                EncryptionOfTextButton_Click(sender, e);
            }
        }
    }
}
