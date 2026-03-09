"use client";

import { useQuery } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import Link from "next/link";
import SpendingPieChart from "../components/charts/SpendingPieChart";
import MonthComparisonChart from "../components/charts/MonthComparisonChart";
import MonthlyTrendChart from "../components/charts/MonthlyTrendChart";
import BudgetAssistant from "../components/chat/BudgetAssistant";
import AnimatedCounter from "../components/ui/AnimatedCounter";
import GlassCard from "../components/ui/GlassCard";
import TrendIndicator from "../components/ui/TrendIndicator";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5280";

interface Expense {
  id: number;
  name: string;
  description: string;
  amount: number;
  date: string;
  category: string;
  subCategory: string;
  paymentMethod: string;
  notes: string;
  userId: number;
}

interface Account {
  id: number;
  name: string;
  email: string;
  accountType: string;
  balance: number;
}

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

interface SavingsGoal {
  id: number;
  name: string;
  description: string;
  targetAmount: number;
  currentAmount: number;
  progressPercentage: number;
  targetDate: string | null;
  category: string;
  isCompleted: boolean;
}

interface RecurringTransaction {
  id: number;
  description: string;
  amount: number;
  type: string;
  frequency: string;
  nextDueDate: string;
  isActive: boolean;
}

async function fetchExpenses(): Promise<Expense[]> {
  const response = await fetch(`${API_BASE_URL}/api/Expense/all`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch expenses");
  return response.json();
}

async function fetchAccounts(): Promise<Account[]> {
  const response = await fetch(`${API_BASE_URL}/api/Account/all`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch accounts");
  return response.json();
}

async function fetchCurrentUser(): Promise<User> {
  const response = await fetch(`${API_BASE_URL}/api/User/me`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Not authenticated");
  return response.json();
}

async function fetchSavingsGoals(): Promise<SavingsGoal[]> {
  const response = await fetch(`${API_BASE_URL}/api/SavingsGoal/all`, {
    credentials: "include",
  });
  if (!response.ok) return [];
  return response.json();
}

async function fetchRecurringTransactions(): Promise<RecurringTransaction[]> {
  const response = await fetch(`${API_BASE_URL}/api/RecurringTransaction/all`, {
    credentials: "include",
  });
  if (!response.ok) return [];
  return response.json();
}

export default function DashboardPage() {
  const router = useRouter();

  const {
    data: user,
    isLoading: userLoading,
    error: userError,
  } = useQuery({
    queryKey: ["currentUser"],
    queryFn: fetchCurrentUser,
    retry: false,
  });

  const { data: expenses = [], isLoading: expensesLoading } = useQuery({
    queryKey: ["expenses"],
    queryFn: fetchExpenses,
    enabled: !!user,
  });

  const { data: savingsGoals = [] } = useQuery({
    queryKey: ["savingsGoals"],
    queryFn: fetchSavingsGoals,
    enabled: !!user,
  });

  const { data: recurringTransactions = [] } = useQuery({
    queryKey: ["recurringTransactions"],
    queryFn: fetchRecurringTransactions,
    enabled: !!user,
  });

  const { data: accounts = [], isLoading: accountsLoading } = useQuery({
    queryKey: ["accounts"],
    queryFn: fetchAccounts,
    enabled: !!user,
  });

  useEffect(() => {
    if (userError) {
      router.push("/login");
    }
  }, [userError, router]);

  if (userLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
        <div className="relative">
          <div className="w-16 h-16 border-4 border-teal-400/30 rounded-full animate-spin border-t-teal-400"></div>
          <div className="absolute inset-0 w-16 h-16 border-4 border-transparent rounded-full animate-ping border-t-teal-400/50"></div>
        </div>
      </div>
    );
  }

  if (!user) {
    return null;
  }

  // Calculate statistics
  const totalExpenses = expenses.reduce((sum, exp) => sum + exp.amount, 0);
  const totalBalance = accounts.reduce((sum, acc) => sum + acc.balance, 0);
  const recentExpenses = [...expenses]
    .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
    .slice(0, 5);

  // Calculate month-over-month change (mock for now - could be from API)
  const balanceChange = 5.2; // Positive = good
  const expenseChange = -3.1; // Negative = spending less = good

  const handleLogout = async () => {
    try {
      await fetch(`${API_BASE_URL}/api/v1/auth/logout`, {
        method: "POST",
        credentials: "include",
      });
    } catch {
      // Ignore logout errors
    }
    router.push("/");
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 relative overflow-hidden">
      {/* Animated background orbs */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute -top-40 -right-40 w-80 h-80 bg-teal-500 rounded-full mix-blend-multiply filter blur-3xl opacity-15 animate-blob"></div>
        <div className="absolute -bottom-40 -left-40 w-80 h-80 bg-emerald-500 rounded-full mix-blend-multiply filter blur-3xl opacity-15 animate-blob animation-delay-2000"></div>
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-cyan-500 rounded-full mix-blend-multiply filter blur-3xl opacity-10 animate-blob animation-delay-4000"></div>
      </div>

      {/* Navigation */}
      <nav className="relative z-10 bg-slate-800/90 backdrop-blur-sm border-b border-slate-700">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center shadow-lg shadow-teal-500/25">
                <span className="text-white font-bold text-lg">F</span>
              </div>
              <span className="text-xl font-bold bg-gradient-to-r from-white to-teal-200 bg-clip-text text-transparent">
                FinanceTracker
              </span>
            </div>
            <div className="flex items-center gap-6">
              <div className="flex items-center gap-3">
                <div className="w-8 h-8 rounded-full bg-gradient-to-br from-teal-400 to-emerald-400 flex items-center justify-center text-white text-sm font-medium">
                  {user.firstName[0]}
                </div>
                <span className="text-white/80 font-medium">
                  Hey, {user.firstName}!
                </span>
              </div>
              <button
                onClick={handleLogout}
                className="px-4 py-2 rounded-xl bg-slate-700 text-slate-300 hover:bg-slate-600 hover:text-white transition-all duration-200 text-sm font-medium cursor-pointer"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main className="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Summary Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          {/* Total Balance */}
          <GlassCard gradient="green" className="p-6">
            <div className="flex items-start justify-between">
              <div>
                <p className="text-sm font-medium text-white/60 mb-1">
                  Total Balance
                </p>
                <div
                  className={`text-3xl font-bold ${totalBalance >= 0 ? "text-emerald-400" : "text-rose-400"}`}
                >
                  <AnimatedCounter value={totalBalance} prefix="$" />
                </div>
                <div className="mt-2">
                  <TrendIndicator value={balanceChange} label="vs last month" />
                </div>
              </div>
              <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-emerald-400 to-emerald-600 flex items-center justify-center shadow-lg shadow-emerald-500/30">
                <svg
                  className="w-7 h-7 text-white"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
              </div>
            </div>
          </GlassCard>

          {/* Total Expenses */}
          <GlassCard gradient="rose" className="p-6">
            <div className="flex items-start justify-between">
              <div>
                <p className="text-sm font-medium text-white/60 mb-1">
                  Total Expenses
                </p>
                <div className="text-3xl font-bold text-white">
                  <AnimatedCounter value={totalExpenses} prefix="$" />
                </div>
                <div className="mt-2">
                  <TrendIndicator
                    value={expenseChange}
                    label="vs last month"
                    inverted
                  />
                </div>
              </div>
              <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-rose-400 to-rose-600 flex items-center justify-center shadow-lg shadow-rose-500/30">
                <svg
                  className="w-7 h-7 text-white"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 14l6-6m-5.5.5h.01m4.99 5h.01M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16l3.5-2 3.5 2 3.5-2 3.5 2z"
                  />
                </svg>
              </div>
            </div>
          </GlassCard>

          {/* Active Accounts */}
          <GlassCard gradient="blue" className="p-6">
            <div className="flex items-start justify-between">
              <div>
                <p className="text-sm font-medium text-white/60 mb-1">
                  Active Accounts
                </p>
                <div className="text-3xl font-bold text-white">
                  <AnimatedCounter value={accounts.length} decimals={0} />
                </div>
                <p className="mt-2 text-sm text-white/40">
                  Across all institutions
                </p>
              </div>
              <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-blue-400 to-blue-600 flex items-center justify-center shadow-lg shadow-blue-500/30">
                <svg
                  className="w-7 h-7 text-white"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
                  />
                </svg>
              </div>
            </div>
          </GlassCard>
        </div>

        {/* Quick Actions */}
        <div className="flex flex-wrap gap-3 mb-8">
          <Link
            href="/add-expense"
            className="flex items-center gap-2 px-4 py-2 rounded-xl bg-teal-600 text-white hover:bg-teal-500 transition-all duration-200 font-medium"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Add Expense
          </Link>
          <Link
            href="/recurring-transactions"
            className="flex items-center gap-2 px-4 py-2 rounded-xl bg-slate-700 text-white hover:bg-slate-600 transition-all duration-200 font-medium border border-slate-600"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
            </svg>
            Recurring
          </Link>
          <Link
            href="/savings-goals"
            className="flex items-center gap-2 px-4 py-2 rounded-xl bg-slate-700 text-white hover:bg-slate-600 transition-all duration-200 font-medium border border-slate-600"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
            </svg>
            Savings Goals
          </Link>
          <Link
            href="/expenses"
            className="flex items-center gap-2 px-4 py-2 rounded-xl bg-slate-700 text-white hover:bg-slate-600 transition-all duration-200 font-medium border border-slate-600"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            All Expenses
          </Link>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Recent Expenses */}
          <GlassCard className="p-6" hover={false}>
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-lg font-semibold text-white">
                Recent Expenses
              </h2>
              <Link
                href="/expenses"
                className="text-teal-400 hover:text-teal-300 text-sm font-medium transition-colors flex items-center gap-1 group"
              >
                View all
                <svg
                  className="w-4 h-4 transform group-hover:translate-x-1 transition-transform"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 5l7 7-7 7"
                  />
                </svg>
              </Link>
            </div>
            {expensesLoading ? (
              <div className="flex justify-center py-12">
                <div className="w-8 h-8 border-2 border-teal-400/30 rounded-full animate-spin border-t-teal-400"></div>
              </div>
            ) : recentExpenses.length === 0 ? (
              <div className="text-center py-12">
                <div className="w-16 h-16 mx-auto mb-4 rounded-full bg-slate-700 flex items-center justify-center">
                  <svg
                    className="w-8 h-8 text-white/30"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"
                    />
                  </svg>
                </div>
                <p className="text-white/40">No expenses yet</p>
              </div>
            ) : (
              <div className="space-y-2">
                {recentExpenses.map((expense, index) => (
                  <div
                    key={expense.id}
                    className="group flex items-center justify-between p-4 rounded-xl bg-slate-700/50 hover:bg-slate-700 transition-all duration-200 cursor-pointer border border-slate-600/50 hover:border-teal-500/50"
                    style={{ animationDelay: `${index * 100}ms` }}
                  >
                    <div className="flex items-center gap-4">
                      <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500/20 to-emerald-500/20 flex items-center justify-center">
                        <span className="text-lg">
                          {expense.category === "Food & Dining" && "🍽️"}
                          {expense.category === "Shopping" && "🛍️"}
                          {expense.category === "Transportation" && "🚗"}
                          {expense.category === "Housing" && "🏠"}
                          {expense.category === "Utilities" && "💡"}
                          {expense.category === "Healthcare" && "🏥"}
                          {expense.category === "Entertainment" && "🎬"}
                          {expense.category === "Subscriptions" && "📱"}
                          {![
                            "Food & Dining",
                            "Shopping",
                            "Transportation",
                            "Housing",
                            "Utilities",
                            "Healthcare",
                            "Entertainment",
                            "Subscriptions",
                          ].includes(expense.category) && "💰"}
                        </span>
                      </div>
                      <div>
                        <p className="font-medium text-white group-hover:text-teal-200 transition-colors">
                          {expense.name}
                        </p>
                        <p className="text-sm text-white/40">
                          {expense.category}
                        </p>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="font-semibold text-rose-400">
                        -${expense.amount.toFixed(2)}
                      </p>
                      <p className="text-sm text-white/40">
                        {new Date(expense.date).toLocaleDateString("en-US", {
                          month: "short",
                          day: "numeric",
                        })}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </GlassCard>

          {/* Spending by Category */}
          <GlassCard className="p-6" hover={false}>
            <h2 className="text-lg font-semibold text-white mb-4">
              Spending by Category
            </h2>
            <SpendingPieChart enabled={!!user} />
          </GlassCard>

          {/* Month Comparison */}
          <GlassCard className="p-6" hover={false}>
            <h2 className="text-lg font-semibold text-white mb-4">
              This Month vs Last Month
            </h2>
            <MonthComparisonChart enabled={!!user} />
          </GlassCard>

          {/* Monthly Trend */}
          <GlassCard className="p-6" hover={false}>
            <h2 className="text-lg font-semibold text-white mb-4">
              Monthly Trend
            </h2>
            <MonthlyTrendChart months={6} enabled={!!user} />
          </GlassCard>

          {/* Accounts List */}
          <GlassCard className="p-6 lg:col-span-2" hover={false}>
            <h2 className="text-lg font-semibold text-white mb-6">
              Your Accounts
            </h2>
            {accountsLoading ? (
              <div className="flex justify-center py-12">
                <div className="w-8 h-8 border-2 border-teal-400/30 rounded-full animate-spin border-t-teal-400"></div>
              </div>
            ) : accounts.length === 0 ? (
              <div className="text-center py-12">
                <div className="w-16 h-16 mx-auto mb-4 rounded-full bg-slate-700 flex items-center justify-center">
                  <svg
                    className="w-8 h-8 text-white/30"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
                    />
                  </svg>
                </div>
                <p className="text-white/40">No accounts found</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                {accounts.map((account, index) => (
                  <div
                    key={account.id}
                    className="group p-5 rounded-xl bg-slate-700/50 hover:bg-slate-700 transition-all duration-300 cursor-pointer border border-slate-600/50 hover:border-teal-500/50 hover:-translate-y-1"
                    style={{ animationDelay: `${index * 100}ms` }}
                  >
                    <div className="flex items-center gap-3 mb-4">
                      <div
                        className={`w-12 h-12 rounded-xl flex items-center justify-center shadow-lg ${
                          account.accountType === "Checking"
                            ? "bg-gradient-to-br from-blue-400 to-blue-600 shadow-blue-500/30"
                            : account.accountType === "Savings"
                              ? "bg-gradient-to-br from-emerald-400 to-emerald-600 shadow-emerald-500/30"
                              : "bg-gradient-to-br from-purple-400 to-purple-600 shadow-purple-500/30"
                        }`}
                      >
                        {account.accountType === "Credit Card" ? (
                          <svg
                            className="w-6 h-6 text-white"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
                            />
                          </svg>
                        ) : (
                          <svg
                            className="w-6 h-6 text-white"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                            />
                          </svg>
                        )}
                      </div>
                      <div>
                        <p className="font-medium text-white group-hover:text-teal-200 transition-colors">
                          {account.name}
                        </p>
                        <p className="text-xs text-white/40">
                          {account.accountType}
                        </p>
                      </div>
                    </div>
                    <p
                      className={`text-2xl font-bold ${account.balance >= 0 ? "text-white" : "text-rose-400"}`}
                    >
                      $
                      {account.balance.toLocaleString("en-US", {
                        minimumFractionDigits: 2,
                      })}
                    </p>
                  </div>
                ))}
              </div>
            )}
          </GlassCard>

          {/* Savings Goals */}
          <GlassCard className="p-6" hover={false}>
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold text-white">Savings Goals</h2>
              <Link
                href="/savings-goals"
                className="text-teal-400 hover:text-teal-300 text-sm font-medium transition-colors"
              >
                View all
              </Link>
            </div>
            {savingsGoals.length === 0 ? (
              <div className="text-center py-8">
                <p className="text-white/40 mb-3">No savings goals yet</p>
                <Link
                  href="/savings-goals"
                  className="text-teal-400 hover:text-teal-300 text-sm font-medium"
                >
                  Create your first goal
                </Link>
              </div>
            ) : (
              <div className="space-y-4">
                {savingsGoals.filter(g => !g.isCompleted).slice(0, 3).map((goal) => (
                  <div key={goal.id} className="space-y-2">
                    <div className="flex justify-between items-center">
                      <span className="text-white font-medium">{goal.name}</span>
                      <span className="text-teal-400 text-sm">
                        ${goal.currentAmount.toFixed(0)} / ${goal.targetAmount.toFixed(0)}
                      </span>
                    </div>
                    <div className="w-full bg-slate-700 rounded-full h-2 overflow-hidden">
                      <div
                        className="h-full bg-gradient-to-r from-teal-500 to-emerald-500 transition-all duration-500"
                        style={{ width: `${Math.min(goal.progressPercentage, 100)}%` }}
                      />
                    </div>
                    <p className="text-xs text-white/40">{goal.progressPercentage.toFixed(0)}% complete</p>
                  </div>
                ))}
              </div>
            )}
          </GlassCard>

          {/* Upcoming Recurring */}
          <GlassCard className="p-6" hover={false}>
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold text-white">Upcoming Bills</h2>
              <Link
                href="/recurring-transactions"
                className="text-teal-400 hover:text-teal-300 text-sm font-medium transition-colors"
              >
                View all
              </Link>
            </div>
            {recurringTransactions.length === 0 ? (
              <div className="text-center py-8">
                <p className="text-white/40 mb-3">No recurring transactions</p>
                <Link
                  href="/recurring-transactions"
                  className="text-teal-400 hover:text-teal-300 text-sm font-medium"
                >
                  Add recurring transaction
                </Link>
              </div>
            ) : (
              <div className="space-y-3">
                {recurringTransactions
                  .filter(rt => rt.isActive)
                  .sort((a, b) => new Date(a.nextDueDate).getTime() - new Date(b.nextDueDate).getTime())
                  .slice(0, 4)
                  .map((rt) => (
                    <div key={rt.id} className="flex items-center justify-between p-3 rounded-lg bg-slate-700/50">
                      <div>
                        <p className="text-white font-medium">{rt.description}</p>
                        <p className="text-xs text-white/40">
                          Due {new Date(rt.nextDueDate).toLocaleDateString("en-US", { month: "short", day: "numeric" })} • {rt.frequency}
                        </p>
                      </div>
                      <span className={`font-semibold ${rt.type === "Income" ? "text-emerald-400" : "text-rose-400"}`}>
                        {rt.type === "Income" ? "+" : "-"}${rt.amount.toFixed(2)}
                      </span>
                    </div>
                  ))}
              </div>
            )}
          </GlassCard>
        </div>
      </main>

      {/* Budget Assistant Chat */}
      <BudgetAssistant />

      {/* CSS for animations */}
      <style jsx>{`
        @keyframes blob {
          0%,
          100% {
            transform: translate(0, 0) scale(1);
          }
          25% {
            transform: translate(20px, -30px) scale(1.1);
          }
          50% {
            transform: translate(-20px, 20px) scale(0.9);
          }
          75% {
            transform: translate(30px, 10px) scale(1.05);
          }
        }
        .animate-blob {
          animation: blob 8s infinite ease-in-out;
        }
        .animation-delay-2000 {
          animation-delay: 2s;
        }
        .animation-delay-4000 {
          animation-delay: 4s;
        }
      `}</style>
    </div>
  );
}
