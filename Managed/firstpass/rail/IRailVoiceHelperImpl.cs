using System;

namespace rail
{
	public class IRailVoiceHelperImpl : RailObject, IRailVoiceHelper
	{
		internal IRailVoiceHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailVoiceHelperImpl()
		{
		}

		public virtual IRailVoiceChannel AsyncCreateVoiceChannel(CreateVoiceChannelOption options, string channel_name, string user_data, out RailResult result)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateVoiceChannelOption__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailVoiceHelper_AsyncCreateVoiceChannel(swigCPtr_, intPtr, channel_name, user_data, out result);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailVoiceChannelImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateVoiceChannelOption(intPtr);
			}
		}

		public virtual IRailVoiceChannel OpenVoiceChannel(RailVoiceChannelID voice_channel_id, string channel_name, out RailResult result)
		{
			IntPtr intPtr = ((voice_channel_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailVoiceChannelID__SWIG_0());
			if (voice_channel_id != null)
			{
				RailConverter.Csharp2Cpp(voice_channel_id, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailVoiceHelper_OpenVoiceChannel(swigCPtr_, intPtr, channel_name, out result);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailVoiceChannelImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailVoiceChannelID(intPtr);
			}
		}

		public virtual EnumRailVoiceChannelSpeakerState GetSpeakerState()
		{
			return (EnumRailVoiceChannelSpeakerState)RAIL_API_PINVOKE.IRailVoiceHelper_GetSpeakerState(swigCPtr_);
		}

		public virtual RailResult MuteSpeaker()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_MuteSpeaker(swigCPtr_);
		}

		public virtual RailResult ResumeSpeaker()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_ResumeSpeaker(swigCPtr_);
		}

		public virtual RailResult SetupVoiceCapture(RailVoiceCaptureOption options, RailCaptureVoiceCallback callback)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailVoiceCaptureOption__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetupVoiceCapture__SWIG_0(swigCPtr_, intPtr, callback);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailVoiceCaptureOption(intPtr);
			}
		}

		public virtual RailResult SetupVoiceCapture(RailVoiceCaptureOption options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailVoiceCaptureOption__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetupVoiceCapture__SWIG_1(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailVoiceCaptureOption(intPtr);
			}
		}

		public virtual RailResult StartVoiceCapturing(uint duration_milliseconds, bool repeat)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_StartVoiceCapturing__SWIG_0(swigCPtr_, duration_milliseconds, repeat);
		}

		public virtual RailResult StartVoiceCapturing(uint duration_milliseconds)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_StartVoiceCapturing__SWIG_1(swigCPtr_, duration_milliseconds);
		}

		public virtual RailResult StartVoiceCapturing()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_StartVoiceCapturing__SWIG_2(swigCPtr_);
		}

		public virtual RailResult StopVoiceCapturing()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_StopVoiceCapturing(swigCPtr_);
		}

		public virtual RailResult GetCapturedVoiceData(byte[] buffer, uint buffer_length, out uint encoded_bytes_written)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_GetCapturedVoiceData(swigCPtr_, buffer, buffer_length, out encoded_bytes_written);
		}

		public virtual RailResult DecodeVoice(byte[] encoded_buffer, uint encoded_length, byte[] pcm_buffer, uint pcm_buffer_length, out uint pcm_buffer_written)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_DecodeVoice(swigCPtr_, encoded_buffer, encoded_length, pcm_buffer, pcm_buffer_length, out pcm_buffer_written);
		}

		public virtual RailResult GetVoiceCaptureSpecification(RailVoiceCaptureSpecification spec)
		{
			IntPtr intPtr = ((spec == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailVoiceCaptureSpecification__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_GetVoiceCaptureSpecification(swigCPtr_, intPtr);
			}
			finally
			{
				if (spec != null)
				{
					RailConverter.Cpp2Csharp(intPtr, spec);
				}
				RAIL_API_PINVOKE.delete_RailVoiceCaptureSpecification(intPtr);
			}
		}

		public virtual RailResult EnableInGameVoiceSpeaking(bool can_speaking)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_EnableInGameVoiceSpeaking(swigCPtr_, can_speaking);
		}

		public virtual RailResult SetPlayerNicknameInVoiceChannel(string nickname)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetPlayerNicknameInVoiceChannel(swigCPtr_, nickname);
		}

		public virtual RailResult SetPushToTalkKeyInVoiceChannel(uint push_to_talk_hot_key)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetPushToTalkKeyInVoiceChannel(swigCPtr_, push_to_talk_hot_key);
		}

		public virtual uint GetPushToTalkKeyInVoiceChannel()
		{
			return RAIL_API_PINVOKE.IRailVoiceHelper_GetPushToTalkKeyInVoiceChannel(swigCPtr_);
		}

		public virtual RailResult ShowOverlayUI(bool show)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_ShowOverlayUI(swigCPtr_, show);
		}

		public virtual RailResult SetMicroVolume(uint volume)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetMicroVolume(swigCPtr_, volume);
		}

		public virtual RailResult SetSpeakerVolume(uint volume)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetSpeakerVolume(swigCPtr_, volume);
		}
	}
}
