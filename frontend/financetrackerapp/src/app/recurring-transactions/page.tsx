"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import Link from "next/link";

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5280";

interface RecurringTransaction {
  id: number;
  accountId: number;
  amount: number;
  type: string;
  frequency: string;
  description: string;
  categoryId: number;
  startDate: string;
  endDate: string | null;
  nextDueDate: string;
  isActive: boolean;
}

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

interface Account {
  id: number;
  name: string;
  type: string;
  balance: number;
}

async function fetchCurrentUser(): Promise<User> {
  const response = await fetch(`${API_BASE_URL}/api/User/me`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Not authenticated");
  return response.json();
}

async function fetchRecurringTransactions(): Promise<RecurringTransaction[]> {
  const response = await fetch(`${API_BASE_URL}/api/RecurringTransaction/all`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch recurring transactions");
  return response.json();
}

async function fetchAccounts(): Promise<Account[]> {
  const response = await fetch(`${API_BASE_URL}/api/Account/all`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch accounts");
  return response.json();
}

async function createRecurringTransaction(data: {
  accountId: number;
  amount: number;
  type: string;
  frequency: string;
  description: string;
  categoryId: number;
  startDate: string;
  endDate?: string;
}): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/api/RecurringTransaction/create`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(data),
    }
  );
  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: "Failed" }));
    throw new Error(error.detail || error.title || "Failed to create");
  }
}

async function deleteRecurringTransaction(id: number): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/api/RecurringTransaction/${id}`,
    {
      method: "DELETE",
      credentials: "include",
    }
  );
  if (!response.ok) throw new Error("Failed to delete");
}

async function processDueTransactions(): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/api/RecurringTransaction/process-due`,
    {
      method: "POST",
      credentials: "include",
    }
  );
  if (!response.ok) throw new Error("Failed to process");
}

const FREQUENCIES = [
  "Daily",
  "Weekly",
  "Biweekly",
  "Monthly",
  "Quarterly",
  "Yearly",
];
const TYPES = ["Income", "Expense"];

export default function RecurringTransactionsPage() {
  const router = useRouter();
  const queryClient = useQueryClient();

  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({
    accountId: "",
    amount: "",
    type: "Expense",
    frequency: "Monthly",
    description: "",
    categoryId: "1",
    startDate: new Date().toISOString().split("T")[0],
    endDate: "",
  });
  const [error, setError] = useState("");

  const {
    data: user,
    isLoading: userLoading,
    error: userError,
  } = useQuery({
    queryKey: ["currentUser"],
    queryFn: fetchCurrentUser,
    retry: false,
  });

  const { data: recurring = [], isLoading: recurringLoading } = useQuery({
    queryKey: ["recurringTransactions"],
    queryFn: fetchRecurringTransactions,
    enabled: !!user,
  });

  const { data: accounts = [] } = useQuery({
    queryKey: ["accounts"],
    queryFn: fetchAccounts,
    enabled: !!user,
  });

  const createMutation = useMutation({
    mutationFn: createRecurringTransaction,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["recurringTransactions"] });
      setShowForm(false);
      setFormData({
        accountId: "",
        amount: "",
        type: "Expense",
        frequency: "Monthly",
        description: "",
        categoryId: "1",
        startDate: new Date().toISOString().split("T")[0],
        endDate: "",
      });
    },
    onError: (error: Error) => setError(error.message),
  });

  const deleteMutation = useMutation({
    mutationFn: deleteRecurringTransaction,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["recurringTransactions"] });
    },
  });

  const processMutation = useMutation({
    mutationFn: processDueTransactions,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["recurringTransactions"] });
      queryClient.invalidateQueries({ queryKey: ["transactions"] });
    },
  });

  useEffect(() => {
    if (userError) router.push("/login");
  }, [userError, router]);

  if (userLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  if (!user) return null;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (!formData.accountId || !formData.amount || !formData.description) {
      setError("Please fill in all required fields");
      return;
    }

    createMutation.mutate({
      accountId: parseInt(formData.accountId),
      amount: parseFloat(formData.amount),
      type: formData.type,
      frequency: formData.frequency,
      description: formData.description,
      categoryId: parseInt(formData.categoryId),
      startDate: formData.startDate,
      endDate: formData.endDate || undefined,
    });
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  // Group by frequency
  const byFrequency = recurring.reduce(
    (acc, rt) => {
      if (!acc[rt.frequency]) acc[rt.frequency] = [];
      acc[rt.frequency].push(rt);
      return acc;
    },
    {} as Record<string, RecurringTransaction[]>
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
              <button
                onClick={() => setShowForm(true)}
                className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
              >
                Add Recurring
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">
              Recurring Transactions
            </h1>
            <p className="text-gray-500">
              {recurring.length} recurring transaction
              {recurring.length !== 1 ? "s" : ""}
            </p>
          </div>
          <button
            onClick={() => processMutation.mutate()}
            disabled={processMutation.isPending}
            className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50"
          >
            {processMutation.isPending ? "Processing..." : "Process Due"}
          </button>
        </div>

        {/* Add Form Modal */}
        {showForm && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-white rounded-xl shadow-xl p-6 w-full max-w-md mx-4">
              <h2 className="text-xl font-bold text-gray-900 mb-4">
                Add Recurring Transaction
              </h2>
              <form onSubmit={handleSubmit} className="space-y-4">
                {error && (
                  <div className="bg-red-50 text-red-600 p-3 rounded-lg text-sm">
                    {error}
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Description *
                  </label>
                  <input
                    type="text"
                    name="description"
                    value={formData.description}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                    placeholder="e.g., Netflix subscription"
                    required
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Amount *
                    </label>
                    <div className="relative">
                      <span className="absolute left-3 top-2 text-gray-500">
                        $
                      </span>
                      <input
                        type="number"
                        name="amount"
                        value={formData.amount}
                        onChange={handleChange}
                        step="0.01"
                        min="0"
                        className="w-full pl-7 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                        required
                      />
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Type
                    </label>
                    <select
                      name="type"
                      value={formData.type}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                    >
                      {TYPES.map((t) => (
                        <option key={t} value={t}>
                          {t}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Frequency
                    </label>
                    <select
                      name="frequency"
                      value={formData.frequency}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                    >
                      {FREQUENCIES.map((f) => (
                        <option key={f} value={f}>
                          {f}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Account *
                    </label>
                    <select
                      name="accountId"
                      value={formData.accountId}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                      required
                    >
                      <option value="">Select account</option>
                      {accounts.map((acc) => (
                        <option key={acc.id} value={acc.id}>
                          {acc.name}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Start Date *
                    </label>
                    <input
                      type="date"
                      name="startDate"
                      value={formData.startDate}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                      required
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      End Date
                    </label>
                    <input
                      type="date"
                      name="endDate"
                      value={formData.endDate}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                    />
                  </div>
                </div>

                <div className="flex justify-end space-x-4 pt-4">
                  <button
                    type="button"
                    onClick={() => setShowForm(false)}
                    className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    disabled={createMutation.isPending}
                    className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-50"
                  >
                    {createMutation.isPending ? "Adding..." : "Add"}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}

        {/* Content */}
        {recurringLoading ? (
          <div className="flex justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600"></div>
          </div>
        ) : recurring.length === 0 ? (
          <div className="bg-white rounded-xl shadow-sm p-12 text-center">
            <p className="text-gray-500 mb-4">No recurring transactions yet</p>
            <button
              onClick={() => setShowForm(true)}
              className="inline-block bg-indigo-600 text-white px-6 py-2 rounded-lg hover:bg-indigo-700"
            >
              Add your first recurring transaction
            </button>
          </div>
        ) : (
          <div className="space-y-6">
            {Object.entries(byFrequency).map(([frequency, items]) => (
              <div key={frequency}>
                <h2 className="text-lg font-semibold text-gray-700 mb-3">
                  {frequency}
                </h2>
                <div className="bg-white rounded-xl shadow-sm divide-y">
                  {items.map((rt) => (
                    <div
                      key={rt.id}
                      className="p-4 flex items-center justify-between hover:bg-gray-50"
                    >
                      <div className="flex-1">
                        <div className="flex items-center">
                          <p className="font-medium text-gray-900">
                            {rt.description}
                          </p>
                          <span
                            className={`ml-2 px-2 py-0.5 text-xs rounded ${
                              rt.isActive
                                ? "bg-green-100 text-green-700"
                                : "bg-gray-100 text-gray-600"
                            }`}
                          >
                            {rt.isActive ? "Active" : "Paused"}
                          </span>
                          <span
                            className={`ml-2 px-2 py-0.5 text-xs rounded ${
                              rt.type === "Income"
                                ? "bg-emerald-100 text-emerald-700"
                                : "bg-rose-100 text-rose-700"
                            }`}
                          >
                            {rt.type}
                          </span>
                        </div>
                        <p className="text-sm text-gray-500 mt-1">
                          Next due:{" "}
                          {new Date(rt.nextDueDate).toLocaleDateString()}
                          {rt.endDate &&
                            ` • Ends: ${new Date(rt.endDate).toLocaleDateString()}`}
                        </p>
                      </div>
                      <div className="flex items-center space-x-4">
                        <p
                          className={`text-lg font-semibold ${
                            rt.type === "Income"
                              ? "text-emerald-600"
                              : "text-rose-600"
                          }`}
                        >
                          {rt.type === "Income" ? "+" : "-"}${rt.amount.toFixed(2)}
                        </p>
                        <button
                          onClick={() => {
                            if (confirm("Delete this recurring transaction?")) {
                              deleteMutation.mutate(rt.id);
                            }
                          }}
                          className="text-gray-400 hover:text-red-600"
                        >
                          <svg
                            className="w-5 h-5"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                            />
                          </svg>
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        )}
      </main>
    </div>
  );
}
