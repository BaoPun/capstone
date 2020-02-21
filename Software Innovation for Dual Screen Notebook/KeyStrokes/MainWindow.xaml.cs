﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections.Generic;

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

        // this is used to get the hWnd for imported functions
        // hWnd is a way to identify windows used in win32 framework
        private WindowInteropHelper helper;
        private List<VirtualKeyShort.Key> shortcut;
        private List<List<int>> buttonLoc;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            shortcut = new List<VirtualKeyShort.Key>();

            buttonLoc = new List<List<int>>();
            for (int i = 0; i < 10; i++)
            {
                buttonLoc.Add(new List<int>()); 
                for (int j = 0; j < 10; j++)
                {
                    buttonLoc[i].Add(0);
                }
            }
            buttonLoc[0][0] = 2;
            buttonLoc[0][2] = 2;
            buttonLoc[0][3] = 2;
            buttonLoc[0][4] = 2;

            // sets the window so that a click does not bring it into focus
            helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVE);

            // this lets WndProc be overriden so that we can get the click massage
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);

            // this activates the window so that when we start it 
            // it does not jump to the back of all windows
            this.Activate();
        }

        // this gets the click message so that 
        // it can still sends the click to the app
        // even though it is out of focus
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


        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddButtonWindow addButton = new AddButtonWindow();
            addButton.InitializeComponent();
            addButton.Show();
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


        // Bottom bar, all the words should be changed to icons and made to look much much better
        private void media_back_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_PREV_TRACK});
        }

        private void media_play_pause_Click(object sender, RoutedEventArgs e)
        {
            // this needs to be changed to accomidate actually checking if the 
            // music is playing to be correct all the time
            if (media_play_pause.Content.Equals("Play"))
            {
                media_play_pause.Content = "Pause";

            } else
            {
                media_play_pause.Content = "Play";
            }
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_PLAY_PAUSE });
        }

        private void media_forward_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_NEXT_TRACK });
        }

        private void undo_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.CONTROL, VirtualKeyShort.Key.KEY_C });
        }

        private void redo_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.CONTROL, VirtualKeyShort.Key.KEY_Y });
        }


         public int getNextCol()
        {
            for(int i = 0; i < buttonLoc.Count; i++)
            {
                for(int j = 0; j < buttonLoc[i].Count; j++)
                {
                    if (buttonLoc[i][j] < 2)
                    {
                        buttonLoc[i][j]++;
                        return i;
                    }
                }
            }
            return -1;
        }

        public int getNextRow()
        {
            for (int i = 0; i < buttonLoc.Count; i++)
            {
                for (int j = 0; j < buttonLoc[i].Count; j++)
                {
                    if (buttonLoc[i][j] < 2)
                    {
                        buttonLoc[i][j]++;
                        return j;
                    }
                }
            }
            return -1;
        }

        private void titleBar_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}   
