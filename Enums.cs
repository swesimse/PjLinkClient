namespace PjlinkClient
{
	public enum ResponseCodes
	{
		UNKNOWN = 0,
		Value,
		OK,
		ERR1,
		ERR2,
		ERR3,
		ERR4
	}

	public enum Statuses
	{
		NoAction = 0,
		Sent,
		Recieved,
		Error
	}

	public enum Commands
	{ 
		PowerControl,
		PowerStatusQuery,
		InputSwitchInstruction,
		InputSwitchQuery,
		MuteInstruction,
		MuteStatusQuery,
		ErrorStatusQuery,
		LampNumberLightingHourQuery,
		InputTogglingListQuery,
		ProjectorNameQuery,
		ManufactureNameInformationQuery,
		ProductNameInformationQuery,
		OtherInformationQuery,
		ClassInformationQuery
	}

	public enum PowerStatus
	{
		Unknown = 0,
		PoweredOff = 1,
		PoweredOn = 2,
		CoolingDown = 3,
		WarmingUp = 4
	}

	public enum Inputs
	{
		VGA1 = 11,
		VGA2 = 12,
		SVideo = 21,
		CVBS = 22,
		HDMI = 31,
		CARD_READER = 51,
		LAN_DISPLAY = 52,
		USB_DISPLAY = 53,
		Speaker_Volume_Up = 42,
		Speaker_Volume_Down = 43,
		Microphone_Volume_Up = 44,
		Microphone_Volume_Down = 45,
		Freeze = 46,
		Change_picture_mode = 47
	}
}
