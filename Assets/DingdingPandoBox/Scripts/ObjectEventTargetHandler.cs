using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;

public class ObjectEventTargetHandler : DefaultTrackableEventHandler
{
    [SerializeField]
    private UnityEvent onTrackingFoundEvent;

    [SerializeField]
    private UnityEvent onTrackingLostEvent;

	protected override void OnTrackingFound()
	{
        Debug.LogFormat("{0}.OnTrackingFound: Tracking found", gameObject.name);
        onTrackingFoundEvent.Invoke();
	}

	protected override void OnTrackingLost()
	{
        Debug.LogFormat("{0}.OnTrackingFound: Tracking lost", gameObject.name);
        onTrackingLostEvent.Invoke();
	}
}
