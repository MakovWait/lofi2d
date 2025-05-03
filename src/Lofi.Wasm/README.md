https://github.com/Kiriller12/RaylibWasm

```
dotnet publish -c Debug
// for WebGL 1.0
dotnet publish -c Debug -p:UseWebGL2=false 

dotnet serve --directory /path/to/AppBundle
```

---

```
dotnet publish -c Debug; dotnet serve --directory ./bin/Debug/net9.0/browser-wasm/AppBundle/ --port 8080
dotnet publish -c Release; dotnet serve --directory ./bin/Release/net9.0/browser-wasm/AppBundle/ --port 8080
```
