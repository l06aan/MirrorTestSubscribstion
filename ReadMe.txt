This project implements a small subscription layer over Mirror NetworkMessages.

The main idea:
- client registers a local handler for HelloMessage;
- client sends SubscribeToMessageRequest to the server;
- server stores this subscription by connectionId;
- server sends HelloMessage only to subscribed clients.

Expected console result:
[HelloMessageClient] Received HelloMessage: Hello Client!
