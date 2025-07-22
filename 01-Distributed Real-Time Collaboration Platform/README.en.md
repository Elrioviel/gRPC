[![Русский](https://img.shields.io/badge/язык-Русский-green)](README.md)

# Collaborative Docs - A Real-Time Document Collaboration Platform

## 📌 About the Project

**Collaborative Docs** is a distributed system for real-time collaborative document editing.  
<br>Multiple users can work on the same document simultaneously, securely, and with minimal delay.  
<br>The system ensures fast data synchronization between clients using **bi-directional gRPC streaming**.

Think of it as Google Docs - but fully under your control.

This project is designed for developers who want to:

- build a self-hosted alternative to Google Docs for internal or corporate use,
- add real-time collaborative features to their own applications.

## 🔧 Technologies

- ⚡ **gRPC** with bi-directional streaming
- 🗃️ **PostgreSQL** for document storage
- 🚦 **Redis** for pub/sub messaging or document locking
- 🔐 **JWT** for user authentication
- 🐳 **Docker** and **docker-compose** for easy setup and deployment
- 🌐 Optional: frontend in **React** via gRPC-web or a WebSocket proxy

## 🚀 How to Run Locally

```bash
docker-compose up --build
```
