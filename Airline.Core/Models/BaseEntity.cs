namespace Airline.Core.Models;

public abstract class BaseEntity
{
    private Guid _id = Guid.NewGuid();

    public Guid Id
    {
        get => _id;
        set
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Ідентифікатор не може бути порожнім.");
            _id = value;
        }
    }
}