# Agri-Energy Connect

## Overview

Agri-Energy Connect is a prototype web application that connects farmers with energy specialists. It supports the exchange of agricultural products while promoting sustainable energy practices in South Africa's agricultural sector. Built with ASP.NET Core MVC and Entity Framework, it encourages collaboration and innovation between farmers and green energy advocates.

### Languages and Frameworks
<p align="left">
<a href="#"><img alt="C#" src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white"></a>
<a href="#"><img alt="ASP.NET Core" src="https://img.shields.io/badge/ASP.NET%20Core-%235C2D91.svg?style=for-the-badge&logo=.net&logoColor=white"></a>
<a href="#"><img alt="HTML5" src="https://img.shields.io/badge/html5-%23E34F26.svg?style=for-the-badge&logo=html5&logoColor=white"></a>
<a href="#"><img alt="CSS3" src="https://img.shields.io/badge/css3-%231572B6.svg?style=for-the-badge&logo=css3&logoColor=white"></a>
<a href="#"><img alt="JavaScript" src="https://img.shields.io/badge/javascript-%23F7DF1E.svg?style=for-the-badge&logo=javascript&logoColor=black"></a>
</p>

### Libraries and Tools
<p align="left">
<a href="#"><img alt="Entity Framework Core" src="https://img.shields.io/badge/Entity%20Framework%20Core-%23512BD4.svg?style=for-the-badge&logo=.net&logoColor=white"></a>
<a href="#"><img alt="Bootstrap" src="https://img.shields.io/badge/bootstrap-%237952B3.svg?style=for-the-badge&logo=bootstrap&logoColor=white"></a>
<a href="#"><img alt="Font Awesome" src="https://img.shields.io/badge/Font%20Awesome-%23528DD7.svg?style=for-the-badge&logo=font-awesome&logoColor=white"></a>
</p>

### IDE/Editors
<p align="left">
<a href="#"><img alt="Visual Studio" src="https://img.shields.io/badge/Visual%20Studio-%235C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white"></a>

---

## 🚀 Getting Started

### Prerequisites
- 🖥 Visual Studio 2022 or Visual Studio Code
- ⚙️ .NET 8.0 SDK or later
- 🗄 SQL Server (LocalDB, Express, or full edition)
- 🧪 Git for version control

### Setup Instructions

```bash
git clone https://github.com/your-organization/agri-energy-connect.git
cd agri-energy-connect
```

Update your `appsettings.json` to reflect your environment:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgriEnergyConnectDb;Trusted_Connection=True;"
}
```

### Build and Run

**Using Visual Studio:**
1. Open `AgriEnergyConnect.sln`
2. Build the solution (F6)
3. Run the app (F5)

**Using .NET CLI:**
```bash
dotnet restore
dotnet build
dotnet run --project AgriEnergyConnect
```

---

## 🧩 NuGet Dependencies

These packages are required and should restore automatically. You can also install them manually using the NuGet Package Manager Console in Visual Studio.

### 🔧 Core Packages

| Package | Description |
|---------|-------------|
| `Microsoft.AspNetCore.Mvc` | Enables MVC pattern: routing, controllers, views |
| `Microsoft.EntityFrameworkCore` | ORM for interacting with the SQL Server database |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server-specific EF Core provider |
| `Microsoft.AspNetCore.Authentication.Cookies` | Cookie-based authentication handling |
| `Microsoft.AspNetCore.Identity` | Password hashing and user identity framework |
| `Microsoft.EntityFrameworkCore.Tools` | Migration and scaffolding commands |

### ⚙️ Optional (but commonly used)

| Package | Description |
|---------|-------------|
| `Microsoft.AspNetCore.Session` | Enables session state for storing transient data |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | Scaffolding views and controllers |
| `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` | EF Core-specific dev error diagnostics |

To restore via CLI:
```bash
dotnet restore
```

---

## 🔐 Demo Logins
All test accounts use the password: `password`

### 👨‍💼 Admin Users
| Username     | Name             |
|--------------|------------------|
| `admin`      | Emily Mathews    |
| `cfrankland` | Clive Frankland  |

### 🚜 Farmer Users
| Username     | Name              | Status     |
|--------------|-------------------|------------|
| `jsmith`     | John Smith        | Active     |
| `scooper`    | Sarah Cooper      | Active     |
| `tmkhize`    | Thabo Mkhize      | Active     |
| `avanwyk`    | Anita van Wyk     | Active     |
| `dpretorius` | Daniel Pretorius  | Active     |
| `lnaidoo`    | Leela Naidoo      | Inactive   |

---

## 🧱 Project Structure

| Folder         | Description |
|----------------|-------------|
| `Controllers/` | Handles user requests via MVC controllers |
| `Models/`      | Entity models and view models with validation |
| `Views/`       | Razor view templates and layout pages |
| `DTOs/`        | Data Transfer Objects for mapping cleanly between layers |
| `Services/`    | Business logic and services handling use cases |
| `Repositories/`| Data access and EF Core queries |
| `Data/`        | `AppDbContext` and database seeders |
| `wwwroot/`     | Static files (JS, CSS, images) |

---

## 👥 User Roles and Functionalities

### 🌾 Farmer
- Register and log in (with admin approval)
- Add, view, search, and filter products
- View dashboard summary (recent activity, categories, total products)
- Read and send messages to administrators and farmers

### 🧑‍💼 Employee
- Approve and manage farmers
- View and filter all products
- Add new farmers
- Send messages to farmers
- Access admin dashboard with statistics

---

## 🔒 Authentication and Authorisation

- Cookie-based authentication with role-based access control
- Hashed passwords stored securely
- Controllers protected with `[Authorize]` and `[Authorize(Roles = "...")]`
- Session timeout and secure cookies configured in `Program.cs`

---

## 🗃️ Database Structure

| Table                  | Description                                    |
|------------------------|------------------------------------------------|
| `Users`                | Stores login and profile data                 |
| `Farmers`              | Contains farm profile and location info       |
| `Products`             | Farmer-created products with metadata         |
| `Messages`             | System messages between admins and farmers    |
| `RegistrationRequests` | Pending account registrations for approval    |

Database is seeded automatically with sample users, products, and messages.

---

## 🛠 Troubleshooting

| Problem                  | Solution |
|--------------------------|----------|
| Database not connecting  | Check SQL Server is running and your connection string is correct |
| Login not working        | Use the seeded credentials; ensure seeding ran on first run |
| Build fails              | Run `dotnet restore`, then clean and rebuild the solution |
| Session issues           | Ensure cookies are enabled and session settings are active in `Program.cs` |

---

## 🌟 Future Enhancements

- 📊 Advanced analytics and reporting
- 📱 Native mobile app (Android/iOS)
- 🔌 Integration with energy provider APIs
- 💬 Real-time messaging and system alerts
- 🛒 In-app product ordering and payments

---

## 📬 Contact

For support or to report issues, please contact the development team or open an issue in the repository.

---
