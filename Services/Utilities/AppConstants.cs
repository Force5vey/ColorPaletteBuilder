using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPaletteBuilder
{
     internal static class AppConstants
     {
          public enum ReturnCode
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

          public static string BackupFileName = "backup.cpb";

          internal static string LastColorPickerHex = "LastColorPickerHex";
          internal static string LastOpenedFilePath = "LastOpenedFilePath";

          //Window Sizing
          internal static string WindowWidth = "WindowWidth";
          internal static string WindowHeight = "WindowHeight";
          internal static string WindowLeft = "WindowLeft";
          internal static string WindowTop = "WindowTop";

     }
}
