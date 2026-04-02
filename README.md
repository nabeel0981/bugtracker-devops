# BugTracker — DevOps Portfolio Project

A production-grade Bug Tracking System built with .NET 9 Core MVC, fully containerized and deployed with a complete DevOps pipeline.

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Docker](https://img.shields.io/badge/Docker-Containerized-blue)
![Kubernetes](https://img.shields.io/badge/Kubernetes-Manifests-blue)
![Terraform](https://img.shields.io/badge/Terraform-IaC-purple)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-black)
![AWS](https://img.shields.io/badge/AWS-EC2-orange)

## Tech Stack

| Layer | Technology |
|---|---|
| Application | .NET 9 Core MVC |
| Containerization | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| Infrastructure | Terraform (AWS) |
| Cloud | AWS EC2 (Ubuntu 22.04) |
| Monitoring | Prometheus + Grafana |
| Orchestration | Kubernetes |

## Features

- Report, assign, resolve and close bugs
- Priority levels — Critical, High, Medium, Low
- Status workflow — Open → In Progress → Resolved → Closed
- Project organization — group bugs under projects
- Activity timeline on each bug
- Real-time monitoring with Prometheus + Grafana
- Full CI/CD pipeline — push to main = auto deploy to DockerHub

## Project Structure
```
bugtracker/
├── src/BugTracker.Web/         # .NET 9 MVC Application
│   ├── Controllers/            # HomeController
│   ├── Models/                 # Bug, Project models
│   ├── Views/                  # Razor views
│   └── Dockerfile              # Multi-stage Docker build
├── monitoring/
│   └── prometheus.yml          # Prometheus scrape config
├── k8s/                        # Kubernetes manifests
├── terraform/                  # AWS Infrastructure as Code
├── docker-compose.yml          # Local dev stack
└── .github/workflows/          # CI/CD pipeline
```

## Quick Start

### Run Locally
```bash
cd src/BugTracker.Web
dotnet run --urls "http://0.0.0.0:5106"
```

### Run with Docker Compose
```bash
docker compose up -d --build
```

Access:
- App: http://localhost:8091
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3000 (admin/admin123)

## CI/CD Pipeline

Every push to `main` automatically:
1. Builds the .NET 9 application
2. Creates Docker image tagged with commit SHA
3. Pushes to DockerHub

## Infrastructure

Provisioned with Terraform on AWS:
- VPC + Subnet + Internet Gateway
- Security Group (ports 22, 8091, 9090, 3000)
- EC2 t3.micro (Ubuntu 22.04)

## Author

Nabeel Ahmed — DevOps Engineer
