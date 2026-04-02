terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = "us-east-1"
}

# VPC
resource "aws_vpc" "bugtracker_vpc" {
  cidr_block           = "10.0.0.0/16"
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = {
    Name = "bugtracker-vpc"
  }
}

# Subnet
resource "aws_subnet" "bugtracker_subnet" {
  vpc_id                  = aws_vpc.bugtracker_vpc.id
  cidr_block              = "10.0.1.0/24"
  availability_zone       = "us-east-1a"
  map_public_ip_on_launch = true

  tags = {
    Name = "bugtracker-subnet"
  }
}

# Internet Gateway
resource "aws_internet_gateway" "bugtracker_igw" {
  vpc_id = aws_vpc.bugtracker_vpc.id

  tags = {
    Name = "bugtracker-igw"
  }
}

# Route Table
resource "aws_route_table" "bugtracker_rt" {
  vpc_id = aws_vpc.bugtracker_vpc.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.bugtracker_igw.id
  }

  tags = {
    Name = "bugtracker-rt"
  }
}

# Route Table Association
resource "aws_route_table_association" "bugtracker_rta" {
  subnet_id      = aws_subnet.bugtracker_subnet.id
  route_table_id = aws_route_table.bugtracker_rt.id
}

# Security Group
resource "aws_security_group" "bugtracker_sg" {
  name        = "bugtracker-sg"
  description = "Security group for BugTracker"
  vpc_id      = aws_vpc.bugtracker_vpc.id

  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 8091
    to_port     = 8091
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 9090
    to_port     = 9090
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 3000
    to_port     = 3000
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "bugtracker-sg"
  }
}

# EC2 Key Pair
resource "aws_key_pair" "bugtracker_key" {
  key_name   = "bugtracker-key"
  public_key = file("~/.ssh/bugtracker-key.pub")
}

# EC2 Instance
resource "aws_instance" "bugtracker_server" {
  ami                    = "ami-0c7217cdde317cfec"
  instance_type          = "t3.micro"
  subnet_id              = aws_subnet.bugtracker_subnet.id
  vpc_security_group_ids = [aws_security_group.bugtracker_sg.id]
  key_name               = aws_key_pair.bugtracker_key.key_name

  user_data = <<-USERDATA
    #!/bin/bash
    set -e
    apt-get update -y
    curl -fsSL https://get.docker.com -o /tmp/get-docker.sh
    sh /tmp/get-docker.sh
    systemctl start docker
    systemctl enable docker
    usermod -aG docker ubuntu
    mkdir -p /home/ubuntu/app
    cat > /home/ubuntu/app/docker-compose.yml << 'COMPOSE'
    version: "3.8"
    services:
      bugtracker:
        image: nabeeldevopsengineer/bugtracker:latest
        container_name: bugtracker
        ports:
          - "8091:8080"
        restart: unless-stopped
      prometheus:
        image: prom/prometheus:latest
        container_name: prometheus
        ports:
          - "9090:9090"
        restart: unless-stopped
      grafana:
        image: grafana/grafana:latest
        container_name: grafana
        ports:
          - "3000:3000"
        environment:
          - GF_SECURITY_ADMIN_PASSWORD=admin123
        restart: unless-stopped
    COMPOSE
    chown -R ubuntu:ubuntu /home/ubuntu/app
    cd /home/ubuntu/app && docker compose up -d
  USERDATA

  tags = {
    Name = "bugtracker-server"
  }
}

# Output
output "server_ip" {
  value = aws_instance.bugtracker_server.public_ip
}

output "app_url" {
  value = "http://${aws_instance.bugtracker_server.public_ip}:8091"
}

output "prometheus_url" {
  value = "http://${aws_instance.bugtracker_server.public_ip}:9090"
}

output "grafana_url" {
  value = "http://${aws_instance.bugtracker_server.public_ip}:3000"
}
