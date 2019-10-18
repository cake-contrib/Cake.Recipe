public class BuildData
{
	public BuildData(ICakeContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}
	}
}
