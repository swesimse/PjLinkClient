namespace PjlinkClient
{
	public class Message
	{
		public Message(
			Commands command,
			Statuses status)
		{
            Command = command;
            Status = status;
        }

		public Commands Command
		{
			get; 
			private set;
		}

		public Statuses Status
		{
			get; set;
		}

		public ResponseCodes ResponseCode
		{
			get; set;
		}

		public int ReturnValueNumber
		{
			get; set;
		}

		public string ReturnValueString
		{
			get; set;
		}

		public string ErrorMessage
		{
			get
			{
				if (ResponseCode != ResponseCodes.UNKNOWN)
				{
					switch (ResponseCode)
					{
						case ResponseCodes.OK:
							return "OK";
						case ResponseCodes.ERR1:
							return "Undefined command";
						case ResponseCodes.ERR2:
							return "Out of parameter";
						case ResponseCodes.ERR3:
							return "Unavailable time";
						case ResponseCodes.ERR4:
							return "Projector failure";
					}
					return null;
				}
				else
					return null;
			}
		}
	}
}
