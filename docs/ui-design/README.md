# ocent UI Design System — Overview

## What This Is

This directory contains the authoritative design concept for **ocent**, a self-hosted personal operations hub covering finance, documents, storage, and contracts. All UI decisions — from color tokens to component naming — are recorded here so they remain consistent across every page and session.

---

## Design Philosophy

Three principles govern every design decision in ocent:

### 1. Structured Calm

ocent handles sensitive, complex, and long-lived personal data. The interface must never feel exciting or urgent by default. Information should surface at the user's pace — not push itself forward. Whitespace, restrained color use, and stable layouts communicate trustworthiness before a single number is shown.

### 2. Dense Without Clutter

Personal finance, contract history, and document archives are inherently data-dense. The design must accommodate high information density — tables with many columns, KPI grids, transaction lists — without collapsing into a spreadsheet aesthetic. Hierarchy, grouping, and contextual emphasis carry the cognitive load, not decorative elements.

### 3. Domain Clarity, System Coherence

Four domains (Finance, Documents, Storage, Contracts) must each feel purposeful and distinct while sharing the same visual grammar. A user should know at a glance which domain they are in, but never feel like they have switched applications.

---

## Visual Identity Summary

**Style:** Neutral-dark, precision-forward. Not corporate enterprise, not playful SaaS — closer to a well-designed accounting tool or a premium portfolio tracker. Quiet authority.

**Tone:** Calm, structured, professional-personal. Language and layout convey: "your data is organized and safe here."

**Atmosphere:** Dark-mode-first with a genuine, carefully crafted light mode. Muted surfaces, precise typographic hierarchy, subtle depth through elevation rather than heavy borders.

**Mood reference:** Think a high-quality private banking interface crossed with a focused developer tool — minus the chrome and marketing patterns.

---

## Domain Color Anchors

Each domain carries a distinct accent hue used sparingly for navigation highlights, domain badges, and contextual emphasis. All other colors are shared across domains.

| Domain    | Accent Hue         | HEX       | HSL               |
|-----------|--------------------|-----------|-------------------|
| Finance   | Emerald            | `#10b981` | `hsl(160, 84%, 39%)` |
| Documents | Indigo             | `#6366f1` | `hsl(239, 84%, 67%)` |
| Storage   | Amber              | `#f59e0b` | `hsl(38, 92%, 50%)` |
| Contracts | Slate-Violet       | `#8b5cf6` | `hsl(258, 90%, 66%)` |

---

## File Index

| File | Contents |
|------|----------|
| `design-system.md` | Full color palette, typography scale, spacing system, elevation, motion tokens, SCSS custom property structure |
| `layout-navigation.md` | Shell architecture, sidebar/topbar/content zones, grid system, responsive breakpoints, navigation patterns |
| `component-philosophy.md` | Component inventory, dominant UI patterns, UX principles, Angular signal conventions, iconography |

---

## Technology Anchors

- **Framework:** Angular 21, standalone signal-based components
- **Style:** SCSS with CSS custom properties for all design tokens
- **Build:** Vite via `@analogjs/vite-plugin-angular`
- **No UI library dependency** — all components built from scratch against this design system
- **Icon set:** Lucide Icons (SVG sprite or `lucide-angular` package)
- **Font delivery:** Self-hosted via `@fontsource` packages (no Google Fonts CDN)

---

## Status

Initial concept. Covers the full design system foundation. Page-level specifications to be added as individual domain areas enter implementation.
