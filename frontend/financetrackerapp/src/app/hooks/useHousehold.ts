"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5280";

export interface HouseholdMember {
  id: number;
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  joinedAt: string;
}

export interface Household {
  id: number;
  name: string;
  inviteCode: string;
  ownerId: number;
  ownerName: string;
  createdAt: string;
  members: HouseholdMember[];
}

export interface HouseholdResponse {
  hasHousehold: boolean;
  household?: Household;
}

export interface HouseholdSummary {
  id: number;
  name: string;
  memberCount: number;
  totalSharedExpenses: number;
  thisMonthSharedExpenses: number;
}

async function fetchHousehold(): Promise<HouseholdResponse> {
  const response = await fetch(`${API_BASE_URL}/api/Household`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch household");
  return response.json();
}

async function fetchHouseholdSummary(): Promise<HouseholdSummary | null> {
  const response = await fetch(`${API_BASE_URL}/api/Household/summary`, {
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to fetch household summary");
  const data = await response.json();
  return data.hasHousehold === false ? null : data;
}

async function createHousehold(name: string): Promise<Household> {
  const response = await fetch(`${API_BASE_URL}/api/Household/create`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({ name }),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: "Failed to create household" }));
    throw new Error(error.detail || "Failed to create household");
  }
  return response.json();
}

async function joinHousehold(inviteCode: string): Promise<Household> {
  const response = await fetch(`${API_BASE_URL}/api/Household/join`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({ inviteCode }),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: "Failed to join household" }));
    throw new Error(error.detail || "Failed to join household");
  }
  return response.json();
}

async function leaveHousehold(): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/Household/leave`, {
    method: "POST",
    credentials: "include",
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: "Failed to leave household" }));
    throw new Error(error.detail || "Failed to leave household");
  }
}

async function deleteHousehold(id: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/Household/${id}`, {
    method: "DELETE",
    credentials: "include",
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: "Failed to delete household" }));
    throw new Error(error.detail || "Failed to delete household");
  }
}

async function regenerateInviteCode(id: number): Promise<string> {
  const response = await fetch(`${API_BASE_URL}/api/Household/${id}/regenerate-code`, {
    method: "POST",
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to regenerate invite code");
  const data = await response.json();
  return data.inviteCode;
}

async function removeMember(householdId: number, userId: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/Household/${householdId}/members/${userId}`, {
    method: "DELETE",
    credentials: "include",
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: "Failed to remove member" }));
    throw new Error(error.detail || "Failed to remove member");
  }
}

export function useHousehold(enabled: boolean = true) {
  return useQuery({
    queryKey: ["household"],
    queryFn: fetchHousehold,
    enabled,
  });
}

export function useHouseholdSummary(enabled: boolean = true) {
  return useQuery({
    queryKey: ["household", "summary"],
    queryFn: fetchHouseholdSummary,
    enabled,
  });
}

export function useCreateHousehold() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createHousehold,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["household"] });
    },
  });
}

export function useJoinHousehold() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: joinHousehold,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["household"] });
    },
  });
}

export function useLeaveHousehold() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: leaveHousehold,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["household"] });
      queryClient.invalidateQueries({ queryKey: ["expenses"] });
    },
  });
}

export function useDeleteHousehold() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteHousehold,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["household"] });
      queryClient.invalidateQueries({ queryKey: ["expenses"] });
    },
  });
}

export function useRegenerateInviteCode() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: regenerateInviteCode,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["household"] });
    },
  });
}

export function useRemoveMember() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ householdId, userId }: { householdId: number; userId: number }) =>
      removeMember(householdId, userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["household"] });
    },
  });
}
