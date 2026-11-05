"use client";

import { useQuery } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import Link from "next/link";
import GlassCard from "../components/ui/GlassCard";
import {
  useHousehold,
  useCreateHousehold,
  useJoinHousehold,
  useLeaveHousehold,
  useDeleteHousehold,
  useRegenerateInviteCode,
  useRemoveMember,
} from "../hooks/useHousehold";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5280";

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

export default function HouseholdPage() {
  const router = useRouter();
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [showJoinForm, setShowJoinForm] = useState(false);
  const [householdName, setHouseholdName] = useState("");
  const [inviteCode, setInviteCode] = useState("");
  const [error, setError] = useState("");
  const [copiedCode, setCopiedCode] = useState(false);

  const { data: user, isLoading: userLoading, error: userError } = useQuery({
    queryKey: ["currentUser"],
    queryFn: fetchCurrentUser,
    retry: false,
  });

  const { data: householdData, isLoading: householdLoading } = useHousehold(!!user);
  const createHousehold = useCreateHousehold();
  const joinHousehold = useJoinHousehold();
  const leaveHousehold = useLeaveHousehold();
  const deleteHousehold = useDeleteHousehold();
  const regenerateCode = useRegenerateInviteCode();
  const removeMember = useRemoveMember();

  useEffect(() => {
    if (userError) {
      router.push("/login");
    }
  }, [userError, router]);

  if (userLoading || householdLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
        <div className="w-16 h-16 border-4 border-teal-400/30 rounded-full animate-spin border-t-teal-400"></div>
      </div>
    );
  }

  if (!user) return null;

  const household = householdData?.household;
  const isOwner = household?.ownerId === user.id;

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    try {
      await createHousehold.mutateAsync(householdName);
      setShowCreateForm(false);
      setHouseholdName("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create household");
    }
  };

  const handleJoin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    try {
      await joinHousehold.mutateAsync(inviteCode);
      setShowJoinForm(false);
      setInviteCode("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to join household");
    }
  };

  const handleLeave = async () => {
    if (!confirm("Are you sure you want to leave this household?")) return;
    try {
      await leaveHousehold.mutateAsync();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to leave household");
    }
  };

  const handleDelete = async () => {
    if (!household) return;
    if (!confirm("Are you sure you want to delete this household? All members will be removed.")) return;
    try {
      await deleteHousehold.mutateAsync(household.id);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to delete household");
    }
  };

  const handleRegenerateCode = async () => {
    if (!household) return;
    try {
      await regenerateCode.mutateAsync(household.id);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to regenerate code");
    }
  };

  const handleRemoveMember = async (userId: number) => {
    if (!household) return;
    if (!confirm("Are you sure you want to remove this member?")) return;
    try {
      await removeMember.mutateAsync({ householdId: household.id, userId });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to remove member");
    }
  };

  const copyInviteCode = () => {
    if (household?.inviteCode) {
      navigator.clipboard.writeText(household.inviteCode);
      setCopiedCode(true);
      setTimeout(() => setCopiedCode(false), 2000);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      {/* Navigation */}
      <nav className="bg-slate-800/90 backdrop-blur-sm border-b border-slate-700">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center gap-3">
              <Link href="/dashboard" className="flex items-center gap-3">
                <div className="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center">
                  <span className="text-white font-bold text-lg">F</span>
                </div>
                <span className="text-xl font-bold text-white">FinanceTracker</span>
              </Link>
            </div>
            <div className="flex items-center">
              <Link href="/dashboard" className="text-slate-400 hover:text-white transition-colors">
                Back to Dashboard
              </Link>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <h1 className="text-2xl font-bold text-white mb-6">Family Budgeting</h1>

        {error && (
          <div className="mb-6 p-4 bg-rose-500/20 border border-rose-500/50 rounded-xl text-rose-300">
            {error}
          </div>
        )}

        {!household ? (
          // No household - show create/join options
          <div className="space-y-6">
            <GlassCard className="p-6" hover={false}>
              <div className="text-center py-8">
                <div className="w-20 h-20 mx-auto mb-4 rounded-full bg-slate-700 flex items-center justify-center">
                  <span className="text-4xl">{"\u{1F3E0}"}</span>
                </div>
                <h2 className="text-xl font-semibold text-white mb-2">No Household Yet</h2>
                <p className="text-white/60 mb-6">
                  Create a household to start sharing expenses with family members, or join an existing one.
                </p>
                <div className="flex justify-center gap-4">
                  <button
                    onClick={() => { setShowCreateForm(true); setShowJoinForm(false); }}
                    className="px-6 py-3 bg-teal-600 text-white rounded-xl hover:bg-teal-500 transition-colors font-medium"
                  >
                    Create Household
                  </button>
                  <button
                    onClick={() => { setShowJoinForm(true); setShowCreateForm(false); }}
                    className="px-6 py-3 bg-slate-700 text-white rounded-xl hover:bg-slate-600 transition-colors font-medium border border-slate-600"
                  >
                    Join with Code
                  </button>
                </div>
              </div>
            </GlassCard>

            {showCreateForm && (
              <GlassCard className="p-6" hover={false}>
                <h3 className="text-lg font-semibold text-white mb-4">Create New Household</h3>
                <form onSubmit={handleCreate} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-white/80 mb-1">
                      Household Name
                    </label>
                    <input
                      type="text"
                      value={householdName}
                      onChange={(e) => setHouseholdName(e.target.value)}
                      placeholder="e.g., Smith Family"
                      className="w-full px-4 py-2 bg-slate-700 border border-slate-600 rounded-xl text-white placeholder-white/40 focus:border-teal-500 focus:ring-1 focus:ring-teal-500"
                      required
                    />
                  </div>
                  <div className="flex gap-3">
                    <button
                      type="submit"
                      disabled={createHousehold.isPending}
                      className="px-6 py-2 bg-teal-600 text-white rounded-xl hover:bg-teal-500 transition-colors disabled:opacity-50"
                    >
                      {createHousehold.isPending ? "Creating..." : "Create"}
                    </button>
                    <button
                      type="button"
                      onClick={() => setShowCreateForm(false)}
                      className="px-6 py-2 bg-slate-700 text-white rounded-xl hover:bg-slate-600 transition-colors"
                    >
                      Cancel
                    </button>
                  </div>
                </form>
              </GlassCard>
            )}

            {showJoinForm && (
              <GlassCard className="p-6" hover={false}>
                <h3 className="text-lg font-semibold text-white mb-4">Join Existing Household</h3>
                <form onSubmit={handleJoin} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-white/80 mb-1">
                      Invite Code
                    </label>
                    <input
                      type="text"
                      value={inviteCode}
                      onChange={(e) => setInviteCode(e.target.value.toUpperCase())}
                      placeholder="Enter 8-character code"
                      className="w-full px-4 py-2 bg-slate-700 border border-slate-600 rounded-xl text-white placeholder-white/40 focus:border-teal-500 focus:ring-1 focus:ring-teal-500 uppercase tracking-wider"
                      maxLength={8}
                      required
                    />
                  </div>
                  <div className="flex gap-3">
                    <button
                      type="submit"
                      disabled={joinHousehold.isPending}
                      className="px-6 py-2 bg-teal-600 text-white rounded-xl hover:bg-teal-500 transition-colors disabled:opacity-50"
                    >
                      {joinHousehold.isPending ? "Joining..." : "Join"}
                    </button>
                    <button
                      type="button"
                      onClick={() => setShowJoinForm(false)}
                      className="px-6 py-2 bg-slate-700 text-white rounded-xl hover:bg-slate-600 transition-colors"
                    >
                      Cancel
                    </button>
                  </div>
                </form>
              </GlassCard>
            )}
          </div>
        ) : (
          // Has household - show details
          <div className="space-y-6">
            <GlassCard className="p-6" hover={false}>
              <div className="flex items-start justify-between mb-6">
                <div>
                  <h2 className="text-xl font-semibold text-white">{household.name}</h2>
                  <p className="text-white/60">
                    Created by {household.ownerName} on{" "}
                    {new Date(household.createdAt).toLocaleDateString()}
                  </p>
                </div>
                <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                  isOwner ? "bg-teal-500/20 text-teal-400" : "bg-slate-700 text-white/60"
                }`}>
                  {isOwner ? "Owner" : "Member"}
                </span>
              </div>

              {/* Invite Code Section */}
              <div className="p-4 bg-slate-700/50 rounded-xl mb-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-white/60 mb-1">Invite Code</p>
                    <p className="text-2xl font-mono font-bold text-teal-400 tracking-wider">
                      {household.inviteCode}
                    </p>
                  </div>
                  <div className="flex gap-2">
                    <button
                      onClick={copyInviteCode}
                      className="px-4 py-2 bg-slate-600 text-white rounded-lg hover:bg-slate-500 transition-colors text-sm"
                    >
                      {copiedCode ? "Copied!" : "Copy"}
                    </button>
                    {isOwner && (
                      <button
                        onClick={handleRegenerateCode}
                        disabled={regenerateCode.isPending}
                        className="px-4 py-2 bg-slate-600 text-white rounded-lg hover:bg-slate-500 transition-colors text-sm disabled:opacity-50"
                      >
                        Regenerate
                      </button>
                    )}
                  </div>
                </div>
                <p className="text-xs text-white/40 mt-2">
                  Share this code with family members to let them join
                </p>
              </div>

              {/* Members List */}
              <div>
                <h3 className="text-lg font-semibold text-white mb-4">
                  Members ({household.members.length})
                </h3>
                <div className="space-y-3">
                  {household.members.map((member) => (
                    <div
                      key={member.id}
                      className="flex items-center justify-between p-4 bg-slate-700/50 rounded-xl"
                    >
                      <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-full bg-gradient-to-br from-teal-400 to-emerald-400 flex items-center justify-center text-white font-medium">
                          {member.firstName[0]}
                        </div>
                        <div>
                          <p className="text-white font-medium">
                            {member.firstName} {member.lastName}
                            {member.userId === user.id && " (You)"}
                          </p>
                          <p className="text-sm text-white/40">{member.email}</p>
                        </div>
                      </div>
                      <div className="flex items-center gap-3">
                        <span className={`px-2 py-1 rounded text-xs font-medium ${
                          member.role === "Owner"
                            ? "bg-teal-500/20 text-teal-400"
                            : "bg-slate-600 text-white/60"
                        }`}>
                          {member.role}
                        </span>
                        {isOwner && member.userId !== user.id && (
                          <button
                            onClick={() => handleRemoveMember(member.userId)}
                            className="text-rose-400 hover:text-rose-300 transition-colors text-sm"
                          >
                            Remove
                          </button>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </GlassCard>

            {/* Actions */}
            <GlassCard className="p-6" hover={false}>
              <h3 className="text-lg font-semibold text-white mb-4">Actions</h3>
              <div className="flex gap-3">
                {isOwner ? (
                  <button
                    onClick={handleDelete}
                    disabled={deleteHousehold.isPending}
                    className="px-6 py-2 bg-rose-600 text-white rounded-xl hover:bg-rose-500 transition-colors disabled:opacity-50"
                  >
                    {deleteHousehold.isPending ? "Deleting..." : "Delete Household"}
                  </button>
                ) : (
                  <button
                    onClick={handleLeave}
                    disabled={leaveHousehold.isPending}
                    className="px-6 py-2 bg-rose-600 text-white rounded-xl hover:bg-rose-500 transition-colors disabled:opacity-50"
                  >
                    {leaveHousehold.isPending ? "Leaving..." : "Leave Household"}
                  </button>
                )}
              </div>
            </GlassCard>
          </div>
        )}
      </main>
    </div>
  );
}
