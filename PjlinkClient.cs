using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using PjlinkClient.Properties;

namespace PjlinkClient
{
	/// <summary>
	/// Client class to control a projector using the PJLINK protocol. Version 2.0.
	/// </summary>
	public class PjlinkClient : IDisposable
	{
		private readonly string hostName = "";
		private readonly int port = 4352;

		/// <summary>
		/// Create a new instance of the client class. It connects to the projector with authentication.
		/// </summary>
		public PjlinkClient(string hostName, int port)
		{
			this.hostName = hostName;
			this.port = port;
		}

		public PjlinkClient(string hostName)
		{
			this.hostName = hostName;
		}

        private CommandResponse SendCommand(Commands command)
        {
            return SendCommand(command, null);
        }

		private CommandResponse SendCommand(Commands command, int? parameter)
		{
			try
			{
				TcpClient tcpClient = new TcpClient();
				tcpClient.ReceiveBufferSize = 300;
				tcpClient.BeginConnect(hostName, port, null, null);

				int timeOut = 2000;
				if (Settings.Default.TcpConnectionTimeOut > 0)
					timeOut = Settings.Default.TcpConnectionTimeOut;

				DateTime start = DateTime.UtcNow;
				while (!tcpClient.Connected)
				{
					TimeSpan elapsed = new TimeSpan(DateTime.UtcNow.Ticks - start.Ticks);
					if (elapsed.TotalMilliseconds > timeOut)
						throw new ProjectorConnectionException("Connection timed out.");
                    Thread.Sleep(500);
				}

				NetworkStream networkStream = tcpClient.GetStream();

				string commandToSend = "%1"; // Header (%) + Class (1)
				commandToSend += GetCommandString(command);

				// Only supply a parameter if it's a control command
				if (!commandToSend.EndsWith("?"))
				{
					commandToSend += " ";
					commandToSend += parameter.ToString();
				}
				commandToSend += "\r"; // Terminating code, Carriage Return (CR)

				networkStream.Write(Encoding.ASCII.GetBytes(commandToSend.ToCharArray()), 0, commandToSend.Length);

				if (networkStream.CanRead)
				{
					byte[] bytes = new byte[tcpClient.ReceiveBufferSize];

                    Thread.Sleep(200);

					networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                    networkStream.Close();
                    tcpClient.Close();

					string returnData = Encoding.UTF8.GetString(bytes).Replace("\0", "");

					string[] commands = returnData.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (string commandString in commands)
					{
						if (commandString.StartsWith("PJLINK"))
							continue;
						if (GetCommandFromString(commandString.Substring(2, 4), parameter != null) == command)
						{
							return GetMessageFromReplyString(commandString, parameter != null);
						}
					}

					return null;
				}
				else
				{
					return null;
				}
			}
			catch (ProjectorConnectionException)
			{
				throw;
			}
			catch (Exception ex)
			{
				LogMessage(ex.ToString());
				return null;
			}
		}

		private string GetCommandString(Commands command)
		{
			switch (command)
			{
				case Commands.PowerControl:
					return "POWR";
				case Commands.PowerStatusQuery:
					return "POWR ?";
				case Commands.InputSwitchInstruction:
					return "INPT";
				case Commands.InputSwitchQuery:
					return "INPT ?";
				case Commands.MuteInstruction:
					return "AVMT";
				case Commands.MuteStatusQuery:
					return "AVMT ?";
				case Commands.ErrorStatusQuery:
					return "ERST ?";
				case Commands.LampNumberLightingHourQuery:
					return "LAMP ?";
				case Commands.InputTogglingListQuery:
					return "INST ?";
				case Commands.ProjectorNameQuery:
					return "NAME ?";
				case Commands.ManufactureNameInformationQuery:
					return "INF1 ?";
				case Commands.ProductNameInformationQuery:
					return "INF2 ?";
				case Commands.OtherInformationQuery:
					return "INFO ?";
				case Commands.ClassInformationQuery:
					return "CLSS ?";
				default:
					return null;
			}
		}

		private static Commands GetCommandFromString(string commandString, bool parameter)
		{
            if (parameter)
            {
                switch (commandString)
                {
                    case "POWR":
                        return Commands.PowerControl;
                    default:
                        throw new ProjectorCommandException("Unknown command: " + commandString, ResponseCodes.UNKNOWN);
                }
            }
			switch (commandString)
			{
				case "POWR":
					return Commands.PowerStatusQuery;
				case "INPT":
					return Commands.InputSwitchQuery;
				case "AVMT":
					return Commands.MuteStatusQuery;
				case "ERST":
					return Commands.ErrorStatusQuery;
				case "LAMP":
					return Commands.LampNumberLightingHourQuery;
				case "INST":
					return Commands.InputTogglingListQuery;
				case "NAME":
					return Commands.ProjectorNameQuery;
				case "INF1":
					return Commands.ManufactureNameInformationQuery;
				case "INF2":
					return Commands.ProductNameInformationQuery;
				case "INFO":
					return Commands.OtherInformationQuery;
				case "CLSS":
					return Commands.ClassInformationQuery;
				default:
					throw new ProjectorCommandException("Unknown command: " + commandString, ResponseCodes.UNKNOWN);
			}
		}

		public void PowerOff()
		{
			CommandResponse response = SendCommand(Commands.PowerControl, 0);
            if (response.ResponseCode != ResponseCodes.OK)
                throw new ProjectorCommandException("PowerOff command was unsuccessful", response.ResponseCode);
		}

		public void PowerOn()
		{
            CommandResponse response = SendCommand(Commands.PowerControl, 1);
            if (response.ResponseCode != ResponseCodes.OK)
                throw new ProjectorCommandException("PowerOn command was unsuccessful", response.ResponseCode);
        }

		public PowerStatus GetPowerStatus()
		{
			CommandResponse response = SendCommand(Commands.PowerStatusQuery);
			if (response != null)
			{
				if (response.ResponseCode == ResponseCodes.Value)
				{
					switch (response.ResponseValue)
					{
						case "0":
							return PowerStatus.PoweredOff;
						case "1":
							return PowerStatus.PoweredOn;
						case "2":
							return PowerStatus.CoolingDown;
						case "3":
							return PowerStatus.WarmingUp;
					}
				}
			}
			return PowerStatus.Unknown;
		}

		public bool InputSwitch(Inputs input)
		{
			try
			{
				CommandResponse response = SendCommand(Commands.InputSwitchInstruction, (int)input);
				if (response.ResponseCode == ResponseCodes.OK)
					return true;
				return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Gets current lamp lighting hours from new
		/// </summary>
		/// <returns>Cumulative lighting time of the lamp: 0–99999</returns>
		public int GetLampTime()
		{
			CommandResponse response = SendCommand(Commands.LampNumberLightingHourQuery);
			if (response != null)
			{
				if (response.ResponseCode == ResponseCodes.Value)
				{
                    int hours = -1;
                    int.TryParse(response.ResponseValue, out hours);
                    return hours;
				}
			}
			return -1;
		}

		private CommandResponse GetMessageFromReplyString(string replyString, bool hasParameter)
		{
			string[] dataSplit = replyString.Split('=');

			string header = dataSplit[0].Substring(0, 2).Trim();
			string command = dataSplit[0].Substring(2).Trim();
			string parameter = dataSplit[1].Trim();

            if (GetCommandFromString(command, hasParameter) == Commands.LampNumberLightingHourQuery &&
				parameter.Contains(' '))
				parameter = parameter.Split(' ')[0];

			CommandResponse commandResponse = new CommandResponse(
                GetCommandFromString(command, hasParameter),
				GetResponseCodeFromString(parameter),
                parameter);

			return commandResponse;
		}


		private static ResponseCodes GetResponseCodeFromString(string responseCodeString)
		{
			switch (responseCodeString)
			{
				case "OK":
					return ResponseCodes.OK;
				case "ERR1":
					return ResponseCodes.ERR1;
				case "ERR2":
					return ResponseCodes.ERR2;
				case "ERR3":
					return ResponseCodes.ERR3;
				case "ERR4":
					return ResponseCodes.ERR4;
				default:
					return ResponseCodes.Value;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			/*
			listenThread.Abort();

			networkStream.Close();
			networkStream = null;

			tcpClient = null;
			*/
		}

		#endregion

        private static void LogMessage(string message)
        {
			try
			{
				StreamWriter sw = new StreamWriter("pjlink.log", true);
				sw.WriteLine(DateTime.Now.ToString() + ": " + message);
				sw.Close();
			}
			catch { }
        }
	}
}
