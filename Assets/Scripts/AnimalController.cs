using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimalController : UnitController
{
    public string[] triggers;
    public float delay;


    protected override void Start()
    {
        base.Start();
        PathWait();
    }

    private async void PathWait() {
        await Task.Delay(((int)delay * 1000));
        if(!isMoving) 
            await TriggerWait();
    }

    private async Task TriggerWait() {
        var t = Random.Range(0, triggers.Length);
        AnimationTrigger(triggers[t]);
        await Task.Delay(1000);
        PathWait();
    }


}
