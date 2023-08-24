using System.Diagnostics;

namespace GUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    
    private void PerformAction()
    {
        Debug.WriteLine($"The action was called ");
    }
}
