# 3D-Delft: Documentation 

6. Loading and storing OBJ
    - Concept
        - Communication framework between client and server (figure)
            - send login request
            - send credentials
            - get bearer token
            - Send obj data
            - Get obj Id
            - send meta data
            - query all entries
            - request each object data
            - callback: request metadata
        - Server: info and structure
            - Flask application
            - connect to apache2 with WSGI
            - connect to DB with psycopg2
    - Server
        - info
            - flask: python package to implement restful servers
            - apache2 for ssl
            - wsgi to connect flask with apache2
        - authentication (/login, /validate)
            - with flask
            - basic auth with uname+pwd
            - Bearer token as session tokens (for now: infinite session)
        - Obj upload/Download
            - Object files: store in local file system; name&id: uuid
            - Metadata
                - JSON Structure (example)
                - Use postgres db
    - Client (Unity WebGL)
        - Object download:
            - Data flow in Unity (i.e. in the WebGL application)
            - Web request flow
            - Obj Download and Layer spawning
            - Meta downloads and overwrites
        - Object upload:
            - Implementation of login interface
            - Object retrieval and upload
            - Metadata collection and upload to server
        - Authentication:
            - Uses basic authentication from HTTP to send uname & pwd
            - Uses the received bearer token when sending future requests
            - Showcase the authentication workflow

[< Making 3D tiles](./making-3D-tiles.md) | [Next: Future >](./future.md)

[Pages info](./pages/example/pages.md)