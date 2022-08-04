// <copyright file="RegistrationWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DataEncryption
{
    using System;
    using System.IO;
    using System.Windows;
    using DataEncryption.Properties;

    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml.
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private const string PathToData = "Data";
        private static string userName = string.Empty;
        private static string userPassword = string.Empty;
        private static bool isLoginExist = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationWindow"/> class.
        /// </summary>
        public RegistrationWindow()
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
        /// Виклик функції авторизації, та перевірка на існування файлу з користувачами.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка на існування файла
            if (!File.Exists(PathToData + @"\Users.txt"))
            {
                FileStream file = File.Create(PathToData + @"\Users.txt");
                file.Close();
            }

            RegistrationProcess();
        }

        /// <summary>
        /// Процес реєстрації.
        /// </summary>
        private void RegistrationProcess()
        {
            userName = string.Empty;
            userName = LoginRegTextBox.Text;
            // Умова на пустий логін та пароль.
            if (userName.Length == 0)
            {
                MessageBox.Show("Логін не може бути порожнім", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            userPassword = PasswordRegTextBox.Password;
            if (userPassword.Length == 0)
            {
                MessageBox.Show("Пароль не може бути порожнім", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string[] usersData = File.ReadAllText(PathToData + @"\Users.txt").Replace("\n", string.Empty).Split('\r');

            // Перевірка на існування ідентичного логіна.
            foreach (string userData in usersData)
            {
                if (userData.Split('|')[0] == userName)
                {
                    isLoginExist = true;
                    MessageBox.Show("Такий користувач вже існує", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            RegistrationDataAdd(isLoginExist);
        }

        /// <summary>
        /// Додавання нового користувача у файл.
        /// </summary>
        /// <param name="isLoginExist">Значення, чи існує логін</param>
        private void RegistrationDataAdd(bool isLoginExist)
        {
            if (LoginRegTextBox.Text.Length > 4 && PasswordRegTextBox.Password.Length > 4 && isLoginExist == false)
            {
                string userPasswordConfirm = PasswordConfirmRegTextBox.Password;
                if (userPasswordConfirm.Length == 0)
                {
                    MessageBox.Show("Введіть пароль ще раз", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (userPassword != userPasswordConfirm)
                {
                    MessageBox.Show("Паролі не співпадають", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Шифрування паролю за BCrypt-шифруванням
                    string hashOfPassword = BCrypt.Net.BCrypt.HashPassword(userPassword);
                    File.AppendAllText(PathToData + @"\Users.txt", "\r" + userName + "|" + hashOfPassword);
                    SaveUserInSettings(userName); // Збереження логіну користувача для подальшого використання
                    MessageBox.Show("Реєстрація успішна", "Успіх!", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainMenuWindow mainMenuWindow = new MainMenuWindow();
                    mainMenuWindow.Show();
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Кількість символів логіну та паролю повинна перевищувати 4", "Помилка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
        /// Викликається, коли натиснуто кнопку Enter.
        /// </summary>
        /// <param name="sender">Елемент керування.</param>
        /// <param name="e">Обробник подій.</param>
        private void WindowAndTextBoxesAndButton_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                RegistrationButton_Click(sender, e);
            }
        }
    }
}
