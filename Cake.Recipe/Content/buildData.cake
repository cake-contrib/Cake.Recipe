public class BuildData
{
	private readonly List<IIssue> issues = new List<IIssue>();

	public DirectoryPath RepositoryRoot { get; }

	public IEnumerable<IIssue> Issues 
	{ 
		get
		{
			return issues.AsReadOnly();
		} 
	}

	public BuildData(ICakeContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		RepositoryRoot = context.MakeAbsolute(context.Directory("./"));
	}

	public void AddIssues(IEnumerable<IIssue> issues)
	{
		this.issues.AddRange(issues);
	}
}