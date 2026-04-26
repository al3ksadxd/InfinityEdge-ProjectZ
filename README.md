# Project Z | Advanced Gameplay & Physics Prototype

This repository showcases high-performance gameplay systems and technical art developed for a AAA-inspired railway environment. The project focuses on solving complex synchronization issues between character controllers and high-speed moving platforms.

## 🚀 Key Technical Showcases

### 1. Moving Platform Locomotion (Physics-Based)
* **The Challenge:** Standard Unity CharacterControllers jitter or fail when parented to fast-moving objects.
* **The Solution:** Implemented a custom kinematic reconciliation system that ensures pixel-perfect player stability on trains moving at high velocities.
* **Features:** Seamless transition between world-space and local-platform space.

### 2. Procedural Weapon Framework
* **Physics-Driven Recoil:** Weapon kickback and sway are calculated procedurally, allowing for "Tarkov-style" realistic weapon feel.
* **State-Machine Logic:** Robust handling of ADS (Aim Down Sights), reloading, and ammunition states using C# delegates and ScriptableObjects.

### 3. Technical Art & Optimization
* **Custom Assets:** 100% of 3D models (Train stations, weapons, environment) were modeled in **Blender** with optimized topology for real-time rendering.
* **Performance:** Utilizing manual LOD (Level of Detail) management and custom shaders to maintain high FPS in dense environments.

## 🛠 Tech Stack
* **Engine:** Unity 2022.x+ (URP/HDRP)
* **Language:** C# (Advanced OOP, Task-based logic)
* **3D Modeling:** Blender
* **Version Control:** Git / Git LFS

## 📁 Repository Structure
* `/Assets/_Game/Scripts` - Core logic, physics controllers, and weapon systems.
* `/Assets/_Game/_Environment` - Optimized 3D models and materials.
* `/ProjectSettings` - Custom physics and input mapping.

---
*Developed by Aleksa @ Infinity Edge Studio*
