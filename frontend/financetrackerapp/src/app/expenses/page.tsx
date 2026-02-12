"use client";

import { useQuery } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import Link from "next/link";

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

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

async function fetchExpenses(): Promise<Expense[]> {
  const response = await fetch(`${API_BASE_URL}/api/Expense/all`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch expenses");
  return response.json();
}

async function fetchCurrentUser(): Promise<User> {
  const response = await fetch(`${API_BASE_URL}/api/User/me`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Not authenticated");
  return response.json();
}

export default function ExpensesPage() {
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

  useEffect(() => {
    if (userError) {
      router.push("/login");
    }
  }, [userError, router]);

  if (userLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  if (!user) {
    return null;
  }

  // Group expenses by month
  const expensesByMonth = expenses.reduce(
    (acc, exp) => {
      const month = new Date(exp.date).toLocaleDateString("en-US", {
        year: "numeric",
        month: "long",
      });
      if (!acc[month]) acc[month] = [];
      acc[month].push(exp);
      return acc;
    },
    {} as Record<string, Expense[]>,
  );

  const sortedMonths = Object.keys(expensesByMonth).sort(
    (a, b) => new Date(b).getTime() - new Date(a).getTime(),
  );

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Navigation */}
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <Link href="/dashboard" className="flex items-center">
                <div className="w-8 h-8 bg-indigo-600 rounded-lg flex items-center justify-center">
                  <span className="text-white font-bold">F</span>
                </div>
                <span className="ml-2 text-lg font-semibold text-gray-900">
                  FinanceTracker
                </span>
              </Link>
            </div>
            <div className="flex items-center space-x-4">
              <Link
                href="/dashboard"
                className="text-gray-600 hover:text-gray-900"
              >
                Dashboard
              </Link>
              <Link
                href="/add-expense"
                className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
              >
                Add Expense
              </Link>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">All Expenses</h1>
          <p className="text-gray-500">{expenses.length} total expenses</p>
        </div>

        {expensesLoading ? (
          <div className="flex justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600"></div>
          </div>
        ) : expenses.length === 0 ? (
          <div className="bg-white rounded-xl shadow-sm p-12 text-center">
            <p className="text-gray-500 mb-4">No expenses yet</p>
            <Link
              href="/add-expense"
              className="inline-block bg-indigo-600 text-white px-6 py-2 rounded-lg hover:bg-indigo-700"
            >
              Add your first expense
            </Link>
          </div>
        ) : (
          <div className="space-y-8">
            {sortedMonths.map((month) => (
              <div key={month}>
                <h2 className="text-lg font-semibold text-gray-700 mb-3">
                  {month}
                </h2>
                <div className="bg-white rounded-xl shadow-sm divide-y">
                  {expensesByMonth[month]
                    .sort(
                      (a, b) =>
                        new Date(b.date).getTime() - new Date(a.date).getTime(),
                    )
                    .map((expense) => (
                      <div
                        key={expense.id}
                        className="p-4 flex items-center justify-between hover:bg-gray-50"
                      >
                        <div className="flex-1">
                          <div className="flex items-center">
                            <p className="font-medium text-gray-900">
                              {expense.name}
                            </p>
                            <span className="ml-2 px-2 py-0.5 text-xs bg-gray-100 text-gray-600 rounded">
                              {expense.category}
                            </span>
                          </div>
                          <p className="text-sm text-gray-500 mt-1">
                            {expense.paymentMethod} •{" "}
                            {new Date(expense.date).toLocaleDateString()}
                          </p>
                          {expense.notes && (
                            <p className="text-sm text-gray-400 mt-1">
                              {expense.notes}
                            </p>
                          )}
                        </div>
                        <p className="text-lg font-semibold text-red-600">
                          -${expense.amount.toFixed(2)}
                        </p>
                      </div>
                    ))}
                </div>
                <p className="text-right text-sm text-gray-500 mt-2">
                  Month total: $
                  {expensesByMonth[month]
                    .reduce((sum, e) => sum + e.amount, 0)
                    .toFixed(2)}
                </p>
              </div>
            ))}
          </div>
        )}
      </main>
    </div>
  );
}
