using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VinValidator.Models;

public class BaseModel : INotifyPropertyChanged
{
    private DateTime createdDate;
    public DateTime CreatedDate
    {
        get => createdDate;
        set
        {
            if (createdDate != value)
            {
                createdDate = value;
                OnPropertyChanged();
            }
        }
    }

    private DateTime modifiedDate;
    public DateTime ModifiedDate
    {
        get => modifiedDate;
        set
        {
            if (modifiedDate != value)
            {
                modifiedDate = value;
                OnPropertyChanged();
            }
        }
    }

    private string? createdBy;
    public string? CreatedBy
    {
        get => createdBy;
        set
        {
            if (createdBy != value)
            {
                createdBy = value;
                OnPropertyChanged();
            }
        }
    }

    private string? modifiedBy;
    public string? ModifiedBy
    {
        get => modifiedBy;
        set
        {
            if (modifiedBy != value)
            {
                modifiedBy = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new(propertyName));
}
