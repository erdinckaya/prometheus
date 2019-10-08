Prometheus is designed as an example of authoritative server client relation with UDP and RUDP.
It has client predictions, server lag compensation and also full new Unity ECS paradigm. Moreover Prometheus is using
[`Yojimbo.Net`](https://github.com/erdinckaya/yojimbo.net) for networking which is very fast and 
lightweight networking library for dedicated servers.

## DEPENDENCIES
* [Unity 2019](https://unity3d.com/get-unity/download)
* [Themis](https://github.com/erdinckaya/themis)

## USAGE

Only thing is just write your server ip and port into `Constants.cs` and run the game.
If you want to run multiple instances and want to see logs of your Prometheus.
There is [`NLOG`](https://github.com/sschmid/NLog) prefab in scene. You can enter your log server's 
ip and port, which can be basically like that
```
#!/usr/bin/env python3
import socket
import sys

HOST = '127.0.0.1'  # Standard loopback interface address (localhost)
PORT = int(sys.argv[1])        # Port to listen on (non-privileged ports are > 1023)

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    conn, addr = s.accept()
    with conn:
        print('Connected by', addr)
        while True:
            data = conn.recv(1024)
            if not data:
                break
            print(str(data, 'utf-8'))
```

