﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Security.Cryptography.X509Certificates;
using Microsoft.UI.Xaml;

namespace ColorPaletteBuilder
{
     internal class SettingsViewModel :INotifyPropertyChanged
     {
          private int _autoSaveInterval;
          private string _autoSaveIntervalMinutes;

          public int ThemeSelection { get; set; }
          
          public int AutoSaveInterval
          {
               get => _autoSaveInterval;
               set
               {
                    if ( SetProperty(ref _autoSaveInterval, value) )
                    {
                         OnPropertyChanged(nameof(AutoSaveIntervalMinutes));
                    }
               }
          }

          public double AutoSaveIntervalMinutes
          {
               get => _autoSaveInterval / 60;

               set
               {
                    int newInterval = (int)(value * 60);
                    if ( SetProperty(ref _autoSaveInterval, newInterval) )
                    {
                         OnPropertyChanged(nameof(AutoSaveInterval));
                    }
               }
          }

          public SettingsViewModel()
          {
               if ( App.UserSettings.Theme == ApplicationTheme.Light )
               {
                    ThemeSelection = 0;
               }
               else if ( App.UserSettings.Theme == ApplicationTheme.Dark )
               {
                    ThemeSelection = 1;
               }
          }

          public async Task SaveSettings()
          {
               switch ( ThemeSelection )
               {
                    case 0: //Light
                    {
                         App.UserSettings.Theme = ApplicationTheme.Light;

                         break;
                    }
                    case 1: //Dark
                    {
                         App.UserSettings.Theme = ApplicationTheme.Dark;
                         break;
                    }
               }

               await SettingsService.SerializeUserSettings_Async();
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