namespace Airline.Core.Exceptions;

public class BaggageWeightLimitExceededException : Exception
{
    public double ActualWeight { get; }
    public double MaxWeight { get; }

    public BaggageWeightLimitExceededException(double actualWeight, double maxWeight)
        : base($"Перевищено ліміт ваги багажу: {actualWeight:F1} кг при максимально допустимих {maxWeight:F1} кг.")
    {
        ActualWeight = actualWeight;
        MaxWeight = maxWeight;
    }
}