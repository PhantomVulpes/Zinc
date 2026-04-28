# API Versioning Guide for Zinc

## Overview
Your API now supports versioning with the following configuration:
- **Current stable version**: v0.7
- **In-development version**: v0.8
- **Default version**: v0.7 (when clients don't specify a version)

## How It Works

### URL Structure
All API endpoints now include the version in the URL:
```
/api/v0.7/projects
/api/v0.8/projects
```

### Current Controller Configuration
All controllers inherit from `ZincController` which has the versioned route: 
```csharp
[Route("api/v{version:apiVersion}/[controller]")]
```

Controllers are marked with `[ApiVersion("0.7")]` to indicate they belong to v0.7.

## How to Maintain v0.7 While Developing v0.8

You have **two strategies** to choose from:

### Strategy 1: Separate Controller Files (Recommended)

Create separate controller files for each version. This is the cleanest approach when versions have significant differences.

#### Step 1: Rename your current controller
```bash
# Rename ProjectController.cs to ProjectControllerV07.cs
mv Api/Controllers/ProjectController.cs Api/Controllers/ProjectControllerV07.cs
```

#### Step 2: Update the class name
```csharp
namespace Vulpes.Zinc.Api.Controllers;

[ApiVersion("0.7")]
public class ProjectControllerV07 : ZincController
{
    // Keep all existing v0.7 endpoints here
    // These remain frozen and unchanged
}
```

#### Step 3: Create v0.8 controller
```csharp
// Api/Controllers/ProjectControllerV08.cs
namespace Vulpes.Zinc.Api.Controllers;

[ApiVersion("0.8")]
public class ProjectControllerV08 : ZincController
{
    // Copy from v0.7 and modify for v0.8
    // Add new features, change responses, etc.
    
    [HttpGet("projects")]
    [ProducesResponseType(typeof(IEnumerable<ProjectV2>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectV2>>> GetAllAccessibleProjectsAsync()
    {
        // New implementation for v0.8
    }
}
```

**Pros**: 
- Complete isolation between versions
- Easy to see what changed
- Safe to modify v0.8 without affecting v0.7

**Cons**: 
- Some code duplication
- Need to maintain multiple files

### Strategy 2: Shared Controller with Multiple Versions

Keep one controller that supports both versions, with version-specific methods.

```csharp
[ApiVersion("0.7")]
[ApiVersion("0.8")]
public class ProjectController : ZincController
{
    // Endpoint available in BOTH v0.7 and v0.8
    [HttpGet("projects")]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Project>>> GetAllAccessibleProjectsAsync()
    {
        // Same implementation for both versions
    }
    
    // Endpoint ONLY in v0.8
    [HttpGet("projects/advanced")]
    [MapToApiVersion("0.8")]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Project>>> GetAdvancedProjectsAsync()
    {
        // New feature in v0.8 only
    }
    
    // Different implementations for different versions
    [HttpGet("projects/{id}/details")]
    [MapToApiVersion("0.7")]
    public async Task<ActionResult<ProjectDetails>> GetProjectDetailsV07(string id)
    {
        // v0.7 implementation
    }
    
    [HttpGet("projects/{id}/details")]
    [MapToApiVersion("0.8")]
    public async Task<ActionResult<ProjectDetailsV2>> GetProjectDetailsV08(string id)
    {
        // v0.8 implementation with enhanced details
    }
}
```

**Pros**: 
- Less code duplication for unchanged endpoints
- Single file to maintain

**Cons**: 
- Can get messy with many version-specific methods
- Risk of accidentally changing v0.7 behavior

## How to Access Different Versions

### Via URL (Recommended)
```bash
# v0.7 endpoints
curl https://localhost:5001/api/v0.7/projects
curl https://localhost:5001/api/v0.7/user/login

# v0.8 endpoints
curl https://localhost:5001/api/v0.8/projects
curl https://localhost:5001/api/v0.8/user/login
```

### Via Header (Alternative)
```bash
curl -H "X-Api-Version: 0.7" https://localhost:5001/api/projects
curl -H "X-Api-Version: 0.8" https://localhost:5001/api/projects
```

### No Version Specified
If clients don't specify a version, they get v0.7 (the default):
```bash
# This will use v0.7
curl https://localhost:5001/api/projects
```

## Swagger Documentation

Swagger UI now shows both versions with separate documentation:
- **v0.7**: http://localhost:5001/swagger/index.html (select "Zinc API v0.7")
- **v0.8**: http://localhost:5001/swagger/index.html (select "Zinc API v0.8")

Use the dropdown at the top-right of Swagger UI to switch between versions.

## Best Practices

### 1. Keep v0.7 Frozen
Once you release v0.7, avoid making changes to it. All new features and breaking changes go to v0.8.

### 2. Versioning Breaking Changes Only
You only need a new version when making **breaking changes**:
- Removing endpoints
- Changing request/response structures
- Changing endpoint behavior significantly

**Non-breaking changes** don't need a new version:
- Adding new optional parameters
- Adding new endpoints
- Adding new fields to responses (if clients ignore unknown fields)

### 3. Document Changes
Keep a CHANGELOG.md noting what changed between versions.

### 4. Deprecation Strategy
When ready to sunset v0.7:

```csharp
[ApiVersion("0.7", Deprecated = true)]
public class ProjectControllerV07 : ZincController
{
    // Marks this version as deprecated in Swagger
}
```

### 5. Testing
Test both versions:
```bash
# Test v0.7 still works
curl https://localhost:5001/api/v0.7/projects

# Test v0.8 new features
curl https://localhost:5001/api/v0.8/projects
```

## Example: Adding a New Feature to v0.8

Let's say you want to add pagination to the projects endpoint in v0.8 only:

```csharp
// Api/Controllers/ProjectControllerV08.cs
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Vulpes.Zinc.Api.Controllers;

[ApiVersion("0.8")]
public class ProjectControllerV08 : ZincController
{
    [HttpGet("projects")]
    [ProducesResponseType(typeof(PaginatedResult<Project>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<Project>>> GetAllAccessibleProjectsAsync(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        // New paginated implementation
        var projects = await mediator.RequestResponseAsync(
            new GetAllAccessibleProjectsPaginatedQuery(RegisteredUser.Key, page, pageSize));
        return Ok(projects);
    }
}
```

v0.7 continues to work exactly as before without pagination, while v0.8 clients can use the new paginated endpoint.

## Updating Your Frontend

Update your API client to use versioned endpoints:
```typescript
// For v0.7
const projects = await fetch('/api/v0.7/projects');

// For v0.8
const projects = await fetch('/api/v0.8/projects?page=1&pageSize=20');
```

## Questions?

- **Should I version every controller?** Yes, add `[ApiVersion("0.7")]` to all controllers initially.
- **Can I have different versions for different controllers?** Yes, but it's confusing. Keep all endpoints at the same version.
- **What about the Core/Infrastructure layers?** Versioning is API-level only. Your domain models may need separate V2 classes if response shapes change significantly.
