namespace PjlinkClient
{
	public class CommandResponse
	{
		public Commands Command { get; private set; }
		public ResponseCodes ResponseCode { get; private set; }
		public string ResponseValue { get; private set; }

		public CommandResponse(Commands command, ResponseCodes responseCode, string responseValue)
		{
			Command = command;
			ResponseCode = responseCode;
			ResponseValue = responseValue;
		}
	}
}
