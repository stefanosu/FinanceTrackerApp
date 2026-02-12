"use client";

import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from "recharts";
import {
  useSpendingByCategory,
  SpendingByCategory,
} from "../../hooks/useAnalytics";
import { useState } from "react";

const COLORS = [
  "#14b8a6", // teal
  "#22c55e", // green
  "#3b82f6", // blue
  "#f97316", // orange
  "#06b6d4", // cyan
  "#8b5cf6", // violet
  "#eab308", // yellow
  "#ef4444", // red
];

interface SpendingPieChartProps {
  startDate?: string;
  endDate?: string;
  enabled?: boolean;
}

export default function SpendingPieChart({
  startDate,
  endDate,
  enabled = true,
}: SpendingPieChartProps) {
  const {
    data: spending,
    isLoading,
    error,
  } = useSpendingByCategory(startDate, endDate, enabled);
  const [activeIndex, setActiveIndex] = useState<number | null>(null);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="w-8 h-8 border-2 border-teal-400/30 rounded-full animate-spin border-t-teal-400"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center h-64 text-rose-400">
        Failed to load spending data
      </div>
    );
  }

  if (!spending || spending.length === 0) {
    return (
      <div className="flex flex-col justify-center items-center h-64">
        <div className="w-16 h-16 mb-4 rounded-full bg-slate-700 flex items-center justify-center">
          <svg
            className="w-8 h-8 text-white/30"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M11 3.055A9.001 9.001 0 1020.945 13H11V3.055z"
            />
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M20.488 9H15V3.512A9.025 9.025 0 0120.488 9z"
            />
          </svg>
        </div>
        <p className="text-white/40">No spending data available</p>
      </div>
    );
  }

  const chartData = spending.map((item: SpendingByCategory) => ({
    name: item.category,
    value: item.amount,
    percentage: item.percentage,
  }));

  interface ChartDataItem {
    name: string;
    value: number;
    percentage: number;
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
          <p className="font-medium text-white">{data.name}</p>
          <p className="text-teal-300 font-semibold">
            ${data.value.toLocaleString("en-US", { minimumFractionDigits: 2 })}
          </p>
          <p className="text-white/50 text-sm">{data.percentage}% of total</p>
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
              {chartData.map((_, index) => (
                <Cell
                  key={`cell-${index}`}
                  fill={COLORS[index % COLORS.length]}
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
      <div className="w-40 flex flex-col justify-center space-y-2 pl-2">
        {chartData.slice(0, 6).map((item, index) => (
          <div
            key={item.name}
            className={`flex items-center gap-2 p-1.5 rounded-lg transition-all duration-200 cursor-pointer ${
              activeIndex === index ? "bg-slate-700" : "hover:bg-slate-700/50"
            }`}
            onMouseEnter={() => setActiveIndex(index)}
            onMouseLeave={() => setActiveIndex(null)}
          >
            <div
              className="w-3 h-3 rounded-full flex-shrink-0"
              style={{ backgroundColor: COLORS[index % COLORS.length] }}
            />
            <div className="min-w-0 flex-1">
              <p className="text-xs text-white/80 truncate">{item.name}</p>
              <p className="text-xs text-white/40">{item.percentage}%</p>
            </div>
          </div>
        ))}
        {chartData.length > 6 && (
          <p className="text-xs text-white/30 pl-5">
            +{chartData.length - 6} more
          </p>
        )}
      </div>
    </div>
  );
}
