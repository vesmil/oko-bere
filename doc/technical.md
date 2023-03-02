# Technical documentation

The game is separated to three main parts - **Client**, **Server** and **Common**.

...

---

### Common part

As it is used by both I will start with the common part.

It contains game implementation - one round, deck, playing cards... even though the parts where real players will be
necessary is only through interfaces which needs to be implemented in other parts.

Here is simplified overview of Common part architecture.

```mermaid
classDiagram

    class Game {
        Deck deck
        GameTable table
    }

    class Deck { 
        +void Shuffle()
        +Card Draw()
        +bool TryDraw(out Card)
        -List<Card> cards
    }

    class Card { 
        +List<int> GetValues() 
        +Rank rank
        +Suite suite
    }

    class GameTable {
        +Dictionary bets
        +List players
        +int Bank
        ...
        +NotifyAll(Notification)
        +SetBanker()
        ...()
    }

    class IPlayer { 
        +void SendNotification(Notification)
        +Reponse AwaitReponse()
    }

    Game *-- Deck
    Deck *-- Card

    Game *-- GameTable
    GameTable *-- IPlayer
```

---

### Client part

This part is WinForm executable which will be used by client to connect to server.

```mermaid
classDiagram
    class GameTableForm {
        ClientLogics clientLogics
        Labels, Buttons ...   
    }

    class ClientLogics { 
        Client client
    }

    class Client { 

    }

    class GameState { 
        # TODO should be held by Logics or by TableForm?
    }

    class Menu { 

    }

    class Dialog { 

    }

    GameTableForm *-- ClientLogics
    ClientLogics *-- Client
```

---

### Server part

The last part implements IPlayer so it can be used and also provides way to run it in parallel with accepting new
clients. ...

```mermaid
classDiagram
    Server *-- TcpPlayer
    IPlayer --> TcpPlayer
```

## Communication

## ...
