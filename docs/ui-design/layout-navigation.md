# ocent Layout & Navigation

---

## Shell Architecture

The application uses a persistent shell that wraps all content. The shell does not reload between navigation events — only the content zone changes.

### Layout Zones

```
┌─────────────────────────────────────────────────────────┐
│  TOPBAR  (56px fixed height, full width)                │
├──────────┬──────────────────────────────────────────────┤
│          │                                              │
│ SIDEBAR  │  CONTENT ZONE                               │
│ (240px   │  (flex-1, scrollable independently)         │
│  fixed)  │                                              │
│          │                                              │
└──────────┴──────────────────────────────────────────────┘
```

No footer. No horizontal scrolling at the shell level. The content zone is the only scrollable region on desktop.

### Sidebar

- **Width:** 240px fixed on desktop, collapses to icon-only rail (56px) at tablet breakpoint, hidden off-canvas at mobile.
- **Background:** `--surface-raised` — slightly elevated from the base page.
- **Right border:** `1px solid var(--border-subtle)`.
- **Position:** Fixed or sticky — sidebar does not scroll with content.
- **Collapse state:** User-controlled via toggle button. Preference persisted to `localStorage`.
- **No nested flyout menus.** Secondary navigation happens via a section header within the sidebar, not hover-triggered sub-menus.

### Topbar

- **Height:** 56px, fixed to top of viewport.
- **Background:** `--surface-raised`, same as sidebar.
- **Bottom border:** `1px solid var(--border-subtle)`.
- **Contents (left to right):**
  - Sidebar toggle button (mobile/tablet) or logo/wordmark (desktop)
  - Page breadcrumb / current context label (domain + page name)
  - Flexible spacer
  - Global search trigger (icon button, opens command palette)
  - Notification bell (with count badge if unread)
  - Theme toggle (sun/moon icon)
  - User/household avatar (opens profile popover)

### Content Zone

- `padding: var(--space-8) var(--space-8)` on desktop (32px all sides).
- `padding: var(--space-6) var(--space-4)` on tablet.
- `padding: var(--space-4)` on mobile.
- Max content width: **1440px**, centered when viewport exceeds that threshold.
- The content zone starts below the topbar (`margin-top: 56px`) and beside the sidebar.

---

## Sidebar Navigation Structure

### Domain Sections

The sidebar is organized by domain. Each domain has a labelled section. Within each section, 3–6 navigation items maximum. No deep trees.

```
─────────────────────────
  ocent                     ← wordmark / logo area (top)
─────────────────────────
  Overview                  ← single top-level item (home/hub dashboard)
─────────────────────────
  FINANCE                   ← section label (--text-muted, --text-xs, uppercase, letter-spaced)
  Dashboard
  Accounts
  Transactions
  Portfolio
  Pension
  Net Worth
─────────────────────────
  DOCUMENTS
  Dashboard
  All Documents
  Inbox
  Pipelines
─────────────────────────
  STORAGE
  Dashboard
  Catalog
  Inventory
  Locations
─────────────────────────
  CONTRACTS
  Dashboard
  All Contracts
  Calendar
─────────────────────────
  SYSTEM                    ← bottom section
  Containers
  Tasks
  Settings
─────────────────────────
  [User avatar + name]      ← bottom of sidebar
─────────────────────────
```

### Navigation Item States

Each nav item is 40px tall with 12px vertical / 16px horizontal padding.

| State    | Background           | Text color           | Left indicator |
|----------|----------------------|----------------------|----------------|
| Default  | transparent          | `--text-secondary`   | none           |
| Hover    | `--surface-overlay`  | `--text-primary`     | none           |
| Active   | domain-`-subtle` token | domain-`-text` token | 2px left bar in domain color |
| Focus    | transparent          | `--text-primary`     | `--color-focus-ring` outline |

The active domain section label also uses the domain accent color — this is the primary way the user knows which domain they are in.

### Collapsed Rail Mode (56px)

- Icons only, no labels.
- Section labels hidden.
- Domain indicator: colored dot above the first icon in each section.
- Tooltip on hover shows the full item label.
- Active item: icon uses domain accent color, background uses domain subtle token.

---

## Page-Level Layout Patterns

### Pattern A: Dashboard Page

Used for all domain dashboards and the Overview hub.

```
┌─────────────────────────────────────────────────────┐
│  Page Header (title, subtitle, primary action btn)  │
├─────────────────────────────────────────────────────┤
│  KPI Strip  [card] [card] [card] [card]             │
├──────────────────────┬──────────────────────────────┤
│                      │                              │
│  Primary Panel       │  Secondary Panel             │
│  (2/3 width)         │  (1/3 width)                 │
│                      │                              │
├──────────────────────┴──────────────────────────────┤
│  Full-width section (table, chart, or list)         │
└─────────────────────────────────────────────────────┘
```

Grid: 12-column CSS Grid with `--space-6` gap. KPI strip = 4 equal columns. Main panels = 8 cols / 4 cols. Full-width section = 12 cols.

### Pattern B: List / Index Page

Used for Transactions, Documents list, Contracts list, Catalog.

```
┌─────────────────────────────────────────────────────┐
│  Page Header + Filter Bar + View Toggle             │
├─────────────────────────────────────────────────────┤
│  Table or Card Grid (main content, full width)      │
│  (Pagination or infinite scroll at bottom)          │
└─────────────────────────────────────────────────────┘
```

No sidebar within the page. Filters are inline above the table, not a right-side panel (avoids hidden state on first visit).

### Pattern C: Detail / Record Page

Used for a single transaction, document, contract, or account.

```
┌──────────────────────────────┬──────────────────────┐
│                              │                      │
│  Record Detail (main)        │  Sidebar Panel       │
│  (title, all fields,         │  (linked records,    │
│   timeline/history)          │   metadata, actions) │
│                              │                      │
└──────────────────────────────┴──────────────────────┘
```

Grid: 8 cols main / 4 cols sidebar on desktop. At tablet, sidebar moves below main. At mobile, sidebar collapses into an accordion below the record.

### Pattern D: Settings / Configuration Page

Used for Settings, Pipeline configuration.

```
┌────────────────┬────────────────────────────────────┐
│  Settings Nav  │  Settings Content                  │
│  (secondary    │  (form, toggle groups)              │
│   left panel)  │                                    │
└────────────────┴────────────────────────────────────┘
```

Secondary settings nav is 200px fixed. Not a full sidebar — it's a secondary navigation panel inside the content zone. On mobile, collapses to a select/tabs above the content.

---

## Responsive Breakpoints

| Name     | Min Width | Shell Behavior |
|----------|-----------|----------------|
| Mobile   | 375px     | Sidebar hidden (off-canvas), topbar shows hamburger, single column layout |
| Tablet   | 768px     | Sidebar collapses to 56px icon rail, content patterns adapt to fewer columns |
| Desktop  | 1024px    | Full 240px sidebar, all patterns as designed |
| Wide     | 1440px    | Content zone max-width applied, no structural change |

Breakpoints as SCSS variables (also in tokens for use in components):

```scss
// styles/_tokens.shared.scss
:root {
  --breakpoint-mobile:  375px;
  --breakpoint-tablet:  768px;
  --breakpoint-desktop: 1024px;
  --breakpoint-wide:    1440px;
}
```

SCSS breakpoint mixins:

```scss
// styles/_breakpoints.scss
@mixin mobile-only {
  @media (max-width: 767px) { @content; }
}

@mixin tablet-up {
  @media (min-width: 768px) { @content; }
}

@mixin desktop-up {
  @media (min-width: 1024px) { @content; }
}

@mixin wide-up {
  @media (min-width: 1440px) { @content; }
}
```

---

## Grid System

All layout grids use CSS Grid directly — no grid framework.

### Standard 12-Column Content Grid

```scss
.o-grid {
  display: grid;
  grid-template-columns: repeat(12, 1fr);
  gap: var(--space-6);

  @include mobile-only {
    grid-template-columns: 1fr;
    gap: var(--space-4);
  }

  @include tablet-up {
    grid-template-columns: repeat(6, 1fr);
  }
}
```

Column span helpers used inline on child elements:

```scss
.o-col-4  { grid-column: span 4; }
.o-col-6  { grid-column: span 6; }
.o-col-8  { grid-column: span 8; }
.o-col-12 { grid-column: span 12; }
```

### KPI Strip Grid

```scss
.o-kpi-strip {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: var(--space-4);

  @include tablet-up {
    grid-template-columns: repeat(2, 1fr);
  }

  @include mobile-only {
    grid-template-columns: 1fr 1fr;
  }
}
```

---

## Shell Angular Component Structure

```
AppShellComponent           (selector: ocent-shell)
├── TopbarComponent         (selector: ocent-topbar)
│   ├── BreadcrumbComponent
│   ├── GlobalSearchTriggerComponent
│   ├── NotificationBellComponent
│   ├── ThemeToggleComponent
│   └── UserAvatarComponent
├── SidebarComponent        (selector: ocent-sidebar)
│   ├── SidebarLogoComponent
│   ├── SidebarNavSectionComponent  (×5 domains + system)
│   │   └── SidebarNavItemComponent (×n per section)
│   └── SidebarUserFooterComponent
└── <router-outlet>         (content zone)
```

The shell is the root component. All domain pages are lazy-loaded via Angular Router and rendered into the router outlet. The shell itself carries no domain-specific logic.

### Signal-Based Sidebar State

```typescript
// sidebar.service.ts
export class SidebarService {
  readonly isCollapsed = signal(false);
  readonly isMobileOpen = signal(false);

  toggle() {
    this.isCollapsed.update(v => !v);
    localStorage.setItem('ocent-sidebar-collapsed', String(this.isCollapsed()));
  }

  openMobile()  { this.isMobileOpen.set(true); }
  closeMobile() { this.isMobileOpen.set(false); }
}
```

No RxJS BehaviorSubject — pure signals throughout the shell. The sidebar component reads `isCollapsed` as a computed CSS class on its host element.

---

## Command Palette / Global Search

Triggered by the search button in the topbar, or by `Cmd+K` / `Ctrl+K` keyboard shortcut. Renders as a modal overlay with:

- Full-width search input (autofocused on open)
- Live results grouped by domain (Finance, Documents, Storage, Contracts, Containers)
- Recent items shown before typing begins
- Keyboard navigable (arrow keys, Enter to navigate, Escape to close)
- Results limited to 5 per group, with "View all in [domain]" link

The command palette is a standalone feature component, lazy-loaded, and injected into the shell template via a dedicated outlet or via ViewContainerRef.

---

## Notification Center

The notification bell opens a panel anchored to the topbar (not a full-page route). The panel is 380px wide, slides in from the top-right corner, and contains:

- Grouped notifications by urgency (Critical, Due Soon, Informational)
- Each notification: icon (domain color), title, body snippet, time relative, link action
- Mark all as read action
- Link to full notification management (a settings sub-page, not a main nav item)

Notification types map to domain urgency — a contract cancellation deadline is Critical; a document successfully processed is Informational.
