using System;
using System.Runtime.InteropServices;
using rail;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void RailCaptureVoiceCallback(EnumRailVoiceCaptureFormat fmt, bool is_last_package, IntPtr encoded_buffer, uint encoded_length);
