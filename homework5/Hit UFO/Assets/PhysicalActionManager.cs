using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalActionManager : SSActionManager, SSActionCallback, IActionManager {

	DiskFactory factory;

	public void ThrowDisk(GameObject disk)
    {
        PhysicalFlyAction action = ScriptableObject.CreateInstance<PhysicalFlyAction>();
        RunAction(disk, action, this);
    }

    protected new void Start()
    {
        factory = SingleTon<DiskFactory>.Instance;
    }

    public void SSActionEvent(SSAction source)
    {
        if (source is PhysicalFlyAction)
        {
            factory.FreeDisk(source.gameobject);
            source.destroy = true;
            source.enable = false;
        }
    }

	public void Reset()
	{
		actions.Clear();
		waitingAdd.Clear();
		waitingDelete.Clear();
	}
}
