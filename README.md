# FinanceTrackerApp


# 💰 Personal Finance Tracker

A full-stack personal finance budgeting app to help users track expenses, visualize spending trends, and manage their financial goals.

Built with:
- **Frontend:** [Next.js](https://nextjs.org/) + [TypeScript](https://www.typescriptlang.org/)
- **Backend:** [.NET Core (C#)](https://dotnet.microsoft.com/)
- **Database:** SQL Server / PostgreSQL (customizable)

---

## 🧠 Features

- User authentication and secure login
- Monthly and yearly budget planning
- Expense categorization (e.g., food, rent, savings)
- Interactive dashboards and charts
- Transaction history and filtering
- Goal tracking and financial summaries

---

## 📸 Preview

_Optional: Insert screenshots or a Loom video demo here._

---

## 🚀 Tech Stack

| Layer        | Tech Stack                          |
|--------------|--------------------------------------|
| Frontend     | Next.js, TypeScript, Tailwind CSS    |
| Backend      | .NET Core, C#                        |
| Database     | SQL Server or PostgreSQL             |
| Auth         | JWT / ASP.NET Identity / NextAuth.js |
| Hosting      | Vercel (frontend), Azure/AWS (API)   |

---

## 📦 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js (v18+)](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/) or SQL Server

### Clone the Repo

```bash
git clone https://github.com/YOUR_USERNAME/finance-tracker.git
cd finance-tracker


## Backend Setup 
cd backend
dotnet restore
dotnet ef database update
dotnet run


## Frontend Setup 
cd frontend
npm install
npm run dev
