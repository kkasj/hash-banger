# peer2peer

## Running the project
1. Run the server (different repo) with `dotnet run`
2. Run the problem-creating node with `dotnet run <port> 1`
3. Run as many nodes as you wish with `dotnet run <port> 0` 

## Generating documentation

### Requirements 
1. Install mermaid extension in vscode
2. Make sure you have access to docfx command line tool

### Steps
1. docfx docfx.json --serve
2. Open the browser and navigate to http://localhost:8080
