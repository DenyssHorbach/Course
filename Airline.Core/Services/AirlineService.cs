using Airline.Core.Comparers;
using Airline.Core.Exceptions;
using Airline.Core.Interfaces;
using Airline.Core.Models;
using Airline.Core.Repositories;

namespace Airline.Core.Services;

public class AirlineService
{
    private readonly IRepository<Airport> _airports = new GenericRepository<Airport>();
    private readonly IRepository<Flight> _flights = new GenericRepository<Flight>();
    private readonly IRepository<Passenger> _passengers = new GenericRepository<Passenger>();
    private readonly IRepository<Ticket> _tickets = new GenericRepository<Ticket>();

    private readonly JsonFileService<Airport> _airportFile;
    private readonly JsonFileService<Flight> _flightFile;
    private readonly JsonFileService<Passenger> _passengerFile;
    private readonly JsonFileService<Ticket> _ticketFile;

    public AirlineService(string dataDirectory = "data")
    {
        _airportFile = new JsonFileService<Airport>(Path.Combine(dataDirectory, "airports.json"));
        _flightFile = new JsonFileService<Flight>(Path.Combine(dataDirectory, "flights.json"));
        _passengerFile = new JsonFileService<Passenger>(Path.Combine(dataDirectory, "passengers.json"));
        _ticketFile = new JsonFileService<Ticket>(Path.Combine(dataDirectory, "tickets.json"));
    }

    public void LoadAll()
    {
        _airports.ReplaceAll(_airportFile.Load());
        _flights.ReplaceAll(_flightFile.Load());
        _passengers.ReplaceAll(_passengerFile.Load());
        _tickets.ReplaceAll(_ticketFile.Load());
    }

    public void SaveAll()
    {
        _airportFile.Save(_airports.GetAll());
        _flightFile.Save(_flights.GetAll());
        _passengerFile.Save(_passengers.GetAll());
        _ticketFile.Save(_tickets.GetAll());
    }

    public void AddAirport(Airport airport)
    {
        if (_airports.Find(a => a.Code.Equals(airport.Code, StringComparison.OrdinalIgnoreCase)).Count > 0)
            throw new ArgumentException($"Аеропорт із кодом {airport.Code} вже існує.");
        _airports.Add(airport);
    }

    public void AddFlight(Flight flight)
    {
        if (_flights.Find(f => f.FlightNumber.Equals(flight.FlightNumber, StringComparison.OrdinalIgnoreCase)).Count > 0)
            throw new ArgumentException($"Рейс {flight.FlightNumber} вже існує.");
        _flights.Add(flight);
    }

    public void AddPassenger(Passenger passenger)
    {
        if (_passengers.Find(p => p.PassportNumber.Equals(passenger.PassportNumber, StringComparison.OrdinalIgnoreCase)).Count > 0)
            throw new ArgumentException($"Пасажир із паспортом {passenger.PassportNumber} вже зареєстрований.");
        _passengers.Add(passenger);
    }

    public List<Airport> GetAirports()
    {
        var list = _airports.GetAll();
        list.Sort();
        return list;
    }

    public List<Flight> GetFlights(bool sortByPrice = false)
    {
        var list = _flights.GetAll();
        if (sortByPrice)
            list.Sort(new FlightPriceComparer());
        else
            list.Sort();
        return list;
    }

    public List<Passenger> GetPassengers()
    {
        var list = _passengers.GetAll();
        list.Sort();
        return list;
    }

    public List<Ticket> GetTickets()
    {
        var list = _tickets.GetAll();
        list.Sort();
        return list;
    }

    public Airport? GetAirportById(Guid id) => _airports.Find(a => a.Id == id).FirstOrDefault();
    public Flight? GetFlightById(Guid id) => _flights.Find(f => f.Id == id).FirstOrDefault();
    public Passenger? GetPassengerById(Guid id) => _passengers.Find(p => p.Id == id).FirstOrDefault();

    public int GetBookedSeatsCount(Guid flightId) => _tickets.Find(t => t.FlightId == flightId).Count;

    public List<Flight> SearchFlightsByDestination(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<Flight>();

        string normalized = query.Trim();
        var matchingAirportIds = _airports
            .Find(a => a.City.Contains(normalized, StringComparison.OrdinalIgnoreCase)
                    || a.Code.Contains(normalized, StringComparison.OrdinalIgnoreCase)
                    || a.Country.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .Select(a => a.Id)
            .ToHashSet();

        var result = _flights.Find(f => matchingAirportIds.Contains(f.ArrivalAirportId));
        result.Sort();
        return result;
    }

    public decimal CalculateExtraBaggageFee(Flight flight, double baggageWeightKg)
    {
        if (baggageWeightKg < 0)
            throw new ArgumentException("Вага багажу не може бути від'ємною.");

        if (baggageWeightKg > flight.MaxBaggageWeightKg)
            throw new BaggageWeightLimitExceededException(baggageWeightKg, flight.MaxBaggageWeightKg);

        if (baggageWeightKg <= flight.FreeBaggageWeightKg)
            return 0m;

        double excessKg = baggageWeightKg - flight.FreeBaggageWeightKg;
        return Math.Round((decimal)excessKg * flight.ExtraBaggagePricePerKg, 2);
    }

    public static decimal GetClassMultiplier(ServiceClass serviceClass) => serviceClass switch
    {
        ServiceClass.Economy => 1.0m,
        ServiceClass.Business => 1.8m,
        ServiceClass.First => 2.5m,
        _ => throw new ArgumentException("Невідомий клас обслуговування.")
    };

    public Ticket BookTicket(Guid flightId, Guid passengerId, ServiceClass serviceClass, double baggageWeightKg)
    {
        Flight flight = GetFlightById(flightId)
            ?? throw new ArgumentException("Обраний рейс не знайдено.");
        _ = GetPassengerById(passengerId)
            ?? throw new ArgumentException("Обраного пасажира не знайдено.");

        int booked = GetBookedSeatsCount(flight.Id);
        if (booked >= flight.MaxPassengers)
            throw new OverbookingException(flight.FlightNumber, flight.MaxPassengers);

        decimal extraBaggageFee = CalculateExtraBaggageFee(flight, baggageWeightKg);
        decimal classPrice = Math.Round(flight.BasePrice * GetClassMultiplier(serviceClass), 2);
        decimal totalPrice = classPrice + extraBaggageFee;

        var ticket = new Ticket(flight.Id, passengerId, serviceClass, baggageWeightKg, extraBaggageFee, totalPrice);
        _tickets.Add(ticket);
        return ticket;
    }

    public bool CancelTicket(Guid ticketId)
    {
        Ticket? ticket = _tickets.Find(t => t.Id == ticketId).FirstOrDefault();
        return ticket is not null && _tickets.Remove(ticket);
    }

    public void SeedSampleData()
    {
        if (_airports.GetAll().Count > 0 || _flights.GetAll().Count > 0)
            return;

        var waw = new Airport("WAW", "Chopin Airport", "Warsaw", "Poland");
        var lhr = new Airport("LHR", "Heathrow", "London", "UK");
        var fra = new Airport("FRA", "Frankfurt Airport", "Frankfurt", "Germany");
        var vie = new Airport("VIE", "Vienna International", "Vienna", "Austria");

        AddAirport(waw);
        AddAirport(lhr);
        AddAirport(fra);
        AddAirport(vie);

        // Рейс із місткістю 2 місця — зручно для демонстрації OverbookingException
        AddFlight(new Flight("LO-281", waw.Id, lhr.Id, DateTime.Now.AddDays(3), 4500m, 2, 20, 32, 200m));
        AddFlight(new Flight("LH-135", fra.Id, vie.Id, DateTime.Now.AddDays(1), 3200m, 5, 15, 28, 150m));
        AddFlight(new Flight("OS-624", vie.Id, lhr.Id, DateTime.Now.AddDays(5), 5100m, 10, 23, 32, 180m));

        AddPassenger(new Passenger("Іваненко Олексій", "FE123456", "+380671112233"));
        AddPassenger(new Passenger("Петренко Марія", "KH654321", "+380931234567"));
        AddPassenger(new Passenger("Коваленко Дмитро", "AB987654", "+380509998877"));
    }
}