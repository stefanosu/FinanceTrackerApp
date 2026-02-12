"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";

export default function Home() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check if user has a valid session by making a request to a protected endpoint
    const checkAuth = async () => {
      try {
        const response = await fetch("http://localhost:5280/api/User/me", {
          credentials: "include",
        });
        if (response.ok) {
          router.push("/dashboard");
        } else {
          setIsLoading(false);
        }
      } catch {
        setIsLoading(false);
      }
    };
    checkAuth();
  }, [router]);

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-50 to-blue-100">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-50 to-blue-100">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Navigation */}
        <nav className="flex justify-between items-center py-6">
          <div className="flex items-center space-x-2">
            <div className="w-10 h-10 bg-indigo-600 rounded-lg flex items-center justify-center">
              <span className="text-white font-bold text-xl">F</span>
            </div>
            <span className="text-xl font-bold text-gray-900">
              FinanceTracker
            </span>
          </div>
          <div className="flex items-center space-x-4">
            <Link
              href="/login"
              className="text-gray-600 hover:text-gray-900 font-medium"
            >
              Sign in
            </Link>
            <Link
              href="/signup"
              className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors font-medium"
            >
              Get Started
            </Link>
          </div>
        </nav>

        {/* Hero Section */}
        <main className="py-20">
          <div className="text-center max-w-3xl mx-auto">
            <h1 className="text-5xl font-extrabold text-gray-900 mb-6">
              Take Control of Your{" "}
              <span className="text-indigo-600">Finances</span>
            </h1>
            <p className="text-xl text-gray-600 mb-10">
              Track expenses, manage budgets, and achieve your financial goals
              with our intuitive finance tracking app. Get insights into your
              spending habits and make smarter money decisions.
            </p>
            <div className="flex justify-center space-x-4">
              <Link
                href="/signup"
                className="bg-indigo-600 text-white px-8 py-3 rounded-lg hover:bg-indigo-700 transition-colors font-medium text-lg shadow-lg shadow-indigo-200"
              >
                Start Free Today
              </Link>
              <Link
                href="/login"
                className="bg-white text-gray-700 px-8 py-3 rounded-lg hover:bg-gray-50 transition-colors font-medium text-lg border border-gray-200"
              >
                Sign In
              </Link>
            </div>
          </div>

          {/* Features */}
          <div className="mt-24 grid md:grid-cols-3 gap-8">
            <div className="bg-white p-8 rounded-2xl shadow-sm">
              <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center mb-4">
                <svg
                  className="w-6 h-6 text-green-600"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
                  />
                </svg>
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">
                Track Expenses
              </h3>
              <p className="text-gray-600">
                Easily log and categorize your daily expenses. See where your
                money goes with detailed breakdowns.
              </p>
            </div>

            <div className="bg-white p-8 rounded-2xl shadow-sm">
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
                <svg
                  className="w-6 h-6 text-blue-600"
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
              <h3 className="text-xl font-semibold text-gray-900 mb-2">
                Manage Accounts
              </h3>
              <p className="text-gray-600">
                Connect multiple accounts and track balances across checking,
                savings, and credit cards in one place.
              </p>
            </div>

            <div className="bg-white p-8 rounded-2xl shadow-sm">
              <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center mb-4">
                <svg
                  className="w-6 h-6 text-purple-600"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"
                  />
                </svg>
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">
                AI Budget Assistant
              </h3>
              <p className="text-gray-600">
                Get personalized insights and recommendations from our AI
                assistant to help you reach your financial goals.
              </p>
            </div>
          </div>
        </main>

        {/* Footer */}
        <footer className="py-8 text-center text-gray-500">
          <p>
            Built as a portfolio project demonstrating full-stack development
          </p>
        </footer>
      </div>
    </div>
  );
}
