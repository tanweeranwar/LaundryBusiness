# LaundryBusiness

A modern Laundry Management System built with **ASP.NET Core**, designed to streamline laundry operations for businesses with support for multiple branches, online order management, pickup & delivery scheduling, subscriptions, loyalty rewards, and digital payments.

---

## 🚀 Features

### Customer Management
- Customer Registration & Login
- Profile Management
- Address Management
- Order History
- Notifications

### Order Management
- Place Laundry Orders
- Schedule Pickup & Delivery
- Express Delivery Option
- Real-time Order Tracking
- Order Status Updates

### Laundry Services
- Wash & Fold
- Dry Cleaning
- Ironing
- Premium Laundry Services
- Custom Service Pricing

### Multi-Branch Support
- Multiple Laundry Branches
- Branch-specific Pricing
- Branch Assignment
- Service Availability by Branch

### Pickup & Delivery
- Schedule Future Pickup
- Delivery Slot Selection
- Delivery Tracking
- Driver Assignment

### Payments
- Online Payments
- Wallet Payments
- Cash on Delivery
- Payment History
- Invoice Generation

### Subscription Plans
- Monthly Laundry Plans
- Service Packages
- Subscription Renewal
- Usage Tracking

### Rewards
- Reward Points
- Referral Bonuses
- Loyalty Programs

### Administration
- Dashboard
- Customer Management
- Branch Management
- Employee Management
- Service Pricing
- Reports & Analytics

---

# 🏗️ Architecture

The project follows **Clean Architecture** principles.

```
LaundryBusiness
│
├── Laundry.API
├── Laundry.Application
├── Laundry.Domain
├── Laundry.Infrastructure
├── Laundry.Persistence
└── Laundry.Shared
```

---

# 🛠️ Technology Stack

| Technology | Version |
|------------|----------|
| ASP.NET Core | .NET 8 |
| Entity Framework Core | Latest |
| SQL Server | 2022+ |
| JWT Authentication | ✔ |
| Swagger / OpenAPI | ✔ |
| AutoMapper | ✔ |
| FluentValidation | ✔ |
| MediatR | ✔ |
| Dependency Injection | Built-in |
| Git | ✔ |

---

# 📦 Database

Database Engine:

- SQL Server

Main Modules:

- Customers
- Branches
- Services
- Orders
- Payments
- Wallet
- Rewards
- Employees
- Delivery
- Subscriptions

---

# 🔐 Authentication

- JWT Authentication
- Role-Based Authorization

Roles include:

- Super Admin
- Branch Admin
- Employee
- Delivery Agent
- Customer

---

# 📁 Project Structure

```
src/
│
├── API
├── Application
├── Domain
├── Infrastructure
├── Persistence
└── Shared

database/
docs/
tests/
```

---

# ⚙️ Getting Started

## Prerequisites

- Visual Studio 2022 Community
- .NET 8 SDK
- SQL Server
- Git

---

## Clone Repository

```bash
git clone https://github.com/tanweeranwar/LaundryBusiness.git
```

---

## Configure Database

Update the connection string in:

```text
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=LaundryBusiness;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Apply Migrations

```bash
dotnet ef database update
```

---

## Run Project

```bash
dotnet run
```

Or simply press:

```
F5
```

in Visual Studio.

---

# 📖 API Documentation

Swagger will be available at:

```
https://localhost:5001/swagger
```

or

```
http://localhost:5000/swagger
```

---

# 📌 Future Enhancements

- Mobile App
- Push Notifications
- SMS Integration
- WhatsApp Notifications
- AI-based Stain Detection
- Dynamic Pricing
- Barcode / QR Tracking
- Customer Wallet
- Coupon System
- Delivery Route Optimization
- Inventory Management
- Franchise Management

---

# 🤝 Contributing

1. Fork the repository
2. Create a feature branch

```
git checkout -b feature/FeatureName
```

3. Commit changes

```
git commit -m "Added new feature"
```

4. Push changes

```
git push origin feature/FeatureName
```

5. Create a Pull Request

---

# 📄 License

This project is licensed under the MIT License.

---

# 👨‍💻 Author

**Tanweer Anwar**

GitHub:
https://github.com/tanweeranwar

---

## ⭐ Project Vision

LaundryBusiness aims to provide a scalable, cloud-ready, enterprise-grade laundry management platform capable of serving local laundries, franchise businesses, and multi-branch organizations with modern technologies and clean architecture.
