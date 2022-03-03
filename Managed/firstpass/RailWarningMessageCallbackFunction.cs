using System.Runtime.InteropServices;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void RailWarningMessageCallbackFunction(int level, string msg);
