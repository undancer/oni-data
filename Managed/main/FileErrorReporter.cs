using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/FileErrorReporter")]
public class FileErrorReporter : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		OnFileError();
		FileUtil.onErrorMessage += OnFileError;
	}

	private void OnFileError()
	{
		if (FileUtil.errorType != 0)
		{
			string text = FileUtil.errorType switch
			{
				FileUtil.ErrorType.UnauthorizedAccess => string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, FileUtil.errorSubject), 
				FileUtil.ErrorType.IOError => string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, FileUtil.errorSubject), 
				_ => string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNKNOWN, FileUtil.errorSubject), 
			};
			GameObject gameObject;
			if (FrontEndManager.Instance != null)
			{
				gameObject = FrontEndManager.Instance.gameObject;
			}
			else if (GameScreenManager.Instance != null && GameScreenManager.Instance.ssOverlayCanvas != null)
			{
				gameObject = GameScreenManager.Instance.ssOverlayCanvas;
			}
			else
			{
				gameObject = new GameObject();
				gameObject.name = "FileErrorCanvas";
				Object.DontDestroyOnLoad(gameObject);
				Canvas canvas = gameObject.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
				canvas.sortingOrder = 10;
				gameObject.AddComponent<GraphicRaycaster>();
			}
			if ((FileUtil.exceptionMessage != null || FileUtil.exceptionStackTrace != null) && !KCrashReporter.hasReportedError)
			{
				KCrashReporter.ReportError(FileUtil.exceptionMessage, FileUtil.exceptionStackTrace, null, null, null);
			}
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, force_active: true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(text, null, null);
			Object.DontDestroyOnLoad(component.gameObject);
		}
	}

	private void OpenMoreInfo()
	{
	}
}
