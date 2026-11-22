namespace Airline.Core.Models;

public class Ticket : BaseEntity, IComparable<Ticket>
{
    private Guid _flightId;
    private Guid _passengerId;
    private ServiceClass _serviceClass;
    private double _baggageWeightKg;
    private decimal _extraBaggageFee;
    private decimal _totalPrice;
    private DateTime _purchaseDate = DateTime.Now;

    public Guid FlightId
    {
        get => _flightId;
        set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Некоректний ідентифікатор рейсу.");
            _flightId = value;
        }
    }

    public Guid PassengerId
    {
        get => _passengerId;
        set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Некоректний ідентифікатор пасажира.");
            _passengerId = value;
        }
    }

    public ServiceClass ServiceClass
    {
        get => _serviceClass;
        set
        {
            if (!Enum.IsDefined(typeof(ServiceClass), value))
                throw new ArgumentException("Невідомий клас обслуговування.");
            _serviceClass = value;
        }
    }

    public double BaggageWeightKg
    {
        get => _baggageWeightKg;
        set
        {
            if (value < 0)
                throw new ArgumentException("Вага багажу не може бути від'ємною.");
            _baggageWeightKg = value;
        }
    }

    public decimal ExtraBaggageFee
    {
        get => _extraBaggageFee;
        set
        {
            if (value < 0)
                throw new ArgumentException("Доплата за багаж не може бути від'ємною.");
            _extraBaggageFee = value;
        }
    }

    public decimal TotalPrice
    {
        get => _totalPrice;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Загальна ціна має бути більшою за 0.");
            _totalPrice = value;
        }
    }

    public DateTime PurchaseDate
    {
        get => _purchaseDate;
        set => _purchaseDate = value;
    }

    public Ticket() { }

    public Ticket(
        Guid flightId,
        Guid passengerId,
        ServiceClass serviceClass,
        double baggageWeightKg,
        decimal extraBaggageFee,
        decimal totalPrice)
    {
        FlightId = flightId;
        PassengerId = passengerId;
        ServiceClass = serviceClass;
        BaggageWeightKg = baggageWeightKg;
        ExtraBaggageFee = extraBaggageFee;
        TotalPrice = totalPrice;
        PurchaseDate = DateTime.Now;
    }

    public int CompareTo(Ticket? other)
    {
        if (other is null) return 1;
        return PurchaseDate.CompareTo(other.PurchaseDate);
    }
}