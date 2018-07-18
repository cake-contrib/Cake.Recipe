public class BuildData
{
	private readonly List<IIssue> issues = new List<IIssue>();

	public IEnumerable<IIssue> Issues 
	{ 
		get
		{
			return issues.AsReadOnly();
		} 
	}

	public BuildData()
	{
	}

	public void AddIssues(IEnumerable<IIssue> issues)
	{
		this.issues.AddRange(issues);
	}
}