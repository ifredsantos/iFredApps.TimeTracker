using MahApps.Metro.Controls;
using System;
using System.Windows;
using TimeTracker.UI.Models;

namespace TimeTracker.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LaunchGitHubSite(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/ifredsantos");
        }

        private void Donation(object sender, RoutedEventArgs e)
        {
            ShowNotification("Available soon!");
        }

        public void ShowNotification(string message, TimeSpan? duration = null)
        {
            snackBar.MessageQueue.Enqueue(message, null, null, null, false, true, duration ?? TimeSpan.FromSeconds(5));
        }

        private void OnNotificationShow(object sender, NotificationEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message))
                return;

            ShowNotification(e.Message, e.Duration);
        }
    }
}
