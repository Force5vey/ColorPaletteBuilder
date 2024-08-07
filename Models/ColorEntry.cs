﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.UI;


namespace ColorPaletteBuilder
{

     public class ColorEntry :INotifyPropertyChanged
     {
          Guid id;

          int elementIndex; // This will be static to the order it was added to the color palette.
          string elementName;
          string elementState;
          string elementGroup;
          string hexCode;
          string displayColor; // This is a placeholder for displaying the color visually.
          string changeColor; // This is a placeholder for listview to add a button to change color
          string sendColor; // This is a placeholder for listview to add a button to send color to color picker
          string note;

          TextWrapping _wrapText;

          public ColorEntry()
          {
               Id = Guid.NewGuid();

               ElementName = string.Empty;
               ElementState = string.Empty;
               ElementGroup = string.Empty;
               HexCode = string.Empty;
               DisplayColor = string.Empty;
               ChangeColor = string.Empty;
               SendColor = string.Empty;
               Note = string.Empty;
               ElementIndex = 0;

               WrapText = TextWrapping.Wrap;
          }

          public Guid Id
          {
               get => id;
               set => SetProperty(ref id, value);
          }

          public int ElementIndex
          {
               get => elementIndex;
               set => SetProperty(ref elementIndex, value);
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

          public string SendColor
          {
               get => sendColor;
               set => SetProperty(ref sendColor, value);
          }

          public string Note
          {
               get => note;
               set => SetProperty(ref note, value);
          }

          public TextWrapping WrapText
          {
               get => _wrapText;
               set => SetProperty(ref _wrapText, value);
          }

          public event PropertyChangedEventHandler PropertyChanged;

          protected bool SetProperty<T>( ref T storage, T value, [CallerMemberName] string propertyName = null )
          {
               if ( Equals(storage, value) )
                    return false;

               storage = value;
               OnPropertyChanged(propertyName);
               return true;
          }

          protected void OnPropertyChanged( string propertyName )
          {
               PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
          }


     }
}