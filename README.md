[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/ZiutltgI)
[![Open in Visual Studio Code](https://classroom.github.com/assets/open-in-vscode-2e0aaae1b6195c2367325f4f02e2d04e9abb55f0b24a779b69b11b9e10269abc.svg)](https://classroom.github.com/online_ide?assignment_repo_id=23515821&assignment_repo_type=AssignmentRepo)

# Agri-Stress: Plant Stress Diagnosis System

A robust, microservices-based Information Systems Engineering project designed to diagnose plant stress levels using Explainable Artificial Intelligence (XAI).

This project demonstrates the implementation of a modern tech stack, featuring Clean Architecture, Containerization, Kubernetes orchestration, and comprehensive system monitoring.

---

## Project Overview

Agri-Stress is an intelligent system that analyzes plant leaf images to detect diseases and stress levels. Unlike traditional black-box AI models, this system provides **Visual Explanations (XAI)**, highlighting the exact areas of the leaf that led to the AI's decision, ensuring transparency and trust in agricultural diagnostics.

### Key Features

- **Microservices Architecture:** Decoupled backend services for scalability and maintainability.
- **Explainable AI (XAI):** Generates heatmaps (visual justifications) for AI predictions.
- **High Availability & Self-Healing:** Orchestrated using Kubernetes.
- **Real-time Monitoring:** Integrated with Prometheus and Grafana.
- **Interactive Dashboard:** A seamless UI to interact with the system.

---

## Architecture & Tech Stack

- **API Gateway / Backend:** .NET 10 (C#) using Clean Architecture principles.
- **AI / Computer Vision Service:** Python (FastAPI / PyTorch).
- **Database:** PostgreSQL.
- **Containerization:** Docker.
- **Orchestration:** Kubernetes (K8s).
- **Observability:** Prometheus & Grafana.
- **Frontend:** HTML, Tailwind CSS, Vanilla JavaScript.

---

## 🚀 Quick Start Guide (Runbook)

Follow these steps to deploy and run the entire system locally.

### Prerequisites

- Docker Desktop installed and running.
- Kubernetes enabled in Docker Desktop.
- `kubectl` CLI installed.

### 1. Build the Docker Images

Open your terminal in the root directory of the project and build the required images:

```bash
# Build the .NET API Image
docker build -t agristress-api:latest .

# Build the Python Vision Service Image
cd VisionProcessingService
docker build -t agristress-vision:latest .
cd ..
2. Deploy to Kubernetes
Apply the Kubernetes manifests to spin up the database, services, and monitoring tools:

kubectl apply -f k8s-agristress.yaml
kubectl apply -f k8s-prometheus.yaml
kubectl apply -f k8s-grafana.yaml

Wait until all pods are in the Running state:

kubectl get pods -w

3. Expose Services (Port-Forwarding)
Open separate terminal windows and run the following commands to access the services locally:

API & Swagger UI:
kubectl port-forward svc/api 8080:8080
(Access: http://localhost:8080/swagger)

Grafana Dashboard:
kubectl port-forward svc/grafana-service 3000:3000
(Access: http://localhost:3000 | User/Pass: admin)


4. Launch the User Interface
Simply open the index.html file in your preferred web browser to interact with the Agri-Stress Dashboard.

Kubernetes Advanced Features Used
This project utilizes advanced Kubernetes features to ensure a production-ready environment:

ReplicaSets: The API service runs with replicas: 3 for High Availability.

Liveness Probes: Automated health checks and self-healing for containers.

Resource Limits: Strict CPU and Memory limits applied to the AI service to prevent cluster starvation.

Author
 HAITHM Baouzar . Information Systems Engineering

Sakarya University, Türkiye
```
