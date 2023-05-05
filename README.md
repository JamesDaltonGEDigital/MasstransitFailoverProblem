# Steps to reproduce the lost message problem

1. The file [./MassTransitCommon/appsettings.Common.json](./MassTransitCommon/appsettings.Common.json) has the ActiveMQ connection settings. It assumes localhost:61616 admin/admin

1. Start ActiveMQ

1. Open a shell and run the MassTransitReceive app in the MassTransitReceive folder. It has a simple consumer that just counts messages. You can press `s` to show the total received, `c` to clear, and `q` to quit

1. Open a second shell and start the MassTransitSend app in the MassTransitSend folder with the command `dotnet run wait=true`

1. Stop and restart ActiveMQ.

1. Return to MassTransitSend window and press any key. By default it will generate 40 messages. 4 threads sending 10 each.

1. Go to ActiveMQ admin page and check the message count. It should be 40, but will be less that that.

1. Or go to the MassTransitReceive window and press s. That should return 40, but won't.
