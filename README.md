# Modular Behaviour System for Unity

Welcome to the Modular Behaviour System! This system is designed to help you write cleaner, more organized, and scalable code in Unity. It's built around the idea of creating complex GameObjects by combining smaller, reusable components. Instead of having one massive script, you can break down functionalities into modular parts that work together.

This approach not only makes your code easier to manage but also promotes reusability and simplifies debugging.

## Core Concepts

The system is built upon two primary classes: `ModularBehaviour<T>` and `ModularComponent<T>`.

### `ModularBehaviour<T>`
This is the central controller for your GameObject. Think of it as the "brain" of the operation. It manages a collection of `ModularComponent` instances and is responsible for calling their lifecycle methods (`OnStart`, `OnUpdate`, etc.). Any class that will manage components should inherit from this one.

### `ModularComponent<T>`
These are the building blocks of your GameObject's logic. Each component handles a specific piece of functionality (e.g., movement, health, attacks). They are attached to the same GameObject as a `ModularBehaviour` and automatically register themselves with it. Components can also depend on and communicate with other components.

## How to Use

### 1. Create a Behaviour Class
First, create a class that will act as the main controller for your components. This class must inherit from `ModularBehaviour<T>`, where `T` is the class itself.

**Example:** Let's create a `Player` class.

```csharp
// Player.cs
public class Player : ModularBehaviour<Player>
{
    // You can add properties or methods specific to the Player here.
    // For instance, a reference to player-wide data or states.
}
```

### 2. Create Component Classes
Next, create the modular components. These will contain the actual logic. Each component must inherit from `ModularComponent<T>`, where `T` is your behaviour class (`Player` in this case).

**Example:** A `PlayerMovement` component.

```csharp
// PlayerMovement.cs
using UnityEngine;

public class PlayerMovement : ModularComponent<Player>
{
    public float speed = 5.0f;

    public override void OnUpdate()
    {
        // This component only handles movement logic.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        transform.parent.Translate(movement * speed * Time.deltaTime);
    }
}
```

### 3. (Optional) Create Components with Data (`ScriptableObject`)
For components that need configuration data, you can use the `ModularComponent<TBehaviour, TData>` class. This allows you to link a `ScriptableObject` to your component, making it easy to manage and reuse settings.

**Example:** A `PlayerHealth` component with a `HealthData` ScriptableObject.

**First, the data container:**
```csharp
// HealthData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "HealthData", menuName = "Player/Health Data")]
public class HealthData : ScriptableObject
{
    [Tooltip("The maximum health of the character.")]
    public float maxHealth = 100f;

    [Tooltip("The starting health of the character.")]
    public float startingHealth = 100f;
}
```

**Now, the component:**
```csharp
// PlayerHealth.cs
using UnityEngine;

public class PlayerHealth : ModularComponent<Player, HealthData>
{
    private float _currentHealth;

    public override void OnStart()
    {
        // Access data from the ScriptableObject.
        _currentHealth = data.startingHealth;
        Debug.Log($"Player health initialized with {_currentHealth} HP.");
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. Current health: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Debug.Log("Player has been defeated!");
        }
    }
}
```

### 4. Assembling in Unity
1.  Create an empty GameObject in your scene and name it "Player".
2.  Attach the `Player.cs` script to it. This is your `ModularBehaviour`.
3.  Create another GameObject as a child of "Player" and name it "MovementComponent".
4.  Attach the `PlayerMovement.cs` script to the "MovementComponent" GameObject.
5.  Create another child GameObject named "HealthComponent" and attach `PlayerHealth.cs` to it.
6.  In the Project window, create a `HealthData` ScriptableObject and drag it into the `Data` field of the `PlayerHealth` component in the Inspector.

Now, when you run the scene, the `PlayerMovement` and `PlayerHealth` components will automatically find and register with the `Player` behaviour. The `Player` script will then call their `OnUpdate`, `OnStart`, etc., methods.

## Full Example: Component Communication

Components can communicate with each other using the `Require<T>()` method. This is useful for creating dependencies.

Let's create a `PlayerAttack` component that needs to know about the player's health to perform a special move when health is low.

```csharp
// PlayerAttack.cs
using UnityEngine;

public class PlayerAttack : ModularComponent<Player>
{
    private PlayerHealth _playerHealth;

    public override void OnStart()
    {
        // Get a reference to another component.
        _playerHealth = Require<PlayerHealth>();
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (_playerHealth == null) return;

        // Example: Check health to perform a stronger attack.
        if (_playerHealth.GetData().currentHealth < 25)
        {
            Debug.Log("Desperation Attack! (Extra Damage)");
        }
        else
        {
            Debug.Log("Normal Attack!");
        }
    }
}
```

In this example, `PlayerAttack` safely gets a reference to `PlayerHealth`. The system ensures that dependencies are met and throws clear errors if a required component is missing.

---

## Scripting API

### `ModularBehaviour<TBehaviour>`
Base class for objects that manage a group of `ModularComponent`s.

| Method | Return Type | Description |
| --- | --- | --- |
| `AddComponent(component)` | `bool` | Adds a `ModularComponent` to the internal registry. Called automatically by components. |
| `RemoveComponent(component)` | `bool` | Removes a `ModularComponent` from the registry. Called automatically by components. |
| `TryGetRequiredComponent<T>(out T)`| `bool` | Attempts to retrieve an active component of type `T`. |

### `ModularComponent<TBehaviour>`
Base class for a functional module.

| Property / Method | Type / Return Type | Description |
| --- | --- | --- |
| `Context` | `TBehaviour` | Gets the parent `ModularBehaviour` instance. |
| `Behaviour` | `ModularBehaviour<TBehaviour>` | Gets the parent `ModularBehaviour` instance. |
| `RequiredComponents` | `Type[]` | An array of component types that this component depends on. Override to define dependencies. |
| `Require<T>()` | `T` | Retrieves a required component of type `T` from the parent `ModularBehaviour`. |
| `OnStart()` | `void` | Called once on initialization. |
| `OnUpdate()` | `void` | Called every frame. |
| `OnPhysicsUpdate()` | `void` | Called every fixed physics step. |
| `OnLateUpdate()` | `void` | Called every frame after all `OnUpdate` calls. |

### `ModularComponent<TBehaviour, TData>`
An extension of `ModularComponent` that includes a reference to a `ScriptableObject` for data.

| Property / Method | Type / Return Type | Description |
| --- | --- | --- |
| `data` | `TData` | The `ScriptableObject` instance containing the component's data. |
| `SetData(data)` | `void` | Sets the data for the component. |
| `GetData()` | `TData` | Gets the data associated with the component. |

# Sistema de Comportamento Modular para Unity

Bem-vindo ao Sistema de Comportamento Modular! Este sistema foi projetado para ajudá-lo a escrever código mais limpo, organizado e escalável na Unity. Ele foi construído em torno da ideia de criar GameObjects complexos combinando componentes menores e reutilizáveis. Em vez de ter um único script gigante, você pode dividir as funcionalidades em partes modulares que trabalham juntas.

Essa abordagem não apenas facilita o gerenciamento do seu código, mas também promove a reutilização e simplifica a depuração.

## Conceitos Principais

O sistema é construído sobre duas classes primárias: `ModularBehaviour<T>` e `ModularComponent<T>`.

### `ModularBehaviour<T>`
Este é o controlador central do seu GameObject. Pense nele como o "cérebro" da operação. Ele gerencia uma coleção de instâncias de `ModularComponent` e é responsável por chamar seus métodos de ciclo de vida (`OnStart`, `OnUpdate`, etc.). Qualquer classe que irá gerenciar componentes deve herdar desta.

### `ModularComponent<T>`
Estes são os blocos de construção da lógica do seu GameObject. Cada componente lida com uma funcionalidade específica (por exemplo, movimento, vida, ataques). Eles são anexados ao mesmo GameObject que um `ModularBehaviour` e se registram automaticamente com ele. Os componentes também podem depender e se comunicar com outros componentes.

## Como Usar

### 1. Crie uma Classe de Comportamento (Behaviour)
Primeiro, crie uma classe que atuará como o controlador principal para seus componentes. Esta classe deve herdar de `ModularBehaviour<T>`, onde `T` é a própria classe.

**Exemplo:** Vamos criar uma classe `Player`.

```csharp
// Player.cs
public class Player : ModularBehaviour<Player>
{
    // Você pode adicionar propriedades ou métodos específicos do Player aqui.
    // Por exemplo, uma referência a dados ou estados gerais do jogador.
}
```

### 2. Crie Classes de Componentes
Em seguida, crie os componentes modulares. Eles conterão a lógica real. Cada componente deve herdar de `ModularComponent<T>`, onde `T` é a sua classe de comportamento (`Player` neste caso).

**Exemplo:** Um componente `PlayerMovement`.

```csharp
// PlayerMovement.cs
using UnityEngine;

public class PlayerMovement : ModularComponent<Player>
{
    public float speed = 5.0f;

    public override void OnUpdate()
    {
        // Este componente lida apenas com a lógica de movimento.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        transform.parent.Translate(movement * speed * Time.deltaTime);
    }
}
```

### 3. (Opcional) Crie Componentes com Dados (`ScriptableObject`)
Para componentes que precisam de dados de configuração, você pode usar a classe `ModularComponent<TBehaviour, TData>`. Isso permite que você vincule um `ScriptableObject` ao seu componente, facilitando o gerenciamento e a reutilização de configurações.

**Exemplo:** Um componente `PlayerHealth` com um ScriptableObject `HealthData`.

**Primeiro, o contêiner de dados:**
```csharp
// HealthData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "HealthData", menuName = "Player/Health Data")]
public class HealthData : ScriptableObject
{
    [Tooltip("A vida máxima do personagem.")]
    public float maxHealth = 100f;

    [Tooltip("A vida inicial do personagem.")]
    public float startingHealth = 100f;
}
```

**Agora, o componente:**
```csharp
// PlayerHealth.cs
using UnityEngine;

public class PlayerHealth : ModularComponent<Player, HealthData>
{
    private float _currentHealth;

    public override void OnStart()
    {
        // Acessa os dados do ScriptableObject.
        _currentHealth = data.startingHealth;
        Debug.Log($"Vida do jogador inicializada com {_currentHealth} HP.");
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        Debug.Log($"O jogador sofreu {amount} de dano. Vida atual: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Debug.Log("O jogador foi derrotado!");
        }
    }
}
```

### 4. Montagem na Unity
1.  Crie um GameObject vazio na sua cena e nomeie-o como "Player".
2.  Anexe o script `Player.cs` a ele. Este é o seu `ModularBehaviour`.
3.  Crie outro GameObject como filho de "Player" e nomeie-o como "MovementComponent".
4.  Anexe o script `PlayerMovement.cs` ao GameObject "MovementComponent".
5.  Crie outro GameObject filho chamado "HealthComponent" e anexe `PlayerHealth.cs` a ele.
6.  Na janela do Projeto, crie um `HealthData` ScriptableObject e arraste-o para o campo `Data` do componente `PlayerHealth` no Inspector.

Agora, quando você executar a cena, os componentes `PlayerMovement` e `PlayerHealth` encontrarão e se registrarão automaticamente com o comportamento `Player`. O script `Player` então chamará seus métodos `OnUpdate`, `OnStart`, etc.

## Exemplo Completo: Comunicação entre Componentes

Componentes podem se comunicar uns com os outros usando o método `Require<T>()`. Isso é útil para criar dependências.

Vamos criar um componente `PlayerAttack` que precisa saber sobre a vida do jogador para realizar um movimento especial quando a vida estiver baixa.

```csharp
// PlayerAttack.cs
using UnityEngine;

public class PlayerAttack : ModularComponent<Player>
{
    private PlayerHealth _playerHealth;

    public override void OnStart()
    {
        // Obtém uma referência a outro componente.
        _playerHealth = Require<PlayerHealth>();
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (_playerHealth == null) return;

        // Exemplo: Verifica a vida para realizar um ataque mais forte.
        if (_playerHealth.GetData().currentHealth < 25)
        {
            Debug.Log("Ataque de Desespero! (Dano Extra)");
        }
        else
        {
            Debug.Log("Ataque Normal!");
        }
    }
}
```

Neste exemplo, `PlayerAttack` obtém com segurança uma referência a `PlayerHealth`. O sistema garante que as dependências sejam atendidas e lança erros claros se um componente necessário estiver faltando.

---

## Scripting API

### `ModularBehaviour<TBehaviour>`
Classe base para objetos que gerenciam um grupo de `ModularComponent`s.

| Método | Tipo de Retorno | Descrição |
| --- | --- | --- |
| `AddComponent(component)` | `bool` | Adiciona um `ModularComponent` ao registro interno. Chamado automaticamente pelos componentes. |
| `RemoveComponent(component)` | `bool` | Remove um `ModularComponent` do registro. Chamado automaticamente pelos componentes. |
| `TryGetRequiredComponent<T>(out T)`| `bool` | Tenta recuperar um componente ativo do tipo `T`. |

### `ModularComponent<TBehaviour>`
Classe base para um módulo funcional.

| Propriedade / Método | Tipo / Tipo de Retorno | Descrição |
| --- | --- | --- |
| `Context` | `TBehaviour` | Obtém a instância do `ModularBehaviour` pai. |
| `Behaviour` | `ModularBehaviour<TBehaviour>` | Obtém a instância do `ModularBehaviour` pai. |
| `RequiredComponents` | `Type[]` | Um array de tipos de componentes dos quais este componente depende. Sobrescreva para definir dependências. |
| `Require<T>()` | `T` | Recupera um componente necessário do tipo `T` do `ModularBehaviour` pai. |
| `OnStart()` | `void` | Chamado uma vez na inicialização. |
| `OnUpdate()` | `void` | Chamado a cada frame. |
| `OnPhysicsUpdate()` | `void` | Chamado a cada passo fixo de física. |
| `OnLateUpdate()` | `void` | Chamado a cada frame após todas as chamadas `OnUpdate`. |

### `ModularComponent<TBehaviour, TData>`
Uma extensão de `ModularComponent` que inclui uma referência a um `ScriptableObject` para dados.

| Propriedade / Método | Tipo / Tipo de Retorno | Descrição |
| --- | --- | --- |
| `data` | `TData` | A instância do `ScriptableObject` contendo os dados do componente. |
| `SetData(data)` | `void` | Define os dados para o componente. |
| `GetData()` | `TData` | Obtém os dados associados ao componente. |
