using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

public class KleiAccount : ThreadedHttps<KleiAccount>
{
	private struct AccountReply
	{
		public string UserID;

		public string Token;

		public bool Error;

		public string SupplementaryData;
	}

	public delegate void GetUserIDdelegate();

	public const string KleiAccountKey = "KleiAccount";

	private const string GameIDFieldName = "Game";

	private const string EmailFieldName = "NoEmail";

	private const string ErrorFieldName = "Error";

	private const string UserIDFieldName = "UserID";

	public static string KleiUserID;

	private GetUserIDdelegate gotUserID;

	private const string AuthTicketKey = "AUTH_TICKET";

	private byte[] authTicket;

	private const string TicketFieldName = "SteamTicket";

	public KleiAccount()
	{
		CLIENT_KEY = "ONI";
		LIVE_ENDPOINT = "login.kleientertainment.com" + DistributionPlatform.Inst.AccountLoginEndpoint;
		serviceName = "KleiAccount";
		ClearAuthTicket();
	}

	protected override void OnReplyRecieved(WebResponse response)
	{
		if (response == null)
		{
			KleiUserID = null;
			gotUserID();
			return;
		}
		Stream responseStream = response.GetResponseStream();
		StreamReader streamReader = new StreamReader(responseStream);
		string text = streamReader.ReadToEnd();
		streamReader.Close();
		responseStream.Close();
		AccountReply accountReply = JsonConvert.DeserializeObject<AccountReply>(text);
		if (!accountReply.Error)
		{
			Debug.Log("[Account] Got login for user " + accountReply.UserID);
			KleiUserID = ((accountReply.UserID == "") ? null : accountReply.UserID);
			gotUserID();
		}
		else
		{
			Debug.Log("[Account] Error logging in: " + text);
			gotUserID();
		}
		End();
	}

	private string EncodeToAsciiHEX(byte[] data)
	{
		string text = "";
		for (int i = 0; i < data.Length; i++)
		{
			text += data[i].ToString("X2");
		}
		return text;
	}

	public string PostRawData(Dictionary<string, object> data)
	{
		string s = JsonConvert.SerializeObject(data);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		PutPacket(bytes);
		return "OK";
	}

	public void AuthenticateUser(GetUserIDdelegate cb)
	{
		if (KleiUserID == null)
		{
			Debug.Log("[Account] Requesting auth ticket from " + DistributionPlatform.Inst.Name);
			gotUserID = cb;
			byte[] array = AuthTicket();
			if (array == null || array.Length == 0)
			{
				if (DistributionPlatform.Initialized)
				{
					DistributionPlatform.Inst.GetAuthTicket(OnAuthTicketObtained);
				}
			}
			else
			{
				OnAuthTicketObtained(array);
			}
		}
		else
		{
			cb();
		}
	}

	public void OnAuthTicketObtained(byte[] ticket)
	{
		if (ticket.Length != 0)
		{
			byte[] array = new byte[ticket.Length];
			Array.Copy(ticket, array, ticket.Length);
			SetAuthTicket(array);
			Start();
			Dictionary<string, object> data = BuildLoginRequest(array);
			PostRawData(data);
		}
		else
		{
			gotUserID();
		}
	}

	public byte[] AuthTicket()
	{
		return authTicket;
	}

	public void SetAuthTicket(byte[] ticket)
	{
		authTicket = ticket;
	}

	public void ClearAuthTicket()
	{
		authTicket = null;
	}

	private Dictionary<string, object> BuildLoginRequest(byte[] ticket)
	{
		return new Dictionary<string, object>
		{
			{
				"SteamTicket",
				EncodeToAsciiHEX(ticket)
			},
			{ "Game", CLIENT_KEY },
			{ "NoEmail", true }
		};
	}
}
