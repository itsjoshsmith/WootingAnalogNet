# WootingAnalogNet

WootingAnalogNet is a .NET Framework library for interfacing with Wooting analog input devices. It provides a managed wrapper around the Wooting Analog SDK, enabling easy access to analog key states and device management from C# applications.

## Features

- Initialize and manage Wooting analog devices
- Read analog values for individual keys or all keys
- Support for multiple keycode modes (HID, ScanCode1, VirtualKey)
- Wait for key events (down, up, fully down, fully up, press)
- Key state queries (is down, is up, is fully down, is fully up)
- Keycode translation utilities

## Getting Started

### Prerequisites

- .NET Framework 4.8
- Compatible Wooting keyboard and drivers
- Wooting Analog SDK DLLs (must be available to your application)

### Installation

1. Clone or download this repository.
2. Reference the `WootingAnalogNet` project or compiled DLL in your solution.
3. Ensure the Wooting Analog SDK native DLLs are accessible (e.g., in your output directory).

### Usage Example
```csharp
using WootingAnalogNet; using WootingAnalogNet.GeneralExtensions;
class Program 
{ 
  static void Main() 
  { 
    var wooting = new WootingAnalog(); 
    if (wooting.Initialise(out int deviceCount)) 
    { 
      Console.WriteLine($"Found {deviceCount} Wooting device(s).");
      // Read analog value for the 'A' key
      float analogValue = wooting.ReadAnalog(KeyCode.A);
      Console.WriteLine($"Analog value for 'A': {analogValue}");
  
      // Wait for 'A' key to be fully pressed
      wooting.WaitKeyFullyDown(KeyCode.A);
      Console.WriteLine("'A' key fully pressed!");
    }
    else
    {
      Console.WriteLine("Failed to initialize Wooting Analog SDK.");
    }
  }
}
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
