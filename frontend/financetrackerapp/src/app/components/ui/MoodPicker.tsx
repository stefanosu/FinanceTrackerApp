"use client";

interface MoodOption {
  value: string;
  label: string;
  emoji: string;
  color: string;
}

const MOODS: MoodOption[] = [
  { value: "Happy", label: "Happy", emoji: "\u{1F60A}", color: "bg-yellow-500/20 border-yellow-500 text-yellow-400" },
  { value: "Excited", label: "Excited", emoji: "\u{1F929}", color: "bg-pink-500/20 border-pink-500 text-pink-400" },
  { value: "Neutral", label: "Neutral", emoji: "\u{1F610}", color: "bg-slate-500/20 border-slate-500 text-slate-400" },
  { value: "Stressed", label: "Stressed", emoji: "\u{1F62B}", color: "bg-orange-500/20 border-orange-500 text-orange-400" },
  { value: "Sad", label: "Sad", emoji: "\u{1F622}", color: "bg-blue-500/20 border-blue-500 text-blue-400" },
  { value: "Anxious", label: "Anxious", emoji: "\u{1F630}", color: "bg-purple-500/20 border-purple-500 text-purple-400" },
  { value: "Bored", label: "Bored", emoji: "\u{1F971}", color: "bg-teal-500/20 border-teal-500 text-teal-400" },
];

interface MoodPickerProps {
  value: string;
  onChange: (mood: string) => void;
  className?: string;
  variant?: "light" | "dark";
}

export default function MoodPicker({
  value,
  onChange,
  className = "",
  variant = "light"
}: MoodPickerProps) {
  const baseStyles = variant === "dark"
    ? "bg-slate-800 border-slate-700"
    : "bg-white border-gray-200";

  const labelStyles = variant === "dark"
    ? "text-slate-300"
    : "text-gray-700";

  return (
    <div className={className}>
      <label className={`block text-sm font-medium mb-2 ${labelStyles}`}>
        How are you feeling? <span className="text-slate-500">(optional)</span>
      </label>
      <div className="flex flex-wrap gap-2">
        {MOODS.map((mood) => (
          <button
            key={mood.value}
            type="button"
            onClick={() => onChange(value === mood.value ? "" : mood.value)}
            className={`
              px-3 py-2 rounded-lg border-2 transition-all duration-200
              flex items-center gap-2 text-sm font-medium
              ${value === mood.value
                ? mood.color + " border-current scale-105"
                : `${baseStyles} hover:scale-105 opacity-70 hover:opacity-100`
              }
            `}
          >
            <span className="text-lg">{mood.emoji}</span>
            <span>{mood.label}</span>
          </button>
        ))}
      </div>
      {value && (
        <p className={`mt-2 text-sm ${variant === "dark" ? "text-slate-400" : "text-gray-500"}`}>
          Tracking your mood helps identify emotional spending patterns
        </p>
      )}
    </div>
  );
}

export { MOODS };
export type { MoodOption };
