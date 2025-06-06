using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WootingAnalogNet;

namespace WootingAnalogNet.GeneralExtensions
{
  public static class WootingAnalogGeneralExtensions
  {

    #region ReadAnalog

    /// <summary>
    /// Reads the Analog value of the key with identifier code from any connected device (or the device with id device_id if specified). The set of key identifiers that is used depends on the Keycode mode set using Set Keycode Mode.
    /// </summary>
    /// <param name="KeyCode"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static float ReadAnalog(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      return Wrapper.ReadAnalog(KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    /// <summary>
    /// Reads the Analog value of the key with identifier code from any connected device (or the device with id device_id if specified). The set of key identifiers that is used depends on the Keycode mode set using Set Keycode Mode.
    /// </summary>
    /// <param name="KeyCode"></param>
    /// <param name="DeviceID"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static float ReadAnalog(this WootingAnalog Wrapper, KeyCode KeyCode, ulong DeviceID)
    {
      return Wrapper.ReadAnalog(KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode), DeviceID);
    }

    #endregion

    #region WaitKeyDown

    public static void WaitKeyDown(this WootingAnalog Wrapper, ushort KeyCode)
    {
      while (Wrapper.ReadAnalog(KeyCode) > 0) { Thread.Sleep(1); }
    }

    public static void WaitKeyDown(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      WaitKeyDown(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    #endregion

    #region WaitKeyUp

    public static void WaitKeyUp(this WootingAnalog Wrapper, ushort KeyCode)
    {
      while (Wrapper.ReadAnalog(KeyCode) < 1) { Thread.Sleep(1); }
    }

    public static void WaitKeyUp(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      WaitKeyUp(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    #endregion

    #region WaitKeyFullyDown

    public static void WaitKeyFullyDown(this WootingAnalog Wrapper, ushort KeyCode)
    {
      while (Wrapper.ReadAnalog(KeyCode) != 1) { Thread.Sleep(1); }
    }

    public static void WaitKeyFullyDown(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      WaitKeyFullyDown(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    #endregion

    #region WaitKeyFullyUp

    public static void WaitKeyFullyUp(this WootingAnalog Wrapper, ushort KeyCode)
    {
      while (Wrapper.ReadAnalog(KeyCode) != 0) { Thread.Sleep(1); }
    }

    public static void WaitKeyFullyUp(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      WaitKeyFullyUp(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    #endregion

    #region IsKeyFullyDown

    public static bool IsKeyFullyDown(this WootingAnalog Wrapper, ushort KeyCode)
    {
      return Wrapper.ReadAnalog(KeyCode) == 1;
    }

    public static bool IsKeyFullyDown(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      return IsKeyFullyDown(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    #endregion

    #region IsKeyFullyUp

    public static bool IsKeyFullyUp(this WootingAnalog Wrapper, ushort KeyCode)
    {
      return Wrapper.ReadAnalog(KeyCode) == 0;
    }

    public static bool IsKeyFullyUp(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      return IsKeyFullyUp(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }


    #endregion

    #region IsKeyDown

    public static bool IsKeyDown(this WootingAnalog Wrapper, ushort KeyCode)
    {
      return Wrapper.ReadAnalog(KeyCode) > 0;
    }

    public static bool IsKeyDown(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      return IsKeyDown(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    #endregion

    #region IsKeyUp

    public static bool IsKeyUp(this WootingAnalog Wrapper, ushort KeyCode)
    {
      return Wrapper.ReadAnalog(KeyCode) < 1;
    }

    public static bool IsKeyUp(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      return IsKeyUp(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }


    #endregion

    #region WaitForAboveThreshold

    public static void WaitForAboveThreshold(this WootingAnalog Wrapper, ushort KeyCode, float Threshold)
    {
      while (Wrapper.ReadAnalog(KeyCode) > Threshold) Thread.Sleep(1);
    }

    public static void WaitForAboveThreshold(this WootingAnalog Wrapper, KeyCode KeyCode, float Threshold)
    {
      WaitForAboveThreshold(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode), Threshold);
    }

    #endregion

    #region WaitForBelowThreshold

    public static void WaitForBelowThreshold(this WootingAnalog Wrapper, ushort KeyCode, float Threshold)
    {
      while (Wrapper.ReadAnalog(KeyCode) < Threshold) Thread.Sleep(1);
    }

    public static void WaitForBelowThreshold(this WootingAnalog Wrapper, KeyCode KeyCode, float Threshold)
    {
      WaitForBelowThreshold(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode), Threshold);
    }

    #endregion

    #region WaitKeyPress

    public static void WaitKeyPress(this WootingAnalog Wrapper, ushort KeyCode)
    {
      // Wait for key down (value == 1)
      while (Wrapper.ReadAnalog(KeyCode) < 1f)
        Thread.Sleep(1);

      // Wait for key up (value == 0)
      while (Wrapper.ReadAnalog(KeyCode) > 0f)
        Thread.Sleep(1);
    }

    public static void WaitKeyPress(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      WaitKeyPress(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    #endregion

    #region GetAnalogState

    public static List<(ushort KeyCode, float Value)> GetAnalogState(this WootingAnalog Wrapper, int BufferSize = 64)
    {
      ushort[] codes = new ushort[BufferSize];
      float[] values = new float[BufferSize];
      int count = Wrapper.ReadFullBuffer(codes, values, (uint)BufferSize);
      List<(ushort, float)> result = new List<(ushort, float)>();
      for (int i = 0; i < count; i++)
        result.Add((codes[i], values[i]));
      return result;
    }

    #endregion

    #region GetKeyValueAsPercent

    public static float GetKeyValueAsPercent(this WootingAnalog Wrapper, ushort KeyCode)
    {
      return Wrapper.ReadAnalog(KeyCode) * 100f;
    }

    public static float GetKeyValueAsPercent(this WootingAnalog Wrapper, KeyCode KeyCode)
    {
      return GetKeyValueAsPercent(Wrapper, KeyCodeTranslator.ToNativeKeyCode(KeyCode, Wrapper.KeyCodeMode));
    }

    #endregion

    #region Trivial

    public static bool Initialise(this WootingAnalog Wrapper, out int DeviceCount)
    {
      DeviceCount = Wrapper.Initialise();
      return Wrapper.IsInitialised();
    }

    public static float ValueToPercent(this WootingAnalog Wrapper, float Value)
    {
      return Value * 100f;
    }

    #endregion

    #region KeyCode translation

    /// <summary>
    /// Provides translation from KeyCode to the native keycode for the current mode.
    /// </summary>
    public static class KeyCodeTranslator
    {
      private static readonly Dictionary<KeyCode, EScanCode1> ScanCode1Map = new Dictionary<KeyCode, EScanCode1>
      {
        { KeyCode.Esc, EScanCode1.Esc },
        { KeyCode.D1, EScanCode1.D1 },
        { KeyCode.D2, EScanCode1.D2 },
        { KeyCode.D3, EScanCode1.D3 },
        { KeyCode.D4, EScanCode1.D4 },
        { KeyCode.D5, EScanCode1.D5 },
        { KeyCode.D6, EScanCode1.D6 },
        { KeyCode.D7, EScanCode1.D7 },
        { KeyCode.D8, EScanCode1.D8 },
        { KeyCode.D9, EScanCode1.D9 },
        { KeyCode.D0, EScanCode1.D0 },
        { KeyCode.Minus, EScanCode1.Minus },
        { KeyCode.Equals, EScanCode1.Equals },
        { KeyCode.Backspace, EScanCode1.Backspace },
        { KeyCode.Tab, EScanCode1.Tab },
        { KeyCode.Q, EScanCode1.Q },
        { KeyCode.W, EScanCode1.W },
        { KeyCode.E, EScanCode1.E },
        { KeyCode.R, EScanCode1.R },
        { KeyCode.T, EScanCode1.T },
        { KeyCode.Y, EScanCode1.Y },
        { KeyCode.U, EScanCode1.U },
        { KeyCode.I, EScanCode1.I },
        { KeyCode.O, EScanCode1.O },
        { KeyCode.P, EScanCode1.P },
        { KeyCode.LeftBracket, EScanCode1.LeftBracket },
        { KeyCode.RightBracket, EScanCode1.RightBracket },
        { KeyCode.Enter, EScanCode1.Enter },
        { KeyCode.LeftCtrl, EScanCode1.LeftCtrl },
        { KeyCode.A, EScanCode1.A },
        { KeyCode.S, EScanCode1.S },
        { KeyCode.D, EScanCode1.D },
        { KeyCode.F, EScanCode1.F },
        { KeyCode.G, EScanCode1.G },
        { KeyCode.H, EScanCode1.H },
        { KeyCode.J, EScanCode1.J },
        { KeyCode.K, EScanCode1.K },
        { KeyCode.L, EScanCode1.L },
        { KeyCode.Semicolon, EScanCode1.Semicolon },
        { KeyCode.Apostrophe, EScanCode1.Apostrophe },
        { KeyCode.Grave, EScanCode1.Grave },
        { KeyCode.LeftShift, EScanCode1.LeftShift },
        { KeyCode.Backslash, EScanCode1.Backslash },
        { KeyCode.Z, EScanCode1.Z },
        { KeyCode.X, EScanCode1.X },
        { KeyCode.C, EScanCode1.C },
        { KeyCode.V, EScanCode1.V },
        { KeyCode.B, EScanCode1.B },
        { KeyCode.N, EScanCode1.N },
        { KeyCode.M, EScanCode1.M },
        { KeyCode.Comma, EScanCode1.Comma },
        { KeyCode.Period, EScanCode1.Period },
        { KeyCode.Slash, EScanCode1.Slash },
        { KeyCode.RightShift, EScanCode1.RightShift },
        { KeyCode.NumpadAsterisk, EScanCode1.NumpadAsterisk },
        { KeyCode.LeftAlt, EScanCode1.LeftAlt },
        { KeyCode.Space, EScanCode1.Space },
        { KeyCode.CapsLock, EScanCode1.CapsLock },
        { KeyCode.F1, EScanCode1.F1 },
        { KeyCode.F2, EScanCode1.F2 },
        { KeyCode.F3, EScanCode1.F3 },
        { KeyCode.F4, EScanCode1.F4 },
        { KeyCode.F5, EScanCode1.F5 },
        { KeyCode.F6, EScanCode1.F6 },
        { KeyCode.F7, EScanCode1.F7 },
        { KeyCode.F8, EScanCode1.F8 },
        { KeyCode.F9, EScanCode1.F9 },
        { KeyCode.F10, EScanCode1.F10 },
        { KeyCode.NumLock, EScanCode1.NumLock },
        { KeyCode.ScrollLock, EScanCode1.ScrollLock },
        { KeyCode.Numpad7, EScanCode1.Numpad7 },
        { KeyCode.Numpad8, EScanCode1.Numpad8 },
        { KeyCode.Numpad9, EScanCode1.Numpad9 },
        { KeyCode.NumpadMinus, EScanCode1.NumpadMinus },
        { KeyCode.Numpad4, EScanCode1.Numpad4 },
        { KeyCode.Numpad5, EScanCode1.Numpad5 },
        { KeyCode.Numpad6, EScanCode1.Numpad6 },
        { KeyCode.NumpadPlus, EScanCode1.NumpadPlus },
        { KeyCode.Numpad1, EScanCode1.Numpad1 },
        { KeyCode.Numpad2, EScanCode1.Numpad2 },
        { KeyCode.Numpad3, EScanCode1.Numpad3 },
        { KeyCode.Numpad0, EScanCode1.Numpad0 },
        { KeyCode.NumpadPeriod, EScanCode1.NumpadPeriod },
        { KeyCode.F11, EScanCode1.F11 },
        { KeyCode.F12, EScanCode1.F12 },
        { KeyCode.RightCtrl, EScanCode1.RightCtrl },
        { KeyCode.RightAlt, EScanCode1.RightAlt },
        { KeyCode.Home, EScanCode1.Home },
        { KeyCode.Up, EScanCode1.Up },
        { KeyCode.PageUp, EScanCode1.PageUp },
        { KeyCode.Left, EScanCode1.Left },
        { KeyCode.Right, EScanCode1.Right },
        { KeyCode.End, EScanCode1.End },
        { KeyCode.Down, EScanCode1.Down },
        { KeyCode.PageDown, EScanCode1.PageDown },
        { KeyCode.Insert, EScanCode1.Insert },
        { KeyCode.Delete, EScanCode1.Delete }
      };

      private static readonly Dictionary<KeyCode, EHIDKey> HidKeyMap = new Dictionary<KeyCode, EHIDKey>
      {
        { KeyCode.Esc, EHIDKey.Escape },
        { KeyCode.D1, EHIDKey.D1 },
        { KeyCode.D2, EHIDKey.D2 },
        { KeyCode.D3, EHIDKey.D3 },
        { KeyCode.D4, EHIDKey.D4 },
        { KeyCode.D5, EHIDKey.D5 },
        { KeyCode.D6, EHIDKey.D6 },
        { KeyCode.D7, EHIDKey.D7 },
        { KeyCode.D8, EHIDKey.D8 },
        { KeyCode.D9, EHIDKey.D9 },
        { KeyCode.D0, EHIDKey.D0 },
        { KeyCode.Minus, EHIDKey.Minus },
        { KeyCode.Equals, EHIDKey.Equals },
        { KeyCode.Backspace, EHIDKey.Backspace },
        { KeyCode.Tab, EHIDKey.Tab },
        { KeyCode.Q, EHIDKey.Q },
        { KeyCode.W, EHIDKey.W },
        { KeyCode.E, EHIDKey.E },
        { KeyCode.R, EHIDKey.R },
        { KeyCode.T, EHIDKey.T },
        { KeyCode.Y, EHIDKey.Y },
        { KeyCode.U, EHIDKey.U },
        { KeyCode.I, EHIDKey.I },
        { KeyCode.O, EHIDKey.O },
        { KeyCode.P, EHIDKey.P },
        { KeyCode.LeftBracket, EHIDKey.LeftBracket },
        { KeyCode.RightBracket, EHIDKey.RightBracket },
        { KeyCode.Enter, EHIDKey.Enter },
        { KeyCode.LeftCtrl, EHIDKey.None },
        { KeyCode.A, EHIDKey.A },
        { KeyCode.S, EHIDKey.S },
        { KeyCode.D, EHIDKey.D },
        { KeyCode.F, EHIDKey.F },
        { KeyCode.G, EHIDKey.G },
        { KeyCode.H, EHIDKey.H },
        { KeyCode.J, EHIDKey.J },
        { KeyCode.K, EHIDKey.K },
        { KeyCode.L, EHIDKey.L },
        { KeyCode.Semicolon, EHIDKey.Semicolon },
        { KeyCode.Apostrophe, EHIDKey.Apostrophe },
        { KeyCode.Grave, EHIDKey.Grave },
        { KeyCode.LeftShift, EHIDKey.None },
        { KeyCode.Backslash, EHIDKey.Backslash },
        { KeyCode.Z, EHIDKey.Z },
        { KeyCode.X, EHIDKey.X },
        { KeyCode.C, EHIDKey.C },
        { KeyCode.V, EHIDKey.V },
        { KeyCode.B, EHIDKey.B },
        { KeyCode.N, EHIDKey.N },
        { KeyCode.M, EHIDKey.M },
        { KeyCode.Comma, EHIDKey.Comma },
        { KeyCode.Period, EHIDKey.Period },
        { KeyCode.Slash, EHIDKey.Slash },
        { KeyCode.RightShift, EHIDKey.None },
        { KeyCode.NumpadAsterisk, EHIDKey.KeypadAsterisk },
        { KeyCode.LeftAlt, EHIDKey.None },
        { KeyCode.Space, EHIDKey.Space },
        { KeyCode.CapsLock, EHIDKey.CapsLock },
        { KeyCode.F1, EHIDKey.F1 },
        { KeyCode.F2, EHIDKey.F2 },
        { KeyCode.F3, EHIDKey.F3 },
        { KeyCode.F4, EHIDKey.F4 },
        { KeyCode.F5, EHIDKey.F5 },
        { KeyCode.F6, EHIDKey.F6 },
        { KeyCode.F7, EHIDKey.F7 },
        { KeyCode.F8, EHIDKey.F8 },
        { KeyCode.F9, EHIDKey.F9 },
        { KeyCode.F10, EHIDKey.F10 },
        { KeyCode.NumLock, EHIDKey.NumLock },
        { KeyCode.ScrollLock, EHIDKey.ScrollLock },
        { KeyCode.Numpad7, EHIDKey.Keypad7 },
        { KeyCode.Numpad8, EHIDKey.Keypad8 },
        { KeyCode.Numpad9, EHIDKey.Keypad9 },
        { KeyCode.NumpadMinus, EHIDKey.KeypadMinus },
        { KeyCode.Numpad4, EHIDKey.Keypad4 },
        { KeyCode.Numpad5, EHIDKey.Keypad5 },
        { KeyCode.Numpad6, EHIDKey.Keypad6 },
        { KeyCode.NumpadPlus, EHIDKey.KeypadPlus },
        { KeyCode.Numpad1, EHIDKey.Keypad1 },
        { KeyCode.Numpad2, EHIDKey.Keypad2 },
        { KeyCode.Numpad3, EHIDKey.Keypad3 },
        { KeyCode.Numpad0, EHIDKey.Keypad0 },
        { KeyCode.NumpadPeriod, EHIDKey.KeypadPeriod },
        { KeyCode.F11, EHIDKey.F11 },
        { KeyCode.F12, EHIDKey.F12 },
        { KeyCode.RightCtrl, EHIDKey.None },
        { KeyCode.RightAlt, EHIDKey.None },
        { KeyCode.Home, EHIDKey.Home },
        { KeyCode.Up, EHIDKey.UpArrow },
        { KeyCode.PageUp, EHIDKey.PageUp },
        { KeyCode.Left, EHIDKey.LeftArrow },
        { KeyCode.Right, EHIDKey.RightArrow },
        { KeyCode.End, EHIDKey.End },
        { KeyCode.Down, EHIDKey.DownArrow },
        { KeyCode.PageDown, EHIDKey.PageDown },
        { KeyCode.Insert, EHIDKey.Insert },
        { KeyCode.Delete, EHIDKey.Delete }
      };

      private static readonly Dictionary<KeyCode, EVirtualKey> VirtualKeyMap = new Dictionary<KeyCode, EVirtualKey>
      {
        { KeyCode.Esc, EVirtualKey.ESCAPE },
        { KeyCode.D1, EVirtualKey.D1 },
        { KeyCode.D2, EVirtualKey.D2 },
        { KeyCode.D3, EVirtualKey.D3 },
        { KeyCode.D4, EVirtualKey.D4 },
        { KeyCode.D5, EVirtualKey.D5 },
        { KeyCode.D6, EVirtualKey.D6 },
        { KeyCode.D7, EVirtualKey.D7 },
        { KeyCode.D8, EVirtualKey.D8 },
        { KeyCode.D9, EVirtualKey.D9 },
        { KeyCode.D0, EVirtualKey.D0 },
        { KeyCode.Minus, EVirtualKey.OEM_MINUS },
        { KeyCode.Equals, EVirtualKey.OEM_PLUS },
        { KeyCode.Backspace, EVirtualKey.BACK },
        { KeyCode.Tab, EVirtualKey.TAB },
        { KeyCode.Q, EVirtualKey.Q },
        { KeyCode.W, EVirtualKey.W },
        { KeyCode.E, EVirtualKey.E },
        { KeyCode.R, EVirtualKey.R },
        { KeyCode.T, EVirtualKey.T },
        { KeyCode.Y, EVirtualKey.Y },
        { KeyCode.U, EVirtualKey.U },
        { KeyCode.I, EVirtualKey.I },
        { KeyCode.O, EVirtualKey.O },
        { KeyCode.P, EVirtualKey.P },
        { KeyCode.LeftBracket, EVirtualKey.OEM_4 },
        { KeyCode.RightBracket, EVirtualKey.OEM_6 },
        { KeyCode.Enter, EVirtualKey.RETURN },
        { KeyCode.LeftCtrl, EVirtualKey.LCONTROL },
        { KeyCode.A, EVirtualKey.A },
        { KeyCode.S, EVirtualKey.S },
        { KeyCode.D, EVirtualKey.D },
        { KeyCode.F, EVirtualKey.F },
        { KeyCode.G, EVirtualKey.G },
        { KeyCode.H, EVirtualKey.H },
        { KeyCode.J, EVirtualKey.J },
        { KeyCode.K, EVirtualKey.K },
        { KeyCode.L, EVirtualKey.L },
        { KeyCode.Semicolon, EVirtualKey.OEM_1 },
        { KeyCode.Apostrophe, EVirtualKey.OEM_7 },
        { KeyCode.Grave, EVirtualKey.OEM_3 },
        { KeyCode.LeftShift, EVirtualKey.LSHIFT },
        { KeyCode.Backslash, EVirtualKey.OEM_5 },
        { KeyCode.Z, EVirtualKey.Z },
        { KeyCode.X, EVirtualKey.X },
        { KeyCode.C, EVirtualKey.C },
        { KeyCode.V, EVirtualKey.V },
        { KeyCode.B, EVirtualKey.B },
        { KeyCode.N, EVirtualKey.N },
        { KeyCode.M, EVirtualKey.M },
        { KeyCode.Comma, EVirtualKey.OEM_COMMA },
        { KeyCode.Period, EVirtualKey.OEM_PERIOD },
        { KeyCode.Slash, EVirtualKey.OEM_2 },
        { KeyCode.RightShift, EVirtualKey.RSHIFT },
        { KeyCode.NumpadAsterisk, EVirtualKey.MULTIPLY },
        { KeyCode.LeftAlt, EVirtualKey.LMENU },
        { KeyCode.Space, EVirtualKey.SPACE },
        { KeyCode.CapsLock, EVirtualKey.CAPITAL },
        { KeyCode.F1, EVirtualKey.F1 },
        { KeyCode.F2, EVirtualKey.F2 },
        { KeyCode.F3, EVirtualKey.F3 },
        { KeyCode.F4, EVirtualKey.F4 },
        { KeyCode.F5, EVirtualKey.F5 },
        { KeyCode.F6, EVirtualKey.F6 },
        { KeyCode.F7, EVirtualKey.F7 },
        { KeyCode.F8, EVirtualKey.F8 },
        { KeyCode.F9, EVirtualKey.F9 },
        { KeyCode.F10, EVirtualKey.F10 },
        { KeyCode.NumLock, EVirtualKey.NUMLOCK },
        { KeyCode.ScrollLock, EVirtualKey.SCROLL },
        { KeyCode.Numpad7, EVirtualKey.NUMPAD7 },
        { KeyCode.Numpad8, EVirtualKey.NUMPAD8 },
        { KeyCode.Numpad9, EVirtualKey.NUMPAD9 },
        { KeyCode.NumpadMinus, EVirtualKey.SUBTRACT },
        { KeyCode.Numpad4, EVirtualKey.NUMPAD4 },
        { KeyCode.Numpad5, EVirtualKey.NUMPAD5 },
        { KeyCode.Numpad6, EVirtualKey.NUMPAD6 },
        { KeyCode.NumpadPlus, EVirtualKey.ADD },
        { KeyCode.Numpad1, EVirtualKey.NUMPAD1 },
        { KeyCode.Numpad2, EVirtualKey.NUMPAD2 },
        { KeyCode.Numpad3, EVirtualKey.NUMPAD3 },
        { KeyCode.Numpad0, EVirtualKey.NUMPAD0 },
        { KeyCode.NumpadPeriod, EVirtualKey.DECIMAL },
        { KeyCode.F11, EVirtualKey.F11 },
        { KeyCode.F12, EVirtualKey.F12 },
        { KeyCode.RightCtrl, EVirtualKey.RCONTROL },
        { KeyCode.RightAlt, EVirtualKey.RMENU },
        { KeyCode.Home, EVirtualKey.HOME },
        { KeyCode.Up, EVirtualKey.UP },
        { KeyCode.PageUp, EVirtualKey.PRIOR },
        { KeyCode.Left, EVirtualKey.LEFT },
        { KeyCode.Right, EVirtualKey.RIGHT },
        { KeyCode.End, EVirtualKey.END },
        { KeyCode.Down, EVirtualKey.DOWN },
        { KeyCode.PageDown, EVirtualKey.NEXT },
        { KeyCode.Insert, EVirtualKey.INSERT },
        { KeyCode.Delete, EVirtualKey.DELETE }
      };

      public static ushort ToNativeKeyCode(KeyCode key, EKeyCodeMode mode)
      {
        switch (mode)
        {
          case EKeyCodeMode.ScanCode1:
            if (ScanCode1Map.TryGetValue(key, out var sc1))
              return (ushort)sc1;
            break;
          case EKeyCodeMode.HID:
            if (HidKeyMap.TryGetValue(key, out var hid))
              return (ushort)hid;
            break;
          case EKeyCodeMode.VirtualKey:
            if (VirtualKeyMap.TryGetValue(key, out var vk))
              return (ushort)vk;
            break;
        }
        throw new ArgumentException($"Key {key} is not mapped for mode {mode}");
      }
    }
  }

  #endregion

  #region Enums

  /// <summary>
  /// Generalized key code enum for use with all keycode modes.
  /// </summary>
  public enum KeyCode : ushort
  {
    Esc,
    D1,
    D2,
    D3,
    D4,
    D5,
    D6,
    D7,
    D8,
    D9,
    D0,
    Minus,
    Equals,
    Backspace,
    Tab,
    Q,
    W,
    E,
    R,
    T,
    Y,
    U,
    I,
    O,
    P,
    LeftBracket,
    RightBracket,
    Enter,
    LeftCtrl,
    A,
    S,
    D,
    F,
    G,
    H,
    J,
    K,
    L,
    Semicolon,
    Apostrophe,
    Grave,
    LeftShift,
    Backslash,
    Z,
    X,
    C,
    V,
    B,
    N,
    M,
    Comma,
    Period,
    Slash,
    RightShift,
    NumpadAsterisk,
    LeftAlt,
    Space,
    CapsLock,
    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,
    F8,
    F9,
    F10,
    NumLock,
    ScrollLock,
    Numpad7,
    Numpad8,
    Numpad9,
    NumpadMinus,
    Numpad4,
    Numpad5,
    Numpad6,
    NumpadPlus,
    Numpad1,
    Numpad2,
    Numpad3,
    Numpad0,
    NumpadPeriod,
    F11,
    F12,
    RightCtrl,
    RightAlt,
    Home,
    Up,
    PageUp,
    Left,
    Right,
    End,
    Down,
    PageDown,
    Insert,
    Delete
  }

  enum EScanCode1 : ushort
  {
    Esc = 0x01,
    D1 = 0x02,
    D2 = 0x03,
    D3 = 0x04,
    D4 = 0x05,
    D5 = 0x06,
    D6 = 0x07,
    D7 = 0x08,
    D8 = 0x09,
    D9 = 0x0A,
    D0 = 0x0B,
    Minus = 0x0C,
    Equals = 0x0D,
    Backspace = 0x0E,
    Tab = 0x0F,
    Q = 0x10,
    W = 0x11,
    E = 0x12,
    R = 0x13,
    T = 0x14,
    Y = 0x15,
    U = 0x16,
    I = 0x17,
    O = 0x18,
    P = 0x19,
    LeftBracket = 0x1A,
    RightBracket = 0x1B,
    Enter = 0x1C,
    LeftCtrl = 0x1D,
    A = 0x1E,
    S = 0x1F,
    D = 0x20,
    F = 0x21,
    G = 0x22,
    H = 0x23,
    J = 0x24,
    K = 0x25,
    L = 0x26,
    Semicolon = 0x27,
    Apostrophe = 0x28,
    Grave = 0x29,
    LeftShift = 0x2A,
    Backslash = 0x2B,
    Z = 0x2C,
    X = 0x2D,
    C = 0x2E,
    V = 0x2F,
    B = 0x30,
    N = 0x31,
    M = 0x32,
    Comma = 0x33,
    Period = 0x34,
    Slash = 0x35,
    RightShift = 0x36,
    NumpadAsterisk = 0x37,
    LeftAlt = 0x38,
    Space = 0x39,
    CapsLock = 0x3A,
    F1 = 0x3B,
    F2 = 0x3C,
    F3 = 0x3D,
    F4 = 0x3E,
    F5 = 0x3F,
    F6 = 0x40,
    F7 = 0x41,
    F8 = 0x42,
    F9 = 0x43,
    F10 = 0x44,
    NumLock = 0x45,
    ScrollLock = 0x46,
    Numpad7 = 0x47,
    Numpad8 = 0x48,
    Numpad9 = 0x49,
    NumpadMinus = 0x4A,
    Numpad4 = 0x4B,
    Numpad5 = 0x4C,
    Numpad6 = 0x4D,
    NumpadPlus = 0x4E,
    Numpad1 = 0x4F,
    Numpad2 = 0x50,
    Numpad3 = 0x51,
    Numpad0 = 0x52,
    NumpadPeriod = 0x53,
    F11 = 0x57,
    F12 = 0x58,
    RightCtrl = 0x1D | 0xE000,
    RightAlt = 0x38 | 0xE000,
    Home = 0x47 | 0xE000,
    Up = 0x48 | 0xE000,
    PageUp = 0x49 | 0xE000,
    Left = 0x4B | 0xE000,
    Right = 0x4D | 0xE000,
    End = 0x4F | 0xE000,
    Down = 0x50 | 0xE000,
    PageDown = 0x51 | 0xE000,
    Insert = 0x52 | 0xE000,
    Delete = 0x53 | 0xE000,
  }

  enum EHIDKey : ushort
  {
    None = 0x00,
    ErrorRollOver = 0x01,
    POSTFail = 0x02,
    ErrorUndefined = 0x03,
    A = 0x04,
    B = 0x05,
    C = 0x06,
    D = 0x07,
    E = 0x08,
    F = 0x09,
    G = 0x0A,
    H = 0x0B,
    I = 0x0C,
    J = 0x0D,
    K = 0x0E,
    L = 0x0F,
    M = 0x10,
    N = 0x11,
    O = 0x12,
    P = 0x13,
    Q = 0x14,
    R = 0x15,
    S = 0x16,
    T = 0x17,
    U = 0x18,
    V = 0x19,
    W = 0x1A,
    X = 0x1B,
    Y = 0x1C,
    Z = 0x1D,
    D1 = 0x1E,
    D2 = 0x1F,
    D3 = 0x20,
    D4 = 0x21,
    D5 = 0x22,
    D6 = 0x23,
    D7 = 0x24,
    D8 = 0x25,
    D9 = 0x26,
    D0 = 0x27,
    Enter = 0x28,
    Escape = 0x29,
    Backspace = 0x2A,
    Tab = 0x2B,
    Space = 0x2C,
    Minus = 0x2D,
    Equals = 0x2E,
    LeftBracket = 0x2F,
    RightBracket = 0x30,
    Backslash = 0x31,
    NonUSHash = 0x32,
    Semicolon = 0x33,
    Apostrophe = 0x34,
    Grave = 0x35,
    Comma = 0x36,
    Period = 0x37,
    Slash = 0x38,
    CapsLock = 0x39,
    F1 = 0x3A,
    F2 = 0x3B,
    F3 = 0x3C,
    F4 = 0x3D,
    F5 = 0x3E,
    F6 = 0x3F,
    F7 = 0x40,
    F8 = 0x41,
    F9 = 0x42,
    F10 = 0x43,
    F11 = 0x44,
    F12 = 0x45,
    PrintScreen = 0x46,
    ScrollLock = 0x47,
    Pause = 0x48,
    Insert = 0x49,
    Home = 0x4A,
    PageUp = 0x4B,
    Delete = 0x4C,
    End = 0x4D,
    PageDown = 0x4E,
    RightArrow = 0x4F,
    LeftArrow = 0x50,
    DownArrow = 0x51,
    UpArrow = 0x52,
    NumLock = 0x53,
    KeypadSlash = 0x54,
    KeypadAsterisk = 0x55,
    KeypadMinus = 0x56,
    KeypadPlus = 0x57,
    KeypadEnter = 0x58,
    Keypad1 = 0x59,
    Keypad2 = 0x5A,
    Keypad3 = 0x5B,
    Keypad4 = 0x5C,
    Keypad5 = 0x5D,
    Keypad6 = 0x5E,
    Keypad7 = 0x5F,
    Keypad8 = 0x60,
    Keypad9 = 0x61,
    Keypad0 = 0x62,
    KeypadPeriod = 0x63,
    NonUSBackslash = 0x64,
    Application = 0x65,
    Power = 0x66,
    KeypadEquals = 0x67,
    F13 = 0x68,
    F14 = 0x69,
    F15 = 0x6A,
    F16 = 0x6B,
    F17 = 0x6C,
    F18 = 0x6D,
    F19 = 0x6E,
    F20 = 0x6F,
    F21 = 0x70,
    F22 = 0x71,
    F23 = 0x72,
    F24 = 0x73,
    Execute = 0x74,
    Help = 0x75,
    Menu = 0x76,
    Select = 0x77,
    Stop = 0x78,
    Again = 0x79,
    Undo = 0x7A,
    Cut = 0x7B,
    Copy = 0x7C,
    Paste = 0x7D,
    Find = 0x7E,
    Mute = 0x7F,
    VolumeUp = 0x80,
    VolumeDown = 0x81,
    LockingCapsLock = 0x82,
    LockingNumLock = 0x83,
    LockingScrollLock = 0x84,
    KeypadComma = 0x85,
    KeypadEqualSign = 0x86,
    International1 = 0x87,
    International2 = 0x88,
    International3 = 0x89,
    International4 = 0x8A,
    International5 = 0x8B,
    International6 = 0x8C,
    International7 = 0x8D,
    International8 = 0x8E,
    International9 = 0x8F,
    LANG1 = 0x90,
    LANG2 = 0x91,
    LANG3 = 0x92,
    LANG4 = 0x93,
    LANG5 = 0x94,
    LANG6 = 0x95,
    LANG7 = 0x96,
    LANG8 = 0x97,
    LANG9 = 0x98,
    AltErase = 0x99,
    SysReq = 0x9A,
    Cancel = 0x9B,
    Clear = 0x9C,
    Prior = 0x9D,
    Return = 0x9E,
    Separator = 0x9F,
    Out = 0xA0,
    Oper = 0xA1,
    ClearAgain = 0xA2,
    CrSel = 0xA3,
    ExSel = 0xA4
  }

  enum EVirtualKey : ushort
  {
    None = 0x00,
    LBUTTON = 0x01,        // Left mouse button
    RBUTTON = 0x02,        // Right mouse button
    CANCEL = 0x03,
    MBUTTON = 0x04,        // Middle mouse button
    XBUTTON1 = 0x05,
    XBUTTON2 = 0x06,
    BACK = 0x08,
    TAB = 0x09,
    CLEAR = 0x0C,
    RETURN = 0x0D,         // Enter
    SHIFT = 0x10,
    CONTROL = 0x11,
    MENU = 0x12,           // Alt
    PAUSE = 0x13,
    CAPITAL = 0x14,        // Caps Lock
    KANA = 0x15,
    HANGUL = 0x15,
    JUNJA = 0x17,
    FINAL = 0x18,
    HANJA = 0x19,
    KANJI = 0x19,
    ESCAPE = 0x1B,
    CONVERT = 0x1C,
    NONCONVERT = 0x1D,
    ACCEPT = 0x1E,
    MODECHANGE = 0x1F,
    SPACE = 0x20,
    PRIOR = 0x21,          // Page Up
    NEXT = 0x22,           // Page Down
    END = 0x23,
    HOME = 0x24,
    LEFT = 0x25,
    UP = 0x26,
    RIGHT = 0x27,
    DOWN = 0x28,
    SELECT = 0x29,
    PRINT = 0x2A,
    EXECUTE = 0x2B,
    SNAPSHOT = 0x2C,       // Print Screen
    INSERT = 0x2D,
    DELETE = 0x2E,
    HELP = 0x2F,
    D0 = 0x30,
    D1 = 0x31,
    D2 = 0x32,
    D3 = 0x33,
    D4 = 0x34,
    D5 = 0x35,
    D6 = 0x36,
    D7 = 0x37,
    D8 = 0x38,
    D9 = 0x39,
    A = 0x41,
    B = 0x42,
    C = 0x43,
    D = 0x44,
    E = 0x45,
    F = 0x46,
    G = 0x47,
    H = 0x48,
    I = 0x49,
    J = 0x4A,
    K = 0x4B,
    L = 0x4C,
    M = 0x4D,
    N = 0x4E,
    O = 0x4F,
    P = 0x50,
    Q = 0x51,
    R = 0x52,
    S = 0x53,
    T = 0x54,
    U = 0x55,
    V = 0x56,
    W = 0x57,
    X = 0x58,
    Y = 0x59,
    Z = 0x5A,
    LWIN = 0x5B,
    RWIN = 0x5C,
    APPS = 0x5D,
    SLEEP = 0x5F,
    NUMPAD0 = 0x60,
    NUMPAD1 = 0x61,
    NUMPAD2 = 0x62,
    NUMPAD3 = 0x63,
    NUMPAD4 = 0x64,
    NUMPAD5 = 0x65,
    NUMPAD6 = 0x66,
    NUMPAD7 = 0x67,
    NUMPAD8 = 0x68,
    NUMPAD9 = 0x69,
    MULTIPLY = 0x6A,
    ADD = 0x6B,
    SEPARATOR = 0x6C,
    SUBTRACT = 0x6D,
    DECIMAL = 0x6E,
    DIVIDE = 0x6F,
    F1 = 0x70,
    F2 = 0x71,
    F3 = 0x72,
    F4 = 0x73,
    F5 = 0x74,
    F6 = 0x75,
    F7 = 0x76,
    F8 = 0x77,
    F9 = 0x78,
    F10 = 0x79,
    F11 = 0x7A,
    F12 = 0x7B,
    F13 = 0x7C,
    F14 = 0x7D,
    F15 = 0x7E,
    F16 = 0x7F,
    F17 = 0x80,
    F18 = 0x81,
    F19 = 0x82,
    F20 = 0x83,
    F21 = 0x84,
    F22 = 0x85,
    F23 = 0x86,
    F24 = 0x87,
    NUMLOCK = 0x90,
    SCROLL = 0x91,
    LSHIFT = 0xA0,
    RSHIFT = 0xA1,
    LCONTROL = 0xA2,
    RCONTROL = 0xA3,
    LMENU = 0xA4,          // Left Alt
    RMENU = 0xA5,          // Right Alt
    BROWSER_BACK = 0xA6,
    BROWSER_FORWARD = 0xA7,
    BROWSER_REFRESH = 0xA8,
    BROWSER_STOP = 0xA9,
    BROWSER_SEARCH = 0xAA,
    BROWSER_FAVORITES = 0xAB,
    BROWSER_HOME = 0xAC,
    VOLUME_MUTE = 0xAD,
    VOLUME_DOWN = 0xAE,
    VOLUME_UP = 0xAF,
    MEDIA_NEXT_TRACK = 0xB0,
    MEDIA_PREV_TRACK = 0xB1,
    MEDIA_STOP = 0xB2,
    MEDIA_PLAY_PAUSE = 0xB3,
    LAUNCH_MAIL = 0xB4,
    LAUNCH_MEDIA_SELECT = 0xB5,
    LAUNCH_APP1 = 0xB6,
    LAUNCH_APP2 = 0xB7,
    OEM_1 = 0xBA,          // ;:
    OEM_PLUS = 0xBB,       // +
    OEM_COMMA = 0xBC,      // ,
    OEM_MINUS = 0xBD,      // -
    OEM_PERIOD = 0xBE,     // .
    OEM_2 = 0xBF,          // /?
    OEM_3 = 0xC0,          // `~
    OEM_4 = 0xDB,          // [{
    OEM_5 = 0xDC,          // \|
    OEM_6 = 0xDD,          // ]}
    OEM_7 = 0xDE,          // '"
    OEM_8 = 0xDF,
    OEM_102 = 0xE2,        // <> or \|
    PROCESSKEY = 0xE5,
    PACKET = 0xE7,
    ATTN = 0xF6,
    CRSEL = 0xF7,
    EXSEL = 0xF8,
    EREOF = 0xF9,
    PLAY = 0xFA,
    ZOOM = 0xFB,
    NONAME = 0xFC,
    PA1 = 0xFD,
    OEM_CLEAR = 0xFE
  }

  #endregion
}
