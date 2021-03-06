﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using UniGraphics.ViewModels;

namespace UniGraphics.ColorModels
{
    public partial class ColorModelsView : Page
    {
        private static readonly string defaultHelperMessage =
            "Щоб почати роботу з колірними моделями RGB та HSL, оберіть зображення у 'Файл' та натисніть кнопку 'Конвертувати";
        private Action goBack;
        
        public ColorModelsView(Action goBack)
        {
            this.goBack = goBack;
            InitializeComponent();
            UnicornHelper.Text = defaultHelperMessage;
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            ((ColorModelsViewModel)DataContext).HandleMouseMove((int)point.X, (int)point.Y);
        }

        private void ImageMouseLeave(object sender, MouseEventArgs e)
        {
            ((ColorModelsViewModel)DataContext).RGBText = "";
            ((ColorModelsViewModel)DataContext).HSLText = "";
        }

        private void HelpClicked(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
        }

        private void CreditsClicked(object sender, RoutedEventArgs e)
        {
            CreditsWindow creditWindow = new CreditsWindow();
            creditWindow.ShowDialog();
        }

        private void StudyMaterialsClicked(object sender, RoutedEventArgs e)
        {
            StudyWindow studyWindow = new StudyWindow();
            studyWindow.ShowDialog();
        }

        private void HuePicked(object sender, MouseButtonEventArgs e)
        {
            var x = e.GetPosition((IInputElement)sender).X;
            var width = ((Rectangle)sender).Width;
            ((ColorModelsViewModel)DataContext).HandleHuePicked((int)x, (int)width);
        }

        private void HueMouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                var x = e.GetPosition((IInputElement)sender).X;
                var width = ((Rectangle)sender).Width;
                ((ColorModelsViewModel)DataContext).HandleHuePicked((int)x, (int)width);
            }
        }

        private void HelpableHover(object sender, MouseEventArgs e)
        {
            var helpable = sender as FrameworkElement;
            UnicornHelper.Text = helpable.Tag.ToString();
        }

        private void HelpableMouseLeave(object sender, MouseEventArgs e)
        {
            UnicornHelper.Text = defaultHelperMessage;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            goBack();
        }
    }
}
