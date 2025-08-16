using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NumericUpDown;

internal class MainWindowViewModel : INotifyPropertyChanged
{
    private int value = 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Value
    {
        get => this.value;
        set
        {
            if (this.value != value)
            {
                this.value = value;
                this.OnPropertyChanged();
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
