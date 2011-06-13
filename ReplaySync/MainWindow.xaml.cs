// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Ascend">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ReplaySync
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += this.MainWindowClosing;
            this.MouseDown += this.MainWindowMouseDown;
        }

        private void MainWindowMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left && this.menuMovement.IsChecked)
            {
                this.DragMove();
            }
        }

        /// <summary> Shuts down the application when the main window is closed. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.Shutdown();
            Application.Current.Shutdown();
        }

        private void MoveWindowChecked(object sender, RoutedEventArgs e)
        {
            this.menuMovement.IsChecked = !this.menuMovement.IsChecked;
        }

        private void MinimizeClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
