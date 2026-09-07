namespace DNDSkoleOpgave.Exceptions;

public sealed class CharacterIsDefeatedException : Exception
{
    public CharacterIsDefeatedException(string name)
        : base($"{name} er besejret og kan ikke handle.") { }
}
