# WootingAnalogNet

WootingAnalogNet is a .NET Framework library for interfacing with Wooting analog devices. It provides a managed wrapper around the Wooting Analog SDK, enabling easy access to analog key values from C# applications.

## Features

- Initialise and query Wooting analog devices information
- Read analog values for individual keys or all keys
- Support for multiple keycode modes (HID, ScanCode1, VirtualKey) (Extension class usage required)
- Wait for key events (down, up, fully down, fully up, press) (Extension class usage required)
- Key state queries (is down, is up, is fully down, is fully up) (Extension class usage required)
- Keycode translation utilities (Extension class usage required)

## Getting Started

### Prerequisites

- .NET Framework
- Compatible Wooting keyboard
- Wooting Analog SDK DLL (wooting_analog_wrapper.dll)

### Installation

1. Download the latest release DLL from the releases page
2. Reference the `WootingAnalogNet` project
3. Ensure the Wooting Analog SDK native DLL (wooting_analog_wrapper.dll) is in the bin directory of your project
4. Ensure your project is built in x64

### Usage Example (without extension methods)
`Please see the Wiki the full documentation and examples`

```csharp
using WootingAnalogNet;

WootingAnalog Keyboard = new WootingAnalog();
WootingAnalogResult LastResult = WootingAnalogResult.UnInitialized;

// The initialise call returns the amount of devices found
int DeviceCount = Keyboard.Initialise();

// To check if the SDK is actually initialised use IsInitialised
// Note: In the extensions class there is an initialise method that covers both these methods
bool SDKInitialised = Keyboard.IsInitialised();

if (!SDKInitialised)
{
  Console.WriteLine("Failed to initialise the SDK");
  return;
}

if (DeviceCount < 1)
{
  Console.WriteLine("Failed to find any Wooting devices");
  return;
}

// Get the devices information
WootingAnalogDeviceInfo[] DeviceInfo;
if (Keyboard.GetConnectedDevicesInfo(out DeviceInfo, DeviceCount) != DeviceCount)
{
  Console.WriteLine("Failed to get device info");
  return;
}

// Print the device information
for (int i = 0; i < DeviceInfo.Length; i++)
  Console.WriteLine($"Device Name: {DeviceInfo[i].DeviceName} " +
    $"| Manufacturer Name:{DeviceInfo[i].ManufacturerName} " +
    $"| Device ID: {DeviceInfo[i].DeviceID} " +
    $"| VID: {DeviceInfo[i].VendorID} " +
    $"| PID: {DeviceInfo[i].ProductID}");

// Default setting is HID, quite important to know which mode you are in as the KeyCodes will
// change
LastResult = Keyboard.SetKeyCodeMode(EKeyCodeMode.ScanCode1);
if(LastResult != WootingAnalogResult.Ok)
{
  Console.WriteLine("Failed to set KeyCodeMode");
  return;
}

// Read all the key values until the escape key is pressed
ushort[] KeyCodesBuffer = new ushort[64];
float[] AnalogReadingsBuffer = new float[64];
int Count = 0;
while (Keyboard.ReadAnalog(0x01) != 1)
{
  Count = Keyboard.ReadFullBuffer(KeyCodesBuffer, AnalogReadingsBuffer, 64);
  if (Count >= 0)
  {
    for (int i = 0; i < Count; i++)
    {
      if (KeyCodesBuffer[i] != 0x01)
      {
        Console.WriteLine($"Key: 0x{KeyCodesBuffer[i]:X2} | Value: {AnalogReadingsBuffer[i]}");
      }
    }
  }
}

// Shut down gracefully
Keyboard.Uninitialise();
Keyboard.Dispose();
```

## API Overview

- `WootingAnalog`  
  Main wrapper class that's aim is to stay as closely tied to the unmanaged SDK as possible.

- `WootingAnalogGeneralExtensions`  
  Extension methods for the main wrapper class that provides extra functionality / nice to have's, such as KeyCode translations based on the current KeyCodeMode.

## License

This project is licensed under the MIT License.

## Acknowledgments

- [Wooting Analog SDK](https://github.com/WootingKb/wooting-analog-sdk)
- Wooting community and contributors
