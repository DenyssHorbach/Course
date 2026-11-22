namespace Airline.Core.Exceptions;

public class OverbookingException : Exception
{
    public string FlightNumber { get; }
    public int MaxPassengers { get; }

    public OverbookingException(string flightNumber, int maxPassengers)
        : base($"Овербукінг! На рейс {flightNumber} більше немає вільних місць (ліміт: {maxPassengers}).")
    {
        FlightNumber = flightNumber;
        MaxPassengers = maxPassengers;
    }
}