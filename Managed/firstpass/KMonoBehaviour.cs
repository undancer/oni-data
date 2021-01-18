using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KMonoBehaviour : MonoBehaviour, IStateMachineTarget, ISaveLoadable, IUniformGridObject
{
	public static GameObject lastGameObject;

	public static KObject lastObj;

	public static bool isPoolPreInit;

	public static bool isLoadingScene;

	private KObject obj;

	private bool isInitialized = false;

	protected bool autoRegisterSimRender = true;

	protected bool simRenderLoadBalance;

	public bool isSpawned
	{
		get;
		private set;
	}

	public new Transform transform => base.transform;

	public bool isNull => this == null;

	public void Awake()
	{
		if (!App.IsExiting)
		{
			InitializeComponent();
		}
	}

	public void InitializeComponent()
	{
		if (isInitialized)
		{
			return;
		}
		if (!isPoolPreInit && Application.isPlaying && lastGameObject != base.gameObject)
		{
			lastGameObject = base.gameObject;
			lastObj = KObjectManager.Instance.GetOrCreateObject(base.gameObject);
		}
		obj = lastObj;
		isInitialized = true;
		MyAttributes.OnAwake(this);
		if (!isPoolPreInit)
		{
			try
			{
				OnPrefabInit();
			}
			catch (Exception e)
			{
				string errorMessage = "Error in " + base.name + "." + GetType().Name + ".OnPrefabInit";
				DebugUtil.LogException(this, errorMessage, e);
			}
		}
	}

	private void OnEnable()
	{
		if (!App.IsExiting)
		{
			OnCmpEnable();
		}
	}

	private void OnDisable()
	{
		if (!App.IsExiting && !isLoadingScene)
		{
			OnCmpDisable();
		}
	}

	public bool IsInitialized()
	{
		return isInitialized;
	}

	public void OnDestroy()
	{
		OnForcedCleanUp();
		if (App.IsExiting)
		{
			return;
		}
		if (isLoadingScene)
		{
			OnLoadLevel();
			return;
		}
		if (KObjectManager.Instance != null)
		{
			KObjectManager.Instance.QueueDestroy(obj);
		}
		OnCleanUp();
		SimAndRenderScheduler.instance.Remove(this);
	}

	public void Start()
	{
		if (!App.IsExiting)
		{
			Spawn();
		}
	}

	public void Spawn()
	{
		if (isSpawned)
		{
			return;
		}
		if (!isInitialized)
		{
			Debug.LogError(base.name + "." + GetType().Name + " is not initialized.");
			return;
		}
		isSpawned = true;
		if (autoRegisterSimRender)
		{
			SimAndRenderScheduler.instance.Add(this, simRenderLoadBalance);
		}
		MyAttributes.OnStart(this);
		try
		{
			OnSpawn();
		}
		catch (Exception e)
		{
			string errorMessage = "Error in " + base.name + "." + GetType().Name + ".OnSpawn";
			DebugUtil.LogException(this, errorMessage, e);
		}
	}

	protected virtual void OnPrefabInit()
	{
	}

	protected virtual void OnSpawn()
	{
	}

	protected virtual void OnCmpEnable()
	{
	}

	protected virtual void OnCmpDisable()
	{
	}

	protected virtual void OnCleanUp()
	{
	}

	protected virtual void OnForcedCleanUp()
	{
	}

	protected virtual void OnLoadLevel()
	{
	}

	public virtual void CreateDef()
	{
	}

	public T FindOrAdd<T>() where T : KMonoBehaviour
	{
		return this.FindOrAddComponent<T>();
	}

	public void FindOrAdd<T>(ref T c) where T : KMonoBehaviour
	{
		c = FindOrAdd<T>();
	}

	public T Require<T>() where T : Component
	{
		return this.RequireComponent<T>();
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		return obj.GetEventSystem().Subscribe(hash, handler);
	}

	public int Subscribe(GameObject target, int hash, Action<object> handler)
	{
		return obj.GetEventSystem().Subscribe(target, hash, handler);
	}

	public int Subscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler) where ComponentType : Component
	{
		return obj.GetEventSystem().Subscribe(hash, handler);
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		if (obj != null)
		{
			obj.GetEventSystem().Unsubscribe(hash, handler);
		}
	}

	public void Unsubscribe(int id)
	{
		obj.GetEventSystem().Unsubscribe(id);
	}

	public void Unsubscribe(GameObject target, int hash, Action<object> handler)
	{
		obj.GetEventSystem().Unsubscribe(target, hash, handler);
	}

	public void Unsubscribe<ComponentType>(int hash, EventSystem.IntraObjectHandler<ComponentType> handler, bool suppressWarnings = false) where ComponentType : Component
	{
		if (obj != null)
		{
			obj.GetEventSystem().Unsubscribe(hash, handler, suppressWarnings);
		}
	}

	public void Trigger(int hash, object data = null)
	{
		if (obj != null && obj.hasEventSystem)
		{
			obj.GetEventSystem().Trigger(base.gameObject, hash, data);
		}
	}

	public static void PlaySound(string sound)
	{
		if (sound == null)
		{
			return;
		}
		try
		{
			if (SoundListenerController.Instance == null)
			{
				KFMOD.PlayUISound(sound);
			}
			else
			{
				KFMOD.PlayOneShot(sound, SoundListenerController.Instance.transform.GetPosition());
			}
		}
		catch
		{
			DebugUtil.LogWarningArgs("AUDIOERROR: Missing [" + sound + "]");
		}
	}

	public static void PlaySound3DAtLocation(string sound, Vector3 location)
	{
		if (SoundListenerController.Instance != null)
		{
			try
			{
				KFMOD.PlayOneShot(sound, location);
			}
			catch
			{
				DebugUtil.LogWarningArgs("AUDIOERROR: Missing [" + sound + "]");
			}
		}
	}

	public void PlaySound3D(string asset)
	{
		try
		{
			KFMOD.PlayOneShot(asset, transform.GetPosition());
		}
		catch
		{
			DebugUtil.LogWarningArgs("AUDIOERROR: Missing [" + asset + "]");
		}
	}

	public virtual Vector2 PosMin()
	{
		return transform.GetPosition();
	}

	public virtual Vector2 PosMax()
	{
		return transform.GetPosition();
	}

	ComponentType IStateMachineTarget.GetComponent<ComponentType>()
	{
		return GetComponent<ComponentType>();
	}

	GameObject IStateMachineTarget.get_gameObject()
	{
		return base.gameObject;
	}

	string IStateMachineTarget.get_name()
	{
		return base.name;
	}
}
