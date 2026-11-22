namespace Airline.Core.Models;

public class Flight : BaseEntity, IComparable<Flight>
{
    private string _flightNumber = string.Empty;
    private Guid _departureAirportId;
    private Guid _arrivalAirportId;
    private DateTime _departureTime;
    private decimal _basePrice;
    private int _maxPassengers;
    private double _freeBaggageWeightKg;
    private double _maxBaggageWeightKg;
    private decimal _extraBaggagePricePerKg;

    public string FlightNumber
    {
        get => _flightNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Номер рейсу не може бути порожнім.");
            _flightNumber = value.Trim().ToUpperInvariant();
        }
    }

    public Guid DepartureAirportId
    {
        get => _departureAirportId;
        set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Некоректний аеропорт вильоту.");
            _departureAirportId = value;
        }
    }

    public Guid ArrivalAirportId
    {
        get => _arrivalAirportId;
        set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Некоректний аеропорт прильоту.");
            _arrivalAirportId = value;
        }
    }

    public DateTime DepartureTime
    {
        get => _departureTime;
        set => _departureTime = value;
    }

    public decimal BasePrice
    {
        get => _basePrice;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Базова ціна квитка має бути більшою за 0.");
            _basePrice = value;
        }
    }

    public int MaxPassengers
    {
        get => _maxPassengers;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Місткість рейсу має бути більшою за 0.");
            _maxPassengers = value;
        }
    }

    public double FreeBaggageWeightKg
    {
        get => _freeBaggageWeightKg;
        set
        {
            if (value < 0)
                throw new ArgumentException("Норма безкоштовного багажу не може бути від'ємною.");
            _freeBaggageWeightKg = value;
        }
    }

    public double MaxBaggageWeightKg
    {
        get => _maxBaggageWeightKg;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Максимальна вага багажу має бути більшою за 0.");
            _maxBaggageWeightKg = value;
        }
    }

    public decimal ExtraBaggagePricePerKg
    {
        get => _extraBaggagePricePerKg;
        set
        {
            if (value < 0)
                throw new ArgumentException("Тариф за наднормативний багаж не може бути від'ємним.");
            _extraBaggagePricePerKg = value;
        }
    }

    public Flight() { }

    public Flight(
        string flightNumber,
        Guid departureAirportId,
        Guid arrivalAirportId,
        DateTime departureTime,
        decimal basePrice,
        int maxPassengers,
        double freeBaggageWeightKg = 20,
        double maxBaggageWeightKg = 32,
        decimal extraBaggagePricePerKg = 150)
    {
        if (departureAirportId == arrivalAirportId)
            throw new ArgumentException("Аеропорт вильоту та прильоту не можуть збігатися.");
        if (maxBaggageWeightKg < freeBaggageWeightKg)
            throw new ArgumentException("Максимальна вага багажу не може бути меншою за безкоштовну норму.");

        FlightNumber = flightNumber;
        DepartureAirportId = departureAirportId;
        ArrivalAirportId = arrivalAirportId;
        DepartureTime = departureTime;
        BasePrice = basePrice;
        MaxPassengers = maxPassengers;
        FreeBaggageWeightKg = freeBaggageWeightKg;
        MaxBaggageWeightKg = maxBaggageWeightKg;
        ExtraBaggagePricePerKg = extraBaggagePricePerKg;
    }

    public int CompareTo(Flight? other)
    {
        if (other is null) return 1;
        return DepartureTime.CompareTo(other.DepartureTime);
    }
}