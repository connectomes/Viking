﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RuneWpfControlLibrary
{
    /// <summary>
    /// Interaction logic for GoToLocationForm.xaml
    /// </summary>
    public partial class GoToLocationForm : Window
    {
        public long LocationID;

        public GoToLocationForm()
        {
            InitializeComponent();
        }
        
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LocationID = System.Convert.ToInt64(this.NumberTextbox.Text);
            }
            catch (FormatException)
            {
                return;
            }
            /*
            if (Store.Locations.GetObjectByID(this.LocationID, true) != null)
            {
                WebAnnotation.AnnotationOverlay.GoToLocation(this.LocationID);
                this.Close();
            }*/
        }

        private void Go_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LocationID = System.Convert.ToInt64(this.NumberTextbox.Text);
            }
            catch (FormatException)
            {
                return;
            }

            //WebAnnotation.AnnotationOverlay.GoToLocation(this.LocationID);
        }


        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]*"); //regex that matches disallowed text
            return regex.IsMatch(text);
        }

        private void NumberTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
