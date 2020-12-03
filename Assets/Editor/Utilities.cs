using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utilities
{
    static class Methods 
    {
        public static IEnumerable<MethodDesc> CollectSupportedMethods(GameObject gameObject)
        {
            if (gameObject == null)
                return Enumerable.Empty<MethodDesc>();

            var supportedMethods = new List<MethodDesc>();
            var behaviours = gameObject.GetComponents<MonoBehaviour>();

            foreach (var behaviour in behaviours)
            {
                if (behaviour == null)
                    continue;

                var type = behaviour.GetType();
                while (type != typeof(MonoBehaviour) && type != null)
                {
                    var methods = type.GetMethods(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        var name = method.Name;

                        if (!IsSupportedMethodName(name))
                            continue;

                        var parameters = method.GetParameters();
                        if (parameters.Length > 1) //methods with multiple parameters are not supported
                            continue;

                        var parameterType = ParameterType.None;
                        if (parameters.Length == 1)
                        {
                            var paramType = parameters[0].ParameterType;
                            if (paramType == typeof(string))
                                parameterType = ParameterType.String;
                            else if (paramType == typeof(float))
                                parameterType = ParameterType.Float;
                            else if (paramType == typeof(int))
                                parameterType = ParameterType.Int;
                            else if (paramType == typeof(Object) || paramType.IsSubclassOf(typeof(Object)))
                                parameterType = ParameterType.Object;
                            else
                                continue;
                        }

                        var supportedMethod = new MethodDesc { name = name, type = parameterType };
                        
                        // Since AnimationEvents only stores method name, it can't handle functions with multiple overloads.
                        // Only retrieve first found function, but discard overloads.
                        var existingMethodIndex = supportedMethods.FindIndex(m => m.name == name);
                        if (existingMethodIndex != -1)
                        {
                            // The method is only ambiguous if it has a different signature to the one we saw before
                            var existingMethod = supportedMethods[existingMethodIndex];
                            existingMethod.isOverload = existingMethod.type != parameterType;
                        }
                        else
                            supportedMethods.Add(supportedMethod);
                    }
                    type = type.BaseType;
                }
            }

            return supportedMethods;
        }
         
        static bool IsSupportedMethodName(string name)
        {
            return name != "Main" && name != "Start" && name != "Awake" && name != "Update";
        }
    }
}

