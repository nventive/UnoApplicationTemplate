# AI Agent Architecture - Layer Separation

## Overview

The AI Agent integration follows a clean architecture pattern with proper separation of concerns across three layers:

```
┌─────────────────────────────────────────────────┐
│         Presentation Layer                       │
│  - Navigation Function Registry                  │
│  - ViewModels (AIChatPageViewModel)             │
│  - UI Views (XAML)                               │
│  - Implements navigation-specific functions      │
└──────────────┬──────────────────────────────────┘
               │ Dependencies
               ↓
┌─────────────────────────────────────────────────┐
│         Business Layer                           │
│  - AIAgentToolExecutor (function dispatcher)     │
│  - AIChatService (conversation orchestration)    │
│  - ChatMessage models                            │
│  - NO UI or Navigation dependencies              │
└──────────────┬──────────────────────────────────┘
               │ Dependencies
               ↓
┌─────────────────────────────────────────────────┐
│         Access Layer                             │
│  - AIAgentApiClient (Azure AI communication)     │
│  - AIAgentConfiguration                          │
│  - HTTP communication layer                      │
└─────────────────────────────────────────────────┘
```

## Key Architectural Principle

**The Business layer does NOT depend on the Presentation layer.**

This is achieved through a **delegate/callback pattern**:

1. **Business Layer**: Defines the `AIAgentToolExecutor` which is a **dispatcher** that routes function calls to registered handlers
2. **Presentation Layer**: Implements the actual navigation functions in `AIAgentNavigationFunctionRegistry` and registers them with the executor

## How It Works

### 1. Business Layer - Function Dispatcher

`AIAgentToolExecutor.cs` (Business layer):
```csharp
public class AIAgentToolExecutor : IAIAgentToolExecutor
{
    private readonly Dictionary<string, Func<JsonDocument, CancellationToken, Task<string>>> _functionHandlers;
    
    // Registers a handler (provided by Presentation layer)
    public void RegisterFunctionHandler(string functionName, Func<...> handler)
    {
        _functionHandlers[functionName] = handler;
    }
    
    // Dispatches to the registered handler
    public async Task<string> ExecuteFunctionAsync(string functionName, string args, CancellationToken ct)
    {
        if (_functionHandlers.TryGetValue(functionName, out var handler))
        {
            return await handler(args, ct);
        }
        // Function not found
    }
}
```

### 2. Presentation Layer - Function Implementations

`AIAgentNavigationFunctionRegistry.cs` (Presentation layer):
```csharp
public class AIAgentNavigationFunctionRegistry
{
    private readonly IAIAgentToolExecutor _toolExecutor;
    private readonly ISectionsNavigator _sectionsNavigator;  // UI navigation - only in Presentation!
    
    public void RegisterFunctions()
    {
        // Register navigation handlers with the Business layer executor
        _toolExecutor.RegisterFunctionHandler("navigate_to_page", NavigateToPageAsync);
        _toolExecutor.RegisterFunctionHandler("get_current_page", GetCurrentPageAsync);
        _toolExecutor.RegisterFunctionHandler("go_back", GoBackAsync);
    }
    
    private async Task<string> NavigateToPageAsync(JsonDocument args, CancellationToken ct)
    {
        // Implementation uses ISectionsNavigator (Presentation layer dependency)
        await _sectionsNavigator.SetActiveSection(ct, "Home");
        return AIAgentToolExecutor.ResponseHelpers.Success("Navigated to Home");
    }
}
```

### 3. Startup Registration

`CoreStartup.cs` (Presentation layer):
```csharp
protected override void OnInitialized(IServiceProvider services)
{
    // ... other initialization
    
    // Register AI agent navigation functions
    InitializeAIAgentFunctions(services);
}

private static void InitializeAIAgentFunctions(IServiceProvider services)
{
    var registry = new AIAgentNavigationFunctionRegistry(
        services.GetRequiredService<IAIAgentToolExecutor>(),  // From Business layer
        services.GetRequiredService<ISectionsNavigator>(),     // From Presentation layer
        logger
    );
    
    registry.RegisterFunctions();
}
```

## Benefits of This Architecture

### ✅ **Separation of Concerns**
- Business layer handles function dispatching and conversation flow
- Presentation layer handles UI navigation
- Access layer handles external API communication

### ✅ **Testability**
- Business layer can be tested without UI dependencies
- Mock function handlers can be registered for testing
- Each layer can be tested independently

### ✅ **Extensibility**
- Add new functions by creating a new registry class
- Register custom handlers at any point after startup
- No need to modify Business layer for new UI features

### ✅ **Flexibility**
- Different platforms can provide different implementations
- Business logic remains platform-agnostic
- Easy to add non-navigation functions (data access, etc.)

## Adding Custom Functions

### For Navigation Functions (Presentation Layer)

1. **Add to `AIAgentNavigationFunctionRegistry.cs`**:
```csharp
public void RegisterFunctions()
{
    _toolExecutor.RegisterFunctionHandler("my_navigation_function", MyNavigationFunctionAsync);
}

private async Task<string> MyNavigationFunctionAsync(JsonDocument args, CancellationToken ct)
{
    // Use _sectionsNavigator here
    await _sectionsNavigator.Navigate(ct, () => new MyPageViewModel());
    return AIAgentToolExecutor.ResponseHelpers.Success("Navigated to My Page");
}
```

2. **Register in `AIAgentApiClient.GetAvailableTools()`** (Access layer)

### For Non-Navigation Functions (Business Layer)

For functions that don't require navigation (e.g., data access, calculations):

1. **Create a new registry class** (Business or Presentation layer):
```csharp
public class AIAgentDataFunctionRegistry
{
    private readonly IAIAgentToolExecutor _toolExecutor;
    private readonly IDataService _dataService;
    
    public void RegisterFunctions()
    {
        _toolExecutor.RegisterFunctionHandler("get_user_data", GetUserDataAsync);
    }
    
    private async Task<string> GetUserDataAsync(JsonDocument args, CancellationToken ct)
    {
        var data = await _dataService.GetUserData(ct);
        return JsonSerializer.Serialize(new { success = true, data });
    }
}
```

2. **Register in `CoreStartup.OnInitialized()`**

## Example: Complete Flow

### Navigation Example
```
User: "Navigate to settings"
    ↓
1. AIChatPageViewModel.SendMessage()
    ↓
2. AIChatService.SendMessageAsync()
    ↓
3. AIAgentApiClient.SendChatCompletionAsync()
    → Azure AI Foundry API
    ← Response: { tool_calls: [{ function: "navigate_to_page", args: { page_name: "Settings" } }] }
    ↓
4. AIChatService detects tool call
    ↓
5. AIAgentToolExecutor.ExecuteFunctionAsync("navigate_to_page", args)
    ↓
6. Executor finds registered handler
    ↓
7. AIAgentNavigationFunctionRegistry.NavigateToPageAsync(args)
    ↓
8. ISectionsNavigator.Navigate() [Presentation layer]
    ↓
9. Return success JSON to executor
    ↓
10. Send tool result back to Azure AI
    ↓
11. Get final response: "I've navigated you to Settings"
    ↓
12. Display in UI
```

### Drawing Example
```
User: "Draw a cat"
    ↓
1. Thread is reused (preserves conversation history)
    ↓
2. AIAgentApiClient.SendMessageToAssistantAsync()
    → Azure AI Foundry API
    ← Response: { tool_calls: [{ function: "draw_content", args: { svg_content: "<svg>...</svg>", title: "Cat", description: "..." } }] }
    ↓
3. ExecuteNavigationTool("draw_content", args)
    ↓
4. Raise NavigationRequested event (type: DrawContent)
    ↓
5. AgenticChatPageViewModel.OnNavigationRequested()
    ↓
6. IDrawingModalService.ShowDrawingAsync(svgContent, title, description)
    ↓
7. Navigate to DrawingModalViewModel
    ↓
8. DrawingModalPage loads SVG in WebView2
    ↓
User sees drawing in modal

User: "Make it bigger"
    ↓
1. Same thread reused - AI sees previous SVG in history
    ↓
2. AI modifies previous SVG (increases size)
    ↓
3. New draw_content tool call with modified SVG
    ↓
4. Updated drawing displayed
```

## Anti-Patterns to Avoid

### ❌ **DON'T**: Put navigation code in Business layer
```csharp
// BAD - in Business layer
public class AIAgentToolExecutor
{
    private readonly ISectionsNavigator _navigator;  // ❌ UI dependency in Business!
}
```

### ✅ **DO**: Use the registry pattern
```csharp
// GOOD - in Presentation layer
public class AIAgentNavigationFunctionRegistry
{
    private readonly ISectionsNavigator _navigator;  // ✅ UI dependency in Presentation
}
```

### ❌ **DON'T**: Hardcode ViewModels in Business layer
```csharp
// BAD
await _navigator.Navigate(ct, () => new SettingsPageViewModel());  // ❌ VM in Business
```

### ✅ **DO**: Keep ViewModels in Presentation layer
```csharp
// GOOD - in Presentation layer registry
await _sectionsNavigator.Navigate(ct, () => new SettingsPageViewModel());  // ✅ VM in Presentation
```

## Summary

The AI Agent architecture properly separates concerns:
- **Business Layer**: Orchestrates function calls (dispatcher pattern)
- **Presentation Layer**: Implements UI-specific functions (navigation)
- **Access Layer**: Handles external API communication

This design allows for clean, testable, and maintainable code while keeping the Business layer free from UI dependencies.
