# 2D Pixelart RPG

A 2D role-playing game inspired by ClanLord and Arindel, built in C# for Unity. Features quadrant-based exploration, stamina-based combat, and three character classes.

## Game Overview

The player character remains centered on screen while the world moves around them. The game world is divided into 100x100 meter quadrants that transition seamlessly when the player reaches the edges. Combat is initiated by walking into enemies and uses a stamina-based evasion system.

## Features

### Core Gameplay
- **Click-to-Move**: Click anywhere on screen to move your character
- **Centered Camera**: Player stays in the middle, world moves underneath
- **Quadrant System**: Seamless transitions between 100x100m world areas
- **Stamina-Based Combat**: Evasion chances range from 5% to 50% based on stamina

### Character Classes
- **Warrior**: High health (15) and stamina (12), medium magic (5), strong attacks (4 damage)
- **Healer**: Balanced stats (8 HP, 8 stamina, 15 magic), weak attacks (2 damage)
- **Mage**: Low health (6) and stamina (6), high magic (18), medium attacks (3 damage)

### UI System
- Health, Stamina, and Magic bars displayed in upper right corner
- Real-time stat updates
- Color-coded bars (Red=Health, Yellow=Stamina, Blue=Magic)

### Combat System
- Walk into enemies to attack (requires stamina)
- Evasion formula: 5% at 1/10 stamina, 50% at 10/10 stamina
- Each attack consumes 1 stamina point
- Both player and enemies use the same evasion mechanics

### Enemy Types
- **Undead Corps**: 15 HP, 5 stamina, slow movement, 3 damage per hit
- **Skeleton**: 2 HP, 10 stamina, fast movement, 1 damage per hit

### World Design
- **First Quadrant**: Stone circle with central fireplace, grassland, scattered trees
- **Additional Quadrants**: Procedurally placed trees and obstacles
- Green grass background with interactive world objects

## Project Structure

```
Assets/
├── Scripts/
│   ├── Player/
│   │   └── PlayerCharacter.cs      # Main player controller and stats
│   ├── UI/
│   │   └── UIManager.cs           # Health/Stamina/Magic bar management
│   ├── World/
│   │   ├── WorldManager.cs        # Quadrant system and world generation
│   │   └── EnemySpawner.cs       # Enemy placement and spawning
│   ├── Enemies/
│   │   ├── Enemy.cs              # Base enemy class with AI
│   │   ├── UndeadCorps.cs        # Slow, tanky undead enemy
│   │   └── Skeleton.cs           # Fast, fragile skeleton enemy
│   └── GameManager.cs            # Scene setup and game initialization
├── Sprites/                      # Placeholder for sprite assets
├── Prefabs/                      # Placeholder for prefabs
└── Scenes/                       # Unity scene files
RPGGame.cs                        # Main game entry point
```

## Core Scripts

### PlayerCharacter.cs
- Handles player movement, stats, and class-specific attributes
- Implements click-to-move mechanics
- Manages stamina-based evasion calculations
- Singleton pattern for global access

### UIManager.cs
- Updates health, stamina, and magic bars
- Color-coded UI elements
- Real-time stat display

### WorldManager.cs
- Quadrant-based world system
- Seamless transitions between areas
- Procedural world generation
- Stone circle and fireplace creation

### Enemy.cs (Base Class)
- AI behavior for enemy movement and combat
- Attack patterns and damage calculations
- Stamina-based evasion for enemies

### GameManager.cs
- Scene initialization
- UI setup and camera configuration
- Player and world system coordination

## Setup Instructions

1. Create a new Unity 2D project
2. Copy all scripts to your project's `Assets/Scripts/` folder (maintaining the folder structure)
3. Create an empty GameObject in your scene
4. Attach the `RPGGame.cs` script to the GameObject
5. In the inspector, select your preferred starting class
6. Play the game!

## Controls

- **Left Click**: Move character toward mouse position
- **Combat**: Walk into enemies to attack (automatic when in range)

## Game Mechanics Details

### Evasion Formula
```csharp
evasionChance = Lerp(0.05f, 0.5f, currentStamina / maxStamina)
```

### Class Stats
| Class   | Health | Stamina | Magic | Attack Damage |
|---------|--------|---------|-------|---------------|
| Warrior | 15     | 12      | 5     | 4             |
| Healer  | 8      | 8       | 15    | 2             |
| Mage    | 6      | 6       | 18    | 3             |

### Enemy Stats
| Enemy Type   | Health | Stamina | Speed | Attack Damage |
|--------------|--------|---------|-------|---------------|
| Undead Corps | 15     | 5       | 1.0   | 3             |
| Skeleton     | 2      | 10      | 3.0   | 1             |

## Technical Notes

- Built for Unity 2D
- Uses sprite-based rendering
- Implements object pooling concepts for enemies
- Modular script architecture for easy expansion
- No external dependencies required

## Future Expansion Ideas

- Additional character classes
- More enemy types
- Magic system implementation
- Item and inventory system
- Multiple world environments
- Save/load functionality
- Multiplayer support