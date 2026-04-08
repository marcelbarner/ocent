# ocent Component Philosophy & UX Principles

---

## Core UX Principles

### 1. Data First, Chrome Minimal

The user opens ocent to see and act on their data — not to admire the interface. Every persistent UI element (navigation, headers, toolbars) consumes screen real estate that could show transactions, KPIs, or documents. UI chrome is kept to the minimum needed for orientation and action. No decorative headers, no banner imagery, no empty state illustrations with mascots.

### 2. Progressive Disclosure

ocent's data model is deeply interconnected and optionally very detailed. The interface never shows everything at once. The primary level shows the summary (a KPI, a row, a card title). The secondary level shows enough to act (a mini-detail panel, a popover). The full detail level is a dedicated page or drawer — never a massive inline expansion by default.

### 3. Explicit Data Provenance

Every field that can be machine-derived (OCR, AI suggestion, CSV import, pipeline result) carries a small origin indicator. Users working with financial and legal data need to know whether a value was entered manually or proposed by automation. This is not a power-user feature — it is part of the standard data display for relevant fields.

### 4. Keyboard-First Interaction

Repetitive actions — categorizing transactions, enriching documents, reviewing contracts — must be executable entirely by keyboard. Tab order is intentional. Actions that appear on hover must also appear on focus. Shortcuts follow platform conventions (Cmd on macOS, Ctrl on Windows/Linux).

### 5. Failure States Are Not Edge Cases

Imports fail. OCR produces garbage. AI pipelines timeout. Every data-bearing component must have a defined error state that is specific enough to help the user act — not just a generic "Something went wrong" message.

---

## Dominant UI Patterns

### A. Data Table (Primary Pattern)

The most used component in ocent. Transactions, documents, contracts, catalog items — all live in tables.

**Characteristics:**
- Sticky column headers with sort indicators
- Configurable visible columns (user preference, stored in localStorage/backend)
- Row-level hover reveals a quick-action menu (3-dot or inline icon buttons)
- Checkbox column for bulk selection
- Selection count + bulk action bar appears above table when rows are selected
- Pagination: page-based for large sets, infinite scroll for recent-first feeds
- No horizontal scrollbar on the table itself — column priorities determine which columns hide at smaller widths (least-important columns hide first)
- Column widths: fixed for status/type/date columns, flexible for name/title/description

**Empty state:** Centered in the table body area — icon (from Lucide, domain-colored), single-sentence explanation, one action button. No full-page empty state illustration.

**Loading state:** Skeleton rows — same height as real rows, animated shimmer in `--border-subtle` color. Column widths reflect expected content widths.

```html
<ocent-data-table
  [columns]="columns()"
  [rows]="transactions()"
  [loading]="isLoading()"
  [error]="error()"
  [selection]="selection()"
  (sortChange)="onSortChange($event)"
  (selectionChange)="onSelectionChange($event)"
  (rowAction)="onRowAction($event)">
</ocent-data-table>
```

### B. KPI Card

Used in dashboard KPI strips. Displays a single metric with context.

**Anatomy:**
- Label (--text-sm, --text-muted): metric name
- Value (--text-2xl or --text-3xl, --text-primary, --font-mono for financial values): the number
- Delta (--text-sm, semantic color): change vs. previous period, with arrow icon
- Sparkline or trend indicator (optional, subtle — single SVG line, no axis labels)
- Period label (--text-xs, --text-muted): "Last 30 days", "YTD", "All time"

No borders on KPI cards — elevation + background separation is sufficient. `--elevation-1` on `--surface-raised`.

```html
<ocent-kpi-card
  label="Net Worth"
  [value]="netWorth()"
  [delta]="netWorthDelta()"
  [deltaLabel]="'vs. last month'"
  [loading]="isLoading()"
  format="currency"
  currency="EUR">
</ocent-kpi-card>
```

### C. Domain Dashboard Card

A medium card used to surface a summary section within a dashboard — e.g., "Recent Transactions", "Pending Documents", "Expiring Contracts".

**Anatomy:**
- Card header: title (--text-md, --text-primary), subtitle (--text-sm, --text-muted), optional "View all" link (--text-sm, domain accent color)
- Card body: compact list of 3–5 recent/relevant items, OR a small chart, OR a status breakdown
- Card footer: optional action button or secondary information line

Cards use `--surface-raised`, `--radius-md`, `--elevation-1`, `padding: var(--space-6)`.

### D. Detail Side Panel (Drawer)

Used when the user needs to inspect a record without leaving the current list/dashboard context. Slides in from the right edge of the content zone (not from the screen edge — it stays within the shell's content column).

- Width: 480px on desktop, full-width on mobile
- Contains: record title, key fields in a definition list layout, linked records section, timeline/history, action buttons
- Keyboard: `Escape` to close, focus trapped inside while open
- Background: `--surface-raised`, `--elevation-3`, no backdrop blur (clean, not modal-style)

### E. Command Action Modal

Used for confirmations, quick-create forms, and destructive actions. True modal with backdrop.

- Max width: 480px (confirmations) or 600px (forms)
- Backdrop: `hsl(220 18% 4% / 0.7)` semi-transparent overlay
- No animation entrance except `opacity 0 → 1` with `--duration-slow` easing
- Focus trapped, `Escape` closes (unless destructive action pending)
- Standard button order: secondary/cancel left, primary/confirm right

### F. Status Chip / Badge

Inline status indicators used in tables and cards.

- Height: 20px (compact) or 24px (default)
- `border-radius: var(--radius-sm)` (4px)
- Background: semantic or domain `-subtle` token
- Text: matching `-text` token, `--text-xs`, `font-weight: 500`
- Optional: small colored dot prefix (4px circle) for extra scanability in dense tables
- Icon-only variant: 20px × 20px square with centered icon, tooltip on hover

Categories: success, warning, error, info, neutral, and one per domain accent.

### G. Form Layout

Forms in ocent are either standalone page sections or embedded in modals/drawers. They are never inline-editable tables (with rare exception for batch categorization).

- Label above input — always (not floating labels, not inline left-of-input)
- Helper text below input (--text-xs, --text-muted) where needed
- Error message replaces helper text (--text-xs, --color-error) — never below both
- Required fields: asterisk (*) beside label, not inline after
- Input height: 40px default, 36px compact (used in filter bars)
- Consistent left alignment of all labels and inputs within a form

### H. Provenance Indicator

A small inline element attached to any field that was not manually entered.

- Appears as a small icon (2 variants: "auto-filled" and "AI-proposed") followed by `--text-xs --text-muted` source label
- Hover tooltip shows full provenance: "Extracted by OCR on 2026-01-14", "Proposed by finance pipeline", "Imported from CSV on 2026-01-10"
- Color: neutral (--text-muted) — not accent-colored, to avoid distracting from the value itself
- User can override any auto-filled value; after override, indicator changes to "manually edited" or disappears

---

## Component Inventory

### Navigation & Shell

| Component | Selector | Purpose |
|-----------|----------|---------|
| App Shell | `ocent-shell` | Root layout container |
| Topbar | `ocent-topbar` | Top navigation bar |
| Sidebar | `ocent-sidebar` | Primary navigation sidebar |
| Sidebar Nav Item | `ocent-sidebar-nav-item` | Single navigation link |
| Breadcrumb | `ocent-breadcrumb` | Page location indicator |
| Command Palette | `ocent-command-palette` | Global search/action overlay |
| Notification Panel | `ocent-notification-panel` | Anchored notification feed |

### Data Display

| Component | Selector | Purpose |
|-----------|----------|---------|
| Data Table | `ocent-data-table` | Full-featured sortable/selectable table |
| KPI Card | `ocent-kpi-card` | Single metric display |
| Domain Card | `ocent-domain-card` | Dashboard summary card |
| Chart Wrapper | `ocent-chart` | Wraps charting library with loading/error states |
| Sparkline | `ocent-sparkline` | Inline mini trend line |
| Status Chip | `ocent-status-chip` | Inline status/label badge |
| Data Definition | `ocent-data-def` | Label + value pair for detail views |
| Timeline | `ocent-timeline` | Chronological event list |

### Forms & Inputs

| Component | Selector | Purpose |
|-----------|----------|---------|
| Input Field | `ocent-input` | Standard text/number/date input with label/error |
| Select Field | `ocent-select` | Dropdown selector |
| Combobox | `ocent-combobox` | Searchable select with autocomplete |
| Amount Input | `ocent-amount-input` | Numeric input with currency display, mono font |
| Date Picker | `ocent-date-picker` | Date/date-range selection |
| File Upload | `ocent-file-upload` | Drag-drop + click upload zone |
| Checkbox | `ocent-checkbox` | Standard checkbox with label |
| Toggle Switch | `ocent-toggle` | Boolean on/off control |
| Tag Input | `ocent-tag-input` | Multi-value text input for tags/labels |
| Filter Bar | `ocent-filter-bar` | Compact row of filter controls above a table |

### Feedback & Overlay

| Component | Selector | Purpose |
|-----------|----------|---------|
| Modal | `ocent-modal` | Command/confirmation overlay |
| Drawer | `ocent-drawer` | Side panel for record detail |
| Toast | `ocent-toast` | Non-blocking success/error notification |
| Alert Banner | `ocent-alert` | Inline persistent message (warning/info) |
| Skeleton | `ocent-skeleton` | Loading placeholder |
| Empty State | `ocent-empty-state` | No-data fallback within a container |
| Error State | `ocent-error-state` | Data-fetch failure display with retry |
| Tooltip | `ocent-tooltip` | Hover/focus auxiliary label |
| Popover | `ocent-popover` | Richer hover/click contextual panel |
| Confirmation | `ocent-confirm` | Destructive action confirmation modal |

### Domain-Specific (Examples — not exhaustive)

| Component | Selector | Domain |
|-----------|----------|--------|
| Transaction Row | `ocent-transaction-row` | Finance |
| Account Summary Card | `ocent-account-card` | Finance |
| Document Thumbnail | `ocent-document-thumb` | Documents |
| Pipeline Status | `ocent-pipeline-status` | Documents |
| Inventory Row | `ocent-inventory-row` | Storage |
| Contract Row | `ocent-contract-row` | Contracts |
| Cancellation Countdown | `ocent-cancellation-countdown` | Contracts |
| Container Badge | `ocent-container-badge` | Cross-domain |
| Provenance Indicator | `ocent-provenance` | Cross-domain |

---

## Angular Signal Conventions

### Signals Over Observables

All component state is `signal()` or `computed()`. Services expose signals, not Observables, except at the data-fetch boundary. The HTTP layer returns Observables (from `HttpClient`), which are immediately converted to signals via `toSignal()` at the service or component level.

```typescript
// finance.service.ts
export class FinanceService {
  private http = inject(HttpClient);

  readonly accounts = toSignal(
    this.http.get<Account[]>('/api/accounts'),
    { initialValue: [] }
  );
}

// transactions-page.component.ts
export class TransactionsPageComponent {
  private finance = inject(FinanceService);

  readonly transactions = toSignal(
    this.http.get<Transaction[]>('/api/transactions'),
    { initialValue: [] }
  );
  readonly isLoading = signal(false);
  readonly error = signal<string | null>(null);
}
```

### Component Input Signals

Use `input()` not `@Input()` for all component inputs.

```typescript
@Component({ selector: 'ocent-kpi-card', ... })
export class KpiCardComponent {
  readonly label   = input.required<string>();
  readonly value   = input.required<number | null>();
  readonly loading = input(false);
  readonly format  = input<'currency' | 'number' | 'percent'>('number');
  readonly delta   = input<number | null>(null);
}
```

### Output Signals

Use `output()` not `@Output() EventEmitter` for all component events.

```typescript
readonly sortChange       = output<SortEvent>();
readonly selectionChange  = output<string[]>();
readonly rowAction        = output<RowActionEvent>();
```

---

## Iconography

### Icon Set: Lucide Icons

Lucide is the chosen icon set. Reasons:
- MIT licensed, self-hostable
- Consistent 24×24 grid with 2px stroke
- Wide coverage for finance, file, and system categories
- Available as `lucide-angular` package (tree-shakeable SVG components)
- All icons are SVG — no icon font (avoids FOUT, better accessibility with `aria-hidden`)

### Usage Rules

- All decorative icons: `aria-hidden="true"` on the SVG
- All meaningful icons (action icons without visible labels): `aria-label` on the button wrapper
- Default icon size: 16px (in dense tables), 20px (default UI), 24px (prominent actions)
- Icon color: inherits from text context via `currentColor` — no hardcoded colors on icons
- Domain icons (used in nav and badges):
  - Finance: `trending-up`
  - Documents: `file-text`
  - Storage: `package`
  - Contracts: `scroll-text`
  - Containers: `layers`
  - Tasks: `check-square`

### Angular Icon Component

Wrap Lucide in a single host component to centralize size and accessibility handling:

```typescript
@Component({
  selector: 'ocent-icon',
  template: `<ng-icon [name]="name()" [size]="sizeStr()" />`,
})
export class IconComponent {
  readonly name  = input.required<string>();
  readonly size  = input<16 | 20 | 24>(20);
  readonly sizeStr = computed(() => String(this.size()));
}
```

---

## Charting

For KPI trend lines, portfolio performance charts, liquidity forecasts, and net worth development, use **Apache ECharts** via `ngx-echarts`. Reasons:
- Handles large datasets well (thousands of data points for transaction history)
- Dark/light theming via custom theme objects keyed to the design token palette
- Canvas-based rendering — no SVG DOM overhead at scale
- `ngx-echarts` provides Angular-idiomatic wrapper with signal-compatible update model

All chart components wrap `ngx-echarts` with:
- Standard loading state (skeleton or spinner)
- Standard error state with retry
- Empty state for no-data periods
- Responsive resize handling via `ResizeObserver`
- Reduced-motion fallback: disable animations if `prefers-reduced-motion` matches

---

## Accessibility Conventions

### Non-Negotiable Requirements

- All interactive elements reachable by keyboard in logical tab order
- Focus ring visible on all focusable elements — never `outline: none` without a replacement
- All images (icons with meaning, charts) have text alternatives
- Form inputs always associated with visible labels via `for`/`id` or `aria-labelledby`
- Error messages associated to inputs via `aria-describedby`
- Dynamic content updates announced to screen readers via `role="status"` or `aria-live` as appropriate
- Color is never the only indicator of state — always accompanied by text or icon

### ARIA Patterns by Component

- Data table: `role="grid"` with `aria-sort` on sortable column headers
- Status chips: inline `<span>` with meaningful text — not icon-only without `aria-label`
- Modal: `role="dialog"`, `aria-modal="true"`, `aria-labelledby` pointing to title
- Drawer: `role="complementary"` or `role="dialog"` depending on content
- Toast notifications: `role="status"` for success, `role="alert"` for errors
- Sidebar navigation: `<nav>` with `aria-label="Primary navigation"`, active item: `aria-current="page"`
- Command palette: `role="combobox"` with `aria-activedescendant`, `aria-expanded`

### Contrast Enforcement

- `--text-primary` on `--surface-raised`: minimum 7:1 (AAA)
- `--text-secondary` on `--surface-raised`: minimum 4.5:1 (AA)
- Domain accent colors on domain subtle backgrounds: verified 4.5:1 minimum for all combinations in both modes
- All interactive element labels: 4.5:1 (AA) minimum against their background
- Focus ring: 3:1 against adjacent colors (WCAG 2.2 §1.4.11)

---

## Naming Conventions

### SCSS Files

```
src/styles/
  _tokens.dark.scss
  _tokens.light.scss
  _tokens.shared.scss
  _reset.scss
  _typography.scss
  _breakpoints.scss
  _utilities.scss

src/app/
  shell/
    shell.component.scss
    topbar/topbar.component.scss
    sidebar/sidebar.component.scss
  shared/ui/
    data-table/data-table.component.scss
    kpi-card/kpi-card.component.scss
    domain-card/domain-card.component.scss
    status-chip/status-chip.component.scss
    modal/modal.component.scss
    drawer/drawer.component.scss
    ...
  domains/
    finance/
      dashboard/finance-dashboard.component.scss
      transactions/transactions-page.component.scss
      ...
    documents/...
    storage/...
    contracts/...
```

### CSS Class Naming

BEM-lite within component scope (Angular ViewEncapsulation handles scope). No global BEM class chains.

```scss
// Within data-table.component.scss
.table          { }
.table__header  { }
.table__row     { }
.table__row--selected { }
.table__cell    { }
.table__cell--numeric { font-family: var(--font-mono); text-align: right; }
```

Global utility classes (defined in `_utilities.scss`) use the `u-` prefix:

```scss
.u-sr-only      { /* visually hidden but screen-reader accessible */ }
.u-mono         { font-family: var(--font-mono); }
.u-truncate     { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
```

Domain utility classes use the `d-` prefix + domain name:

```scss
.d-finance    { color: var(--color-finance); }
.d-documents  { color: var(--color-documents); }
.d-storage    { color: var(--color-storage); }
.d-contracts  { color: var(--color-contracts); }
```
