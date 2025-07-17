using Microsoft.OpenApi.Models;
using WorkflowEngine.Domain;
using WorkflowEngine.Persistence;
using WorkflowEngine.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryStore>();          // registers both interfaces
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "State-Machine API", Version = "v1" }));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// ---- Workflow definitions --------------------------------------------------
app.MapPost("/workflows", (InMemoryStore store, WorkflowDefinition def) =>
{
    try
    {
        DefinitionValidator.Validate(def);
        store.SaveDefinition(def);
        return Results.Created($"/workflows/{def.Id}", def);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/workflows/{defId:guid}", (InMemoryStore store, Guid defId) =>
    store.GetDefinition(defId) is { } def ? Results.Ok(def) : Results.NotFound());

// ---- Instances -------------------------------------------------------------
app.MapPost("/workflows/{defId:guid}/instances", (InMemoryStore store, Guid defId) =>
{
    var def = store.GetDefinition(defId);
    if (def is null) return Results.NotFound();

    var inst = new WorkflowInstance(def);
    store.SaveInstance(inst);
    return Results.Created($"/instances/{inst.Id}", new { inst.Id });
});

app.MapGet("/instances/{instId:guid}", (InMemoryStore store, Guid instId) =>
    store.GetInstance(instId) is { } inst ? Results.Ok(inst) : Results.NotFound());

// ---- Execute action --------------------------------------------------------
app.MapPost("/instances/{instId:guid}/actions/{actionId:guid}", (
    InMemoryStore store, Guid instId, Guid actionId) =>
{
    var inst = store.GetInstance(instId);
    if (inst is null) return Results.NotFound(new { error = "Instance not found." });

    var def = store.GetDefinition(inst.DefinitionId)!;
    var act = def.Actions.SingleOrDefault(a => a.Id == actionId);
    if (act is null) return Results.BadRequest(new { error = "Action not in definition." });

    try
    {
        TransitionValidator.Validate(def, inst, act);
        inst.Apply(act.Id, act.ToState);
        return Results.Ok(inst);
    }
    catch (Exception ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
});

app.Run();
