# Battleship Game

Battleship is a classic strategy game where the objective is to sink all of your opponent's ships before they sink yours.

## Table of Contents

- [Description](#description)
- [Features](#features)
- [Getting Started](#getting-started)
- [How to Play](#how-to-play)
- [Contribution](#contribution)
- [License](#license)

## Description

This implementation of the Battleship game is a console application developed using C#. It incorporates the standard rules of Battleship with the addition of Dependency Injection for easier testing and future scalability.

## Features

- Simple and intuitive console interface.
- Battleship and Destroyers to try and sink(in this version only one Battleship and 2 Destroyers).
- Error handling for invalid or duplicate guesses.
- Legend and grid view to keep track of hits, misses, and ship status.
- Due to separate Core implementation - multiple UI implementations are possible
- Big amount of unit tests

## Getting Started
Ensure you have .NET 5.0 or newer installed on your machine.

## How to Play

1. Once the game starts, you'll be greeted with a grid representing the ocean.
1. Input coordinates where you wish to fire a shot (for example, `A5`).
1. After each attempt, the game will give feedback on the outcome.
1. The main goal is to sink every enemy ship.
1. The game goes on until you've sunk all the ships.

## Contribution
Contributions, issues, and feature requests are welcome! Feel free to check the issues page. Remember to adhere to this project's code of conduct.

## License
This project is licensed under the MIT license.
