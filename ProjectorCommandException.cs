using System;

namespace PjlinkClient
{
	public class ProjectorCommandException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public ProjectorCommandException() { }

		public ProjectorCommandException(string message, ResponseCodes responseCode)
			: base(message)
		{
			ResponseCode = responseCode;
		}

		public ProjectorCommandException(string message, Exception inner, ResponseCodes responseCode)
			: base(message, inner)
		{
			ResponseCode = responseCode;
		}

		protected ProjectorCommandException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }

		public ResponseCodes ResponseCode
		{
			get;
			private set;
		}
	}
}
