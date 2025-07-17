# WorkflowEngine

A minimal, in-memory workflow engine built with .NET 8 Minimal APIs. This project provides a basic but functional state machine that allows you to define workflows, create instances of them, and transition them through various states by executing actions.

## Core Concepts

The engine is built around a few key concepts:

*   **Workflow Definition**: The blueprint or template for a workflow. It consists of a collection of possible states and the actions that can transition between them.
*   **State**: A specific status or step within a workflow. A state can be an `initial` starting point or a `final` ending point.
*   **Action (Transition)**: A defined operation that moves a workflow instance from a set of allowed source states (`FromStates`) to a single target state (`ToState`).
*   **Workflow Instance**: A live, running version of a a Workflow Definition. Each instance has its own ID, tracks its `CurrentState`, and maintains a `History` of the actions that have been applied to it.

## Features

*   **RESTful API**: Manage workflows via a clean HTTP API.
*   **Dynamic Definitions**: Define and register new workflows at runtime.
*   **State Management**: Create and track individual instances of each workflow definition.
*   **Validation**: Built-in validation ensures that workflow definitions are logical (e.g., have one initial state) and that transitions are valid (e.g., an action can only be run from a permitted state).
*   **In-Memory Storage**: Simple, dependency-free persistence for definitions and instances (data is lost on restart).
*   **Swagger/OpenAPI**: Comes with a built-in Swagger UI for easy API exploration and testing.

## Prerequisites

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Getting Started

1.  **Create the Project Files**: Ensure all the files from the prompt are created with the correct folder structure.
2.  **Open a terminal** in the root `WorkflowEngine` folder.
3.  **Restore dependencies**:
    ```
    dotnet restore
    ```
4.  **Run the application**:
    ```
    dotnet run
    ```
    Alternatively, open the folder in Visual Studio or VS Code and press F5.
5.  **Access the API**: The application will start and listen on a local port (e.g., `https://localhost:7123`). The console output will show the exact URL.

    Open your browser and navigate to the Swagger UI to interact with the API:
    **`https://localhost:XXXX/swagger`**

## API Endpoints

### Workflow Definitions

#### `POST /workflows`
Creates a new workflow definition.

*   **Request Body Example**: A `WorkflowDefinition` JSON object.

    ```
    {
      "id": "f2e1a3b4-8b9c-4d7e-af8f-1e9a2b3c4d5e",
      "name": "Document Approval Workflow",
      "states": [
        { "id": "1a1a1a1a-0000-0000-0000-000000000001", "name": "Draft", "isInitial": true },
        { "id": "1a1a1a1a-0000-0000-0000-000000000002", "name": "In Review" },
        { "id": "1a1a1a1a-0000-0000-0000-000000000003", "name": "Approved", "isFinal": true },
        { "id": "1a1a1a1a-0000-0000-0000-000000000004", "name": "Rejected", "isFinal": true }
      ],
      "actions": [
        {
          "id": "2b2b2b2b-0000-0000-0000-000000000001",
          "name": "Submit for Review",
          "fromStates": ["1a1a1a1a-0000-0000-0000-000000000001"],
          "toState": "1a1a1a1a-0000-0000-0000-000000000002"
        },
        {
          "id": "2b2b2b2b-0000-0000-0000-000000000002",
          "name": "Approve",
          "fromStates": ["1a1a1a1a-0000-0000-0000-000000000002"],
          "toState": "1a1a1a1a-0000-0000-0000-000000000003"
        },
        {
          "id": "2b2b2b2b-0000-0000-0000-000000000003",
          "name": "Reject",
          "fromStates": ["1a1a1a1a-0000-0000-0000-000000000002"],
          "toState": "1a1a1a1a-0000-0000-0000-000000000004"
        }
      ]
    }
    ```
*   **Success Response**: `201 Created` with the definition object.
*   **Error Response**: `400 Bad Request` if the definition is invalid (e.g., no initial state, duplicate IDs).

#### `GET /workflows/{defId}`
Retrieves a specific workflow definition by its ID.

*   **Success Response**: `200 OK` with the definition object.
*   **Error Response**: `404 Not Found`.

### Workflow Instances

#### `POST /workflows/{defId}/instances`
Creates a new instance of a specified workflow definition. The instance is automatically placed in the definition's initial state.

*   **Success Response**: `201 Created` with the new instance's ID.
    ```
    {
      "id": "c7d8e9f0-a1b2-c3d4-e5f6-a7b8c9d0e1f2"
    }
    ```
*   **Error Response**: `404 Not Found` if the definition ID does not exist.

#### `GET /instances/{instId}`
Retrieves a specific workflow instance by its ID, showing its current state and history.

*   **Success Response**: `200 OK` with the instance object.
    ```
    {
        "id": "c7d8e9f0-a1b2-c3d4-e5f6-a7b8c9d0e1f2",
        "definitionId": "f2e1a3b4-8b9c-4d7e-af8f-1e9a2b3c4d5e",
        "currentState": "1a1a1a1a-0000-0000-0000-000000000001",
        "history": []
    }
    ```
*   **Error Response**: `404 Not Found`.

#### `POST /instances/{instId}/actions/{actionId}`
Executes an action on a workflow instance, attempting to transition it to a new state. The system validates that the action is valid for the instance's current state.

*   **Success Response**: `200 OK` with the updated instance object.
*   **Error Responses**:
    *   `404 Not Found` if the instance doesn't exist.
    *   `400 Bad Request` if the action is not part of the workflow definition.
    *   `409 Conflict` if the transition is invalid (e.g., action cannot be fired from the current state, action is disabled, or instance is in a final state).
