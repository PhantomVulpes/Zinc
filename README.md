# Zinc

# Hosting on Linux
Set up publish (write this here, Noah)

Copy publish folder to chosen Linux directory.

Run the following command in the terminal:
```
cd path/to/your/published/files
dotnet Vulpes.Zinc.Web.dll
```

Ensure firewall configuration is set up to allow traffic on the port specified in the appsettings.json file.
```
sudo ufw allow 5000/tcp
```

Access the application by navigating to the IP address of the server in a web browser, followed by the port number specified in the appsettings.json file.
```
ip addr show
```