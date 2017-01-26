/*
Programming block to open/close door.

*** Notes:
	- This documentation is incomplete in terms of all declarations, but is complete in terms of explaining usage (i.e. what arguments to use on Main() method).
	- Check Doors-Guide.txt for more details and recommendations for what parameters to send and what to name things.

*** Declarations:

** public void Main(string actionArgument)
* Params:
 - argument: a comma separated string, separating the following parameters:
  - door name: the name of the door to open.
  - actionArgument: check method `door_actionName_for_actionArgument(string actionArgument)`.
  - timerBlock_close_name: the name of the timer block that should be fired on to fire this programming block again to close the door.
	 This timer block should be set up ahead of time with the following actions:
	 1) Fire this programming block with argument:
		`d:`[door name]+` a:Close`
		For example:
		d:Sliding Door- Reactor Room -> Outside d:Sliding Door- Outside -> Reactor Room a:Close
* Notes:
	- When called by a button, should be called with argument:
	`d:`[door name]+` a:Open t:`[timer block name]
	For example:
	d:Sliding Door- Reactor Room -> Outside d:Sliding Door- Outside -> Reactor Room a:Open t:Timer Block- Automatic Doors- Close- Reactor Room <-> Outside
 - Names cannot have any colons, as we use them to separate key-value pairs.

** public string door_actionName_for_actionArgument(string actionArgument)
* Params:
 - actionArgument: can be "Open" or "Close"
* Returns:
 - the api action name for the door.

** public bool timerBlock_start(string timerBlock_name)
* Description: Attempts to start a timer block.
* Params:
 - timerBlock_name: name of timer block to start.
* Returns:
 - If the timer was successfully started, returns `true`, otherwise returns `false`.
*/

const string door_actionName_open = "Open_On";
const string door_actionName_close = "Open_Off";

List<string> debugText = new List<string>();
public string debugText_string_generate()
{
	string debugText_string= "";
	for(
		int debugText_index = 0;
		debugText_index < debugText.Count;
		debugText_index++
	)
	{
		if (debugText_string.Length >= 0)
		{
			debugText_string += "\n";
		}
		debugText_string += debugText[debugText_index];
	}

	return debugText_string;
}

public class DoorActionContainer
{
	public List<string> doorNames;
	public string action;
	public string timerBlock_name;

	public string errorMessage;

    public DoorActionContainer(string argument)
    {
		string[] arguments_colonSeparated = argument.Split(':');
		int arguments_colonSeparated_length = arguments_colonSeparated.Length;
		if (arguments_colonSeparated_length <= 1)
		{
			errorMessage = "You need at least one colon to define arguments in " + argument;
			return;
		}
		
		doorNames = new List<string>();
		
		string key = arguments_colonSeparated[0];
		for(
			int arguments_colonSeparated_index = 1;
			arguments_colonSeparated_index < arguments_colonSeparated_length;
			arguments_colonSeparated_index++
			)
		{
			string valueForKey = "";
			string argumentAtIndex = arguments_colonSeparated[arguments_colonSeparated_index];
			if (arguments_colonSeparated_index + 1 == arguments_colonSeparated_length)
			{
				valueForKey = argumentAtIndex;
				SetValueForKey(key,valueForKey);
			}
			else
			{
				string[] components = argumentAtIndex.Split(' ');
				int components_length = components.Length;
				if (components_length <= 1)
				{
					errorMessage = "You need to space out your components in argumentAtIndex: " + argumentAtIndex;
					return;
				}
				
				for(
					int components_index = 0;
					components_index + 1 < components_length; //We don't need to loop to the last item, since we know the last item is the next key.
					components_index++
					)
				{
					string component = components[components_index];
					
					if (valueForKey.Length == 0)
					{
						valueForKey = component;
					}
					else
					{
						valueForKey += " " + component;
					}
				}
				
				SetValueForKey(key,valueForKey);
				key = components[components_length - 1];
			}
		}
    }
	
	private void SetValueForKey(string argumentKey, string argumentValueForKey)
	{
		switch (argumentKey)
		{
			case "d":
				doorNames.Add(argumentValueForKey);
				break;

			case "a":
				action = argumentValueForKey;
				break;

			case "t":
				timerBlock_name = argumentValueForKey;
				break;
		}
	}

	public string Description()
	{
		string Description = "";
		Description += "doorNames: ";
		for(
			int doorNames_index = 0;
			doorNames_index < doorNames.Count;
			doorNames_index++
			)
		{
			Description += "\n" + doorNames_index + ") " + doorNames[doorNames_index];
		}
		Description += "\n";
		Description += "action: " + action;
		if (timerBlock_name != null)
		{
			Description += "\n";
			Description += "timerBlock_name: " + timerBlock_name;
		}	
		
		return Description;
	}
}

public void Main(string argument)
{
	/*
	DoorActionContainer doorActionContainer_close_2doors = new DoorActionContainer("d:Sliding Door- TEST d:Sliding Door- TEST2 a:Close");
	Echo("doorActionContainer_close_2doors: " + doorActionContainer_close_2doors.Description());

	DoorActionContainer doorActionContainer_open_2doors = new DoorActionContainer("d:Sliding Door- TEST d:Sliding Door- TEST2 a:Open t:Timer Block- Sliding Door- TEST- Close Programming");
	Echo("doorActionContainer_open_2doors: " + doorActionContainer_open_2doors.Description());
	*/
	
	/*
	string[] arguments = argument.Split(',');
	string doorName = arguments[0];
	string actionArgument = arguments[1];
	*/

	DoorActionContainer doorActionContainer_argument = new DoorActionContainer(argument);
	if (doorActionContainer_argument.errorMessage != null)
	{
		Echo(
			"doorActionContainer_argument had error " + doorActionContainer_argument.errorMessage
			+
			"\n"
			+
			"from argument: " + argument
			);
		return;
	}
	
	List<string> doorNames = doorActionContainer_argument.doorNames;
	int doorNames_count = doorNames.Count;
	if (doorNames_count <= 0)
	{
		Echo("Must send at least one door");
		return;
	}

	for(
		int doorNames_index = 0;
		doorNames_index < doorNames_count;
		doorNames_index++
		)
	{
		performDoorAction(doorNames_index, doorActionContainer_argument);
	}

	string finalEcho = "";
	for(
		int debugText_index = 0;
		debugText_index < debugText.Count;
		debugText_index++
	)
	{
		if (finalEcho.Length >= 0)
		{
			finalEcho += "\n";
		}
		finalEcho += debugText[debugText_index];
	}
	debugText.Add("Finished performing door action " + doorActionContainer_argument.Description());

	string debugText_string = debugText_string_generate();
	debugText.Clear();
	Echo(debugText_string);
}

public void performDoorAction(int doorNames_index, DoorActionContainer doorActionContainer)
{
	if (doorActionContainer == null)
	{
		debugText.Add("Cannot use a nil doorActionContainer");
		return;
	}

	List<string> doorNames = doorActionContainer.doorNames;
	if (doorNames_index >= doorNames.Count)
	{
		debugText.Add(
						"doorNames_index " + doorNames_index
						+
						"is outside bounds of doorNames with count " + doorNames.Count
						);
		return;
	}

	string doorName = doorNames[doorNames_index];

	IMyDoor door = (IMyDoor)GridTerminalSystem.GetBlockWithName(doorName);
	if (door == null)
	{
		debugText.Add("Could not find door named " + doorName);
		return;
	}
	door.GetActionWithName("OnOff_On").Apply(door);

	string action = doorActionContainer.action;
	string actionName = door_actionName_for_actionArgument(action);
	if (actionName == null)
	{
		debugText.Add("Could not find door named " + doorName);
		return;
	}

	door.GetActionWithName(actionName).Apply(door);

	if (actionName == door_actionName_open)
	{
		string timerBlock_name = doorActionContainer.timerBlock_name;
		bool success = timerBlock_start(timerBlock_name);
		if (success == false)
		{
			return;
		}
	}
}

public string door_actionName_for_actionArgument(string actionArgument)
{
	switch (actionArgument)
	{
		case "Open":
			return door_actionName_open;

		case "Close":
			return door_actionName_close;

		default:
			debugText.Add("Unhandled actionArgument " + actionArgument);
			return null;
	}
}

public bool timerBlock_start(string timerBlock_name)
{
	IMyTimerBlock timerBlock = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName(timerBlock_name);
	if (timerBlock == null)
	{
		debugText.Add("Error finding timer block with name " + timerBlock_name);
		return false;
	}

	timerBlock.GetActionWithName("OnOff_On").Apply(timerBlock);
	timerBlock.GetActionWithName("Start").Apply(timerBlock);
	return true;
}
