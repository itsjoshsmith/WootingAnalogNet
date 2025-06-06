using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WootingAnalogNet
{
  public class WootingAnalog : IDisposable
  {
    private IntPtr DLLHandle = IntPtr.Zero;
    public EKeyCodeMode KeyCodeMode { get; private set; }

    #region Delegates

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WootingAnalogInitialiseDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WootingAnalogUninitialiseDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float WootingAnalogReadAnalogDelegate(ushort KeyCode);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float WootingAnalogReadAnalogDeviceDelegate(ushort KeyCode, ulong DeviceID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WootingAnalogSetKeyCodeModeDelegate(int Mode);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WootingAnalogGetDevicesDelegate(out IntPtr Devices, int Count);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool WootingAnalogIsInitialisedDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WootingAnalogReadFullBufferDelegate([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] ushort[] CodeBuffer, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] float[] AnalogBuffer, uint Length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WootingAnalogReadFullBufferDeviceDelegate([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] ushort[] CodeBuffer, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] float[] AnalogBuffer, uint Length, ulong DeviceID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DeviceEventCallback(EWootingAnalogDeviceEventType EventType, ref WootingAnalogDeviceInfo DeviceInfo);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WootingAnalogSetDeviceEventCbDelegate(DeviceEventCallback cb);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WootingAnalogClearDeviceCallbackDelegate();

    private WootingAnalogInitialiseDelegate WootingAnalogInitialise;
    private WootingAnalogUninitialiseDelegate WootingAnalogUninitialise;
    private WootingAnalogReadAnalogDelegate WootingAnalogReadAnalog;
    private WootingAnalogReadAnalogDeviceDelegate WootingAnalogReadAnalogDevice;
    private WootingAnalogSetKeyCodeModeDelegate WootingAnalogSetKeyCodeMode;
    private WootingAnalogGetDevicesDelegate WootingAnalogGetConnectedDevicesInfo;
    private WootingAnalogIsInitialisedDelegate WootingAnalogIsInitialised;
    private WootingAnalogReadFullBufferDelegate WootingAnalogReadFullBuffer;
    private WootingAnalogReadFullBufferDeviceDelegate WootingAnalogReadFullBufferDevice;
    private WootingAnalogSetDeviceEventCbDelegate WootingAnalogSetDeviceEventCb;
    private DeviceEventCallback ManagedDeviceEventCallback; // Keep a reference to prevent GC
    private WootingAnalogClearDeviceCallbackDelegate WootingClearDeviceCallback; // Keep a reference to prevent GC

    #endregion

    #region Wrapper Functions

    /// <summary>
    /// Initialises the Analog SDK, this needs to be successfully called before any other functions of the SDK can be called
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public int Initialise()
    {
      KeyCodeMode = EKeyCodeMode.HID;

      if (LoadLibrary("wooting_analog_wrapper.dll"))
      {
        if (WootingAnalogInitialise == null)
          throw new InvalidOperationException("Function pointer for wooting_analog_initialise not loaded.");
        return WootingAnalogInitialise();
      }

      throw new InvalidOperationException("Failed to load wooting_analog_wrapper.dll");
    }

    /// <summary>
    /// Uninitialises the SDK, returning it to an empty state, similar to how it would be before first initialisation
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public WootingAnalogResult Uninitialise()
    {
      if (WootingAnalogUninitialise == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_uninitialise not loaded.");

      return (WootingAnalogResult)WootingAnalogUninitialise();
    }

    /// <summary>
    /// Reads the Analog value of the key with identifier code from any connected device (or the device with id device_id if specified). The set of key identifiers that is used depends on the Keycode mode set using Set Keycode Mode.
    /// </summary>
    /// <param name="KeyCode"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public float ReadAnalog(ushort KeyCode)
    {
      if (WootingAnalogReadAnalog == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_read_analog not loaded.");
      return WootingAnalogReadAnalog(KeyCode);
    }

    /// <summary>
    /// Reads the Analog value of the key with identifier code from any connected device (or the device with id device_id if specified). The set of key identifiers that is used depends on the Keycode mode set using Set Keycode Mode.
    /// </summary>
    /// <param name="KeyCode"></param>
    /// <param name="DeviceID"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public float ReadAnalog(ushort KeyCode, ulong DeviceID)
    {
      if (WootingAnalogReadAnalogDevice == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_read_analog_device not loaded.");
      return WootingAnalogReadAnalogDevice(KeyCode, DeviceID);
    }

    /// <summary>
    /// Sets the type of Keycodes the Analog SDK will receive (in read_analog) and output (in read_full_buffer).
    /// By default, the mode is set to HID
    /// </summary>
    /// <param name="Mode"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public WootingAnalogResult SetKeyCodeMode(EKeyCodeMode Mode)
    {
      if (WootingAnalogSetKeyCodeMode == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_set_keycode_mode not loaded.");
      WootingAnalogResult Result = (WootingAnalogResult)WootingAnalogSetKeyCodeMode((int)Mode);
      if(Result == WootingAnalogResult.Ok)
        KeyCodeMode = Mode;
      return Result;
    }

    /// <summary>
    /// Fills up the given buffer (that has length len) with pointers to the DeviceInfo structs for all connected devices (as many that can fit in the buffer)
    /// </summary>
    /// <param name="Devices"></param>
    /// <param name="Count"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public int GetConnectedDevicesInfo(out WootingAnalogDeviceInfo[] Devices, int Count)
    {
      Devices = null;
      if (WootingAnalogGetConnectedDevicesInfo == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_get_connected_devices_info not loaded.");

      IntPtr nativeArray;

      int result = WootingAnalogGetConnectedDevicesInfo(out nativeArray, Count);

      if (result >= 0 && nativeArray != IntPtr.Zero)
      {
        Devices = new WootingAnalogDeviceInfo[Count];
        int structSize = Marshal.SizeOf(typeof(WootingAnalogDeviceInfo));
        for (int i = 0; i < Count; i++)
        {
          IntPtr structPtr = IntPtr.Add(nativeArray, i * structSize);
          Devices[i] = Marshal.PtrToStructure<WootingAnalogDeviceInfo>(structPtr);
        }
      }

      return result;
    }

    /// <summary>
    /// Returns a bool indicating if the Analog SDK has been initialised
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool IsInitialised()
    {
      if (WootingAnalogIsInitialised == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_is_initialised not loaded.");
      return WootingAnalogIsInitialised();
    }

    /// <summary>
    /// Reads all the analog values for pressed keys for all devices and combines their values (or reads from a single device with id device_id [if specified]), 
    /// filling up code_buffer with the keycode identifying the pressed key and fills up analog_buffer with the corresponding float analog values. 
    /// i.e. The analog value for they key at index 0 of code_buffer, is at index 0 of analog_buffer.
    /// </summary>
    /// <param name="CodeBuffer"></param>
    /// <param name="AnalogBuffer"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public int ReadFullBuffer(ushort[] CodeBuffer, float[] AnalogBuffer, uint Length)
    {
      if (WootingAnalogReadFullBuffer == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_read_full_buffer not loaded.");
      if (CodeBuffer == null || AnalogBuffer == null)
        throw new ArgumentNullException("Buffers must not be null.");
      if (CodeBuffer.Length < Length || AnalogBuffer.Length < Length)
        throw new ArgumentException("Buffers must be at least 'Length' in size.");
      return WootingAnalogReadFullBuffer(CodeBuffer, AnalogBuffer, Length);
    }

    /// <summary>
    /// Reads all the analog values for pressed keys for all devices and combines their values (or reads from a single device with id device_id [if specified]), 
    /// filling up code_buffer with the keycode identifying the pressed key and fills up analog_buffer with the corresponding float analog values. 
    /// i.e. The analog value for they key at index 0 of code_buffer, is at index 0 of analog_buffer.
    /// </summary>
    /// <param name="CodeBuffer"></param>
    /// <param name="AnalogBuffer"></param>
    /// <param name="Length"></param>
    /// <param name="DeviceID"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public int ReadFullBuffer(ushort[] CodeBuffer, float[] AnalogBuffer, uint Length, ulong DeviceID)
    {
      if (WootingAnalogReadFullBufferDevice == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_read_full_buffer_device not loaded.");
      if (CodeBuffer == null || AnalogBuffer == null)
        throw new ArgumentNullException("Buffers must not be null.");
      if (CodeBuffer.Length < Length || AnalogBuffer.Length < Length)
        throw new ArgumentException("Buffers must be at least 'len' in size.");
      return WootingAnalogReadFullBufferDevice(CodeBuffer, AnalogBuffer, Length, DeviceID);
    }

    /// <summary>
    /// Set the callback which is called when there is a DeviceEvent. 
    /// Currently these events can either be Disconnected or Connected(Currently not properly implemented).
    /// The callback gets given the type of event DeviceEventType and a pointer to the DeviceInfo struct that the event applies to.
    /// </summary>
    /// <param name="Callback"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public WootingAnalogResult SetDeviceEventCallback(DeviceEventCallback Callback)
    {
      if (WootingAnalogSetDeviceEventCb == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_set_device_event_cb not loaded.");
      ManagedDeviceEventCallback = Callback; // Prevent delegate from being garbage collected
      return (WootingAnalogResult)WootingAnalogSetDeviceEventCb(ManagedDeviceEventCallback);
    }

    /// <summary>
    /// Clears the device event callback that has been set
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public WootingAnalogResult ClearDeviceEventCallback()
    {
      if (WootingClearDeviceCallback == null)
        throw new InvalidOperationException("Function pointer for wooting_analog_clear_device_event_cb not loaded.");
      return (WootingAnalogResult)WootingClearDeviceCallback();
    }

    #endregion

    #region Native Methods / DLL Handling

    private bool LoadLibrary(string dllPath)
    {
      DLLHandle = NativeMethods.LoadLibrary(dllPath);
      if (DLLHandle != IntPtr.Zero)
      {
        IntPtr proc = GetProcAddress("wooting_analog_initialise");
        WootingAnalogInitialise = proc != IntPtr.Zero ? (WootingAnalogInitialiseDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogInitialiseDelegate)) : null;

        proc = GetProcAddress("wooting_analog_uninitialise");
        WootingAnalogUninitialise = proc != IntPtr.Zero ? (WootingAnalogUninitialiseDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogUninitialiseDelegate)) : null;
        
        proc = GetProcAddress("wooting_analog_read_analog");
        WootingAnalogReadAnalog = proc != IntPtr.Zero ? (WootingAnalogReadAnalogDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogReadAnalogDelegate)) : null;
        
        proc = GetProcAddress("wooting_analog_read_analog_device");
        WootingAnalogReadAnalogDevice = proc != IntPtr.Zero ? (WootingAnalogReadAnalogDeviceDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogReadAnalogDeviceDelegate)) : null;
        
        proc = GetProcAddress("wooting_analog_set_keycode_mode");
        WootingAnalogSetKeyCodeMode = proc != IntPtr.Zero ? (WootingAnalogSetKeyCodeModeDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogSetKeyCodeModeDelegate)) : null;
        
        proc = GetProcAddress("wooting_analog_get_connected_devices_info");
        WootingAnalogGetConnectedDevicesInfo = proc != IntPtr.Zero ? (WootingAnalogGetDevicesDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogGetDevicesDelegate)) : null;
        
        proc = GetProcAddress("wooting_analog_is_initialised");
        WootingAnalogIsInitialised = proc != IntPtr.Zero ? (WootingAnalogIsInitialisedDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogIsInitialisedDelegate)) : null;
        
        proc = GetProcAddress("wooting_analog_read_full_buffer");
        WootingAnalogReadFullBuffer = proc != IntPtr.Zero ? (WootingAnalogReadFullBufferDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogReadFullBufferDelegate)) : null;
        
        proc = GetProcAddress("wooting_analog_read_full_buffer_device");
        WootingAnalogReadFullBufferDevice = proc != IntPtr.Zero ? (WootingAnalogReadFullBufferDeviceDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogReadFullBufferDeviceDelegate)) : null;
        
        proc = GetProcAddress("wooting_analog_set_device_event_cb");
        WootingAnalogSetDeviceEventCb = proc != IntPtr.Zero ? (WootingAnalogSetDeviceEventCbDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogSetDeviceEventCbDelegate)) : null;

        proc = GetProcAddress("wooting_analog_clear_device_event_cb");
        WootingClearDeviceCallback = proc != IntPtr.Zero ? (WootingAnalogClearDeviceCallbackDelegate)Marshal.GetDelegateForFunctionPointer(proc, typeof(WootingAnalogClearDeviceCallbackDelegate)) : null;

      }
      return DLLHandle != IntPtr.Zero;
    }

    private void FreeLibrary()
    {
      if (DLLHandle != IntPtr.Zero)
      {
        NativeMethods.FreeLibrary(DLLHandle);
        DLLHandle = IntPtr.Zero;
        WootingAnalogInitialise = null;
      }
    }

    private IntPtr GetProcAddress(string procName)
    {
      if (DLLHandle == IntPtr.Zero)
        throw new InvalidOperationException("DLL not loaded.");
      return NativeMethods.GetProcAddress(DLLHandle, procName);
    }

    public void Dispose()
    {
      FreeLibrary();
      GC.SuppressFinalize(this);
    }

    ~WootingAnalog()
    {
      FreeLibrary();
    }

    private static class NativeMethods
    {
      [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
      public static extern IntPtr LoadLibrary(string lpFileName);

      [DllImport("kernel32", SetLastError = true)]
      public static extern bool FreeLibrary(IntPtr hModule);

      [DllImport("kernel32", SetLastError = true)]
      public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    }

    #endregion
  }

  #region Enums and Structs

  public enum EKeyCodeMode
  {
    /// <summary>USB HID Keycodes https://www.usb.org/document-library/hid-usage-tables-112 pg53</summary>
    HID = 0,
    /// <summary>Scan code set 1</summary>
    ScanCode1 = 1,
    /// <summary>Windows Virtual Keys</summary>
    VirtualKey = 2,
    /// <summary>Windows Virtual Keys which are translated to the current keyboard locale</summary>
    VirtualKeyTranslate = 3
  }

  public enum WootingAnalogResult
  {
    /// <summary>Success</summary>
    Ok = 1,
    /// <summary>Item hasn't been initialised</summary>
    UnInitialised = -2000,
    /// <summary>No Devices are connected</summary>
    NoDevices = -1999,
    /// <summary>Device has been disconnected</summary>
    DeviceDisconnected = -1998,
    /// <summary>Generic Failure</summary>
    Failure = -1997,
    /// <summary>A given parameter was invalid</summary>
    InvalidArgument = -1996,
    /// <summary>No Plugins were found</summary>
    NoPlugins = -1995,
    /// <summary>The specified function was not found in the library</summary>
    FunctionNotFound = -1994,
    /// <summary>No Keycode mapping to HID was found for the given Keycode</summary>
    NoMapping = -1993,
    /// <summary>Indicates that it isn't available on this platform</summary>
    NotAvailable = -1992,
    /// <summary>Indicates that the operation that is trying to be used is for an older version</summary>
    IncompatibleVersion = -1991,
    /// <summary>Indicates that the Analog SDK could not be found on the system</summary>
    DLLNotFound = -1990
  }

  public enum EWootingAnalogDeviceEventType
  {
    /// <summary>Device has been connected</summary>
    DeviceConnected = 1,
    /// <summary>Device has been disconnected</summary>
    DeviceDisconnected = 2,
  }

  public enum EWootingDeviceType
  {
    /// <summary>Device is of type Keyboard</summary>
    Keyboard = 1,
    /// <summary>Device is of type Keypad</summary>
    Keypad,
    /// <summary>Device</summary>
    Other
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct WootingAnalogDeviceInfo
  {
    /// <summary>Device Vendor ID `vid`</summary>
    public ushort VendorID;
    /// <summary>Device Product ID `pid`</summary>
    public ushort ProductID;
    /// <summary>Pointer to the raw ManName str</summary>
    private IntPtr ManufacturerNamePtr;
    /// <summary>Pointer to the raw DevName str</summary>
    private IntPtr DeviceNamePtr;
    /// <summary>Unique device ID, which should be generated using `generate_device_id`</summary>
    public ulong DeviceID;
    /// <summary>Hardware type of the Device</summary>
    public EWootingDeviceType DeviceType;
    /// <summary>Device Manufacturer name</summary>
    public string ManufacturerName => Marshal.PtrToStringAnsi(ManufacturerNamePtr);
    /// <summary>Device name</summary>
    public string DeviceName => Marshal.PtrToStringAnsi(DeviceNamePtr);
  }

  #endregion

}