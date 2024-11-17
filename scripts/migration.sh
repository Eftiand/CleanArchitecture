#!/bin/bash

# Navigate to the project root directory if necessary
# cd /path/to/your/project/root

# Run the EF Core migration command
dotnet ef migrations add "initial" --project src/Infrastructure --startup-project src/Web --output-dir Data/Migrations
