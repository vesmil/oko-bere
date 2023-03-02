# Oko bere Card game

*Final project - NPRG035 a NPRG038*

---

## Introduction

This repository contains two applications: **Oko bere - server** and **Oko bere - client**. Combined they allow multiple
players to play Oko bere.

Oko bere is a card game that is popular in the Czech Republic. It is typically played with a deck of 32 cards (7, 8, 9,
10, Jack, Queen, King, and Ace in hearts, clubs, spades, and diamonds) among friends in social settings. The game is led
by the banker, who is the player holding the bank. The goal of each player is to get as close to 21 without overdrawing.

*For more details, see the `doc` folder*

---

## Setup

### Prerequisites

...

---

## TODO

- [x] Server part

    - [x] TCP and object serialization

    - [x] Proper IPlayer implementation

- [x] Common part

    - [x] Game logic with use of IPlayer

    - [x] Notifications and response preparation

- [x] Client part

    - [x] TCP and object serialization

    - [x] GUI

        - [x] Base

        - [ ] Dynamic

    - [ ] Initialization based on game state

    - [ ] Proper updates with response

        - [ ] Player connected, banker changed, etc. -> Change in game state -> Reload game state

        - [ ] Player's turn, cut player, ... -> Show UI and send back to server...

        - [ ] Different for banker

    - [ ] Server logging
