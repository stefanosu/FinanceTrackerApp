"use client";

import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from "recharts";
import { useSpendingByMood, SpendingByMood } from "../../hooks/useEmotionalAnalytics";
import { useState } from "react";

const MOOD_COLORS: Record<string, string> = {
  Happy: "#eab308",      // yellow
  Excited: "#ec4899",    // pink
  Neutral: "#64748b",    // slate
  Stressed: "#f97316",   // orange
  Sad: "#3b82f6",        // blue
  Anxious: "#a855f7",    // purple
  Bored: "#14b8a6",      // teal
};

const MOOD_EMOJIS: Record<string, string> = {
  Happy: "\u{1F60A}",
  Excited: "\u{1F929}",
  Neutral: "\u{1F610}",
  Stressed: "\u{1F62B}",
  Sad: "\u{1F622}",
  Anxious: "\u{1F630}",
  Bored: "\u{1F971}",
};

interface EmotionalSpendingChartProps {
  startDate?: string;
  endDate?: string;
  enabled?: boolean;
}

export default function EmotionalSpendingChart({
  startDate,
  endDate,
  enabled = true,
}: EmotionalSpendingChartProps) {
  const {
    data: spending,
    isLoading,
    error,
  } = useSpendingByMood(startDate, endDate, enabled);
  const [activeIndex, setActiveIndex] = useState<number | null>(null);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="w-8 h-8 border-2 border-purple-400/30 rounded-full animate-spin border-t-purple-400"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center h-64 text-rose-400">
        Failed to load emotional spending data
      </div>
    );
  }

  if (!spending || spending.length === 0) {
    return (
      <div className="flex flex-col justify-center items-center h-64">
        <div className="w-16 h-16 mb-4 rounded-full bg-slate-700 flex items-center justify-center">
          <span className="text-3xl">{"\u{1F9E0}"}</span>
        </div>
        <p className="text-white/40 text-center">
          No emotional spending data yet
        </p>
        <p className="text-white/30 text-sm mt-1">
          Start tracking your mood when adding expenses
        </p>
      </div>
    );
  }

  const chartData = spending.map((item: SpendingByMood) => ({
    name: item.mood,
    value: item.amount,
    percentage: item.percentage,
    count: item.transactionCount,
    average: item.averageTransaction,
    emoji: MOOD_EMOJIS[item.mood] || "\u{1F610}",
  }));

  interface ChartDataItem {
    name: string;
    value: number;
    percentage: number;
    count: number;
    average: number;
    emoji: string;
  }

  const CustomTooltip = ({
    active,
    payload,
  }: {
    active?: boolean;
    payload?: Array<{ payload: ChartDataItem }>;
  }) => {
    if (active && payload && payload.length) {
      const data = payload[0].payload;
      return (
        <div className="bg-slate-800/90 backdrop-blur-sm p-3 shadow-xl rounded-xl border border-white/10">
          <p className="font-medium text-white flex items-center gap-2">
            <span>{data.emoji}</span>
            {data.name}
          </p>
          <p className="text-purple-300 font-semibold">
            ${data.value.toLocaleString("en-US", { minimumFractionDigits: 2 })}
          </p>
          <p className="text-white/50 text-sm">{data.percentage}% of emotional spending</p>
          <p className="text-white/40 text-xs mt-1">
            {data.count} transactions (avg ${data.average.toFixed(2)})
          </p>
        </div>
      );
    }
    return null;
  };

  return (
    <div className="h-64 flex">
      {/* Pie Chart */}
      <div className="flex-1">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={chartData}
              cx="50%"
              cy="50%"
              innerRadius={50}
              outerRadius={activeIndex !== null ? 85 : 80}
              paddingAngle={3}
              dataKey="value"
              onMouseEnter={(_, index) => setActiveIndex(index)}
              onMouseLeave={() => setActiveIndex(null)}
              animationBegin={0}
              animationDuration={800}
              animationEasing="ease-out"
            >
              {chartData.map((item, index) => (
                <Cell
                  key={`cell-${index}`}
                  fill={MOOD_COLORS[item.name] || "#64748b"}
                  stroke="transparent"
                  style={{
                    filter:
                      activeIndex === index
                        ? "brightness(1.2)"
                        : "brightness(1)",
                    transform:
                      activeIndex === index ? "scale(1.05)" : "scale(1)",
                    transformOrigin: "center",
                    transition: "all 0.2s ease-out",
                  }}
                />
              ))}
            </Pie>
            <Tooltip content={<CustomTooltip />} />
          </PieChart>
        </ResponsiveContainer>
      </div>

      {/* Legend */}
      <div className="w-44 flex flex-col justify-center space-y-2 pl-2">
        {chartData.map((item, index) => (
          <div
            key={item.name}
            className={`flex items-center gap-2 p-1.5 rounded-lg transition-all duration-200 cursor-pointer ${
              activeIndex === index ? "bg-slate-700" : "hover:bg-slate-700/50"
            }`}
            onMouseEnter={() => setActiveIndex(index)}
            onMouseLeave={() => setActiveIndex(null)}
          >
            <span className="text-lg flex-shrink-0">{item.emoji}</span>
            <div className="min-w-0 flex-1">
              <p className="text-xs text-white/80 truncate">{item.name}</p>
              <p className="text-xs text-white/40">{item.percentage}%</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
