// <copyright file="InformationWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DataEncryption
{
    using System.Windows;
    using DataEncryption.Properties;

    /// <summary>
    /// Логика взаимодействия для InformationWindow.xaml.
    /// </summary>
    public partial class InformationWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformationWindow"/> class.
        /// </summary>
        public InformationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Повернення до попереднього вікна.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void PreviousWindowButton_Click(object sender, RoutedEventArgs e)
        {
            switch (Settings.Default.PreviousWindow)
            {
                case "BaseMenu":
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                    break;
                case "MainMenu":
                    MainMenuWindow mainMenuWindow = new MainMenuWindow();
                    mainMenuWindow.Show();
                    this.Close();
                    break;
            }
        }

        /// <summary>
        /// Вивід інформації про автора.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void InformationOfAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Автор - Мартин Василь, студент групи КН-21 ПГФК ДВНЗ 'УжНУ', майбутній програміст.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Викликається коли натискається кнопка Enter.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void WindowAndTextBoxesAndButton_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                InformationOfAuthorButton_Click(sender, e);
            }
        }
    }
}
