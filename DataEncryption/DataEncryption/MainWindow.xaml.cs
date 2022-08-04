// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DataEncryption
{
    using System.Windows;
    using DataEncryption.Properties;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Відкриття вікна авторизації.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();
            authorizationWindow.Show();
            Close();
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
        /// Збереження інформації про попереднє вікно.
        /// </summary>
        private void SavePreviousWindowFromInformationInSettings()
        {
            MainWindow mainWindow = new MainWindow();
            Settings.Default.PreviousWindow = mainWindow.Name;
            Settings.Default.Save();
        }

        /// <summary>
        /// Викликається, коли натиснуто кнопку Enter.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void WindowAndTextBoxesAndButton_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AuthorizationButton_Click(sender, e);
            }
        }
    }
}
