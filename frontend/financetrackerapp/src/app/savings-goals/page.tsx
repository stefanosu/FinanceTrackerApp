"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import Link from "next/link";

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5280";

interface SavingsGoal {
  id: number;
  name: string;
  description: string;
  targetAmount: number;
  currentAmount: number;
  progressPercentage: number;
  targetDate: string | null;
  accountId: number | null;
  category: string;
  isCompleted: boolean;
  completedAt: string | null;
}

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
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
  if (!response.ok) throw new Error("Failed to fetch savings goals");
  return response.json();
}

async function createSavingsGoal(data: {
  name: string;
  description?: string;
  targetAmount: number;
  initialAmount?: number;
  targetDate?: string;
  category: string;
}): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/SavingsGoal/create`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: "Failed" }));
    throw new Error(error.detail || error.title || "Failed to create goal");
  }
}

async function contributeToGoal(
  id: number,
  amount: number
): Promise<SavingsGoal> {
  const response = await fetch(
    `${API_BASE_URL}/api/SavingsGoal/${id}/contribute`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ amount }),
    }
  );
  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: "Failed" }));
    throw new Error(error.detail || error.title || "Failed to contribute");
  }
  return response.json();
}

async function deleteSavingsGoal(id: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/SavingsGoal/${id}`, {
    method: "DELETE",
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to delete");
}

const CATEGORIES = [
  "Emergency Fund",
  "Vacation",
  "Home",
  "Car",
  "Education",
  "Retirement",
  "Wedding",
  "Electronics",
  "Other",
];

export default function SavingsGoalsPage() {
  const router = useRouter();
  const queryClient = useQueryClient();

  const [showForm, setShowForm] = useState(false);
  const [contributeGoalId, setContributeGoalId] = useState<number | null>(null);
  const [contributeAmount, setContributeAmount] = useState("");
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    targetAmount: "",
    initialAmount: "",
    targetDate: "",
    category: "Emergency Fund",
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

  const { data: goals = [], isLoading: goalsLoading } = useQuery({
    queryKey: ["savingsGoals"],
    queryFn: fetchSavingsGoals,
    enabled: !!user,
  });

  const createMutation = useMutation({
    mutationFn: createSavingsGoal,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["savingsGoals"] });
      setShowForm(false);
      setFormData({
        name: "",
        description: "",
        targetAmount: "",
        initialAmount: "",
        targetDate: "",
        category: "Emergency Fund",
      });
    },
    onError: (error: Error) => setError(error.message),
  });

  const contributeMutation = useMutation({
    mutationFn: ({ id, amount }: { id: number; amount: number }) =>
      contributeToGoal(id, amount),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["savingsGoals"] });
      setContributeGoalId(null);
      setContributeAmount("");
    },
  });

  const deleteMutation = useMutation({
    mutationFn: deleteSavingsGoal,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["savingsGoals"] });
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

    if (!formData.name || !formData.targetAmount) {
      setError("Please fill in all required fields");
      return;
    }

    createMutation.mutate({
      name: formData.name,
      description: formData.description || undefined,
      targetAmount: parseFloat(formData.targetAmount),
      initialAmount: formData.initialAmount
        ? parseFloat(formData.initialAmount)
        : undefined,
      targetDate: formData.targetDate || undefined,
      category: formData.category,
    });
  };

  const handleContribute = (e: React.FormEvent) => {
    e.preventDefault();
    if (!contributeGoalId || !contributeAmount) return;

    contributeMutation.mutate({
      id: contributeGoalId,
      amount: parseFloat(contributeAmount),
    });
  };

  const handleChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement
    >
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const activeGoals = goals.filter((g) => !g.isCompleted);
  const completedGoals = goals.filter((g) => g.isCompleted);
  const totalSaved = goals.reduce((sum, g) => sum + g.currentAmount, 0);
  const totalTarget = goals.reduce((sum, g) => sum + g.targetAmount, 0);

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
                New Goal
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Summary */}
        <div className="grid grid-cols-3 gap-4 mb-8">
          <div className="bg-white rounded-xl shadow-sm p-4">
            <p className="text-sm text-gray-500">Total Saved</p>
            <p className="text-2xl font-bold text-emerald-600">
              ${totalSaved.toFixed(2)}
            </p>
          </div>
          <div className="bg-white rounded-xl shadow-sm p-4">
            <p className="text-sm text-gray-500">Total Target</p>
            <p className="text-2xl font-bold text-gray-900">
              ${totalTarget.toFixed(2)}
            </p>
          </div>
          <div className="bg-white rounded-xl shadow-sm p-4">
            <p className="text-sm text-gray-500">Goals Completed</p>
            <p className="text-2xl font-bold text-indigo-600">
              {completedGoals.length}/{goals.length}
            </p>
          </div>
        </div>

        <h1 className="text-2xl font-bold text-gray-900 mb-6">Savings Goals</h1>

        {/* Create Form Modal */}
        {showForm && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-white rounded-xl shadow-xl p-6 w-full max-w-md mx-4">
              <h2 className="text-xl font-bold text-gray-900 mb-4">
                Create New Goal
              </h2>
              <form onSubmit={handleSubmit} className="space-y-4">
                {error && (
                  <div className="bg-red-50 text-red-600 p-3 rounded-lg text-sm">
                    {error}
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Goal Name *
                  </label>
                  <input
                    type="text"
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                    placeholder="e.g., Emergency Fund"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Category
                  </label>
                  <select
                    name="category"
                    value={formData.category}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                  >
                    {CATEGORIES.map((c) => (
                      <option key={c} value={c}>
                        {c}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Target Amount *
                    </label>
                    <div className="relative">
                      <span className="absolute left-3 top-2 text-gray-500">
                        $
                      </span>
                      <input
                        type="number"
                        name="targetAmount"
                        value={formData.targetAmount}
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
                      Starting Amount
                    </label>
                    <div className="relative">
                      <span className="absolute left-3 top-2 text-gray-500">
                        $
                      </span>
                      <input
                        type="number"
                        name="initialAmount"
                        value={formData.initialAmount}
                        onChange={handleChange}
                        step="0.01"
                        min="0"
                        className="w-full pl-7 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                      />
                    </div>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Target Date
                  </label>
                  <input
                    type="date"
                    name="targetDate"
                    value={formData.targetDate}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Description
                  </label>
                  <textarea
                    name="description"
                    value={formData.description}
                    onChange={handleChange}
                    rows={2}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                    placeholder="Why are you saving for this?"
                  />
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
                    {createMutation.isPending ? "Creating..." : "Create Goal"}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}

        {/* Contribute Modal */}
        {contributeGoalId && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-white rounded-xl shadow-xl p-6 w-full max-w-sm mx-4">
              <h2 className="text-xl font-bold text-gray-900 mb-4">
                Add Contribution
              </h2>
              <form onSubmit={handleContribute} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Amount
                  </label>
                  <div className="relative">
                    <span className="absolute left-3 top-2 text-gray-500">
                      $
                    </span>
                    <input
                      type="number"
                      value={contributeAmount}
                      onChange={(e) => setContributeAmount(e.target.value)}
                      step="0.01"
                      min="0.01"
                      className="w-full pl-7 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
                      required
                      autoFocus
                    />
                  </div>
                </div>

                <div className="flex justify-end space-x-4">
                  <button
                    type="button"
                    onClick={() => {
                      setContributeGoalId(null);
                      setContributeAmount("");
                    }}
                    className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    disabled={contributeMutation.isPending}
                    className="px-4 py-2 bg-emerald-600 text-white rounded-lg hover:bg-emerald-700 disabled:opacity-50"
                  >
                    {contributeMutation.isPending ? "Adding..." : "Add"}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}

        {/* Content */}
        {goalsLoading ? (
          <div className="flex justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600"></div>
          </div>
        ) : goals.length === 0 ? (
          <div className="bg-white rounded-xl shadow-sm p-12 text-center">
            <p className="text-gray-500 mb-4">No savings goals yet</p>
            <button
              onClick={() => setShowForm(true)}
              className="inline-block bg-indigo-600 text-white px-6 py-2 rounded-lg hover:bg-indigo-700"
            >
              Create your first goal
            </button>
          </div>
        ) : (
          <div className="space-y-6">
            {/* Active Goals */}
            {activeGoals.length > 0 && (
              <div className="grid gap-4">
                {activeGoals.map((goal) => (
                  <div
                    key={goal.id}
                    className="bg-white rounded-xl shadow-sm p-6"
                  >
                    <div className="flex justify-between items-start mb-4">
                      <div>
                        <h3 className="text-lg font-semibold text-gray-900">
                          {goal.name}
                        </h3>
                        <p className="text-sm text-gray-500">
                          {goal.category}
                          {goal.targetDate &&
                            ` • Target: ${new Date(goal.targetDate).toLocaleDateString()}`}
                        </p>
                        {goal.description && (
                          <p className="text-sm text-gray-400 mt-1">
                            {goal.description}
                          </p>
                        )}
                      </div>
                      <button
                        onClick={() => {
                          if (confirm("Delete this goal?")) {
                            deleteMutation.mutate(goal.id);
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

                    {/* Progress Bar */}
                    <div className="mb-4">
                      <div className="flex justify-between text-sm mb-1">
                        <span className="font-medium text-gray-900">
                          ${goal.currentAmount.toFixed(2)}
                        </span>
                        <span className="text-gray-500">
                          ${goal.targetAmount.toFixed(2)}
                        </span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-3 overflow-hidden">
                        <div
                          className="h-full bg-gradient-to-r from-indigo-500 to-emerald-500 transition-all duration-500"
                          style={{
                            width: `${Math.min(goal.progressPercentage, 100)}%`,
                          }}
                        />
                      </div>
                      <p className="text-sm text-gray-500 mt-1">
                        {goal.progressPercentage.toFixed(1)}% complete • $
                        {(goal.targetAmount - goal.currentAmount).toFixed(2)}{" "}
                        remaining
                      </p>
                    </div>

                    <button
                      onClick={() => setContributeGoalId(goal.id)}
                      className="w-full py-2 bg-emerald-600 text-white rounded-lg hover:bg-emerald-700"
                    >
                      Add Contribution
                    </button>
                  </div>
                ))}
              </div>
            )}

            {/* Completed Goals */}
            {completedGoals.length > 0 && (
              <div>
                <h2 className="text-lg font-semibold text-gray-700 mb-3">
                  Completed
                </h2>
                <div className="grid gap-4">
                  {completedGoals.map((goal) => (
                    <div
                      key={goal.id}
                      className="bg-white rounded-xl shadow-sm p-4 border-l-4 border-emerald-500"
                    >
                      <div className="flex justify-between items-center">
                        <div>
                          <h3 className="font-semibold text-gray-900">
                            {goal.name}
                          </h3>
                          <p className="text-sm text-gray-500">
                            ${goal.targetAmount.toFixed(2)} •{" "}
                            {goal.completedAt &&
                              `Completed ${new Date(goal.completedAt).toLocaleDateString()}`}
                          </p>
                        </div>
                        <span className="text-emerald-600 font-semibold">
                          ✓ Done
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        )}
      </main>
    </div>
  );
}
