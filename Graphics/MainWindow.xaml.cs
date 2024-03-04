﻿using System.Windows;

namespace Charts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        public MainWindow()
        {
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }
    }
}