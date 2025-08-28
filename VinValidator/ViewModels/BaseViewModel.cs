﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VinValidator.ViewModels;

public partial class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new(propertyName));
}
