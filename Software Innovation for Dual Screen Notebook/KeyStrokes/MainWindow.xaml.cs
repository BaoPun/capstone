﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 3;
        private const int WS_EX_NOACTIVE = 0x08000000;
        private const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVE);

            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            switch (msg)
            {
                case WM_MOUSEACTIVATE:
                    return (IntPtr)MA_NOACTIVATE;
                default:
                    break;
            }

            return IntPtr.Zero;
        }


        //Keys register on window.
        private void KeyInteractor(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.D1) {
                MessageBox.Show("The \'1\' key was pressed, opening Legends of Runeterra", "Button 1 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button1_KeyDown(sender, e);
            }
            else if(e.Key == Key.D2) {
                MessageBox.Show("The \'2\' key was pressed, opening r/leagueoflegends", "Button 2 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button2_KeyDown(sender, e);
            }
            else if(e.Key == Key.D3) {
                MessageBox.Show("The \'3\' key was pressed, opening YouTube", "Button 3 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button3_KeyDown(sender, e);
            }
            else if(e.Key == Key.OemPlus)
            {
                MessageBox.Show("The \'+\' key was pressed, time to create a new button", "Button + Press");
                e.Handled = true; //prevent the action from happening twice.
                Add_KeyDown(sender, e);
            }
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button was clicked, time to create a new button", "Button + Click");
            Form1 createButton = new Form1();
            createButton.Show();
        }

        private void Add_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                Form1 createButton = new Form1();
                createButton.Show();
            }
        }

        


        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button was clicked, opening Legends of Runeterra", "Button 1 Click");
            Process.Start("D:\\Riot Games\\LoR\\live\\Game\\LoR");
        }


        private void Button1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.D1)
                Process.Start("D:\\Riot Games\\LoR\\live\\Game\\LoR");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button was clicked, opening League of Legends Subreddit", "Button 2 Click");
            Process.Start("https://www.reddit.com/r/leagueoflegends");
        }

        private void Button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D2)
                Process.Start("https://www.youtube.com");
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button was clicked, opening YouTube", "Button 1 Click");
            Process.Start("D:\\Riot Games\\LoR\\live\\Game\\LoR");
        }


        private void Button3_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.D3)
                Process.Start("https://www.youtube.com");
        }
    }
}
