/*
Programming block to open/close door.

Declarations:

** public void Main(string actionArgument)
* Params:
 - argument: a comma separated string, separating the following parameters:
  - door name: the name of the door to open.
  - actionArgument: check method `door_actionName_for_actionArgument(string actionArgument)`.
  - timerBlock_close_name: the name of the timer block that should be fired on to fire this programming block again to close the door.
	 This timer block should be set up ahead of time with the following actions:
	 1) Fire this programming block with argument "[door name],Close"
		 For example:
		  - "Sliding Door- TEST,Close".
		  - "Sliding Door- TEST2,Close".
* Notes:
 - When called by a button, should be called with argument "[door name],Open,[timerBlock_close_name]",
	For example:
	 - "Sliding Door- TEST,Open,Timer Block- Sliding Door- TEST- Close Programming".
	 - "Sliding Door- TEST2,Open,Timer Block- Sliding Door- TEST2- Close Programming".
 - Names cannot have any commas, as we use the comma to separate parameters.

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

string door_actionName_open = "Open_On";
string door_actionName_close = "Open_Off";

public void Main(string argument)
{
	string[] arguments = argument.Split(',');
	string doorName = arguments[0];
	string actionArgument = arguments[1];

	IMyDoor door = (IMyDoor)GridTerminalSystem.GetBlockWithName(doorName);
	if (door == null)
	{
		Echo("Could not find door named " + doorName);
		return;
	}
	door.GetActionWithName("OnOff_On").Apply(door);

	string action = door_actionName_for_actionArgument(actionArgument);
	if (action == null)
	{
		return;
	}

	door.GetActionWithName(action).Apply(door);

	if (action == door_actionName_open)
	{
		string timerBlock_name = arguments[2];
		bool success = timerBlock_start(timerBlock_name);
		if (success == false)
		{
			return;
		}
	}

	Echo("successfully performed action " + actionArgument + " on door " + doorName);
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
			Echo("Unhandled actionArgument " + actionArgument);
			return null;
	}
}

public bool timerBlock_start(string timerBlock_name)
{
	IMyTimerBlock timerBlock = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName(timerBlock_name);
	if (timerBlock == null)
	{
		Echo("Error finding timer block with name " + timerBlock_name);
		return false;
	}

	timerBlock.GetActionWithName("OnOff_On").Apply(timerBlock);
	timerBlock.GetActionWithName("Start").Apply(timerBlock);
	return true;
}
