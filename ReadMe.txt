# Mirror NetworkMessage Subscription Test

Unity version: 2022.3.62f3

## Description

Test task implementation for sending Mirror NetworkMessages only to clients that explicitly subscribed to a specific message type.

## Scenario

1. Server or Host starts.
2. Client connects.
3. Client registers a handler for HelloMessage.
4. Client sends a subscription request for HelloMessage.
5. Server stores the subscription.
6. Server sends HelloMessage only to the subscribed client.
7. Client receives HelloMessage and prints the message text to Unity Console.

Expected console output:

```text
[HelloMessageClient] Received HelloMessage: Hello Client!