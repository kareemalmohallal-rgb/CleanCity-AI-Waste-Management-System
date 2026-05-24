# CleanCity — Smart Waste Management System 🚮🤖

## Overview

CleanCity is an AI-powered smart waste management platform designed to improve urban cleanliness through community participation and intelligent automation.
The system allows citizens to report garbage locations using a Flutter mobile application, while an ASP.NET Core backend and a YOLOv8x AI model automatically analyze submitted images, validate reports, and notify waste collection teams in real time.

The platform combines:

* 📱 Flutter Mobile Application
* 🌐 ASP.NET Core 8 Web API
* 🖥 ASP.NET Core MVC Dashboard
* 🤖 Python AI Service (YOLOv8x + FastAPI)
* 🔔 Firebase Cloud Messaging (FCM)
* 🗄 SQL Server
* 🧱 Clean Architecture

---

# System Architecture

```text
Citizen (Flutter App)
        │
        ▼
ASP.NET Core Web API
        │
        ├── SQL Server Database
        │
        ├── Python AI Service (YOLOv8x)
        │
        └── Firebase Cloud Messaging (FCM)
                │
                ▼
Drivers & Admin Notifications
```

---

# Features ✨

## Citizen Features

* Submit garbage reports with:

  * Image
  * GPS location
  * Optional description
* Real-time report tracking
* Push notifications for report updates
* Offline-first support
* Arabic RTL interface

## Driver Features

* Receive assigned reports instantly
* Accept / Reject reports
* Mark reports as completed
* Real-time notifications

## Admin Dashboard

* Manage reports
* Manage drivers and trucks
* Manage geographic areas
* Monitor statistics and analytics
* Manual report assignment

## AI Features

* Automatic garbage detection using YOLOv8x
* Reject invalid or fake reports
* Bounding-box object detection
* Real-time image validation

---

# Technologies Used 🛠

## Backend

* ASP.NET Core 8
* Entity Framework Core 8
* SQL Server
* JWT Authentication
* Cookie Authentication
* AutoMapper
* Firebase Admin SDK

## Frontend

* Flutter (Dart 3+)
* BLoC State Management
* Clean Architecture
* SharedPreferences
* Connectivity Plus

## AI Service

* Python
* FastAPI
* Ultralytics YOLOv8x
* PyTorch

## Notifications

* Firebase Cloud Messaging (FCM)

---

# Project Structure 📂

```text
CleanCity/
│
├── CleanCity.Api/           # ASP.NET Core Web API
├── CleanCity.Mvc/           # Admin Dashboard (MVC)
├── CleanCity.Mobile/        # Flutter Mobile App
├── CleanCity.AI/            # Python AI Service
├── docs/                    # Screenshots & documentation
├── README.md
└── .gitignore
```

---

# AI Model Information 🤖

## Model

YOLOv8x (Ultralytics)

## Dataset

* 1,875 total images
* Garbage & Others classes
* Roboflow datasets + manually collected images

## Performance

| Metric    | Result |
| --------- | ------ |
| Precision | ≥ 88%  |
| Recall    | ≥ 85%  |
| mAP@0.5   | ≥ 86%  |
| F1-Score  | ≥ 86%  |

## Inference Time

1.1s – 3.8s per image

---

# Authentication & Authorization 🔐

## API

* JWT Bearer Authentication
* Role-based Authorization

  * Admin
  * Driver

## MVC Dashboard

* Cookie Authentication
* Session expiration
* Sliding expiration

---

# Offline-First Support 📡

The Flutter application supports offline report submission:

1. Reports are stored locally first.
2. When internet connectivity returns, reports are automatically synced with the server.

---

# Notifications 🔔

Firebase Cloud Messaging is used for:

* Report received
* Driver assignment
* Report completion
* AI validation updates

---

# Installation 🚀

## 1. Clone Repository

```bash
git clone https://github.com/USERNAME/CleanCity.git
cd CleanCity
```

---

# Backend Setup (ASP.NET Core)

```bash
cd CleanCity.Api
dotnet restore
dotnet ef database update
dotnet run
```

---

# Flutter Setup

```bash
cd CleanCity.Mobile
flutter pub get
flutter run
```

---

# Python AI Service Setup

```bash
cd CleanCity.AI

pip install -r requirements.txt

uvicorn main:app --reload
```

---

# Environment Variables ⚙

Create your own configuration files:

```text
appsettings.json
firebase-service-account.json
.env
```

⚠ Never upload secrets or private keys to GitHub.

---

# Testing 🧪

The project includes:

* Unit Testing
* Integration Testing
* API Contract Testing
* Security Testing
* AI Model Evaluation
* User Acceptance Testing (UAT)

---

# Security 🔒

* HTTPS/TLS
* JWT Authentication
* Role-based access control
* Secure Firebase integration
* Foreign key protection
* Token expiration validation

---

# Future Improvements 🚀

* Cloud deployment (Azure/AWS)
* Redis caching
* CQRS architecture
* Full offline mode
* Heatmaps & analytics
* Drone integration
* Multi-class waste classification

---

# Screenshots 📸

## Mobile Application

Add screenshots inside:

```text
/docs/screenshots/mobile/
```

## Dashboard

Add screenshots inside:

```text
/docs/screenshots/dashboard/
```

---

# Team 👨‍💻

Developed by a team of five students as a graduation project.

---

# License 📄

This project is developed for educational and research purposes.

---

# Keywords

```text
ASP.NET Core
Flutter
YOLOv8
AI
Clean Architecture
SQL Server
Firebase
Smart City
Waste Management
FastAPI
```
