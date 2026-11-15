using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Labb_3.ViewModels.ConfigurationViewModel;


namespace Labb_3.ViewModels;

class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }

    public event Action<DialogType> OpenDialog;
    public void RaiseOpenDialog(DialogType dialogtype)
    {
        OpenDialog?.Invoke(dialogtype);
    }

}
