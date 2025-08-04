using System;
using UnityEngine;

/// <summary>
/// BaseComponent serves as a base class for components that can be added to a ComponentBehaviour.
/// It provides lifecycle methods and manages its association with a ComponentBehaviour.
/// </summary>

[DisallowMultipleComponent]
public abstract class BaseComponent<TBehaviour> : MonoBehaviour
{
    /// <summary>
    /// Stores the parent ComponentBehaviour that this BaseComponent is associated with.
    /// This is used to manage the component's lifecycle within the context of a ComponentBehaviour.
    /// </summary>
    private TBehaviour _context;

    /// <summary>
    /// Gets the parent ComponentBehaviour that this BaseComponent is associated with.
    /// This property is used to access the context in which this component operates.
    /// </summary>
    public TBehaviour Context => _context;

    /// <summary>
    /// Stores the parent BaseComponent that this component is associated with.
    /// This is used to manage the component's lifecycle within the context of a BaseComponent.
    /// </summary>
    private ComponentBehaviour<TBehaviour> _behaviour;

    /// <summary>
    /// Gets the parent ComponentBehaviour that this BaseComponent is associated with.
    /// This property is used to access the context in which this component operates.
    /// </summary>
    public ComponentBehaviour<TBehaviour> Behaviour => _behaviour;

    /// <summary>
    /// Checks if this BaseComponent has a parent ComponentBehaviour.
    /// This is used to determine if the component is part of a ComponentBehaviour context.
    /// </summary>
    private bool HasComponentBehaviour => _context != null;

    /// <summary>
    /// Gets the type of components that this BaseComponent requires.
    /// This can be overridden by derived classes to specify required components.
    /// Example: protected override Type[] RequiredComponents => new[] { typeof(MyRequiredComponent) };
    /// </summary>
    protected virtual Type[] RequiredComponents => Array.Empty<Type>();

    /// <summary>
    /// Requires a specific type of component from the parent ComponentBehaviour.
    /// This method checks if the component exists and returns it if found.
    /// </summary>
    protected T Require<T>() where T : BaseComponent<TBehaviour>
    {
        if (_context == null)
        {
            Debug.LogError($"{GetType().Name} tried to require {typeof(T).Name}, but it has no parent ComponentBehaviour (on {gameObject.name}).");
            return null;
        }

        if (_behaviour.TryGetRequiredComponent(out T result))
            return result;

        Debug.LogError($"{GetType().Name} require {typeof(T).Name}, but it was not found.");
        return null;
    }

    /// <summary>
    /// Lifecycle method called when the component is started.
    /// This is where initialization logic should be placed.
    /// </summary>
    public virtual void OnStart() { }

    /// <summary>
    /// Lifecycle method called on each frame update.
    /// This is where per-frame logic should be placed.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Lifecycle method called on each physics update.
    /// This is where physics-related logic should be placed.
    /// </summary>
    public virtual void OnPhysicsUpdate() { }

    /// <summary>
    /// Lifecycle method called after all Update methods have been called.
    /// This is where logic that needs to run after all updates should be placed.
    /// </summary>
    public virtual void OnLateUpdate() { }

    /// <summary>
    /// Lifecycle method called when the component is enabled or instantiated.
    /// This is where the component should register itself with its parent ComponentBehaviour.
    /// </summary>
    private void OnEnable() => SelfAdd();

    /// <summary>
    /// Lifecycle method called when the component is disabled or destroyed.
    /// This is where the component should unregister itself from its parent ComponentBehaviour.
    /// </summary>
    private void OnDisable() => SelfRemove();

    /// <summary>
    /// Adds this component to the parent ComponentBehaviour if it exists.
    /// If the component is already part of a ComponentBehaviour, it will not be added again.
    /// </summary>
    private void SelfAdd()
    {
        if (!HasComponentBehaviour)
        {
            _context = GetComponentInParent<TBehaviour>();
            _behaviour = _context as ComponentBehaviour<TBehaviour>;
        }

        if (!_behaviour.AddComponent(this))
        {
            Debug.LogError($"Failed to add {name} to {nameof(ComponentBehaviour<TBehaviour>)}.");
        }
    }

    /// <summary>
    /// Removes this component from the parent ComponentBehaviour if it exists.
    /// This is called when the component is disabled or destroyed.
    /// </summary>
    private void SelfRemove()
    {
        if (HasComponentBehaviour)
        {
            if (!_behaviour.RemoveComponent(this))
            {
                Debug.LogError($"Failed to remove {name} from {nameof(ComponentBehaviour<TBehaviour>)}.");
            }
        }
    }
}

/// <summary>
/// BaseComponent serves as a base class for components that can be added to a ComponentBehaviour.
/// It provides lifecycle methods and manages its association with a ComponentBehaviour.
/// </summary>
/// <typeparam name="TBehaviour"></typeparam>
/// <typeparam name="TData"></typeparam>

[DisallowMultipleComponent]
public abstract class BaseComponent<TBehaviour, TData> : BaseComponent<TBehaviour> where TData : ScriptableObject
{
    /// <summary>
    /// Stores the data associated with this BaseComponent.
    /// This is used to manage the component's data within the context of a ComponentBehaviour.
    /// </summary>
    [SerializeField]
    protected TData data;

    /// <summary>
    /// Sets the data for this BaseComponent.
    /// This method is used to initialize or update the data associated with this component.
    /// </summary>
    /// <param name="data"></param>
    public void SetData(TData data) => this.data = data;

    /// <summary>
    /// Gets the data associated with this BaseComponent.
    /// This method is used to retrieve the data that this component operates on.
    /// </summary>
    /// <returns></returns>
    public TData GetData() => data;
}