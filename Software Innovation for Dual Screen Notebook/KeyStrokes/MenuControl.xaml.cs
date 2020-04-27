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
using System.Drawing;


namespace KeyStrokes
{
    public partial class MenuControl : UserControl
    {

        private readonly MainWindow main;
        public static Boolean currentInstance = false;
        public static SolidColorBrush currentBrush;
        public static SolidColorBrush transparentBrush;

        public MenuControl()
        {
            InitializeComponent();    
            main = ((MainWindow)App.Current.MainWindow);
            currentBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD4D4E4"));
            transparentBrush = currentBrush;

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {   
            if (addButton.Visibility == Visibility.Visible)
            {
                addButton.Visibility = Visibility.Hidden;
            } else
            {
                menu.Visibility = Visibility.Hidden;
            }
        }

        private void add_button_Click(object sender, RoutedEventArgs e)
        {
            addButton.Visibility = Visibility.Visible;
            addButton.Open();
        }

        private void open_gaming_case(object sender, RoutedEventArgs e)
        {
            // If the application is already open, then don't open another instance...
            if (currentInstance)
            {
                MessageBox.Show("What are you doing?  You have an instance of this window open already!", "Already opened");
                return;
            }
            currentInstance = true;
            GamingUseCase game = new GamingUseCase();
            game.Show();
            

        }

        private void layout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem ComboItem = (ComboBoxItem)layout_box.SelectedItem;
            string name = layout_box.SelectedItem.ToString();
            Trace.WriteLine(name.ToString());
            //Trace.WriteLine(name.ToString().Substring(35,38));
            string[] x = name.ToString().Split('x');
            string a = x[1];
            string b = "";
            if (x.Length > 2)
            {
                b = x[2];
                a = a.ToString().Split(' ')[1];
                main.grid.set_grid(Int16.Parse(a), Int16.Parse(b));
            }
        }

        private void bottom_bar_Click(object sender, RoutedEventArgs e)
        {


            if (main.bottomBar.Visibility == Visibility.Hidden)
            {
                main.bottomBar.Visibility = Visibility.Visible;
            }
            else
            {
                main.bottomBar.Visibility = Visibility.Hidden;
            }

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main.DragMove();
        }
        private void background_design_change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem ComboItem = (ComboBoxItem)background_design_box.SelectedItem;
            string name = background_design_box.SelectedItem.ToString();
            Trace.WriteLine(name.ToString());
            string[] selectedVal = name.ToString().Split(' ');
            if (selectedVal.Length > 1)
            {
                Trace.WriteLine("Result: " + selectedVal[1]);
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(selectedVal[1]);
                SolidColorBrush brush = new SolidColorBrush(color);
                if (selectedVal[1] != "Transparent")
                    currentBrush = brush;
                else
                    currentBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD4D4E4");
                main.Background = brush;
            }
        }
    }
}
