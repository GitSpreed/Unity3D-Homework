using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCActionManager : SSActionManager, SSActionCallback, IActionManager {

    DiskFactory factory;

    protected new void Start()
    {
        factory = SingleTon<DiskFactory>.Instance;
    }

    public void SSActionEvent(SSAction source, int param)
    {
        factory.FreeDisk(source.gameobject);
        source.destroy = true;
        source.enable = false;   
    }

	public void Reset()
	{
		actions.Clear();
		waitingAdd.Clear();
		waitingDelete.Clear();
	}


}
