namespace rail
{
	public interface IRailIMEHelper
	{
		RailResult EnableIMEHelperTextInputWindow(bool enable, RailTextInputImeWindowOption option);

		RailResult UpdateIMEHelperTextInputWindowPosition(RailWindowPosition position);
	}
}
