namespace GameForestTestAssignment.Core.ScreenSystem;

public interface IScreenNavigation
{
    public void PushScreen(Screen screen);
    public void PopToFirstScreen();
}