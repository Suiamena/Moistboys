using System;
public interface ISocialEncounter
{
	void Initialize (Action proceedToExecute);
	void Execute (Action proceedToEnd);
	void End (Action endEncounter);
}