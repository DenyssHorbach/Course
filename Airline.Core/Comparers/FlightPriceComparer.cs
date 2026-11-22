using Airline.Core.Models;

namespace Airline.Core.Comparers;

public class FlightPriceComparer : IComparer<Flight>
{
    public int Compare(Flight? x, Flight? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        return x.BasePrice.CompareTo(y.BasePrice);
    }
}