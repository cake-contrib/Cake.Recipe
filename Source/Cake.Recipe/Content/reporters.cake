public interface IReporterBase 
{
    public abstract string Name { get; }
    public abstract bool CanBeUsed { get; }
    public bool ShouldBeUsed { get; set; }
}

public interface ISuccessReporter : IReporterBase
{
    void ReportSuccess(ICakeContext context, BuildVersion buildVersion);
}

public interface IFailureReporter : IReporterBase
{
    void ReportFailure(ICakeContext context, BuildVersion buildVersion, Exception thrownException);
}

public abstract class SuccessReporter : ISuccessReporter
{
    public abstract string Name { get; }
    public abstract bool CanBeUsed { get; }
    public bool ShouldBeUsed { get; set; }

    public abstract void ReportSuccess(ICakeContext context, BuildVersion buildVersion);
}

public abstract class FailureReporter : IFailureReporter
{
    public abstract string Name { get; }
    public abstract bool CanBeUsed { get; }
    public bool ShouldBeUsed { get; set; }

    public abstract void ReportFailure(ICakeContext context, BuildVersion buildVersion, Exception thrownException);
}

public abstract class ReporterList<TReporter> : IEnumerable<TReporter>
where TReporter: IReporterBase
{
    private readonly Dictionary<string, TReporter> _backing = new Dictionary<string, TReporter>();

    public void Add(TReporter reporter)
    {
        _backing.Add(reporter.Name.ToUpperInvariant(), reporter);
    }

    public bool HasReporter(string name)
    {
        return _backing.Keys.Contains(name.ToUpperInvariant());
    }

    IEnumerator<TReporter> IEnumerable<TReporter>.GetEnumerator()
        => _backing.Values.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => _backing.Values.GetEnumerator();

    public TReporter this[string name] 
    {
        get
        {
            if(_backing.TryGetValue(name.ToUpperInvariant(), out var reporter))
            {
                return reporter;
            }

            return default(TReporter);
        }
    }
}

public class SuccessReporterList : ReporterList<ISuccessReporter>
{}

public class FailureReporterList : ReporterList<IFailureReporter>
{}