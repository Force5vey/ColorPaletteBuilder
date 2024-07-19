using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ColorPaletteBuilder
{
     public static class AppConstants
     {
          internal static readonly string UserSettingsLocation = ApplicationData.Current.LocalFolder.Path + "\\UserSettings.json";

          public const string BackupFileName = "backup.cpb";
          public static string BackupFilePath = ApplicationData.Current.LocalFolder.Path;


          internal static string LastColorPickerHex = "LastColorPickerHex";
          internal static string LastOpenedFilePath = "LastOpenedFilePath";
          internal static string DefaultComboBoxText = "All";

          //Window Sizing
          internal static string WindowWidth = "WindowWidth";
          internal static string WindowHeight = "WindowHeight";
          internal static string WindowLeft = "WindowLeft";
          internal static string WindowTop = "WindowTop";


          internal enum SortCriteria
          {
               Index,
               Name,
               State,
               Group,
               Color,
               Note
          }

          internal enum ReturnCode
          {
               // General
               Success = 0,
               GeneralFailure = 1,

               // File Operations
               FileNotFound = 100,
               FileReadError = 101,
               FileWriteError = 102,
               FileFormatError = 103,

               // Data Validation
               InvalidData = 200,
               NullData = 201,
               DataConflict = 202,

               // User Actions
               UserCancelled = 300,
               Unauthorized = 301,
               NotImplemented = 302,

               // Network/Connection
               ConnectionFailed = 400,
               Timeout = 401,
               NetworkError = 402,

               // Processing
               ProcessingError = 500,
               InvalidState = 501
          }

          public enum SnippetLanguage
          {
               CSharp,
               Python,
               Javascript
          }

     }
}
