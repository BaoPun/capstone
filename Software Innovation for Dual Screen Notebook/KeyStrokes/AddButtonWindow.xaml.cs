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
    public sealed partial class AddButtonWindow : Window
    {


        private List<VirtualKeyShort.Key> shortcut;
        private MainWindow main;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            shortcut = new List<VirtualKeyShort.Key>();

            main = ((MainWindow)App.Current.MainWindow);

        }

        private void Click_Addkey(object sender, RoutedEventArgs e)
        {

            if (shortcut.Count != 0)
            {
                hotkeyDisplay.Children.Add(new TextBlock { Text = " + " });
            }
            shortcut.Add((VirtualKeyShort.Key)keyEnum.SelectedItem);
            var newKey = new TextBlock {
                Name = keyEnum.SelectedItem.ToString(),
                Text = keyEnum.SelectedItem.ToString()
            };
            hotkeyDisplay.Children.Add(newKey);
           
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Click_Confirm(object sender, RoutedEventArgs e)
        {
            ///* adds generic elements to a button */
            Button format = main.addButton;
            Button newButton = new Button();
            newButton.Content = "no shortcut name";
            newButton.Width = format.Width;
            newButton.Height = format.Height;
            //newButton.CornerRadius = btn_add.CornerRadius; 
            newButton.Opacity = format.Opacity;
            newButton.Background = format.Background;
            newButton.BorderBrush = format.BorderBrush;
            newButton.FontSize = 30;
            newButton.Padding = format.Padding;
            newButton.Margin = format.Margin;

            // option to remove the button
            //newButton.RightTapped += async (s, en) =>
            newButton.MouseDown += async (s, en) =>
            {
                MessageBoxResult result = MessageBox.Show("Remove?", "", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        main.myGrid.Children.Remove(newButton);
                        break;
                }
            };


            // assigns the app name 
            if (String.IsNullOrEmpty(nameInput.Text))
            {
                newButton.Content = "";
            }
            else
            {
                newButton.Content = this.nameInput.Text;    // name of the app 
            }


            string hold = appInput.Text;
            if (!(String.IsNullOrEmpty(hold)))
            {
                newButton.Click += (se, ev) =>
                {
                    string str = "";

                    String[] spear = { "," };
                    String[] strlist = hold.Split(spear, StringSplitOptions.RemoveEmptyEntries);
                    foreach (String st in strlist)
                    {
                        str = "";
                        str += "start ";
                        str += st;
                        Process cmd = new Process();
                        cmd.StartInfo.FileName = "cmd.exe";
                        cmd.StartInfo.RedirectStandardInput = true;
                        cmd.StartInfo.RedirectStandardOutput = true;
                        cmd.StartInfo.CreateNoWindow = true;   // true hides cmd prompt
                        cmd.StartInfo.UseShellExecute = false;
                        cmd.Start();
                        cmd.StandardInput.WriteLine(str);
                        cmd.StandardInput.Flush();
                        cmd.StandardInput.Close();
                        cmd.WaitForExit();
                        Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                    }
                };
            }


            ///*
            //// assigns the application to launch:
            //if (!(String.IsNullOrEmpty(appInput.Text)))
            //{
            //    newButton.Tag = appInput.Text; //use tag to store the uri location, then can be accessed in the calling function
            //    newButton.Click += (se, ev) => this.launchApp(se, ev);
            //}

            //*/

            // assigns the keyboard shortcuts to launch
            if (shortcut.Count != 0)
            {
                newButton.Click += (se, ev) =>
                {
                    Shortcut.send(shortcut.ToArray());
                };
            }

            //// adds the button to the grid
            main.myGrid.Children.Add(newButton);

            this.Close();
        }
    }
}
