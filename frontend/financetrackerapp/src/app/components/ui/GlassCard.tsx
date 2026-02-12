"use client";

import { ReactNode } from "react";

interface GlassCardProps {
  children: ReactNode;
  className?: string;
  hover?: boolean;
  gradient?: "none" | "blue" | "green" | "purple" | "rose" | "amber";
}

export default function GlassCard({
  children,
  className = "",
  hover = true,
}: GlassCardProps) {
  return (
    <div
      className={`
        relative overflow-hidden
        bg-slate-800
        border border-slate-700
        rounded-2xl
        shadow-xl shadow-black/20
        ${hover ? "transition-all duration-300 hover:shadow-2xl hover:shadow-black/30 hover:-translate-y-1 hover:border-slate-600" : ""}
        ${className}
      `}
    >
      {children}
    </div>
  );
}
