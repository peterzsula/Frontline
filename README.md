# Frontline

A Unity-based 10v10 competitive tank game inspired by World of Tanks, featuring skill-based matchmaking and no grinding mechanics.

## Game Overview

Frontline is a prototype for a competitive tank combat game where skill matters more than time invested. Players engage in tactical 10v10 battles with realistic tank physics and strategic gameplay.

## Key Features

### Core Gameplay
- **Tank Combat**: Realistic tank movement, rotation, and shooting mechanics
- **Health System**: Damage-based combat with visual feedback
- **Projectile Physics**: Realistic shell ballistics and impact effects
- **AI Opponents**: Intelligent enemy tanks for testing and practice

### Matchmaking System
- **Skill-Based Matching**: Players matched by skill rating, not time played
- **Balanced Teams**: Automatic team balancing based on player performance
- **No Grinding**: Focus on skill improvement rather than unlocking content
- **Dynamic Wait Times**: Expanding skill ranges for faster queue times

### Game Management
- **Match Flow**: Complete game state management from start to finish
- **Win Conditions**: Team elimination and time-based victory conditions
- **Real-time Stats**: Live tracking of team status and match timer

## Technical Implementation

### Scripts Structure
```
Assets/Scripts/
├── Tank/
│   ├── TankController.cs      # Player movement and input
│   ├── TankShooting.cs        # Weapon systems and ammo
│   ├── TankHealth.cs          # Health and damage management
│   ├── AITankController.cs    # AI behavior and tactics
│   └── Projectile.cs          # Shell physics and damage
├── GameManagement/
│   ├── GameManager.cs         # Match flow and win conditions
│   ├── MatchmakingSystem.cs   # Skill-based player matching
│   └── CameraController.cs    # Third-person camera system
└── UI/
    └── GameUI.cs              # HUD and user interface
```

### Game Systems

#### Tank Physics
- Rigidbody-based movement with realistic constraints
- Separate turret and hull rotation
- Physics-based projectile system

#### Combat Mechanics
- Damage-based health system
- Ammunition management with reload mechanics
- Audio-visual feedback for all combat actions

#### AI System
- State-based AI (Wandering, Chasing, Attacking)
- Dynamic target acquisition and engagement
- Tactical movement and positioning

#### Matchmaking Algorithm
- Skill rating system based on wins/losses and performance
- Team balancing to ensure fair matches
- Adaptive skill ranges based on queue wait times

## Getting Started

### Requirements
- Unity 2022.3.0f1 or later
- Basic understanding of Unity physics and scripting

### Setup
1. Open the project in Unity
2. Load the main game scene
3. Configure spawn points for teams
4. Assign tank prefabs to the GameManager
5. Set up UI elements and camera

### Controls
- **WASD**: Tank movement and rotation
- **Mouse**: Camera control
- **Left Click**: Fire main gun
- **R**: Manual reload
- **Escape**: Toggle cursor lock

## Future Development

### Planned Features
- **Multiplayer Networking**: Full 10v10 online matches
- **Map Variety**: Multiple battle arenas with unique layouts
- **Tank Customization**: Visual customization without gameplay advantages
- **Spectator Mode**: Watch ongoing matches
- **Replay System**: Review and analyze past battles
- **Tournament Mode**: Organized competitive events

### Technical Improvements
- **Optimization**: Better performance for large-scale battles
- **Anti-Cheat**: Server-side validation and security
- **Ladder System**: Seasonal rankings and progression
- **Analytics**: Match data collection and analysis

## Architecture Notes

This prototype focuses on core gameplay mechanics and matchmaking concepts. It demonstrates:

1. **Skill-based progression** rather than time-based grinding
2. **Balanced competitive gameplay** through algorithmic team matching
3. **Modular code structure** for easy expansion and maintenance
4. **Realistic physics simulation** for authentic tank combat

The matchmaking system uses a dynamic skill rating that adjusts based on both wins/losses and individual performance, ensuring players are matched with opponents of similar ability regardless of time invested in the game.

## Contributing

This is a prototype project demonstrating game design concepts. The codebase is structured to be easily extensible for full production development.