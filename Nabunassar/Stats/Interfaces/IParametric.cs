namespace Nabunassar.Stats.Interfaces;

public interface IParametric
{
    ICollection<Parameter> Parameters { get; }

    public Parameter this[string key]
    {
        get;
        set;
    }
}