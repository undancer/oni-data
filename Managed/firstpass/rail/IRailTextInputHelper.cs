namespace rail
{
	public interface IRailTextInputHelper
	{
		RailResult ShowTextInputWindow(RailTextInputWindowOption options);

		void GetTextInputContent(out string content);

		RailResult HideTextInputWindow();
	}
}
