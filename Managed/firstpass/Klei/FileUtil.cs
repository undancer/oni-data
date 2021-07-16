using System;
using System.IO;
using System.Threading;

namespace Klei
{
	public static class FileUtil
	{
		private enum Test
		{
			NoTesting,
			RetryOnce
		}

		public enum ErrorType
		{
			None,
			UnauthorizedAccess,
			IOError
		}

		private const Test TEST = Test.NoTesting;

		private const int DEFAULT_RETRY_COUNT = 0;

		private const int RETRY_MILLISECONDS = 100;

		public static ErrorType errorType;

		public static string errorSubject;

		public static string exceptionMessage;

		public static string exceptionStackTrace;

		public static event System.Action onErrorMessage;

		public static void ErrorDialog(ErrorType errorType, string errorSubject, string exceptionMessage, string exceptionStackTrace)
		{
			Debug.Log($"Error encountered during file access: {errorType} error: {errorSubject}");
			FileUtil.errorType = errorType;
			FileUtil.errorSubject = errorSubject;
			FileUtil.exceptionMessage = exceptionMessage;
			FileUtil.exceptionStackTrace = exceptionStackTrace;
			if (FileUtil.onErrorMessage != null)
			{
				FileUtil.onErrorMessage();
			}
		}

		public static T DoIOFunc<T>(Func<T> io_op, int retry_count = 0)
		{
			UnauthorizedAccessException ex = null;
			IOException ex2 = null;
			Exception ex3 = null;
			for (int i = 0; i <= retry_count; i++)
			{
				try
				{
					return io_op();
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				catch (IOException ex5)
				{
					ex2 = ex5;
				}
				catch (Exception ex6)
				{
					ex3 = ex6;
				}
				Thread.Sleep(i * 100);
			}
			if (ex != null)
			{
				throw ex;
			}
			if (ex2 != null)
			{
				throw ex2;
			}
			if (ex3 != null)
			{
				throw ex3;
			}
			throw new Exception("Unreachable code path in FileUtil::DoIOFunc()");
		}

		public static void DoIOAction(System.Action io_op, int retry_count = 0)
		{
			UnauthorizedAccessException ex = null;
			IOException ex2 = null;
			Exception ex3 = null;
			for (int i = 0; i <= retry_count; i++)
			{
				try
				{
					io_op();
					return;
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				catch (IOException ex5)
				{
					ex2 = ex5;
				}
				catch (Exception ex6)
				{
					ex3 = ex6;
				}
				Thread.Sleep(i * 100);
			}
			if (ex != null)
			{
				throw ex;
			}
			if (ex2 != null)
			{
				throw ex2;
			}
			if (ex3 != null)
			{
				throw ex3;
			}
			throw new Exception("Unreachable code path in FileUtil::DoIOAction()");
		}

		public static void DoIODialog(System.Action io_op, string io_subject, int retry_count = 0)
		{
			try
			{
				DoIOAction(io_op, retry_count);
			}
			catch (UnauthorizedAccessException ex)
			{
				DebugUtil.LogArgs("UnauthorizedAccessException during IO on ", io_subject, ", squelching. Stack trace was:\n", ex.Message, "\n", ex.StackTrace);
				ErrorDialog(ErrorType.UnauthorizedAccess, io_subject, ex.Message, ex.StackTrace);
			}
			catch (IOException ex2)
			{
				DebugUtil.LogArgs("IOException during IO on ", io_subject, ", squelching. Stack trace was:\n", ex2.Message, "\n", ex2.StackTrace);
				ErrorDialog(ErrorType.IOError, io_subject, ex2.Message, ex2.StackTrace);
			}
			catch
			{
				throw;
			}
		}

		public static T DoIODialog<T>(Func<T> io_op, string io_subject, T fail_result, int retry_count = 0)
		{
			try
			{
				return DoIOFunc(io_op, retry_count);
			}
			catch (UnauthorizedAccessException ex)
			{
				DebugUtil.LogArgs("UnauthorizedAccessException during IO on ", io_subject, ", squelching. Stack trace was:\n", ex.Message, "\n", ex.StackTrace);
				ErrorDialog(ErrorType.IOError, io_subject, ex.Message, ex.StackTrace);
				return fail_result;
			}
			catch (IOException ex2)
			{
				DebugUtil.LogArgs("IOException during IO on ", io_subject, ", squelching. Stack trace was:\n", ex2.Message, "\n", ex2.StackTrace);
				ErrorDialog(ErrorType.IOError, io_subject, ex2.Message, ex2.StackTrace);
				return fail_result;
			}
			catch
			{
				throw;
			}
		}

		public static FileStream Create(string filename, int retry_count = 0)
		{
			return DoIODialog(() => File.Create(filename), filename, null, retry_count);
		}

		public static bool CreateDirectory(string path, int retry_count = 0)
		{
			return DoIODialog(delegate
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return true;
			}, path, fail_result: false, retry_count);
		}

		public static bool DeleteDirectory(string path, int retry_count = 0)
		{
			return DoIODialog(delegate
			{
				if (!Directory.Exists(path))
				{
					return true;
				}
				Directory.Delete(path, recursive: true);
				return true;
			}, path, fail_result: false, retry_count);
		}

		public static bool FileExists(string filename, int retry_count = 0)
		{
			return DoIODialog(() => File.Exists(filename), filename, fail_result: false, retry_count);
		}
	}
}
