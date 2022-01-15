using System;

namespace PjlinkClient
{
	public class ProjectorConnectionException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public ProjectorConnectionException() { }

		public ProjectorConnectionException(string message)
			: base(message)
		{ }

		public ProjectorConnectionException(string message, Exception inner)
			: base(message, inner)
		{ }

		protected ProjectorConnectionException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
