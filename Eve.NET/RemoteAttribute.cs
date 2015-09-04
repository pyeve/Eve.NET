using System;

namespace Eve
{
	/// <summary>
	/// Remote attribute used to flag properties that map to API meta fields.
	/// </summary>
	[AttributeUsage (AttributeTargets.Property, Inherited = true)]
	public class RemoteAttribute : Attribute
	{
		private readonly Meta _field;

		public RemoteAttribute (Meta field)
		{
			_field = field;
		}

		public Meta Field { get { return _field; } }
	}

	/// <summary>
	/// Remote (Eve) API meta fields.
	/// </summary>
	public enum Meta
	{
		DocumentId,
		ETag,
		LastUpdated,
		DateCreated,
        Deleted
	}
}

