"use client";

interface TrendIndicatorProps {
  value: number;
  label?: string;
  inverted?: boolean; // For expenses, negative is good
}

export default function TrendIndicator({
  value,
  label,
  inverted = false,
}: TrendIndicatorProps) {
  const isPositive = inverted ? value < 0 : value > 0;
  const isNeutral = value === 0;
  const absValue = Math.abs(value);

  if (isNeutral) {
    return (
      <div className="flex items-center gap-1 text-gray-400 text-sm">
        <span className="w-4 h-4 flex items-center justify-center">—</span>
        <span>No change{label ? ` ${label}` : ""}</span>
      </div>
    );
  }

  return (
    <div
      className={`flex items-center gap-1 text-sm font-medium ${
        isPositive ? "text-emerald-400" : "text-rose-400"
      }`}
    >
      <span
        className={`w-5 h-5 rounded-full flex items-center justify-center ${
          isPositive ? "bg-emerald-400/20" : "bg-rose-400/20"
        }`}
      >
        {(inverted ? value < 0 : value > 0) ? (
          <svg
            className="w-3 h-3"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M5 10l7-7m0 0l7 7m-7-7v18"
            />
          </svg>
        ) : (
          <svg
            className="w-3 h-3"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M19 14l-7 7m0 0l-7-7m7 7V3"
            />
          </svg>
        )}
      </span>
      <span>
        {absValue.toFixed(1)}%{label ? ` ${label}` : ""}
      </span>
    </div>
  );
}
