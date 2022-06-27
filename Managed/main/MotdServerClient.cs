using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using STRINGS;
using UnityEngine;
using UnityEngine.Networking;

public class MotdServerClient
{
	public class MotdUpdateData
	{
		public string last_update_time { get; set; }

		public string next_update_time { get; set; }

		public string update_text_override { get; set; }
	}

	public class MotdResponse
	{
		public int version { get; set; }

		public string image_header_text { get; set; }

		public int image_version { get; set; }

		public string image_url { get; set; }

		public string image_link_url { get; set; }

		public string image_rail_link_url { get; set; }

		public string news_header_text { get; set; }

		public string news_body_text { get; set; }

		public string patch_notes_summary { get; set; }

		public string patch_notes_link_url { get; set; }

		public string patch_notes_rail_link_url { get; set; }

		public MotdUpdateData vanilla_update_data { get; set; }

		public MotdUpdateData expansion1_update_data { get; set; }

		public string latest_update_build { get; set; }

		[JsonIgnore]
		public Texture2D image_texture { get; set; }
	}

	private Action<MotdResponse, string> m_callback;

	private MotdResponse m_localMotd;

	private static string MotdServerUrl => "https://klei-motd.s3.amazonaws.com/oni/" + GetLocalePathSuffix();

	private static string MotdLocalPath => "motd_local/" + GetLocalePathSuffix();

	private static string MotdLocalImagePath(int imageVersion)
	{
		return MotdLocalImagePath(imageVersion, Localization.GetLocale());
	}

	private static string FallbackMotdLocalImagePath(int imageVersion)
	{
		return MotdLocalImagePath(imageVersion, null);
	}

	private static string MotdLocalImagePath(int imageVersion, Localization.Locale locale)
	{
		return "motd_local/" + GetLocalePathModifier(locale) + "image_" + imageVersion;
	}

	private static string GetLocalePathModifier()
	{
		return GetLocalePathModifier(Localization.GetLocale());
	}

	private static string GetLocalePathModifier(Localization.Locale locale)
	{
		string result = "";
		if (locale != null)
		{
			Localization.Language lang = locale.Lang;
			if (lang == Localization.Language.Chinese || (uint)(lang - 2) <= 1u)
			{
				result = locale.Code + "/";
			}
		}
		return result;
	}

	private static string GetLocalePathSuffix()
	{
		return GetLocalePathModifier() + "motd.json";
	}

	public void GetMotd(Action<MotdResponse, string> cb)
	{
		m_callback = cb;
		MotdResponse localResponse = GetLocalMotd(MotdLocalPath);
		GetWebMotd(MotdServerUrl, localResponse, delegate(MotdResponse response, string err)
		{
			MotdResponse motdResponse;
			if (err == null)
			{
				Debug.Assert(response.image_texture != null, "Attempting to return response with no image texture");
				motdResponse = response;
			}
			else
			{
				Debug.LogWarning("Could not retrieve web motd from " + MotdServerUrl + ", falling back to local - err: " + err);
				motdResponse = localResponse;
			}
			if (Localization.GetSelectedLanguageType() == Localization.SelectedLanguageType.UGC)
			{
				Debug.Log("Language Mod detected, MOTD strings falling back to local file");
				motdResponse.image_header_text = UI.FRONTEND.MOTD.IMAGE_HEADER;
				motdResponse.news_header_text = UI.FRONTEND.MOTD.NEWS_HEADER;
				motdResponse.news_body_text = UI.FRONTEND.MOTD.NEWS_BODY;
				motdResponse.patch_notes_summary = UI.FRONTEND.MOTD.PATCH_NOTES_SUMMARY;
				motdResponse.vanilla_update_data.update_text_override = UI.FRONTEND.MOTD.UPDATE_TEXT;
				motdResponse.expansion1_update_data.update_text_override = UI.FRONTEND.MOTD.UPDATE_TEXT_EXPANSION1;
			}
			doCallback(motdResponse, null);
		});
	}

	private MotdResponse GetLocalMotd(string filePath)
	{
		TextAsset textAsset = Resources.Load<TextAsset>(filePath.Replace(".json", ""));
		m_localMotd = JsonConvert.DeserializeObject<MotdResponse>(textAsset.ToString());
		string text = MotdLocalImagePath(m_localMotd.image_version);
		m_localMotd.image_texture = Resources.Load<Texture2D>(text);
		if (m_localMotd.image_texture == null)
		{
			string text2 = FallbackMotdLocalImagePath(m_localMotd.image_version);
			if (text2 != text)
			{
				Debug.Log("Could not load " + text + ", falling back to " + text2);
				text = text2;
				m_localMotd.image_texture = Resources.Load<Texture2D>(text);
			}
		}
		Debug.Assert(m_localMotd.image_texture != null, "Failed to load " + text);
		return m_localMotd;
	}

	private void GetWebMotd(string url, MotdResponse localMotd, Action<MotdResponse, string> cb)
	{
		Action<string, string> cb2 = delegate(string response, string err)
		{
			DebugUtil.DevAssert(localMotd.image_texture != null, "Local MOTD image_texture is no longer loaded");
			if (localMotd.image_texture == null)
			{
				cb(null, "Local image_texture has been unloaded since we requested the MOTD");
			}
			else if (err != null)
			{
				cb(null, err);
			}
			else
			{
				MotdResponse responseStruct = JsonConvert.DeserializeObject<MotdResponse>(response, new JsonSerializerSettings
				{
					Error = delegate(object sender, ErrorEventArgs args)
					{
						args.ErrorContext.Handled = true;
					}
				});
				if (responseStruct == null)
				{
					cb(null, "Invalid json from server:" + response);
				}
				else if (responseStruct.version <= localMotd.version)
				{
					Debug.Log("Using local MOTD at version: " + localMotd.version + ", web version at " + responseStruct.version);
					cb(localMotd, null);
				}
				else
				{
					UnityWebRequest data_wr = new UnityWebRequest
					{
						downloadHandler = new DownloadHandlerTexture()
					};
					SimpleNetworkCache.LoadFromCacheOrDownload("motd_image", responseStruct.image_url, responseStruct.image_version, data_wr, delegate(UnityWebRequest wr)
					{
						string arg = null;
						if (string.IsNullOrEmpty(wr.error))
						{
							Debug.Log("Using web MOTD at version: " + responseStruct.version + ", local version at " + localMotd.version);
							responseStruct.image_texture = DownloadHandlerTexture.GetContent(wr);
						}
						else
						{
							arg = "SimpleNetworkCache - " + wr.error;
						}
						cb(responseStruct, arg);
						wr.Dispose();
					});
				}
			}
		};
		getAsyncRequest(url, cb2);
	}

	private void getAsyncRequest(string url, Action<string, string> cb)
	{
		UnityWebRequest motdRequest = UnityWebRequest.Get(url);
		motdRequest.SetRequestHeader("Content-Type", "application/json");
		motdRequest.SendWebRequest().completed += delegate
		{
			cb(motdRequest.downloadHandler.text, motdRequest.error);
			motdRequest.Dispose();
		};
	}

	public void UnregisterCallback()
	{
		m_callback = null;
	}

	private void doCallback(MotdResponse response, string error)
	{
		if (m_callback != null)
		{
			m_callback(response, error);
		}
		else
		{
			Debug.Log("Motd Response receieved, but callback was unregistered");
		}
	}
}
