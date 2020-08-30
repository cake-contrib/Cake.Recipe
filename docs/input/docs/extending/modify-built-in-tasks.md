---
Order: 20
Title: Modify built in tasks
Description: How to modify/replace the built in tasks within Cake.Recipe
---

If there is a task that doesn't work how you need it to you can modify it in your `recipe.cake` file.
Once you have identified the task you wish to modify find the property it is assigned to in the `BuildTasks` class in the `tasks.cake` file. This reference will be needed to modify the task.

You may wish to remove any existing actions before proceeding to give the task a new action.
This can be done with the following code
To clear existing actions from the task call `Tasks.Actions.Clear()` on the reference to the task.

```csharp
// Clear the InspectCode tasks actions
BuildParameters.Tasks.InspectCodeTask.Task.Actions.Clear();
```

New actions, dependencies, criteria, etc. can be added at any time to Cake tasks. You can even have multiple calls to the `.Does(...)` method and each action will be executed.

```csharp
BuildParameters.Tasks.InspectCodeTask
    .IsDependentOn("AnotherTask")
    .Does(() => {
        Information("Do something...");
        ...
    })
    .Does(() => {
        Information("Do something else...");
    });
```

This should allow you to modify any of the tasks to suit your needs.