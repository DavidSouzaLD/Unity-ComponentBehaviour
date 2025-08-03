using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ComponentBehaviour serves as a base class for GameObjects that can manage multiple components derived from BaseComponent.
/// It provides methods to add and remove components, and handles their lifecycle methods.
/// </summary>

[DisallowMultipleComponent]
public abstract class ComponentBehaviour<TBehaviour> : MonoBehaviour
{
    /// <summary>
    /// Stores active components associated with this ComponentBehaviour.
    /// The key is the type of the component, and the value is the component instance.
    /// </summary>
    private Dictionary<Type, BaseComponent<TBehaviour>> _activeComponents = new();

    /// <summary>
    /// Adds a component to this ComponentBehaviour.
    /// This method is used to register components that derive from BaseComponent.
    /// </summary>
    public bool AddComponent(BaseComponent<TBehaviour> component)
    {
        _activeComponents ??= new Dictionary<Type, BaseComponent<TBehaviour>>();

        if (component == null || _activeComponents.ContainsKey(component.GetType()))
            return false;

        _activeComponents.Add(component.GetType(), component);
        return true;
    }

    /// <summary>
    /// Removes a component from this ComponentBehaviour.
    /// This method is used to unregister components that derive from BaseComponent.
    /// </summary>
    public bool RemoveComponent(BaseComponent<TBehaviour> component)
    {
        if (component == null || !_activeComponents.ContainsKey(component.GetType()))
            return false;

        _activeComponents.Remove(component.GetType());
        return true;
    }

    /// <summary>
    /// Tries to get a component of the specified type from this ComponentBehaviour.
    /// This method checks if the component exists and returns it if found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool TryGetRequiredComponent<T>(out T component) where T : BaseComponent<TBehaviour>
    {
        if (_activeComponents.TryGetValue(typeof(T), out var value))
        {
            component = value as T;
            return true;
        }

        component = null;
        return false;
    }

    /// <summary>
    /// Lifecycle method called when the component is started.
    /// This is where components can perform initialization logic.
    /// </summary>
    private void Start() => RunComponents(component => component.OnStart());

    /// <summary>
    /// Lifecycle method called when the component is enabled.
    /// This is where the component should be added to the parent ComponentBehaviour.
    /// </summary>
    private void Update() => RunComponents(component => component.OnUpdate());

    /// <summary>
    /// Lifecycle method called on each physics update.
    /// This is where components can perform physics-related logic.
    /// </summary>
    private void FixedUpdate() => RunComponents(component => component.OnPhysicsUpdate());

    /// <summary>
    /// Lifecycle method called on each late update.
    /// This is where components can perform actions that need to happen after all regular updates.
    /// </summary>
    private void LateUpdate() => RunComponents(component => component.OnLateUpdate());

    /// <summary>
    /// Runs the specified action on all active components.
    /// This is used to call lifecycle methods on each component.
    /// Note: Ensure that the action does not modify the collection while iterating.
    /// </summary>
    private void RunComponents(Action<BaseComponent<TBehaviour>> action)
    {
        foreach (var pair in _activeComponents)
        {
            var component = pair.Value;
            if (component != null && component.gameObject.activeInHierarchy && component.enabled)
                action(component);
        }
    }
}