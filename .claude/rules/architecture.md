# Unity Engineering Architecture & Performance Optimization

## 1. Decoupled Architecture & ScriptableObjects
- **Data-Driven Architecture:** Favor ScriptableObjects for configuration data, game variables, runtime event networks, and global channel routing. Decouple data from the visual GameObject scene hierarchy wherever possible.
- **Assembly Boundaries (`.asmdef`):** Enforce clean physical structural divisions by segmenting subsystems (e.g., Core Systems, Audio Framework, UI Components, AI Systems) into isolated Assembly Definitions. This stops runtime domain bloating and cuts code recompilation loops.
- **Interface Dependency Injection:** Minimize naked, tightly coupled type coupling. Program against abstraction layers, interfaces, or use explicit light dependency injection systems to manage initialization orders cleanly.

## 2. Memory Optimization & Garbage Collection (GC) Prevention
- **Zero Update Allocs:** Banish all heap allocation logic from high-frequency lifecycle execution sweeps (`Update()`, `FixedUpdate()`, `LateUpdate()`). 
- **The Execution Ban List:** Completely ban these high-frequency execution patterns:
  - Do not call `GetComponent<T>()` inside updates. Cache component references locally within `Awake()` or `Start()`.
  - Avoid raw string concatenations inside UI loops; utilize pre-allocated string builders or localized text tokens.
  - Ban `instantiate` or `destroy` actions for high-frequency runtime objects. Implement centralized **Object Pooling** systems for game entities, particle parameters, and physics assets.
- **Physics Query Optimization:** All raycasting or spatial volume searches must use non-allocating collection structures (e.g., use `Physics.RaycastNonAlloc` instead of `Physics.RaycastAll`).

## 3. Asset Pipeline & Resource Loading Boundaries
- **Explicit Asset Management:** Avoid utilizing the legacy `Resources/` folder layout pattern due to startup runtime memory penalties. Utilize the **Addressable Asset System** for asset retrieval, instantiation, and memory release tracking.
- **Resource Cleanup:** Ensure all dynamically allocated asset handlers, asset pointers, and texture maps are explicitly unloaded via lifecycle teardown hooks to eliminate system memory leaks.