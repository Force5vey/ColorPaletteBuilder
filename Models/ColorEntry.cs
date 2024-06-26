﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.UI;


namespace ColorPaletteBuilder
{

    public class ColorEntry : INotifyPropertyChanged
    {
        private Guid id;

        private string elementName;
        private string elementState;
        private string elementGroup;
        private string hexCode;
        private string displayColor; // This is a placeholder for the actual color value
        private string changeColor; // This is a placeholder for listview to add a button to change color


        private bool isColorAssignEnabled;


        public Guid Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public bool IsColorAssignEnabled
        {
            get => isColorAssignEnabled;
            set => SetProperty(ref isColorAssignEnabled, value);
        }

        public string ElementName
        {
            get => elementName;
            set => SetProperty(ref elementName, value);
        }

        public string ElementState
        {
            get => elementState;
            set => SetProperty(ref elementState, value);
        }

        public string ElementGroup
        {
            get => elementGroup;
            set => SetProperty(ref elementGroup, value);
        }

        public string HexCode
        {
            get => hexCode;
            set => SetProperty(ref hexCode, value);
        }

        public string DisplayColor
        {
            get => displayColor;
            set => SetProperty(ref displayColor, value);
        }

        public string ChangeColor
        {
            get => changeColor;
            set => SetProperty(ref changeColor, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ColorEntry()
        {
            Id = Guid.NewGuid();

            ElementName = string.Empty;
            ElementState = string.Empty;
            ElementGroup = string.Empty;
            HexCode = string.Empty;
            DisplayColor = string.Empty;
            IsColorAssignEnabled = false;
        }
    }
}