sc2-replaysync
================

.NET 4.0 Application for assisting in syncing replay positions between casters. Currently only supports
1920x1080 resolution on the main streamers monitor. The application does all communication across UDP Port 11000

**Installation**

No installation is necessary, just run the application. Requires .NET 4.0 Client Profile to be installed.

**Instructions**

**For the main caster/streamer:** Open the application and click listen. If on Windows 7, 
the firewall will ask about opening Port 11000 (the current default port). After this, you should see
a screen capture take place. If you are in a replay in SC2, it will capture the game timer. You can now
shrink the window and allow others to connect to you. *You may need to open UDP port 11000 on your router.*

**For all other casters:** Type in the IP address of the streamer, and click Sync. You should see the
game timer appear from the main caster. You can then shrink the window, place it near your own game timer,
and check periodiacally to make sure you are close to the same time.