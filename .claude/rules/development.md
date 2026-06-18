# C# Coding Conventions & Unity Runtime Practices

## 1. Code Style & Inspector Visibility Rules
- **Encapsulation Default:** Keep all class properties restricted via private visibility modifiers by default. Use explicit serialized backing properties for Inspector exposure:
```csharp
  [SerializeField] private int _playerHealth = 100;
  public int PlayerHealth => _playerHealth;