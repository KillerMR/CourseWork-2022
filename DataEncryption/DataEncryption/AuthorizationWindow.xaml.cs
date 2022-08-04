// <copyright file="AuthorizationWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DataEncryption
{
    using System;
    using System.IO;
    using System.Windows;
    using DataEncryption.Properties;

    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml.
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private const string PathToData = "Data";
        private static string userName = string.Empty;
        private static string userPassword = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationWindow"/> class.
        /// </summary>
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Виклик функції авторизації.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            // Виконується процес авторизації
            if (File.Exists(PathToData + @"\Users.txt"))
            {
                userName = LoginAuthTextBox.Text;
                userPassword = PasswordAuthTextBox.Password;
                VerificationEnter(userName, userPassword);
                AuthorizationProcess();
            }
            else
            {
                MessageBox.Show("Помилка з авторизацією", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Перевіряє логін та пароль на довжину.
        /// </summary>
        /// <param name="login">Логін користувача.</param>
        /// <param name="password">Пароль користува.</param>
        private void VerificationEnter(string login, string password)
        {
            if (login.Length == 0)
            {
                MessageBox.Show("Логін не може бути порожнім", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (password.Length == 0)
            {
                MessageBox.Show("Пароль не може бути порожнім", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Процес авторизації.
        /// </summary>
        private void AuthorizationProcess()
        {
            bool isExistLogin = true;

            string[] usersData = File.ReadAllText(PathToData + @"\Users.txt").Replace("\n", string.Empty).Split('\r');

            // Перевірка кожного логіна із файлу із логіном введеним користувачем
            foreach (string userData in usersData)
            {
                try
                {
                    if ((userData.Split('|')[0] == userName) && BCrypt.Net.BCrypt.Verify(userPassword, userData.Split('|')[1]) == true)
                    {
                        SaveUserInSettings(userData.Split('|')[0]);
                        isExistLogin = false;
                        MessageBox.Show("Авторизація успішна", "Успіх!", MessageBoxButton.OK, MessageBoxImage.Information);
                        MainMenuWindow mainMenuWindow = new MainMenuWindow();
                        mainMenuWindow.Show();
                        Close();
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (userName.Length != 0 && userPassword.Length != 0)
            {
                if (isExistLogin == true)
                {
                    MessageBox.Show("Такого акаунту не існує", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        /// <summary>
        /// Відкриття вікна авторизації.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registration = new RegistrationWindow();
            registration.Show();
            Close();
        }

        /// <summary>
        /// Відкриття головного вікна.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void OpenMainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        /// <summary>
        /// Зберігання логіну користувача для майбутнього виводу.
        /// </summary>
        /// <param name="userName">Логін користувача.</param>
        private void SaveUserInSettings(string userName)
        {
            Settings.Default.ActiveUser = userName;
            Settings.Default.Save();
        }

        /// <summary>
        /// Перевірка на натискання кнопки Enter.
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
