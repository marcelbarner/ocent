# ocent Design System — Colors, Typography, Spacing, Tokens

---

## Color System

### Philosophy

- Dark mode is the primary mode. Light mode is a genuine, fully-specified alternative — not an afterthought.
- The neutral palette is the foundation. Domain accent colors appear only where they carry semantic meaning.
- Semantic colors (success, warning, error, info) are used strictly for status — never decoratively.
- All surface colors use very low saturation to avoid visual fatigue during long sessions.

### Neutral Palette (Shared Foundation)

These neutrals form all backgrounds, borders, text, and surface layers. They use a slightly warm-cool slate that avoids pure gray sterility.

| Token Name          | Dark Mode HEX | Dark Mode HSL            | Light Mode HEX | Light Mode HSL           |
|---------------------|---------------|--------------------------|----------------|--------------------------|
| `--surface-base`    | `#0d0f12`     | `hsl(220, 18%, 7%)`      | `#f5f6f8`      | `hsl(220, 14%, 96%)`     |
| `--surface-raised`  | `#141720`     | `hsl(228, 20%, 10%)`     | `#ffffff`      | `hsl(0, 0%, 100%)`       |
| `--surface-overlay` | `#1c2030`     | `hsl(228, 22%, 14%)`     | `#eef0f5`      | `hsl(228, 20%, 94%)`     |
| `--surface-sunken`  | `#0a0c0f`     | `hsl(220, 20%, 5%)`      | `#e8eaef`      | `hsl(228, 18%, 92%)`     |
| `--border-subtle`   | `#242838`     | `hsl(228, 22%, 18%)`     | `#d8dae3`      | `hsl(228, 16%, 86%)`     |
| `--border-default`  | `#30364a`     | `hsl(228, 22%, 24%)`     | `#b8bccb`      | `hsl(228, 14%, 76%)`     |
| `--border-strong`   | `#444c66`     | `hsl(228, 20%, 34%)`     | `#8f96af`      | `hsl(228, 14%, 63%)`     |

### Text Palette

| Token Name          | Dark Mode HEX | Dark Mode HSL            | Light Mode HEX | Light Mode HSL           |
|---------------------|---------------|--------------------------|----------------|--------------------------|
| `--text-primary`    | `#e8eaf0`     | `hsl(228, 24%, 93%)`     | `#0d0f12`      | `hsl(220, 18%, 7%)`      |
| `--text-secondary`  | `#9aa0b8`     | `hsl(228, 18%, 66%)`     | `#444c66`      | `hsl(228, 20%, 34%)`     |
| `--text-muted`      | `#5c6480`     | `hsl(228, 18%, 43%)`     | `#6b748f`      | `hsl(228, 16%, 50%)`     |
| `--text-disabled`   | `#3a4058`     | `hsl(228, 20%, 29%)`     | `#a0a8bf`      | `hsl(228, 16%, 69%)`     |
| `--text-inverse`    | `#0d0f12`     | `hsl(220, 18%, 7%)`      | `#e8eaf0`      | `hsl(228, 24%, 93%)`     |
| `--text-on-accent`  | `#ffffff`     | `hsl(0, 0%, 100%)`       | `#ffffff`      | `hsl(0, 0%, 100%)`       |

### Primary Brand Color

A refined slate-blue. Not a pure indigo (which belongs to Documents). Distinct enough to signal "system action" without competing with domain colors.

| Token Name             | HEX       | HSL                    | Usage |
|------------------------|-----------|------------------------|-------|
| `--color-primary-50`   | `#eef1fc` | `hsl(232, 76%, 96%)`   | Tints, backgrounds |
| `--color-primary-100`  | `#d5ddf8` | `hsl(232, 72%, 89%)`   | Hover backgrounds |
| `--color-primary-300`  | `#8898ef` | `hsl(232, 74%, 73%)`   | Decorative use |
| `--color-primary-500`  | `#4f65e3` | `hsl(232, 70%, 60%)`   | Default interactive |
| `--color-primary-600`  | `#3d52d6` | `hsl(232, 67%, 54%)`   | Hover state |
| `--color-primary-700`  | `#2e3fc8` | `hsl(232, 65%, 48%)`   | Active/pressed |
| `--color-primary-900`  | `#1a2488` | `hsl(232, 68%, 32%)`   | Strong emphasis |

Interactive token aliases:

```scss
--color-interactive:          var(--color-primary-500);
--color-interactive-hover:    var(--color-primary-600);
--color-interactive-active:   var(--color-primary-700);
--color-interactive-subtle:   var(--color-primary-100);
--color-focus-ring:           var(--color-primary-500);
```

### Domain Accent Palettes

Each domain exposes three tokens: default, subtle background, and text-on-subtle. Used in navigation badges, domain headers, and status chips — never for large fills.

**Finance (Emerald)**
```scss
--color-finance:              #10b981;  // hsl(160, 84%, 39%)
--color-finance-subtle:       #0d2d22;  // hsl(160, 52%, 12%) — dark mode
--color-finance-text:         #34d399;  // hsl(160, 68%, 52%) — on subtle, dark mode
// Light mode:
--color-finance-subtle-light: #d1fae5;  // hsl(152, 76%, 90%)
--color-finance-text-light:   #065f46;  // hsl(161, 94%, 21%)
```

**Documents (Indigo)**
```scss
--color-documents:              #6366f1;  // hsl(239, 84%, 67%)
--color-documents-subtle:       #1a1b3d;  // hsl(239, 40%, 17%) — dark mode
--color-documents-text:         #a5b4fc;  // hsl(239, 88%, 82%) — on subtle, dark mode
--color-documents-subtle-light: #e0e7ff;  // hsl(239, 88%, 94%)
--color-documents-text-light:   #3730a3;  // hsl(243, 48%, 41%)
```

**Storage (Amber)**
```scss
--color-storage:              #f59e0b;  // hsl(38, 92%, 50%)
--color-storage-subtle:       #2d1f06;  // hsl(38, 68%, 10%) — dark mode
--color-storage-text:         #fcd34d;  // hsl(43, 96%, 65%) — on subtle, dark mode
--color-storage-subtle-light: #fef3c7;  // hsl(48, 96%, 89%)
--color-storage-text-light:   #92400e;  // hsl(26, 82%, 32%)
```

**Contracts (Slate-Violet)**
```scss
--color-contracts:              #8b5cf6;  // hsl(258, 90%, 66%)
--color-contracts-subtle:       #1f1240;  // hsl(258, 56%, 16%) — dark mode
--color-contracts-text:         #c4b5fd;  // hsl(258, 88%, 84%) — on subtle, dark mode
--color-contracts-subtle-light: #ede9fe;  // hsl(258, 84%, 96%)
--color-contracts-text-light:   #5b21b6;  // hsl(263, 70%, 42%)
```

### Semantic Colors

| Token                    | HEX Dark  | HSL Dark               | HEX Light | HSL Light              |
|--------------------------|-----------|------------------------|-----------|------------------------|
| `--color-success`        | `#22c55e` | `hsl(142, 71%, 45%)`   | `#16a34a` | `hsl(142, 72%, 37%)`   |
| `--color-success-subtle` | `#0a2617` | `hsl(142, 54%, 10%)`   | `#dcfce7` | `hsl(140, 84%, 94%)`   |
| `--color-warning`        | `#eab308` | `hsl(48, 96%, 48%)`    | `#ca8a04` | `hsl(45, 93%, 40%)`    |
| `--color-warning-subtle` | `#291c01` | `hsl(45, 94%, 8%)`     | `#fef9c3` | `hsl(55, 96%, 94%)`    |
| `--color-error`          | `#ef4444` | `hsl(0, 84%, 60%)`     | `#dc2626` | `hsl(0, 72%, 51%)`     |
| `--color-error-subtle`   | `#2d0a0a` | `hsl(0, 64%, 11%)`     | `#fee2e2` | `hsl(0, 88%, 95%)`     |
| `--color-info`           | `#38bdf8` | `hsl(199, 92%, 60%)`   | `#0284c7` | `hsl(200, 98%, 39%)`   |
| `--color-info-subtle`    | `#041e2d` | `hsl(201, 76%, 10%)`   | `#e0f2fe` | `hsl(204, 92%, 94%)`   |

### WCAG Compliance Notes

- All `--text-primary` on `--surface-raised` combinations exceed 7:1 (AAA) in both modes.
- All `--text-secondary` combinations meet 4.5:1 (AA) minimum.
- `--text-muted` is used only for non-essential decorative text — not for interactive labels or data values.
- Domain accent colors are used for backgrounds (`-subtle` tokens) with matching text tokens that always meet 4.5:1 on that background.
- Focus rings use `--color-focus-ring` with a 2px offset and 2px width — visible against all surfaces.

---

## Typography

### Font Selection

**Primary (UI):** Inter — geometric sans, excellent at small sizes, legible in dense data tables. Self-hosted via `@fontsource/inter`.

**Mono (Code/Values/IDs):** JetBrains Mono — used for transaction IDs, account numbers, IBAN displays, technical metadata, and any monospaced data field.

No serif typeface. No display typeface. This is a data tool, not a publication.

### Type Scale

Base size: `16px`. Scale ratio: 1.25 (Major Third), with adjustments for readability at extremes.

| Token               | Size (rem) | Size (px) | Weight    | Line Height | Usage |
|---------------------|------------|-----------|-----------|-------------|-------|
| `--text-xs`         | `0.75rem`  | `12px`    | 400       | `1.5`       | Captions, timestamps, helper text |
| `--text-sm`         | `0.875rem` | `14px`    | 400 / 500 | `1.5`       | Secondary labels, table cells, badges |
| `--text-base`       | `1rem`     | `16px`    | 400       | `1.6`       | Body text, form inputs, default paragraphs |
| `--text-md`         | `1.125rem` | `18px`    | 500       | `1.5`       | Section subheadings, card titles |
| `--text-lg`         | `1.25rem`  | `20px`    | 600       | `1.4`       | Page section headings |
| `--text-xl`         | `1.5rem`   | `24px`    | 600       | `1.3`       | Page titles, domain dashboard headings |
| `--text-2xl`        | `1.875rem` | `30px`    | 700       | `1.25`      | KPI values, hero numbers |
| `--text-3xl`        | `2.25rem`  | `36px`    | 700       | `1.2`       | Major financial totals, net worth display |

### Weight Rules

- **400 (Regular):** Body text, table cell content, secondary labels.
- **500 (Medium):** UI labels, navigation items, card subtitles. Default for interactive elements.
- **600 (Semibold):** Section headers, card titles, column headers. Most headings below page level.
- **700 (Bold):** KPI values, page-level headings, large financial numbers.

### Mono Usage

Apply `font-family: var(--font-mono)` to:
- Transaction IDs and reference numbers
- IBAN, BIC, account number fields
- Amounts in dense tables (aligns decimal points)
- Technical metadata (file hashes, document IDs)
- Any field where column alignment of digits matters

```scss
--font-ui:   'Inter', system-ui, sans-serif;
--font-mono: 'JetBrains Mono', 'Cascadia Code', 'Fira Code', monospace;
```

---

## Spacing System

Base unit: **4px**. All spacing values are multiples of this base.

| Token         | Value  | px  | Usage |
|---------------|--------|-----|-------|
| `--space-0`   | `0`    | 0   | Reset |
| `--space-1`   | `0.25rem` | 4px | Tightest internal gaps (icon-to-label) |
| `--space-2`   | `0.5rem`  | 8px | Compact padding, chip/badge internal |
| `--space-3`   | `0.75rem` | 12px | Input internal vertical padding, list item gaps |
| `--space-4`   | `1rem`    | 16px | Standard component padding, card inner margins |
| `--space-5`   | `1.25rem` | 20px | Section internal padding |
| `--space-6`   | `1.5rem`  | 24px | Card padding (default), form group gaps |
| `--space-8`   | `2rem`    | 32px | Between sections within a page |
| `--space-10`  | `2.5rem`  | 40px | Major section separators |
| `--space-12`  | `3rem`    | 48px | Page vertical rhythm, between content blocks |
| `--space-16`  | `4rem`    | 64px | Large layout zones |
| `--space-20`  | `5rem`    | 80px | Splash/hero areas (rare in this app) |

### Component Spacing Conventions

- **Card padding:** `--space-6` (24px) on all sides. Dense variant: `--space-4` (16px).
- **Table cell padding:** `--space-3` vertical, `--space-4` horizontal.
- **Form field gap:** `--space-4` between label and input, `--space-6` between fields.
- **Sidebar item padding:** `--space-3` vertical, `--space-4` horizontal.
- **Content page top padding:** `--space-8` (32px).
- **Section gap within page:** `--space-8` between major sections.

---

## Border Radius

Restrained. Slightly rounded to soften without feeling playful.

| Token          | Value    | Usage |
|----------------|----------|-------|
| `--radius-sm`  | `4px`    | Chips, badges, small inputs, table cells with status |
| `--radius-md`  | `8px`    | Cards (default), inputs, buttons, dropdowns |
| `--radius-lg`  | `12px`   | Modals, panels, drawers, larger cards |
| `--radius-xl`  | `16px`   | Sheet overlays, side panels (optional) |
| `--radius-full`| `9999px` | Pills, avatar, toggle switches |

No sharp corners (`0px`) except in intentionally table-derived contexts (e.g., full-width data table without card wrapper).

---

## Elevation & Shadow System

Elevation is communicated through background color contrast and subtle shadows — never through heavy drop shadows.

| Level | Token                 | Shadow Value (Dark Mode)                           | Usage |
|-------|-----------------------|----------------------------------------------------|-------|
| 0     | `--elevation-0`       | none                                               | Base page background |
| 1     | `--elevation-1`       | `0 1px 3px hsl(220 20% 3% / 0.4)`                | Cards on base surface |
| 2     | `--elevation-2`       | `0 4px 12px hsl(220 20% 3% / 0.5)`               | Dropdowns, popovers |
| 3     | `--elevation-3`       | `0 8px 24px hsl(220 20% 3% / 0.6)`               | Modals, drawers |
| 4     | `--elevation-4`       | `0 16px 48px hsl(220 20% 3% / 0.7)`              | Command palette, full overlays |

Light mode shadows use the same structure but with `hsl(228 20% 40% / 0.08)` to `0.20` opacity range — much lighter.

---

## Motion Tokens

Motion should be imperceptible as "animation" and perceived only as "response." No decorative animation.

| Token                  | Value     | Usage |
|------------------------|-----------|-------|
| `--duration-instant`   | `60ms`    | Focus ring appearance |
| `--duration-fast`      | `120ms`   | Button states, hover fills |
| `--duration-default`   | `200ms`   | Most UI transitions (expand, collapse, color) |
| `--duration-slow`      | `320ms`   | Panel slide-in, modal entry |
| `--duration-deliberate`| `480ms`   | Page-level transitions (used sparingly) |
| `--ease-default`       | `cubic-bezier(0.16, 1, 0.3, 1)` | Most transitions — fast start, smooth settle |
| `--ease-in`            | `cubic-bezier(0.4, 0, 1, 1)`    | Exit animations |
| `--ease-out`           | `cubic-bezier(0, 0, 0.2, 1)`    | Entry animations |

Respect `prefers-reduced-motion`: all transitions collapse to instant or near-instant when the media query matches.

---

## CSS Custom Properties — Full Token Structure

### SCSS File Structure

```
src/
  styles.scss                   # Global entry — imports tokens and resets
  styles/
    _tokens.dark.scss           # All --color-*, --surface-*, --text-* for dark mode (:root)
    _tokens.light.scss          # Overrides for [data-theme="light"]
    _tokens.shared.scss         # Theme-invariant tokens (spacing, radius, font, motion)
    _reset.scss                 # Box-sizing, margin reset, font smoothing
    _typography.scss            # Base html/body font rules, heading defaults
    _utilities.scss             # Minimal single-purpose utility classes (sr-only, etc.)
```

### Root Token Block (excerpt)

```scss
// styles/_tokens.dark.scss
:root {
  // Surfaces
  --surface-base:    hsl(220, 18%, 7%);
  --surface-raised:  hsl(228, 20%, 10%);
  --surface-overlay: hsl(228, 22%, 14%);
  --surface-sunken:  hsl(220, 20%, 5%);

  // Borders
  --border-subtle:   hsl(228, 22%, 18%);
  --border-default:  hsl(228, 22%, 24%);
  --border-strong:   hsl(228, 20%, 34%);

  // Text
  --text-primary:    hsl(228, 24%, 93%);
  --text-secondary:  hsl(228, 18%, 66%);
  --text-muted:      hsl(228, 18%, 43%);
  --text-disabled:   hsl(228, 20%, 29%);
  --text-on-accent:  hsl(0, 0%, 100%);

  // Interactive
  --color-interactive:        hsl(232, 70%, 60%);
  --color-interactive-hover:  hsl(232, 67%, 54%);
  --color-interactive-active: hsl(232, 65%, 48%);
  --color-interactive-subtle: hsl(232, 72%, 89%);
  --color-focus-ring:         hsl(232, 70%, 60%);

  // Domain accents (dark mode)
  --color-finance:            hsl(160, 84%, 39%);
  --color-finance-subtle:     hsl(160, 52%, 12%);
  --color-finance-text:       hsl(160, 68%, 52%);

  --color-documents:          hsl(239, 84%, 67%);
  --color-documents-subtle:   hsl(239, 40%, 17%);
  --color-documents-text:     hsl(239, 88%, 82%);

  --color-storage:            hsl(38, 92%, 50%);
  --color-storage-subtle:     hsl(38, 68%, 10%);
  --color-storage-text:       hsl(43, 96%, 65%);

  --color-contracts:          hsl(258, 90%, 66%);
  --color-contracts-subtle:   hsl(258, 56%, 16%);
  --color-contracts-text:     hsl(258, 88%, 84%);

  // Semantic
  --color-success:        hsl(142, 71%, 45%);
  --color-success-subtle: hsl(142, 54%, 10%);
  --color-warning:        hsl(48, 96%, 48%);
  --color-warning-subtle: hsl(45, 94%, 8%);
  --color-error:          hsl(0, 84%, 60%);
  --color-error-subtle:   hsl(0, 64%, 11%);
  --color-info:           hsl(199, 92%, 60%);
  --color-info-subtle:    hsl(201, 76%, 10%);

  // Elevation
  --elevation-1: 0 1px 3px hsl(220 20% 3% / 0.4);
  --elevation-2: 0 4px 12px hsl(220 20% 3% / 0.5);
  --elevation-3: 0 8px 24px hsl(220 20% 3% / 0.6);
  --elevation-4: 0 16px 48px hsl(220 20% 3% / 0.7);
}

// styles/_tokens.light.scss
[data-theme='light'] {
  --surface-base:    hsl(220, 14%, 96%);
  --surface-raised:  hsl(0, 0%, 100%);
  --surface-overlay: hsl(228, 20%, 94%);
  --surface-sunken:  hsl(228, 18%, 92%);

  --border-subtle:   hsl(228, 16%, 86%);
  --border-default:  hsl(228, 14%, 76%);
  --border-strong:   hsl(228, 14%, 63%);

  --text-primary:    hsl(220, 18%, 7%);
  --text-secondary:  hsl(228, 20%, 34%);
  --text-muted:      hsl(228, 16%, 50%);
  --text-disabled:   hsl(228, 16%, 69%);

  --color-finance-subtle: hsl(152, 76%, 90%);
  --color-finance-text:   hsl(161, 94%, 21%);

  --color-documents-subtle: hsl(239, 88%, 94%);
  --color-documents-text:   hsl(243, 48%, 41%);

  --color-storage-subtle: hsl(48, 96%, 89%);
  --color-storage-text:   hsl(26, 82%, 32%);

  --color-contracts-subtle: hsl(258, 84%, 96%);
  --color-contracts-text:   hsl(263, 70%, 42%);

  --elevation-1: 0 1px 3px hsl(228 20% 40% / 0.08);
  --elevation-2: 0 4px 12px hsl(228 20% 40% / 0.12);
  --elevation-3: 0 8px 24px hsl(228 20% 40% / 0.16);
  --elevation-4: 0 16px 48px hsl(228 20% 40% / 0.20);
}

// styles/_tokens.shared.scss  — unchanged between themes
:root {
  // Typography
  --font-ui:   'Inter', system-ui, sans-serif;
  --font-mono: 'JetBrains Mono', 'Cascadia Code', monospace;

  --text-xs:   0.75rem;
  --text-sm:   0.875rem;
  --text-base: 1rem;
  --text-md:   1.125rem;
  --text-lg:   1.25rem;
  --text-xl:   1.5rem;
  --text-2xl:  1.875rem;
  --text-3xl:  2.25rem;

  // Spacing
  --space-1:  0.25rem;
  --space-2:  0.5rem;
  --space-3:  0.75rem;
  --space-4:  1rem;
  --space-5:  1.25rem;
  --space-6:  1.5rem;
  --space-8:  2rem;
  --space-10: 2.5rem;
  --space-12: 3rem;
  --space-16: 4rem;
  --space-20: 5rem;

  // Border radius
  --radius-sm:   4px;
  --radius-md:   8px;
  --radius-lg:   12px;
  --radius-xl:   16px;
  --radius-full: 9999px;

  // Motion
  --duration-instant:    60ms;
  --duration-fast:       120ms;
  --duration-default:    200ms;
  --duration-slow:       320ms;
  --duration-deliberate: 480ms;
  --ease-default: cubic-bezier(0.16, 1, 0.3, 1);
  --ease-in:      cubic-bezier(0.4, 0, 1, 1);
  --ease-out:     cubic-bezier(0, 0, 0.2, 1);
}

@media (prefers-reduced-motion: reduce) {
  :root {
    --duration-fast:       0ms;
    --duration-default:    0ms;
    --duration-slow:       60ms;
    --duration-deliberate: 60ms;
  }
}
```

### Theme Switching in Angular

Theme is controlled by `data-theme` attribute on `<html>`. An Angular service manages the preference with a signal, persists to `localStorage`, and respects `prefers-color-scheme` as the default.

```typescript
// theme.service.ts
export class ThemeService {
  readonly theme = signal<'dark' | 'light'>('dark');

  constructor() {
    const stored = localStorage.getItem('ocent-theme');
    const systemDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const resolved = (stored as 'dark' | 'light') ?? (systemDark ? 'dark' : 'light');
    this.theme.set(resolved);
    this.applyTheme(resolved);
  }

  toggle() {
    const next = this.theme() === 'dark' ? 'light' : 'dark';
    this.theme.set(next);
    localStorage.setItem('ocent-theme', next);
    this.applyTheme(next);
  }

  private applyTheme(theme: 'dark' | 'light') {
    document.documentElement.setAttribute('data-theme', theme);
  }
}
```

Dark mode is the default — no `data-theme` attribute required for dark, only `[data-theme="light"]` overrides.
