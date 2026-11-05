"use client";

import { useQuery } from "@tanstack/react-query";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5280";

export interface SpendingByMood {
  mood: string;
  amount: number;
  percentage: number;
  transactionCount: number;
  averageTransaction: number;
}

export interface MoodTrend {
  month: string;
  year: number;
  dominantMood: string;
  totalSpending: number;
  moodBreakdown: Record<string, number>;
}

export interface EmotionalInsights {
  highestSpendingMood: string;
  highestSpendingMoodAmount: number;
  mostFrequentMood: string;
  mostFrequentMoodCount: number;
  averageEmotionalSpending: number;
  percentageWithMoodTracked: number;
}

export interface EmotionalAnalyticsSummary {
  spendingByMood: SpendingByMood[];
  moodTrend: MoodTrend[];
  insights: EmotionalInsights;
}

async function fetchEmotionalAnalyticsSummary(
  startDate?: string,
  endDate?: string,
): Promise<EmotionalAnalyticsSummary> {
  const params = new URLSearchParams();
  if (startDate) params.append("startDate", startDate);
  if (endDate) params.append("endDate", endDate);

  const url = `${API_BASE_URL}/api/EmotionalAnalytics/summary${params.toString() ? `?${params}` : ""}`;
  const response = await fetch(url, {
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to fetch emotional analytics summary");
  return response.json();
}

async function fetchSpendingByMood(
  startDate?: string,
  endDate?: string,
): Promise<SpendingByMood[]> {
  const params = new URLSearchParams();
  if (startDate) params.append("startDate", startDate);
  if (endDate) params.append("endDate", endDate);

  const url = `${API_BASE_URL}/api/EmotionalAnalytics/spending-by-mood${params.toString() ? `?${params}` : ""}`;
  const response = await fetch(url, {
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to fetch spending by mood");
  return response.json();
}

async function fetchEmotionalInsights(
  startDate?: string,
  endDate?: string,
): Promise<EmotionalInsights> {
  const params = new URLSearchParams();
  if (startDate) params.append("startDate", startDate);
  if (endDate) params.append("endDate", endDate);

  const url = `${API_BASE_URL}/api/EmotionalAnalytics/insights${params.toString() ? `?${params}` : ""}`;
  const response = await fetch(url, {
    credentials: "include",
  });

  if (!response.ok) throw new Error("Failed to fetch emotional insights");
  return response.json();
}

async function fetchMoodTrend(months: number = 6): Promise<MoodTrend[]> {
  const response = await fetch(
    `${API_BASE_URL}/api/EmotionalAnalytics/mood-trend?months=${months}`,
    {
      credentials: "include",
    },
  );

  if (!response.ok) throw new Error("Failed to fetch mood trend");
  return response.json();
}

export function useEmotionalAnalyticsSummary(
  startDate?: string,
  endDate?: string,
  enabled: boolean = true,
) {
  return useQuery({
    queryKey: ["emotional-analytics", "summary", startDate, endDate],
    queryFn: () => fetchEmotionalAnalyticsSummary(startDate, endDate),
    enabled,
  });
}

export function useSpendingByMood(
  startDate?: string,
  endDate?: string,
  enabled: boolean = true,
) {
  return useQuery({
    queryKey: ["emotional-analytics", "spending-by-mood", startDate, endDate],
    queryFn: () => fetchSpendingByMood(startDate, endDate),
    enabled,
  });
}

export function useEmotionalInsights(
  startDate?: string,
  endDate?: string,
  enabled: boolean = true,
) {
  return useQuery({
    queryKey: ["emotional-analytics", "insights", startDate, endDate],
    queryFn: () => fetchEmotionalInsights(startDate, endDate),
    enabled,
  });
}

export function useMoodTrend(months: number = 6, enabled: boolean = true) {
  return useQuery({
    queryKey: ["emotional-analytics", "mood-trend", months],
    queryFn: () => fetchMoodTrend(months),
    enabled,
  });
}
