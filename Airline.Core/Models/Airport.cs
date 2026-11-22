namespace Airline.Core.Models;

public class Airport : BaseEntity, IComparable<Airport>
{
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _city = string.Empty;
    private string _country = string.Empty;

    public string Code
    {
        get => _code;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim().Length != 3)
                throw new ArgumentException("Код аеропорту (IATA) має складатися рівно з 3 літер (наприклад, WAW).");
            _code = value.Trim().ToUpperInvariant();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва аеропорту не може бути порожньою.");
            _name = value.Trim();
        }
    }

    public string City
    {
        get => _city;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Місто не може бути порожнім.");
            _city = value.Trim();
        }
    }

    public string Country
    {
        get => _country;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Країна не може бути порожньою.");
            _country = value.Trim();
        }
    }

    public Airport() { }

    public Airport(string code, string name, string city, string country)
    {
        Code = code;
        Name = name;
        City = city;
        Country = country;
    }

    public int CompareTo(Airport? other)
    {
        if (other is null) return 1;
        return string.Compare(Code, other.Code, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() => $"{Code} — {Name} ({City}, {Country})";
}