# Ripsaw

### Connections To Servers:
* Authentication => "ripsawstudios.ddns.net", 39450
* Database => "ripsawstudios.ddns.net", 39451
* LoadBalancer => "ripsawstudios.ddns.net", 39452
* SpinUp / Lobby => "ripsawstudios.ddns.net", 39453
* Matchmaking => "ripsawstudios.ddns.net", 39454
* Game => "ripsawstudios.ddns.net", 39455
* Chat => "ripsawstudios.ddns.net", 39456
* Purchasing => "ripsawstudios.ddns.net", TBD

The servers receive and send xml strings. These strings are converted into byte[] (byte arrays). The size of the byte[] (byte array) needs to be prepended as well like "[***]".

### Example:
If an xml string is turned into a byte array, the size of this is the length of the array, so we take "[length]" and turn that into a byte array.
We then combine the two byte[]'s together with the size at the beginning.

		[CODE]
		xmlString = "testString";
		byte[] xmlByteArray = Encoding.ASCII.GetBytes(xmlString);
		byte[] toPrepend = Encoding.ASCII.GetBytes("[" + xmlByteArray.Length.ToString() + "]";
		byte[] combinedToSendToServer = Combine(toPrepend, xmlByteArray);

		byte[] Combine(byte[] prepend, byte[] xml)
		{
			byte[] toReturn = new byte[prepend.Length + xml.Length);
			System.Buffer.BlockCopy(prepend, 0, toReturn, 0, prepend.Length);
			System.Buffer.BlockCopy(xml, 0, toReturn, prepend.Length, xml.Length);
			return toReturn;
		}
		[/CODE]

### Authentication Server
	*Accepts*
		- <RipsawMessage><MessageType>Login</MessageType><Login><Email>EmailHere</Email><Password>PasswordHere</Password></Login></RipsawMessage>
		- <RipsawMessage><MessageType>Register</MessageType><Register><Email>EmailHere</Email><Password>PasswordHere</Password></Register></RipsawMessage>
	*Returns*
		- <RipsawMessage><MessageType>Warning</MessageType><Message>Email/Password combination does not exist</Message></RipsawMessage>
		- <RipsawMessage><MessageType>Login</MessageType><Account><ID>AccountIDHere</ID><Guid>AccountGuidHere</Guid></Account><Reward></Reward></RipsawMessage>
		- <RipsawMessage><MessageType>Register</MessageType><Message>MessageHere</Message></RipsawMessage>
		
### LoadBalancer Server
	*Accepts*
		- <RipsawMessage><MessageType>ServerList</MessageType><Account><ID>AccountIDHere</ID><Guid>AccountGuidHere</Guid></Account></RipsawMessage>
	*Returns*
		- <RipsawMessage><MessageType>Warning</MessageType><Message>WarningMessageHere</Message></RipsawMessage>
		- <RipsawMessage><MessageType>ServerList</MessageType>
			<Servers>
				<Server><IP>ServerIPHere</IP><Port>ServerPortHere</Port></Server>
				<Server><IP>ServerIPHere</IP><Port>ServerPortHere</Port></Server>
				<Server><IP>ServerIPHere</IP><Port>ServerPortHere</Port></Server>
				<Server><IP>ServerIPHere</IP><Port>ServerPortHere</Port></Server>
			</Servers></RipsawMessage>
		  
### SpinUp / Lobby Server
	*Accepts*
		<RipsawMessage><MessageType>MatchmakingAdd</MessageType><Account><ID>AccountIDHere</ID><Guid>AccountGuidHere</Guid></Account><Join><Match><Players>1v1</Players><Team></Team><Type>Default</Type></Match></Join></RipsawMessage>
		
