# 🧩 Modular Component System for Unity

A lightweight, flexible component architecture built on top of Unity’s `MonoBehaviour`. This system allows you to build modular and scalable logic using dynamic component registration, custom lifecycle methods, and safe dependency resolution—all without relying on Unity’s default component structure.

---

## ✨ Features

- ✅ **Custom Lifecycle**  
  Components implement `OnStart()`, `OnUpdate()`, `OnPhysicsUpdate()`, and `OnLateUpdate()` to replace Unity's built-in lifecycle methods.

- ⚡ **Runtime Registration**  
  Each component registers itself to its parent `ComponentBehaviour` automatically via `OnEnable()`.

- 📦 **Dependency Declaration**  
  Components declare dependencies via `RequiredComponents` and access them safely using `Require<T>()`.

- 🚀 **Fast Access**  
  Internal `Dictionary<Type, BaseComponent>` ensures fast and type-safe lookups—much faster than Unity's `GetComponent()`.

- 🧱 **Clean and Decoupled Design**  
  Promotes separation of concerns, making systems more maintainable and testable.

---

## 📁 Structure

### 🔹 `ComponentBehaviour`

A controller that manages multiple active `BaseComponent` instances. It handles registration and executes lifecycle methods on all components each frame.

### 🔹 `BaseComponent`

An abstract class for self-registering logic modules. It defines the lifecycle and dependency access structure for all derived components.

---

## 🧠 Usage Example

```csharp
public class FuelComponent : BaseComponent
{
    public float Fuel = 100f;
}

public class EngineComponent : BaseComponent
{
    protected override Type[] RequiredComponents => new[] { typeof(FuelComponent) };

    public override void OnUpdate()
    {
        var fuel = Require<FuelComponent>();
        if (fuel != null && fuel.Fuel > 0f)
        {
            fuel.Fuel -= Time.deltaTime * 10f;
            Debug.Log("Engine running...");
        }
    }
}
