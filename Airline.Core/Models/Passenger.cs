namespace Airline.Core.Models;

public class Passenger : BaseEntity, IComparable<Passenger>
{
    private string _fullName = string.Empty;
    private string _passportNumber = string.Empty;
    private string _phoneNumber = string.Empty;

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ПІБ пасажира не може бути порожнім.");
            _fullName = value.Trim();
        }
    }

    public string PassportNumber
    {
        get => _passportNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 5)
                throw new ArgumentException("Номер паспорта має містити щонайменше 5 символів.");
            _passportNumber = value.Trim().ToUpperInvariant();
        }
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Номер телефону не може бути порожнім.");
            _phoneNumber = value.Trim();
        }
    }

    public Passenger() { }

    public Passenger(string fullName, string passportNumber, string phoneNumber)
    {
        FullName = fullName;
        PassportNumber = passportNumber;
        PhoneNumber = phoneNumber;
    }

    public int CompareTo(Passenger? other)
    {
        if (other is null) return 1;
        return string.Compare(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() => $"{FullName} | Паспорт: {PassportNumber} | Тел: {PhoneNumber}";
}