---

# ✨ Modular Component System for Unity ✨

Welcome to a flexible and powerful component system for Unity! If you've ever found yourself lost in a sea of MonoBehavior or struggled with the dreaded "spaghetti code," this system might be the hero you didn't know you needed. 🦸‍♂️

It's designed to transform chaos into clean, organized architecture—think of swapping spaghetti for a well-structured lasagna. 🍝 -> lasagna

---

## 📜 Summary

*   [🇬🇧 English Documentation](#-english-documentation)
*   [🇧🇷 Documentação em Português](#-documentação-em-português)

---

## 🇬🇧 English Documentation

### 🚀 Overview

This repository provides a powerful and flexible component-based architecture for Unity, designed to enhance code organization, reusability, and lifecycle management. The main goal is to allow developers to create modular, independent components that are managed by a central `ComponentBehaviour`.

This `ComponentBehaviour` orchestrates the lifecycle of its components (`OnStart`, `OnUpdate`, etc.), promoting a clean, decoupled, and highly structured development workflow.

### 🏛️ Core Classes

The architecture is built upon three key classes:

---

#### `⚙️ ComponentBehaviour<TBehaviour>`

> The Conductor of the Orchestra 🎼

`ComponentBehaviour<T>` is the cornerstone of this architecture. It acts as a central manager for a collection of `BaseComponent` instances. It's responsible for discovering its components and calling their lifecycle methods in the correct order.

**Key Responsibilities:**
*   **Component Management:** Dynamically adds and removes components from its managed collection.
*   **Lifecycle Orchestration:** Calls `OnStart`, `OnUpdate`, `OnPhysicsUpdate`, and `OnLateUpdate` on all active components.
*   **Service Locator:** Allows components to find and communicate with each other safely.

**How to Use:**
Create a class that inherits from `ComponentBehaviour<T>`, passing itself as the generic type. This tells the manager what kind of components it should be looking for.

**Example:**
```csharp
// Player.cs
// This class is now the central brain for a Player character.
public class Player : ComponentBehaviour<Player>
{
    // Player-specific logic can go here, or you can keep it empty.
    // Its main job is to manage all BaseComponent<Player> components
    // attached to this GameObject or its children.
}```

---

#### `🧱 BaseComponent<TBehaviour>`

> The Hard-Working Lego Brick

`BaseComponent<T>` is an abstract class that your gameplay components will inherit from. It provides a standardized structure with lifecycle methods that are automatically managed by a parent `ComponentBehaviour`.

**Key Features:**
*   **Automatic Registration:** When enabled, a `BaseComponent` automatically finds its `ComponentBehaviour` and registers itself. It also unregisters when disabled, so you don't have components ghosting their manager. 👻
*   **Lifecycle Hooks:** Provides virtual methods (`OnStart`, `OnUpdate`, etc.) to override with your component's logic.
*   **Dependency Injection:** Use `Require<T>()` to safely get a reference to other components managed by the same `ComponentBehaviour`. The system ensures dependencies are met or gives a clear error.

**How to Use:**
Create a class that inherits from `BaseComponent<T>`, where `T` is the type of the `ComponentBehaviour` that will manage it.

**Example:**
```csharp
// PlayerMovement.cs
// This component only cares about moving the player.
public class PlayerMovement : BaseComponent<Player>
{
    private PlayerInput _playerInput;

    public override void OnStart()
    {
        // Ask the manager for the PlayerInput component. No more FindObjectOfType!
        _playerInput = Require<PlayerInput>();
    }

    public override void OnUpdate()
    {
        if (_playerInput != null)
        {
            Vector2 moveDirection = _playerInput.GetMoveDirection();
            transform.Translate(moveDirection * Time.deltaTime * 5f);
        }
    }
}

// PlayerInput.cs
// This component only cares about reading input.
public class PlayerInput : BaseComponent<Player>
{
    public Vector2 GetMoveDirection()
    {
        // Simple input logic.
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
```

---

#### `💾 BaseComponent<TBehaviour, TData>`

> The Data-Driven Specialist 📊

This is a specialized version of `BaseComponent<T>` that includes built-in support for a `ScriptableObject` data container. It's perfect for separating your component's logic from its configuration data.

**Key Features:**
*   **Data-Driven Design:** Includes a `data` field for a specific `ScriptableObject` type (`TData`), making it easy to tweak values in the Inspector without touching the code.
*   **Easy Data Access:** Comes with `SetData()` and `GetData()` methods to manage and retrieve the associated data.

**How to Use:**
1.  Define a `ScriptableObject` to hold your data.
2.  Create a component class that inherits from `BaseComponent<T, TData>`.
3.  Create a data asset in your project and assign it in the Inspector.

**Example:**
```csharp
// PlayerStatsData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Player/Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    public float health = 100f;
    public float mana = 50f;
    public float speed = 5f;
}

// PlayerStats.cs
public class PlayerStats : BaseComponent<Player, PlayerStatsData>
{
    public override void OnStart()
    {
        // Access data directly from the assigned ScriptableObject.
        Debug.Log($"Player starting with {data.health} HP and running at {data.speed} speed!");
    }

    public void TakeDamage(float amount)
    {
        // Don't worry, this is just a simulation. Probably.
        data.health -= amount;
        Debug.Log($"Ouch! Health is now {data.health}");
    }
}
```

### 🚀 Getting Started

1.  **1️⃣ Add the Scripts:** Drop `ComponentBehaviour.cs` and `BaseComponent.cs` into your Unity project's script folder.
2.  **2️⃣ Create the Manager:** Create an empty `GameObject` (e.g., "Player"). Add a new script that inherits from `ComponentBehaviour<T>` (e.g., `Player.cs`).
3.  **3️⃣ Create Components:** Write your gameplay scripts (`PlayerMovement`, `PlayerStats`, etc.) inheriting from `BaseComponent<T>` or `BaseComponent<T, TData>`.
4.  **4️⃣ Attach Components:** Attach your new components to the "Player" `GameObject` or as children of it. The system will find them automatically.
5.  **5️⃣ Hit Play:** Run the scene and watch the magic happen! The `ComponentBehaviour` will manage everything, and your console will show any debug logs you've added.

---

## 🇧🇷 Documentação em Português

### 🚀 Visão Geral

Este repositório fornece uma arquitetura de componentes poderosa e flexível para a Unity, projetada para aprimorar a organização do código, a reutilização e o gerenciamento do ciclo de vida. O objetivo principal é permitir que os desenvolvedores criem componentes modulares e independentes, gerenciados por um `ComponentBehaviour` central.

Este `ComponentBehaviour` orquestra o ciclo de vida de seus componentes (`OnStart`, `OnUpdate`, etc.), promovendo um fluxo de trabalho de desenvolvimento limpo, desacoplado e altamente estruturado.

### 🏛️ Classes Principais

A arquitetura é construída sobre três classes-chave:

---

#### `⚙️ ComponentBehaviour<TBehaviour>`

> O Maestro da Orquestra 🎼

`ComponentBehaviour<T>` é a pedra angular desta arquitetura. Ele atua como um gerenciador central para uma coleção de instâncias de `BaseComponent`. É responsável por descobrir seus componentes e chamar seus métodos de ciclo de vida na ordem correta.

**Principais Responsabilidades:**
*   **Gerenciamento de Componentes:** Adiciona e remove componentes dinamicamente de sua coleção gerenciada.
*   **Orquestração do Ciclo de Vida:** Chama `OnStart`, `OnUpdate`, `OnPhysicsUpdate` e `OnLateUpdate` em todos os componentes ativos.
*   **Localizador de Serviços:** Permite que componentes encontrem e se comuniquem uns com os outros de forma segura.

**Como Usar:**
Crie uma classe que herde de `ComponentBehaviour<T>`, passando ela mesma como o tipo genérico. Isso diz ao gerenciador que tipo de componentes ele deve procurar.

**Exemplo:**
```csharp
// Player.cs
// Esta classe é agora o cérebro central de um personagem Jogador.
public class Player : ComponentBehaviour<Player>
{
    // A lógica específica do jogador pode ir aqui, ou você pode mantê-la vazia.
    // Seu principal trabalho é gerenciar todos os componentes BaseComponent<Player>
    // anexados a este GameObject ou a seus filhos.
}
```

---

#### `🧱 BaseComponent<TBehaviour>`

> O Bloco de Lego que Trabalha Duro

`BaseComponent<T>` é uma classe abstrata da qual seus componentes de gameplay herdarão. Ela fornece uma estrutura padronizada com métodos de ciclo de vida que são gerenciados automaticamente por um `ComponentBehaviour` pai.

**Principais Características:**
*   **Registro Automático:** Quando ativado, um `BaseComponent` encontra automaticamente seu `ComponentBehaviour` e se registra. Ele também se desregistra quando desativado, para que você não tenha componentes te dando um perdido (ghosting). 👻
*   **Gatilhos de Ciclo de Vida:** Fornece métodos virtuais (`OnStart`, `OnUpdate`, etc.) para sobrescrever com a lógica do seu componente.
*   **Injeção de Dependência:** Use `Require<T>()` para obter com segurança uma referência a outros componentes gerenciados pelo mesmo `ComponentBehaviour`. O sistema garante que as dependências sejam atendidas ou fornece um erro claro.

**Como Usar:**
Crie uma classe que herde de `BaseComponent<T>`, onde `T` é o tipo do `ComponentBehaviour` que irá gerenciá-la.

**Exemplo:**
```csharp
// PlayerMovement.cs
// Este componente se preocupa apenas em mover o jogador.
public class PlayerMovement : BaseComponent<Player>
{
    private PlayerInput _playerInput;

    public override void OnStart()
    {
        // Pede ao gerenciador pelo componente PlayerInput. Chega de FindObjectOfType!
        _playerInput = Require<PlayerInput>();
    }

    public override void OnUpdate()
    {
        if (_playerInput != null)
        {
            Vector2 moveDirection = _playerInput.GetMoveDirection();
            transform.Translate(moveDirection * Time.deltaTime * 5f);
        }
    }
}

// PlayerInput.cs
// Este componente se preocupa apenas em ler o input.
public class PlayerInput : BaseComponent<Player>
{
    public Vector2 GetMoveDirection()
    {
        // Lógica de input simples.
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
```

---

#### `💾 BaseComponent<TBehaviour, TData>`

> O Especialista Orientado a Dados 📊

Esta é uma versão especializada do `BaseComponent<T>` que inclui suporte integrado para um contêiner de dados `ScriptableObject`. É perfeito para separar a lógica do seu componente de seus dados de configuração.

**Principais Características:**
*   **Design Orientado a Dados:** Inclui um campo `data` para um tipo específico de `ScriptableObject` (`TData`), tornando fácil ajustar valores no Inspector sem tocar no código.
*   **Acesso Fácil a Dados:** Vem com os métodos `SetData()` e `GetData()` para gerenciar e recuperar os dados associados.

**Como Usar:**
1.  Defina um `ScriptableObject` para conter seus dados.
2.  Crie uma classe de componente que herde de `BaseComponent<T, TData>`.
3.  Crie um asset de dados em seu projeto e atribua-o no Inspector.

**Exemplo:**
```csharp
// PlayerStatsData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Player/Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    public float health = 100f;
    public float mana = 50f;
    public float speed = 5f;
}

// PlayerStats.cs
public class PlayerStats : BaseComponent<Player, PlayerStatsData>
{
    public override void OnStart()
    {
        // Acesse os dados diretamente do ScriptableObject atribuído.
        Debug.Log($"Jogador começando com {data.health} HP e correndo a {data.speed} de velocidade!");
    }

    public void TakeDamage(float amount)
    {
        // Não se preocupe, é apenas uma simulação. Provavelmente.
        data.health -= amount;
        Debug.Log($"Ai! A vida agora é {data.health}");
    }
}
```

### 🚀 Como Começar

1.  **1️⃣ Adicione os Scripts:** Jogue os arquivos `ComponentBehaviour.cs` e `BaseComponent.cs` na pasta de scripts do seu projeto Unity.
2.  **2️⃣ Crie o Gerenciador:** Crie um `GameObject` vazio (ex: "Player"). Adicione um novo script que herde de `ComponentBehaviour<T>` (ex: `Player.cs`).
3.  **3️⃣ Crie Componentes:** Escreva seus scripts de gameplay (`PlayerMovement`, `PlayerStats`, etc.) herdando de `BaseComponent<T>` ou `BaseComponent<T, TData>`.
4.  **4️⃣ Anexe os Componentes:** Anexe seus novos componentes ao `GameObject` "Player" ou como filhos dele. O sistema os encontrará automaticamente.
5.  **5️⃣ Aperte o Play:** Execute a cena e veja a mágica acontecer! O `ComponentBehaviour` gerenciará tudo, e seu console mostrará quaisquer logs de depuração que você tenha adicionado.
