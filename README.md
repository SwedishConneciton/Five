# Five
Our sandbox code for learning Asp Net Core (RC2).  Please see the release pane to find an version with particular functions.  For example, the initial release (1.0.0) showcases how to configure the Open-Id Connect server to issue JWT tokens for authentication and authorization of resources with the password grant type.

[We](http://www.connection.se/) started our journey with Asp Net by first looking at authentication and authorization.  Read more about this on the [Asp Net Core with Open-Id minus Identity](https://medium.com/@matthew47671280/asp-net-core-with-open-id-minus-identity-5d4ad615019d#.n3warlobq).  That blog highlights code written for a nightly build of Asp Net Core RC2 which offically was published just a week ago.  

The 1.0.0 release of this project had startup logic geared to the [Dot Cli client](https://www.microsoft.com/net/core) but locally and through Docker we still used the [dnx, dnu and dnvm tools](http://johnatten.com/2015/05/17/dnvm-dnx-and-dnu-understanding-the-asp-net-5-runtime-options/).  Around second quarter 2016, the project could be [fired up](https://github.com/SwedishConneciton/Five/wiki/Execute-locally) on Windows and Linux (via Docker) with dnu and dnx.

As of the 1.0.1 release, We use the offical RC2 with the dotnet CLI.
