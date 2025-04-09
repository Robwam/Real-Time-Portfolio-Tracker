# Real-Time Portfolio Tracker

## Overview

A **real-time portfolio tracker** that allows users to monitor their investment portfolio and receive alerts for price changes, percentage thresholds, or specific events. Built with **ASP.NET Core**, **Blazor Server**, **Azure Service Bus**, and **PostgreSQL**, this project simulates a distributed system architecture with microservices, using modern backend practices to provide an interactive user experience.

### Key Features:
- **Track Portfolio**: View the real-time value of assets (stocks, crypto, etc.).
- **Set Alerts**: Receive notifications based on custom conditions (price changes, percentage shifts).
- **Microservices**: Built with a microservices architecture to decouple services like portfolio management and notifications.
- **Asynchronous Communication**: Utilizes **Azure Service Bus** for message-based communication between services.
- **Real-Time Updates**: Real-time front-end updates using **Blazor Server**.

## Tech Stack:
- **Backend**: ASP.NET Core, Azure Service Bus, PostgreSQL, Serilog (logging)
- **Frontend**: Blazor Server (for real-time updates and interactive UI)
- **CI/CD**: GitHub Actions (for automated builds and deployments)
- **Deployment**: Azure App Services (cloud hosting)

## Architecture:
The system is divided into independent microservices:
- **Portfolio Service**: Handles user portfolio data and asset management.
- **Notification Service**: Sends alerts based on user-defined conditions (price changes, percentage thresholds).
- **Asset Data Service**: Fetches live data from third-party APIs (stocks, cryptocurrencies).

![Portfolio Tracker Design](https://github.com/user-attachments/assets/1b642cbc-6b6d-44a8-8f1e-dcd8e9986ce1)


### Service Communication:
- **REST APIs** for synchronous calls between services.
- **Azure Service Bus** to send and receive asynchronous messages between services.

## Future Improvements:

- **User Authentication**: Add user registration and authentication.

- **Advanced Alerts**: Support more complex alerts based on portfolio performance or trends.

- **Additional Asset Classes**: Track more asset types like commodities, real estate, and cryptocurrencies.

- **Real-Time Web Updates**: Implement WebSockets for live updates to the UI when asset prices change.
