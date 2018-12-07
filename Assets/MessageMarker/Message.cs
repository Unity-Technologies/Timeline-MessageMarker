using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

public enum ParameterType
{
    Int,
    Float,
    String,
    Object,
    None
}

[Serializable, DisplayName("Message Marker")]
public class Message : Marker, INotification, INotificationOptionProvider 
{
    public string method;
    public bool retroactive;
    public bool emitOnce;
    
    public ParameterType parameterType;
    public int Int;
    public string String;
    public float Float;
    public ExposedReference<Object> Object;
    
    PropertyName INotification.id
    {
        get
        {
            return new PropertyName(method);
        }
    }
    
    NotificationFlags INotificationOptionProvider.flags
    {
        get
        {
            return (retroactive ? NotificationFlags.Retroactive : default(NotificationFlags)) |
                (emitOnce ? NotificationFlags.TriggerOnce : default(NotificationFlags));
        }
    }
}
