#!/bin/bash

echo "================================================"
echo "NotesApp - Automated Setup Script"
echo "================================================"
echo ""

echo "[1/5] Checking .NET installation..."
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET 9 SDK not found!"
    echo "Please install .NET 9 SDK from: https://dotnet.microsoft.com/download/dotnet/9.0"
    exit 1
fi
dotnet --version
echo ".NET SDK found!"
echo ""

echo "[2/5] Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to restore packages"
    exit 1
fi
echo "Packages restored successfully!"
echo ""

echo "[3/5] Installing EF Core tools..."
dotnet tool install --global dotnet-ef --version 9.0.0
echo ""

echo "[4/5] Creating database migration..."
dotnet ef migrations add InitialCreate
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to create migration"
    exit 1
fi
echo "Migration created successfully!"
echo ""

echo "[5/5] Applying migration to database..."
dotnet ef database update
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to update database"
    exit 1
fi
echo "Database created successfully!"
echo ""

echo "================================================"
echo "Setup completed successfully!"
echo "================================================"
echo ""
echo "To run the application, execute: dotnet run"
echo "Then open browser to: https://localhost:5001"
echo ""
