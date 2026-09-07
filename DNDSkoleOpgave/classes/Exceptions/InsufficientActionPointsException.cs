namespace DNDSkoleOpgave.Exceptions;

public sealed class InsufficientActionPointsException : Exception
{
    public InsufficientActionPointsException(int required, int remaining)
        : base($"Handlingen koster {required} AP, men du har kun {remaining}.") { }
}
