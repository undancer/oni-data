using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Klei;
using Newtonsoft.Json;
using STRINGS;
using UnityEngine;

namespace KMod
{
	public class Manager
	{
		public delegate void OnUpdate(object change_source);

		private class PersistentData
		{
			public int version;

			public List<Mod> mods;

			public PersistentData()
			{
			}

			public PersistentData(int version, List<Mod> mods)
			{
				this.version = version;
				this.mods = mods;
			}
		}

		public const Content all_content = Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation;

		public const Content boot_content = Content.Strings | Content.DLL | Content.Translation | Content.Animation;

		public const Content install_content = Content.DLL;

		public const Content on_demand_content = Content.LayerableFiles;

		public List<IDistributionPlatform> distribution_platforms = new List<IDistributionPlatform>();

		public List<Mod> mods = new List<Mod>();

		public List<Event> events = new List<Event>();

		private bool dirty = true;

		public OnUpdate on_update;

		private const int IO_OP_RETRY_COUNT = 5;

		private bool load_user_mod_loader_dll = true;

		private const int MAX_DIALOG_ENTRIES = 30;

		private int current_version = 1;

		public static string GetDirectory()
		{
			return Path.Combine(Util.RootFolder(), "mods/");
		}

		public void LoadModDBAndInitialize()
		{
			string filename = GetFilename();
			try
			{
				if (FileUtil.FileExists(filename))
				{
					PersistentData persistentData = JsonConvert.DeserializeObject<PersistentData>(File.ReadAllText(filename));
					mods = persistentData.mods;
				}
			}
			catch (Exception)
			{
				Debug.LogWarningFormat(UI.FRONTEND.MODS.DB_CORRUPT, filename);
				mods = new List<Mod>();
			}
			foreach (Mod mod in mods)
			{
				if (mod.enabledForDlc == null)
				{
					mod.SetEnabledForDlc("", mod.enabled);
				}
			}
			List<Mod> list = new List<Mod>();
			bool flag = false;
			foreach (Mod mod2 in mods)
			{
				switch (mod2.status)
				{
				case Mod.Status.UninstallPending:
					Debug.LogFormat("Latent uninstall of mod {0} from {1}", mod2.title, mod2.label.install_path);
					if (mod2.Uninstall())
					{
						list.Add(mod2);
					}
					else
					{
						DebugUtil.Assert(mod2.status == Mod.Status.UninstallPending);
						Debug.LogFormat("\t...failed to uninstall mod {0}", mod2.title);
					}
					if (mod2.status != Mod.Status.UninstallPending)
					{
						flag = true;
					}
					break;
				case Mod.Status.ReinstallPending:
					Debug.LogFormat("Latent reinstall of mod {0}", mod2.title);
					if (!string.IsNullOrEmpty(mod2.reinstall_path) && File.Exists(mod2.reinstall_path))
					{
						mod2.IsEnabledForActiveDlc();
						mod2.file_source = new ZipFile(mod2.reinstall_path);
						mod2.SetEnabledForActiveDlc(enabled: false);
						if (mod2.Uninstall())
						{
							mod2.Install();
							if (mod2.status == Mod.Status.Installed)
							{
								mod2.SetEnabledForActiveDlc(enabled: true);
							}
						}
						flag = true;
					}
					else if (mod2.IsEnabledForActiveDlc())
					{
						mod2.SetEnabledForActiveDlc(enabled: false);
						flag = true;
					}
					break;
				}
				if (!string.IsNullOrEmpty(mod2.reinstall_path))
				{
					mod2.reinstall_path = null;
					flag = true;
				}
			}
			foreach (Mod item in list)
			{
				mods.Remove(item);
			}
			foreach (Mod mod3 in mods)
			{
				mod3.ScanContent();
			}
			if (flag)
			{
				Save();
			}
		}

		public void Shutdown()
		{
			foreach (Mod mod in mods)
			{
				mod.Unload(Content.LayerableFiles);
			}
		}

		public void Sanitize(GameObject parent)
		{
			ListPool<Label, Manager>.PooledList pooledList = ListPool<Label, Manager>.Allocate();
			foreach (Mod mod in mods)
			{
				if (!mod.is_subscribed)
				{
					pooledList.Add(mod.label);
				}
			}
			foreach (Label item in pooledList)
			{
				Unsubscribe(item, this);
			}
			pooledList.Recycle();
			Report(parent);
		}

		public bool HaveMods()
		{
			foreach (Mod mod in mods)
			{
				if (mod.status == Mod.Status.Installed && mod.HasContent())
				{
					return true;
				}
			}
			return false;
		}

		public List<Mod> GetAllCrashableMods()
		{
			List<Mod> list = new List<Mod>();
			foreach (Mod mod in mods)
			{
				if (mod.DevModCrashTriggered || (mod.status != 0 && mod.IsActive() && !mod.HasOnlyTranslationContent()))
				{
					list.Add(mod);
				}
			}
			return list;
		}

		public bool HasCrashableMods()
		{
			return GetAllCrashableMods().Count > 0;
		}

		private void Install(Mod mod)
		{
			if (mod.status == Mod.Status.NotInstalled)
			{
				Debug.LogFormat("\tInstalling mod: {0}", mod.title);
				mod.Install();
				Event item;
				if (mod.status == Mod.Status.Installed)
				{
					Debug.Log("\tSuccessfully installed.");
					List<Event> list = events;
					item = new Event
					{
						event_type = EventType.Installed,
						mod = mod.label
					};
					list.Add(item);
				}
				else
				{
					Debug.Log("\tFailed install. Will install on restart.");
					List<Event> list2 = events;
					item = new Event
					{
						event_type = EventType.InstallFailed,
						mod = mod.label
					};
					list2.Add(item);
					List<Event> list3 = events;
					item = new Event
					{
						event_type = EventType.RestartRequested,
						mod = mod.label
					};
					list3.Add(item);
				}
			}
		}

		private void Uninstall(Mod mod)
		{
			if (mod.status != 0)
			{
				Debug.LogFormat("\tUninstalling mod {0}", mod.title);
				mod.Uninstall();
				if (mod.status == Mod.Status.UninstallPending)
				{
					Debug.Log("\tFailed. Will re-install on restart.");
					mod.status = Mod.Status.ReinstallPending;
					events.Add(new Event
					{
						event_type = EventType.RestartRequested,
						mod = mod.label
					});
				}
			}
		}

		public void Subscribe(Mod mod, object caller)
		{
			Debug.LogFormat("Subscribe to mod {0}", mod.title);
			Mod mod2 = mods.Find((Mod candidate) => mod.label.Match(candidate.label));
			mod.is_subscribed = true;
			if (mod2 == null)
			{
				mods.Add(mod);
				Install(mod);
			}
			else
			{
				Event item;
				if (mod2.status == Mod.Status.UninstallPending)
				{
					mod2.status = Mod.Status.Installed;
					List<Event> list = events;
					item = new Event
					{
						event_type = EventType.Installed,
						mod = mod2.label
					};
					list.Add(item);
				}
				bool num = mod2.label.version != mod.label.version;
				bool flag = mod2.available_content != mod.available_content;
				bool flag2 = num || flag || mod2.status == Mod.Status.ReinstallPending;
				if (num)
				{
					List<Event> list2 = events;
					item = new Event
					{
						event_type = EventType.VersionUpdate,
						mod = mod.label
					};
					list2.Add(item);
				}
				if (flag)
				{
					List<Event> list3 = events;
					item = new Event
					{
						event_type = EventType.AvailableContentChanged,
						mod = mod.label
					};
					list3.Add(item);
				}
				string root = mod.file_source.GetRoot();
				mod2.CopyPersistentDataTo(mod);
				int index = mods.IndexOf(mod2);
				mods.RemoveAt(index);
				mods.Insert(index, mod);
				if (flag2 || mod.status == Mod.Status.NotInstalled)
				{
					if (mod.IsEnabledForActiveDlc())
					{
						mod.reinstall_path = root;
						mod.status = Mod.Status.ReinstallPending;
						List<Event> list4 = events;
						item = new Event
						{
							event_type = EventType.RestartRequested,
							mod = mod.label
						};
						list4.Add(item);
					}
					else
					{
						if (flag2)
						{
							Uninstall(mod);
						}
						Install(mod);
					}
				}
				else
				{
					mod.file_source = mod2.file_source;
				}
			}
			dirty = true;
			Update(caller);
		}

		public void Update(Mod mod, object caller)
		{
			Debug.LogFormat("Update mod {0}", mod.title);
			Mod mod2 = mods.Find((Mod candidate) => mod.label.Match(candidate.label));
			DebugUtil.DevAssert(!string.IsNullOrEmpty(mod2.label.id), "Should be subscribed to a mod we are getting an Update notification for");
			if (mod2.status != Mod.Status.UninstallPending)
			{
				List<Event> list = events;
				Event item = new Event
				{
					event_type = EventType.VersionUpdate,
					mod = mod.label
				};
				list.Add(item);
				string root = mod.file_source.GetRoot();
				mod2.CopyPersistentDataTo(mod);
				mod.is_subscribed = mod2.is_subscribed;
				int index = mods.IndexOf(mod2);
				mods.RemoveAt(index);
				mods.Insert(index, mod);
				if (mod.IsEnabledForActiveDlc())
				{
					mod.reinstall_path = root;
					mod.status = Mod.Status.ReinstallPending;
					List<Event> list2 = events;
					item = new Event
					{
						event_type = EventType.RestartRequested,
						mod = mod.label
					};
					list2.Add(item);
				}
				else
				{
					Uninstall(mod);
					Install(mod);
				}
				dirty = true;
				Update(caller);
			}
		}

		public void Unsubscribe(Label label, object caller)
		{
			Debug.LogFormat("Unsubscribe from mod {0}", label.ToString());
			int num = 0;
			foreach (Mod mod2 in mods)
			{
				if (mod2.label.Match(label))
				{
					Debug.LogFormat("\t...found it: {0}", mod2.title);
					break;
				}
				num++;
			}
			if (num == mods.Count)
			{
				Debug.LogFormat("\t...not found");
				return;
			}
			Mod mod = mods[num];
			mod.SetEnabledForActiveDlc(enabled: false);
			mod.Unload(Content.LayerableFiles);
			List<Event> list = events;
			Event item = new Event
			{
				event_type = EventType.Uninstalled,
				mod = mod.label
			};
			list.Add(item);
			if (mod.IsActive())
			{
				Debug.LogFormat("\tCould not unload all content provided by mod {0} : {1}\nUninstall will likely fail", mod.title, mod.label.ToString());
				List<Event> list2 = events;
				item = new Event
				{
					event_type = EventType.RestartRequested,
					mod = mod.label
				};
				list2.Add(item);
			}
			if (mod.status == Mod.Status.Installed)
			{
				Debug.LogFormat("\tUninstall mod {0} : {1}", mod.title, mod.label.ToString());
				mod.Uninstall();
			}
			if (mod.status == Mod.Status.NotInstalled)
			{
				Debug.LogFormat("\t...success. Removing from management list {0} : {1}", mod.title, mod.label.ToString());
				mods.RemoveAt(num);
			}
			dirty = true;
			Update(caller);
		}

		public bool IsInDevMode()
		{
			return mods.Exists((Mod mod) => mod.IsEnabledForActiveDlc() && mod.label.distribution_platform == Label.DistributionPlatform.Dev);
		}

		public void Load(Content content)
		{
			if ((content & Content.DLL) != 0 && load_user_mod_loader_dll)
			{
				if (!DLLLoader.LoadUserModLoaderDLL())
				{
					Debug.Log("Using builtin mod system.");
				}
				else
				{
					Debug.LogWarning("Using ModLoader.DLL for custom mod loading! This is not the standard mod loading method.");
				}
				load_user_mod_loader_dll = false;
			}
			foreach (Mod mod in mods)
			{
				if (mod.IsEnabledForActiveDlc())
				{
					mod.Load(content);
				}
			}
			if ((content & Content.DLL) != 0)
			{
				IReadOnlyList<Mod> readOnlyList = mods.AsReadOnly();
				foreach (Mod mod2 in mods)
				{
					if (mod2.IsEnabledForActiveDlc())
					{
						mod2.PostLoad(readOnlyList);
					}
				}
			}
			bool flag = false;
			foreach (Mod mod3 in mods)
			{
				Content content2 = mod3.loaded_content & content;
				Content content3 = mod3.available_content & content;
				if (mod3.IsEnabledForActiveDlc() && content2 != content3)
				{
					mod3.SetCrashed();
					Event item;
					if (!mod3.IsEnabledForActiveDlc())
					{
						flag = true;
						List<Event> list = events;
						item = new Event
						{
							event_type = EventType.Deactivated,
							mod = mod3.label
						};
						list.Add(item);
					}
					Debug.LogFormat("Failed to load mod {0}...disabling", mod3.title);
					List<Event> list2 = events;
					item = new Event
					{
						event_type = EventType.LoadError,
						mod = mod3.label
					};
					list2.Add(item);
				}
			}
			if (flag)
			{
				Save();
			}
		}

		public void Unload(Content content)
		{
			foreach (Mod mod in mods)
			{
				mod.Unload(content);
			}
		}

		public void Update(object change_source)
		{
			if (dirty)
			{
				dirty = false;
				Save();
				if (on_update != null)
				{
					on_update(change_source);
				}
			}
		}

		public bool MatchFootprint(List<Label> footprint, Content relevant_content)
		{
			if (footprint == null)
			{
				return true;
			}
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			int num = -1;
			Func<Label, Mod, bool> is_match = (Label label, Mod mod) => mod.label.Match(label);
			Event item;
			foreach (Label label2 in footprint)
			{
				bool flag4 = false;
				for (int i = num + 1; i != mods.Count; i++)
				{
					Mod mod2 = mods[i];
					num = i;
					Content content = mod2.available_content & relevant_content;
					bool flag5 = content != (Content)0;
					if (!is_match(label2, mod2))
					{
						if (flag5 && mod2.IsEnabledForActiveDlc())
						{
							List<Event> list = events;
							item = new Event
							{
								event_type = EventType.ExpectedInactive,
								mod = mod2.label
							};
							list.Add(item);
							flag3 = true;
						}
						continue;
					}
					if (flag5)
					{
						if (!mod2.IsEnabledForActiveDlc())
						{
							List<Event> list2 = events;
							item = new Event
							{
								event_type = EventType.ExpectedActive,
								mod = label2
							};
							list2.Add(item);
							flag = false;
						}
						else if (!mod2.AllActive(content))
						{
							List<Event> list3 = events;
							item = new Event
							{
								event_type = EventType.LoadError,
								mod = label2
							};
							list3.Add(item);
						}
					}
					flag4 = true;
					break;
				}
				if (!flag4)
				{
					List<Event> list4 = events;
					item = new Event
					{
						event_type = ((!mods.Exists((Mod candidate) => is_match(label2, candidate))) ? EventType.NotFound : EventType.OutOfOrder),
						mod = label2
					};
					list4.Add(item);
					flag2 = false;
				}
			}
			for (int j = num + 1; j != mods.Count; j++)
			{
				Mod mod3 = mods[j];
				if ((mod3.available_content & relevant_content) != 0 && mod3.IsEnabledForActiveDlc())
				{
					List<Event> list5 = events;
					item = new Event
					{
						event_type = EventType.ExpectedInactive,
						mod = mod3.label
					};
					list5.Add(item);
					flag3 = true;
				}
			}
			if (flag2 && flag)
			{
				return !flag3;
			}
			return false;
		}

		private string GetFilename()
		{
			return FileSystem.Normalize(Path.Combine(GetDirectory(), "mods.json"));
		}

		public static void Dialog(GameObject parent = null, string title = null, string text = null, string confirm_text = null, System.Action on_confirm = null, string cancel_text = null, System.Action on_cancel = null, string configurable_text = null, System.Action on_configurable_clicked = null, Sprite image_sprite = null)
		{
			((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent ?? Global.Instance.globalCanvas)).PopupConfirmDialog(text, on_confirm, on_cancel, configurable_text, on_configurable_clicked, title, confirm_text, cancel_text, image_sprite);
		}

		private static string MakeModList(List<Event> events, EventType event_type)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			int num = 30;
			foreach (Event @event in events)
			{
				if (@event.event_type == event_type)
				{
					stringBuilder.AppendLine(@event.mod.title);
					if (--num <= 0)
					{
						stringBuilder.AppendLine(UI.FRONTEND.MOD_DIALOGS.ADDITIONAL_MOD_EVENTS);
						break;
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static string MakeEventList(List<Event> events)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			string title = null;
			string title_tooltip = null;
			int num = 30;
			foreach (Event @event in events)
			{
				Event.GetUIStrings(@event.event_type, out title, out title_tooltip);
				stringBuilder.AppendFormat("{0}: {1}", title, @event.mod.title);
				if (!string.IsNullOrEmpty(@event.details))
				{
					stringBuilder.AppendFormat(" ({0})", @event.details);
				}
				stringBuilder.Append("\n");
				if (--num <= 0)
				{
					stringBuilder.AppendLine(UI.FRONTEND.MOD_DIALOGS.ADDITIONAL_MOD_EVENTS);
					break;
				}
			}
			return stringBuilder.ToString();
		}

		private static string MakeModList(List<Event> events)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			HashSetPool<string, Manager>.PooledHashSet pooledHashSet = HashSetPool<string, Manager>.Allocate();
			int num = 30;
			foreach (Event @event in events)
			{
				if (pooledHashSet.Add(@event.mod.title))
				{
					stringBuilder.AppendLine(@event.mod.title);
					if (--num <= 0)
					{
						stringBuilder.AppendLine(UI.FRONTEND.MOD_DIALOGS.ADDITIONAL_MOD_EVENTS);
						break;
					}
				}
			}
			pooledHashSet.Recycle();
			return stringBuilder.ToString();
		}

		private void LoadFailureDialog(GameObject parent)
		{
			if (events.Count == 0)
			{
				return;
			}
			foreach (Event @event in events)
			{
				if (@event.event_type != 0)
				{
					continue;
				}
				foreach (Mod mod in mods)
				{
					if (!mod.IsLocal && mod.label.Match(@event.mod))
					{
						mod.status = Mod.Status.ReinstallPending;
					}
				}
			}
			dirty = true;
			Update(this);
			Dialog(parent, UI.FRONTEND.MOD_DIALOGS.LOAD_FAILURE.TITLE, string.Format(UI.FRONTEND.MOD_DIALOGS.LOAD_FAILURE.MESSAGE, MakeModList(events, EventType.LoadError)), UI.FRONTEND.MOD_DIALOGS.RESTART.OK, cancel_text: UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL, on_confirm: App.instance.Restart, on_cancel: delegate
			{
			});
			events.Clear();
		}

		private void DevRestartDialog(GameObject parent, bool is_crash)
		{
			if (events.Count == 0)
			{
				return;
			}
			if (is_crash)
			{
				Dialog(parent, UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.TITLE, string.Format(UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.DEV_MESSAGE, MakeEventList(events)), UI.FRONTEND.MOD_DIALOGS.RESTART.OK, delegate
				{
					foreach (Mod mod in mods)
					{
						mod.SetEnabledForActiveDlc(enabled: false);
					}
					dirty = true;
					Update(this);
					App.instance.Restart();
				}, UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL, delegate
				{
				});
			}
			else
			{
				Dialog(parent, UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE, string.Format(UI.FRONTEND.MOD_DIALOGS.RESTART.DEV_MESSAGE, MakeEventList(events)), UI.FRONTEND.MOD_DIALOGS.RESTART.OK, delegate
				{
					App.instance.Restart();
				}, UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL, delegate
				{
				});
			}
			events.Clear();
		}

		public void RestartDialog(string title, string message_format, System.Action on_cancel, bool with_details, GameObject parent, string cancel_text = null)
		{
			if (events.Count != 0)
			{
				Dialog(parent, title, string.Format(message_format, with_details ? MakeEventList(events) : MakeModList(events)), UI.FRONTEND.MOD_DIALOGS.RESTART.OK, cancel_text: cancel_text ?? ((string)UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL), on_confirm: App.instance.Restart, on_cancel: on_cancel);
				events.Clear();
			}
		}

		public void NotifyDialog(string title, string message_format, GameObject parent)
		{
			if (events.Count != 0)
			{
				Dialog(parent, title, string.Format(message_format, MakeEventList(events)));
				events.Clear();
			}
		}

		public void SearchForModsInStackTrace(StackTrace stackTrace)
		{
			StackFrame[] frames = stackTrace.GetFrames();
			foreach (StackFrame stackFrame in frames)
			{
				if (stackFrame == null)
				{
					continue;
				}
				Assembly assembly = null;
				MethodBase method = stackFrame.GetMethod();
				if (method != null)
				{
					Type declaringType = method.DeclaringType;
					if (declaringType != null)
					{
						assembly = declaringType.Assembly;
					}
				}
				foreach (Mod mod in mods)
				{
					if (mod.loaded_mod_data != null && !mod.foundInStackTrace)
					{
						if (assembly != null && mod.loaded_mod_data.dlls.Contains(assembly))
						{
							Debug.Log($"{mod.title}'s assembly declared the method {method.DeclaringType}:{method.Name} in the stack trace, adding to referenced mods list");
							mod.foundInStackTrace = true;
						}
						else if (method != null && mod.loaded_mod_data.patched_methods.Contains(method))
						{
							Debug.Log($"{mod.title}'s patched_method {method.DeclaringType}:{method.Name} appears in the stack trace, adding to referenced mods list");
							mod.foundInStackTrace = true;
						}
					}
				}
			}
			string stackStr = stackTrace.ToString();
			SearchForModsInStackTrace(stackStr);
		}

		public void SearchForModsInStackTrace(string stackStr)
		{
			foreach (Mod mod in mods)
			{
				if (mod.loaded_mod_data == null || mod.foundInStackTrace)
				{
					continue;
				}
				foreach (MethodBase patched_method in mod.loaded_mod_data.patched_methods)
				{
					if (new Regex(Regex.Escape(patched_method.DeclaringType.ToString()) + "[.:]" + Regex.Escape(patched_method.Name.ToString())).Match(stackStr).Success)
					{
						Debug.Log($"{mod.title}'s patched_method {patched_method.DeclaringType}.{patched_method.Name} matched in the stack trace, adding to referenced mods list");
						mod.foundInStackTrace = true;
						break;
					}
				}
			}
		}

		public void HandleErrors(List<YamlIO.Error> world_gen_errors)
		{
			string value = FileSystem.Normalize(GetDirectory());
			ListPool<Mod, Manager>.PooledList pooledList = ListPool<Mod, Manager>.Allocate();
			Event item;
			foreach (YamlIO.Error world_gen_error in world_gen_errors)
			{
				string text = ((world_gen_error.file.source != null) ? FileSystem.Normalize(world_gen_error.file.source.GetRoot()) : string.Empty);
				YamlIO.LogError(world_gen_error, text.Contains(value));
				if (world_gen_error.severity == YamlIO.Error.Severity.Recoverable || !text.Contains(value))
				{
					continue;
				}
				foreach (Mod mod in mods)
				{
					if (mod.IsEnabledForActiveDlc() && text.Contains(mod.label.install_path))
					{
						List<Event> list = events;
						item = new Event
						{
							event_type = EventType.BadWorldGen,
							mod = mod.label,
							details = Path.GetFileName(world_gen_error.file.full_path)
						};
						list.Add(item);
						break;
					}
				}
			}
			foreach (Mod item2 in pooledList)
			{
				item2.SetCrashed();
				if (!item2.IsDev)
				{
					List<Event> list2 = events;
					item = new Event
					{
						event_type = EventType.Deactivated,
						mod = item2.label
					};
					list2.Add(item);
				}
				dirty = true;
			}
			pooledList.Recycle();
			Update(this);
		}

		public void Report(GameObject parent)
		{
			if (events.Count == 0)
			{
				return;
			}
			for (int i = 0; i < events.Count; i++)
			{
				Event @event = events[i];
				for (int num = events.Count - 1; num != i; num--)
				{
					if (events[num].event_type == @event.event_type && events[num].mod.Match(@event.mod) && events[num].details == @event.details)
					{
						events.RemoveAt(num);
					}
				}
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (Event event2 in events)
			{
				switch (event2.event_type)
				{
				case EventType.ActiveDuringCrash:
					flag = true;
					break;
				case EventType.LoadError:
					flag2 = true;
					break;
				case EventType.RestartRequested:
					flag3 = true;
					break;
				case EventType.Deactivated:
					if ((FindMod(event2.mod).available_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)) != 0)
					{
						flag3 = true;
					}
					break;
				}
			}
			flag3 = flag || flag2 || flag3;
			bool flag4 = IsInDevMode();
			if (flag3 && flag4)
			{
				DevRestartDialog(parent, flag);
			}
			else if (flag2)
			{
				LoadFailureDialog(parent);
			}
			else if (flag)
			{
				RestartDialog(UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.TITLE, UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.MESSAGE, null, with_details: false, parent);
			}
			else if (flag3)
			{
				RestartDialog(UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE, UI.FRONTEND.MOD_DIALOGS.RESTART.MESSAGE, null, with_details: true, parent);
			}
			else
			{
				NotifyDialog(UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE, flag4 ? UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.DEV_MESSAGE : UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.MESSAGE, parent);
			}
		}

		public bool Save()
		{
			if (!FileUtil.CreateDirectory(GetDirectory(), 5))
			{
				return false;
			}
			FileStream stream = FileUtil.Create(GetFilename(), 5);
			try
			{
				if (stream == null)
				{
					return false;
				}
				using StreamWriter streamWriter = FileUtil.DoIODialog(() => new StreamWriter(stream), GetFilename(), null, 5);
				if (streamWriter == null)
				{
					return false;
				}
				string value = JsonConvert.SerializeObject(new PersistentData(current_version, mods), Formatting.Indented);
				streamWriter.Write(value);
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
			return true;
		}

		public Mod FindMod(Label label)
		{
			foreach (Mod mod in mods)
			{
				if (mod.label.Equals(label))
				{
					return mod;
				}
			}
			return null;
		}

		public bool IsModEnabled(Label id)
		{
			return FindMod(id)?.IsEnabledForActiveDlc() ?? false;
		}

		public bool EnableMod(Label id, bool enabled, object caller)
		{
			Mod mod = FindMod(id);
			if (mod == null)
			{
				return false;
			}
			if (mod.IsEmpty())
			{
				return false;
			}
			if (mod.IsEnabledForActiveDlc() == enabled)
			{
				return false;
			}
			mod.SetEnabledForActiveDlc(enabled);
			if (enabled)
			{
				mod.Load(Content.LayerableFiles);
			}
			else
			{
				mod.Unload(Content.LayerableFiles);
			}
			dirty = true;
			Update(caller);
			return true;
		}

		public void Reinsert(int source_index, int target_index, bool move_to_end, object caller)
		{
			if (move_to_end)
			{
				target_index = mods.Count;
			}
			DebugUtil.Assert(source_index != target_index);
			if (source_index >= -1 && mods.Count > source_index && target_index >= -1 && mods.Count > target_index)
			{
				Mod item = mods[source_index];
				mods.RemoveAt(source_index);
				if (source_index > target_index)
				{
					target_index++;
				}
				if (target_index == mods.Count)
				{
					mods.Add(item);
				}
				else
				{
					mods.Insert(target_index, item);
				}
				dirty = true;
				Update(caller);
			}
		}

		public void SendMetricsEvent()
		{
			ListPool<string, Manager>.PooledList pooledList = ListPool<string, Manager>.Allocate();
			foreach (Mod mod in mods)
			{
				if (mod.IsEnabledForActiveDlc())
				{
					pooledList.Add(mod.title);
				}
			}
			DictionaryPool<string, object, Manager>.PooledDictionary pooledDictionary = DictionaryPool<string, object, Manager>.Allocate();
			pooledDictionary["ModCount"] = pooledList.Count;
			pooledDictionary["Mods"] = pooledList;
			ThreadedHttps<KleiMetrics>.Instance.SendEvent(pooledDictionary, "Mods");
			pooledDictionary.Recycle();
			pooledList.Recycle();
			KCrashReporter.haveActiveMods = pooledList.Count > 0;
		}
	}
}
