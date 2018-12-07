using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class MessageReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        //An INotificationReceiver will receive all the triggered notifications. We need to 
        //have a filter to use only the notifications that we can process.
        var message = notification as Message;
        if (message == null)
            return;
        var methodToCall = message.method;
        var argument = ArgumentForMessage(message, origin.GetGraph().GetResolver());
        
        if (EditorApplication.isPlaying)
            SendMessage(methodToCall, argument);
    }

    static object ArgumentForMessage(Message emitter, IExposedPropertyTable resolver)
    {
        switch (emitter.parameterType)
        {
            case ParameterType.Int:
                return emitter.Int;
            case ParameterType.Float:
                return emitter.Float;
            case ParameterType.Object:
                return emitter.Object.Resolve(resolver);
            case ParameterType.String:
                return emitter.String;
            default:
                return null;
        }
    }
}
