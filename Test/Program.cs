using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WootingAnalogNet;
using WootingAnalogNet.GeneralExtensions;

namespace Test
{
  internal class Program
  {
    static void Main(string[] args)
    {
      WootingAnalog Keyboard = null;
      try
      {
        WootingAnalogResult LastResult = WootingAnalogResult.UnInitialized;

        Keyboard = new WootingAnalog();
        
        //int DeviceCount = Keyboard.Initialise();
        //bool Initialised = Keyboard.IsInitialised();

        int DeviceCount = 0;
        bool Initialised = Keyboard.Initialise(out DeviceCount);

        if (DeviceCount > 0 && Initialised)
        {
          Console.WriteLine($"Initialised with {DeviceCount} device");

          LastResult = Keyboard.SetDeviceEventCallback(OnDeviceEvent);
          if (CheckLastStatus(LastResult, "SetDeviceCallback") == false)
            return;

          Console.WriteLine("Callback registered OK");

          LastResult = Keyboard.SetKeyCodeMode(EKeyCodeMode.ScanCode1);
          if (CheckLastStatus(LastResult, "SetKeyCodeMode") == false)
            return;

          Console.WriteLine($"Keycode mode set to ScanCode1");

          WootingAnalogDeviceInfo[] DeviceInfo;
          if (Keyboard.GetConnectedDevicesInfo(out DeviceInfo, DeviceCount) != DeviceCount)
          {
            Console.WriteLine("Failed to get device info");
            return;
          }

          for (int i = 0; i < DeviceInfo.Length; i++)
            Console.WriteLine($"Device Name: {DeviceInfo[i].DeviceName} | Manufacturer Name:{DeviceInfo[i].ManufacturerName} | Device ID: {DeviceInfo[i].DeviceID} | VID: {DeviceInfo[i].VendorID} | PID: {DeviceInfo[i].ProductID}");

          Console.WriteLine("Press Enter to continue");

          Keyboard.WaitKeyPress(KeyCode.Enter);

          Thread.Sleep(TimeSpan.FromSeconds(1));

          ushort[] KeyCodesBuffer = new ushort[64];
          float[] AnalogReadingsBuffer = new float[64];
          int Count = 0;

          Console.WriteLine("Continuously reading keys....press Esc to finish.");
          while (Keyboard.IsKeyFullyDown(KeyCode.Esc) == false)
          {
            Count = Keyboard.ReadFullBuffer(KeyCodesBuffer, AnalogReadingsBuffer, 64);

            if (Count >= 0)
            {
              for (int i = 0; i < Count; i++)
              {
                if (KeyCodesBuffer[i] != (ushort)KeyCode.Esc)
                {
                  switch (Keyboard.KeyCodeMode)
                  {
                    case EKeyCodeMode.ScanCode1:
                      Console.WriteLine($"Key: 0x{KeyCodesBuffer[i]:X2} | Value: {AnalogReadingsBuffer[i]} | Percent: {Keyboard.ValueToPercent(AnalogReadingsBuffer[i]).ToString("0")}%");
                      break;
                    case EKeyCodeMode.HID:
                      Console.WriteLine($"Key: 0x{KeyCodesBuffer[i]:X2} | Value: {AnalogReadingsBuffer[i]} | Percent: {Keyboard.ValueToPercent(AnalogReadingsBuffer[i]).ToString("0")}%");
                      break;
                    case EKeyCodeMode.VirtualKey:
                      Console.WriteLine($"Key: 0x{KeyCodesBuffer[i]:X2} | Value: {AnalogReadingsBuffer[i]} | Percent: {Keyboard.ValueToPercent(AnalogReadingsBuffer[i]).ToString("0")}%");
                      break;
                  }
                }
              }
            }
          }

          LastResult = Keyboard.ClearDeviceEventCallback();
          if (CheckLastStatus(LastResult, "ClearDeviceCallback") == false)
            return;
          Console.WriteLine("Cleared device callback OK");

        }
        else if (Initialised == false)
        {
          Console.WriteLine($"Failed to initialise SDK");
        }
        else
        {
          Console.WriteLine($"SDK initialised but {DeviceCount} devices found");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"{ex.Message}");
      }
      finally
      {
        if (Keyboard != null)
        {
          Keyboard.Uninitialise();
          Keyboard.Dispose();
        }
      }
    }

    private static bool CheckLastStatus(WootingAnalogResult Result, string Call)
    {
      if (Result != WootingAnalogResult.Ok)
      {
        Console.WriteLine($"Failed {Call} with error {Result}");
        return false;
      }

      return true;
    }

    private static void OnDeviceEvent(EWootingAnalogDeviceEventType eventType, ref WootingAnalogDeviceInfo deviceInfo)
    {
      switch (eventType)
      {
        case EWootingAnalogDeviceEventType.DeviceConnected:
          Console.WriteLine($"Device connected: {deviceInfo.DeviceName} (VID: {deviceInfo.VendorID}, PID: {deviceInfo.ProductID})");
          break;
        case EWootingAnalogDeviceEventType.DeviceDisconnected:
          Console.WriteLine($"Device disconnected: {deviceInfo.DeviceName} (VID: {deviceInfo.VendorID} , PID:  {deviceInfo.ProductID})");
          break;
        default:
          Console.WriteLine($"Unknown device event: {eventType}");
          break;
      }
    }
  }
}
