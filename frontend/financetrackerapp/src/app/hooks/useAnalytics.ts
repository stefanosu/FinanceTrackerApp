"use client";

import { useQuery } from "@tanstack/react-query";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5280";

export interface SpendingByCategory {
  category: string;
  amount: number;
  percentage: number;
}

export interface FinancialSummary {
  totalIncome: number;
  totalExpenses: number;
  netSavings: number;
  savingsRate: number;
  totalBalance: number;
}

export interface MonthlyTrend {
  month: string;
  year: number;
  income: number;
  expenses: number;
  netSavings: number;
}

export interface MonthComparison {
  category: string;
  thisMonth: number;
  lastMonth: number;
  change: number;
  changePercent: number;
}

export interface AnalyticsSummary {
  financialSummary: FinancialSummary;
  spendingByCategory: SpendingByCategory[];
  monthlyTrend: MonthlyTrend[];
}

async function fetchAnalyticsSummary(
  startDate?: string,
  endDate?: string,
): Promise<AnalyticsSummary> {
  const params = new URLSearchParams();
  if (startDate) params.append("startDate", startDate);
  if (endDate) params.append("endDate", endDate);

  const url = `${API_BASE_URL}/api/Analytics/summary${params.toString() ? `?${params}` : ""}`;
  const response = await fetch(url, {
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to fetch analytics summary");
  return response.json();
}

async function fetchSpendingByCategory(
  startDate?: string,
  endDate?: string,
): Promise<SpendingByCategory[]> {
  const params = new URLSearchParams();
  if (startDate) params.append("startDate", startDate);
  if (endDate) params.append("endDate", endDate);

  const url = `${API_BASE_URL}/api/Analytics/spending-by-category${params.toString() ? `?${params}` : ""}`;
  const response = await fetch(url, {
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to fetch spending by category");
  return response.json();
}

async function fetchFinancialSummary(
  startDate?: string,
  endDate?: string,
): Promise<FinancialSummary> {
  const params = new URLSearchParams();
  if (startDate) params.append("startDate", startDate);
  if (endDate) params.append("endDate", endDate);

  const url = `${API_BASE_URL}/api/Analytics/financial-summary${params.toString() ? `?${params}` : ""}`;
  const response = await fetch(url, {
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to fetch financial summary");
  return response.json();
}

async function fetchMonthlyTrend(months: number = 6): Promise<MonthlyTrend[]> {
  const response = await fetch(
    `${API_BASE_URL}/api/Analytics/monthly-trend?months=${months}`,
    {
      credentials: "include",
    },
  );

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Monthly trend API error:", response.status, errorText);
    throw new Error(`Failed to fetch monthly trend: ${response.status}`);
  }
  return response.json();
}

export function useAnalyticsSummary(
  startDate?: string,
  endDate?: string,
  enabled: boolean = true,
) {
  return useQuery({
    queryKey: ["analytics", "summary", startDate, endDate],
    queryFn: () => fetchAnalyticsSummary(startDate, endDate),
    enabled,
  });
}

export function useSpendingByCategory(
  startDate?: string,
  endDate?: string,
  enabled: boolean = true,
) {
  return useQuery({
    queryKey: ["analytics", "spending-by-category", startDate, endDate],
    queryFn: () => fetchSpendingByCategory(startDate, endDate),
    enabled,
  });
}

export function useFinancialSummary(
  startDate?: string,
  endDate?: string,
  enabled: boolean = true,
) {
  return useQuery({
    queryKey: ["analytics", "financial-summary", startDate, endDate],
    queryFn: () => fetchFinancialSummary(startDate, endDate),
    enabled,
  });
}

export function useMonthlyTrend(months: number = 6, enabled: boolean = true) {
  return useQuery({
    queryKey: ["analytics", "monthly-trend", months],
    queryFn: () => fetchMonthlyTrend(months),
    enabled,
  });
}

async function fetchMonthComparison(): Promise<MonthComparison[]> {
  const response = await fetch(
    `${API_BASE_URL}/api/Analytics/month-comparison`,
    {
      credentials: "include",
    },
  );

  if (!response.ok) {
    const errorText = await response.text();
    console.error("Month comparison API error:", response.status, errorText);
    throw new Error(`Failed to fetch month comparison: ${response.status}`);
  }
  return response.json();
}

export function useMonthComparison(enabled: boolean = true) {
  return useQuery({
    queryKey: ["analytics", "month-comparison"],
    queryFn: fetchMonthComparison,
    enabled,
  });
}
